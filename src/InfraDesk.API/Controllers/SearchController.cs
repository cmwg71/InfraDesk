// Dateipfad: src/InfraDesk.API/Controllers/SearchController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchResultDto>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2) return Ok(new List<SearchResultDto>());

        var results = new List<SearchResultDto>();
        var query = q.ToLower();

        // 1. ASSETS: Suche in Name, Tag, S/N, Status, Typ, Location, Hersteller, BESITZER
        var assets = await _context.Assets
            .Include(a => a.AssetType)
            .Include(a => a.Location)
            .Include(a => a.Manufacturer)
            .Include(a => a.Owner) // WICHTIG: Owner laden, sonst wird z.B. "Christian" nie gefunden
            .Where(a =>
                a.Name.ToLower().Contains(query) ||
                (a.AssetTag != null && a.AssetTag.ToLower().Contains(query)) ||
                (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(query)) ||
                (a.LifecycleStatus != null && a.LifecycleStatus.ToLower().Contains(query)) ||
                (a.AssetType != null && a.AssetType.Name.ToLower().Contains(query)) ||
                (a.Location != null && a.Location.Name.ToLower().Contains(query)) ||
                (a.Manufacturer != null && a.Manufacturer.Name.ToLower().Contains(query)) ||
                (a.Owner != null && (a.Owner.FirstName.ToLower().Contains(query) || a.Owner.LastName.ToLower().Contains(query)))
            // HINWEIS: Die direkte .ToLower() Suche auf das DynamicDataJson-Feld (PostgreSQL JSONB) 
            // wurde entfernt, da dies eine Translation-Exception im EF Core Provider auslöst.
            )
            .Take(25)
            .ToListAsync();

        results.AddRange(assets.Select(a => new SearchResultDto
        {
            Id = a.Id,
            Type = "Asset",
            Title = a.Name,
            Subtitle = $"Tag: {a.AssetTag} | S/N: {a.SerialNumber} | Typ: {a.AssetType?.Name} | Besitzer: {a.Owner?.FirstName} {a.Owner?.LastName}",
            Url = $"/assets/{a.Id}",
            Icon = "Computer",
            Status = a.LifecycleStatus ?? "Unbekannt"
        }));

        // 2. TICKETS: Suche in Titel, Beschr., Status, Priorität, Kategorien, Kontakt
        var tickets = await _context.Tickets
            .Include(t => t.Requester)
            .Where(t =>
                t.Title.ToLower().Contains(query) ||
                t.TicketNumber.ToString().Contains(query) ||
                (t.Description != null && t.Description.ToLower().Contains(query)) ||
                (t.Status != null && t.Status.ToLower().Contains(query)) ||
                (t.Priority != null && t.Priority.ToLower().Contains(query)) ||
                (t.Category1 != null && t.Category1.ToLower().Contains(query)) ||
                (t.Tags != null && t.Tags.ToLower().Contains(query)) ||
                (t.Requester != null && (t.Requester.FirstName.ToLower().Contains(query) || t.Requester.LastName.ToLower().Contains(query)))
            )
            .Take(25)
            .ToListAsync();

        results.AddRange(tickets.Select(t => new SearchResultDto
        {
            Id = t.Id,
            Type = "Ticket",
            Title = $"#{t.TicketNumber} - {t.Title}",
            Subtitle = $"Kontakt: {t.Requester?.FirstName} {t.Requester?.LastName} | Prio: {t.Priority}",
            Url = $"/tickets/{t.Id}",
            Icon = "ConfirmationNumber",
            Status = t.Status ?? "Unbekannt"
        }));

        // 3. IPAM / IPs: Suche in Adressen, MAC, DNS, Beschreibungen, Subnet-Namen
        var ips = await _context.IpAddresses
            .Include(ip => ip.Subnet)
            .Include(ip => ip.AssignedAsset)
            .Where(ip =>
                ip.Address.Contains(query) ||
                (ip.MacAddress != null && ip.MacAddress.ToLower().Contains(query)) ||
                (ip.Description != null && ip.Description.ToLower().Contains(query)) ||
                (ip.DnsHostname != null && ip.DnsHostname.ToLower().Contains(query)) ||
                (ip.IpStatus != null && ip.IpStatus.ToLower().Contains(query)) ||
                (ip.Subnet != null && ip.Subnet.Name.ToLower().Contains(query)) ||
                (ip.AssignedAsset != null && ip.AssignedAsset.Name.ToLower().Contains(query))
            )
            .Take(15)
            .ToListAsync();

        results.AddRange(ips.Select(ip => new SearchResultDto
        {
            Id = ip.Id,
            Type = "IP-Adresse",
            Title = ip.Address,
            Subtitle = $"MAC: {ip.MacAddress ?? "N/A"} | Netz: {ip.Subnet?.Name} {(ip.AssignedAsset != null ? $"| Asset: {ip.AssignedAsset.Name}" : "")}",
            Url = $"/ipam",
            Icon = "SettingsEthernet",
            Status = ip.IpStatus ?? "Unbekannt"
        }));

        // Sortiert die Treffer logisch nach Kategorien
        return Ok(results.OrderBy(r => r.Type).ToList());
    }
}

public class SearchResultDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = "";
    public string Title { get; set; } = "";
    public string Subtitle { get; set; } = "";
    public string Url { get; set; } = "";
    public string Icon { get; set; } = "";
    public string Status { get; set; } = "";
}