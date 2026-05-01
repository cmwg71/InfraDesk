// Dateipfad: src/InfraDesk.API/Controllers/SettingsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;
using System.Text.Json;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    private readonly Dictionary<string, List<string>> _defaultLifecycle = new()
    {
        { "Planung", new List<string> { "In Evaluierung", "Budgetiert", "Bestellt" } },
        { "Lager", new List<string> { "Auf Lager", "Ersatzteil", "Defekt" } },
        { "Produktiv", new List<string> { "Aktiv", "Wartung", "Gesperrt" } },
        { "End of Life", new List<string> { "Ausgemustert", "Verkauft", "Verschrottet" } }
    };

    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("asset-lifecycle")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> GetLifecycle()
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "AssetLifecycle");
        if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
        {
            try
            {
                var config = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(setting.Value);
                if (config != null) return Ok(config);
            }
            catch { }
        }
        return Ok(_defaultLifecycle);
    }

    [HttpPost("asset-lifecycle")]
    public async Task<ActionResult> SaveLifecycle([FromBody] Dictionary<string, List<string>> config)
    {
        var json = JsonSerializer.Serialize(config);
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "AssetLifecycle");

        if (setting == null) _context.SystemSettings.Add(new SystemSetting { Id = Guid.NewGuid(), Key = "AssetLifecycle", Value = json });
        else setting.Value = json;

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("masterdata")]
    public async Task<ActionResult<MasterDataDto>> GetMasterData()
    {
        var dto = new MasterDataDto();

        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant != null)
        {
            dto.CompanyName = tenant.Name;
            dto.Domain = tenant.Domain ?? "";
        }

        var prefixSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "CompanyPrefix");
        dto.CompanyPrefix = prefixSetting?.Value ?? "";

        // Zeitzone laden
        var tzSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "SystemTimeZone");
        dto.TimeZoneId = tzSetting?.Value ?? "W. Europe Standard Time";

        var addressSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "HQAddress");
        if (addressSetting != null && !string.IsNullOrWhiteSpace(addressSetting.Value))
        {
            try
            {
                using var doc = JsonDocument.Parse(addressSetting.Value);
                var root = doc.RootElement;
                if (root.TryGetProperty("Street", out var st)) dto.Street = st.GetString() ?? "";
                if (root.TryGetProperty("ZipCity", out var zc)) dto.ZipCity = zc.GetString() ?? "";
                if (root.TryGetProperty("State", out var stt)) dto.State = stt.GetString() ?? "";
                if (root.TryGetProperty("Country", out var c)) dto.Country = c.GetString() ?? "";
            }
            catch { }
        }

        return Ok(dto);
    }

    [HttpPost("masterdata")]
    public async Task<ActionResult> SaveMasterData([FromBody] MasterDataDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.CompanyName) || string.IsNullOrWhiteSpace(dto.CompanyPrefix) || string.IsNullOrWhiteSpace(dto.ZipCity))
        {
            return BadRequest("Firmenname, Kürzel und PLZ/Ort sind Pflichtfelder.");
        }

        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant == null)
        {
            tenant = new Tenant { Id = Guid.NewGuid(), Name = dto.CompanyName, Domain = dto.Domain };
            _context.Tenants.Add(tenant);
        }
        else
        {
            tenant.Name = dto.CompanyName;
            tenant.Domain = dto.Domain;
        }

        // Zeitzone speichern
        var tzSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "SystemTimeZone");
        if (tzSetting == null) _context.SystemSettings.Add(new SystemSetting { Id = Guid.NewGuid(), Key = "SystemTimeZone", Value = dto.TimeZoneId });
        else tzSetting.Value = dto.TimeZoneId;

        string cleanPrefix = dto.CompanyPrefix.Substring(0, Math.Min(3, dto.CompanyPrefix.Length)).ToUpper();
        var prefixSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "CompanyPrefix");
        if (prefixSetting == null) _context.SystemSettings.Add(new SystemSetting { Id = Guid.NewGuid(), Key = "CompanyPrefix", Value = cleanPrefix });
        else prefixSetting.Value = cleanPrefix;

        var addressJson = JsonSerializer.Serialize(new { dto.Street, dto.ZipCity, dto.State, dto.Country });
        var addrSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "HQAddress");
        if (addrSetting == null) _context.SystemSettings.Add(new SystemSetting { Id = Guid.NewGuid(), Key = "HQAddress", Value = addressJson });
        else addrSetting.Value = addressJson;

        var hqName = $"HQ {dto.ZipCity.Split(' ').LastOrDefault()}".Trim();
        var hqAddressString = $"{dto.Street}, {dto.ZipCity}, {dto.State}, {dto.Country}".Trim().Trim(',').Trim();

        var hqLoc = await _context.Locations.FirstOrDefaultAsync(l => l.Name.StartsWith("HQ") || l.Name.Contains("Zentrale"));
        if (hqLoc == null)
        {
            _context.Locations.Add(new Location { Id = Guid.NewGuid(), TenantId = tenant.Id, Name = hqName, Address = hqAddressString });
        }
        else
        {
            hqLoc.Name = hqName;
            hqLoc.Address = hqAddressString;
        }

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("assettag-schema")]
    public async Task<ActionResult<object>> GetAssetTagSchema()
    {
        var prefixSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "CompanyPrefix");
        string prefix = prefixSetting != null ? prefixSetting.Value : "";

        return Ok(new
        {
            CompanyPrefix = prefix,
            FormatInfo = "{Company(3)} - {Standort(3)} - {Kategorie(3)} - {Zähler(5)}"
        });
    }

    [HttpGet("timezones")]
    public ActionResult<IEnumerable<string>> GetTimeZones()
    {
        return Ok(TimeZoneInfo.GetSystemTimeZones().Select(z => z.Id).OrderBy(id => id).ToList());
    }
}

public class MasterDataDto
{
    public string CompanyName { get; set; } = "";
    public string Domain { get; set; } = "";
    public string CompanyPrefix { get; set; } = "";
    public string Country { get; set; } = "";
    public string State { get; set; } = "";
    public string ZipCity { get; set; } = "";
    public string Street { get; set; } = "";
    public string TimeZoneId { get; set; } = "W. Europe Standard Time";

    public bool IsSetupComplete => !string.IsNullOrWhiteSpace(CompanyName)
                                && !string.IsNullOrWhiteSpace(CompanyPrefix)
                                && !string.IsNullOrWhiteSpace(ZipCity);
}