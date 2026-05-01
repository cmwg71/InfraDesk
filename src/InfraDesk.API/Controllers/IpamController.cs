// Dateipfad: src/InfraDesk.API/Controllers/IpamController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;
using System.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IpamController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IServiceProvider _serviceProvider;

    public IpamController(ApplicationDbContext context, IServiceProvider serviceProvider)
    {
        _context = context;
        _serviceProvider = serviceProvider;
    }

    // 1. Liefert alle Subnetze inklusive Meta-Statistiken (Belegung)
    [HttpGet("subnets")]
    public async Task<ActionResult<IEnumerable<object>>> GetSubnets()
    {
        var subnets = await _context.Subnets
            .Select(s => new {
                s.Id,
                s.Name,
                s.NetworkAddress,
                s.CidrMask,
                s.VlanId,
                s.Gateway,
                s.IsFullScanEnabled, // WICHTIG: Damit der Worker weiß, ob er scannen darf!
                TotalIps = s.IpAddresses.Count(),
                UsedIps = s.IpAddresses.Count(ip => ip.AssignedAssetId != null || ip.IpStatus == "Used"),
                ReservedIps = s.IpAddresses.Count(ip => ip.IsReserved || ip.IpStatus == "Reserved"),
                FreeIps = s.IpAddresses.Count(ip => ip.AssignedAssetId == null && !ip.IsReserved && ip.IpStatus == "Free")
            })
            .OrderBy(s => s.VlanId).ThenBy(s => s.Name)
            .ToListAsync();

        return Ok(subnets);
    }

    // 2. Liefert ein einzelnes Subnetz für den Edit-Modus
    [HttpGet("subnets/{id}")]
    public async Task<ActionResult<Subnet>> GetSubnet(Guid id)
    {
        var subnet = await _context.Subnets.FindAsync(id);
        if (subnet == null) return NotFound();
        return subnet;
    }

    [HttpGet("check-duplicate")]
    public async Task<ActionResult<bool>> CheckDuplicate([FromQuery] string network, [FromQuery] int cidr, [FromQuery] Guid? locationId)
    {
        var exists = await _context.Subnets.AnyAsync(s => s.NetworkAddress == network && s.CidrMask == cidr && s.LocationId == locationId);
        return Ok(exists);
    }

    // 3. Liefert alle detaillierten IPs eines spezifischen Subnetzes
    [HttpGet("subnets/{subnetId}/ips")]
    public async Task<ActionResult<IEnumerable<IpAddress>>> GetIpsForSubnet(Guid subnetId)
    {
        var ips = await _context.IpAddresses
            .Include(ip => ip.AssignedAsset)
                .ThenInclude(a => a.AssetType)
            .Where(ip => ip.SubnetId == subnetId)
            .ToListAsync();

        return Ok(ips.OrderBy(ip => ParseIpToUint(ip.Address)));
    }

    // 4. Neues Subnetz anlegen UND IPs automatisch generieren
    [HttpPost("subnets")]
    public async Task<ActionResult> CreateSubnet(Subnet subnet)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant == null) return BadRequest("Kein Mandant gefunden.");

        subnet.Id = Guid.NewGuid();
        subnet.TenantId = tenant.Id;
        subnet.CreatedAt = DateTime.UtcNow;

        _context.Subnets.Add(subnet);

        if (IPAddress.TryParse(subnet.NetworkAddress, out var ipAddress))
        {
            var ipBytes = ipAddress.GetAddressBytes();
            uint ipUint = (uint)ipBytes[0] << 24 | (uint)ipBytes[1] << 16 | (uint)ipBytes[2] << 8 | (uint)ipBytes[3];
            uint mask = ~((1u << (32 - subnet.CidrMask)) - 1);
            uint networkUint = ipUint & mask;
            uint broadcastUint = networkUint | ~mask;

            if (subnet.CidrMask < 16) return BadRequest("Netzmaske zu groß (< /16). Blockgenerierung verweigert.");

            var newIps = new List<IpAddress>();
            for (uint i = networkUint + 1; i < broadcastUint; i++)
            {
                var hostIp = UintToIpString(i);
                bool isGateway = hostIp == subnet.Gateway;

                newIps.Add(new IpAddress
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenant.Id,
                    SubnetId = subnet.Id,
                    Address = hostIp,
                    IpStatus = isGateway ? "Reserved" : "Free",
                    IsReserved = isGateway,
                    Description = isGateway ? "Subnet Gateway" : ""
                });
            }

            _context.IpAddresses.AddRange(newIps);
        }
        else { return BadRequest("Ungültige Netzwerkadresse."); }

        await _context.SaveChangesAsync();

        // Initialer Background-Scan des neuen Subnetzes
        if (subnet.IsFullScanEnabled)
        {
            _ = Task.Run(async () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var bgContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                await Task.Delay(2000);

                var ipsToScan = await bgContext.IpAddresses.Where(ip => ip.SubnetId == subnet.Id).ToListAsync();
                var semaphore = new SemaphoreSlim(15);
                var results = new List<PingScanResultDto>();
                var tasks = new List<Task>();

                foreach (var ip in ipsToScan)
                {
                    tasks.Add(Task.Run(async () =>
                    {
                        await semaphore.WaitAsync();
                        try
                        {
                            using var ping = new Ping();
                            var reply = await ping.SendPingAsync(ip.Address, 2000);
                            if (reply.Status == IPStatus.Success)
                            {
                                lock (results) { results.Add(new PingScanResultDto { IpAddress = ip.Address, IsAlive = true, RoundtripTime = reply.RoundtripTime }); }
                            }
                        }
                        catch { }
                        finally { semaphore.Release(); }
                    }));
                }

                await Task.WhenAll(tasks);

                if (results.Any())
                {
                    var aliveIps = results.Where(r => r.IsAlive).ToDictionary(r => r.IpAddress);
                    foreach (var ip in ipsToScan)
                    {
                        if (aliveIps.TryGetValue(ip.Address, out var result))
                        {
                            ip.LastPingDate = DateTime.UtcNow;
                            if (ip.AssignedAssetId == null && !ip.IsReserved)
                            {
                                ip.RequiresManualReview = true;
                                ip.IpStatus = "Rogue";
                                ip.Description = $"Auto-Discovery (Init-Scan)! Latenz: {result.RoundtripTime}ms";
                            }
                        }
                    }
                    await bgContext.SaveChangesAsync();
                }
            });
        }

        return Ok(new { Message = "Subnetz und IP-Bereich erfolgreich generiert. Initialer Discovery-Scan läuft im Hintergrund!" });
    }

    // 5. Bestehendes Subnetz bearbeiten
    [HttpPut("subnets/{id}")]
    public async Task<IActionResult> UpdateSubnet(Guid id, Subnet subnet)
    {
        if (id != subnet.Id) return BadRequest("ID Mismatch");

        var existing = await _context.Subnets.FindAsync(id);
        if (existing == null) return NotFound("Subnetz nicht gefunden.");

        existing.Name = subnet.Name;
        existing.Gateway = subnet.Gateway;
        existing.VlanId = subnet.VlanId;
        existing.LocationId = subnet.LocationId;
        existing.IsDhcpManaged = subnet.IsDhcpManaged;
        existing.DhcpScopeStart = subnet.DhcpScopeStart;
        existing.DhcpScopeEnd = subnet.DhcpScopeEnd;
        existing.IsFullScanEnabled = subnet.IsFullScanEnabled;
        existing.DnsServerSecondary = subnet.DnsServerSecondary;
        existing.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrWhiteSpace(subnet.Gateway))
        {
            var gwIp = await _context.IpAddresses.FirstOrDefaultAsync(ip => ip.SubnetId == id && ip.Address == subnet.Gateway);
            if (gwIp != null)
            {
                gwIp.IsReserved = true;
                gwIp.IpStatus = "Reserved";
                gwIp.Description = "Subnet Gateway";
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 6. Subnetz löschen
    [HttpDelete("subnets/{id}")]
    public async Task<IActionResult> DeleteSubnet(Guid id)
    {
        var subnet = await _context.Subnets.Include(s => s.IpAddresses).FirstOrDefaultAsync(s => s.Id == id);
        if (subnet == null) return NotFound("Subnetz nicht gefunden.");

        _context.IpAddresses.RemoveRange(subnet.IpAddresses);
        _context.Subnets.Remove(subnet);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 7. Endpunkt für den Discovery-Worker (Ping-Ergebnisse & MAC-Adressen empfangen)
    [HttpPost("subnets/{subnetId}/scan-results")]
    public async Task<IActionResult> ProcessScanResults(Guid subnetId, [FromBody] List<PingScanResultDto> results)
    {
        var ips = await _context.IpAddresses.Where(ip => ip.SubnetId == subnetId).ToListAsync();
        var aliveIps = results.Where(r => r.IsAlive).ToDictionary(r => r.IpAddress);

        foreach (var ip in ips)
        {
            if (aliveIps.TryGetValue(ip.Address, out var result))
            {
                ip.LastPingDate = DateTime.UtcNow;

                // NEU: Automatische MAC-Adressen Pflege
                if (!string.IsNullOrEmpty(result.MacAddress))
                {
                    ip.MacAddress = result.MacAddress;
                }

                if (ip.AssignedAssetId == null && !ip.IsReserved)
                {
                    if (ip.IpStatus == "Free")
                    {
                        ip.RequiresManualReview = true;
                        ip.Description = $"Auto-Discovery (Worker)! Latenz: {result.RoundtripTime}ms";
                    }
                    ip.IpStatus = "Rogue";
                }
                else if (ip.AssignedAssetId != null)
                {
                    ip.IpStatus = "Used";
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    // 8. IP-Adresse manuell bestätigen
    [HttpPost("ips/{id}/confirm")]
    public async Task<IActionResult> ConfirmIp(Guid id)
    {
        var ip = await _context.IpAddresses.FindAsync(id);
        if (ip == null) return NotFound();

        ip.RequiresManualReview = false;
        ip.IpStatus = "Used";
        ip.Description = "Manuell geprüft " + (string.IsNullOrWhiteSpace(ip.Description) ? "" : $"({ip.Description})");

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // 9. IP-Adresse manuell bearbeiten
    [HttpPut("ips/{id}")]
    public async Task<IActionResult> UpdateIp(Guid id, [FromBody] IpUpdateDto dto)
    {
        var ip = await _context.IpAddresses.FindAsync(id);
        if (ip == null) return NotFound();

        ip.AssignedAssetId = dto.AssignedAssetId;
        ip.MacAddress = dto.MacAddress;
        ip.Description = dto.Description;

        if (ip.AssignedAssetId.HasValue)
        {
            ip.IpStatus = "Used";
            ip.RequiresManualReview = false;
        }
        else if (ip.IpStatus == "Used" || ip.IpStatus == "Rogue")
        {
            ip.IpStatus = ip.IsReserved ? "Reserved" : "Free";
            ip.RequiresManualReview = false;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Hilfsfunktionen
    private uint ParseIpToUint(string ipAddress)
    {
        if (IPAddress.TryParse(ipAddress, out var ip))
        {
            var bytes = ip.GetAddressBytes();
            return (uint)bytes[0] << 24 | (uint)bytes[1] << 16 | (uint)bytes[2] << 8 | (uint)bytes[3];
        }
        return 0;
    }

    private string UintToIpString(uint ip)
    {
        return $"{(ip >> 24) & 255}.{(ip >> 16) & 255}.{(ip >> 8) & 255}.{ip & 255}";
    }
}

// DTO Update: Enthält nun die MacAddress
public class PingScanResultDto
{
    public string IpAddress { get; set; } = string.Empty;
    public bool IsAlive { get; set; }
    public long RoundtripTime { get; set; }
    public string? MacAddress { get; set; }
}

public class IpUpdateDto
{
    public Guid? AssignedAssetId { get; set; }
    public string? MacAddress { get; set; }
    public string? Description { get; set; }
}