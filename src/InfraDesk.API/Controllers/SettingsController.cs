// Dateipfad: src/InfraDesk.API/Controllers/SettingsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;
using InfraDesk.Core.Common;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("timezones")]
    public ActionResult<IEnumerable<string>> GetTimeZones()
    {
        return Ok(TimeZoneInfo.GetSystemTimeZones().Select(z => z.Id).OrderBy(id => id).ToList());
    }

    // FIX: Fehlender Endpunkt, der den 404-Fehler und Absturz verursacht hat.
    // WICHTIG: Gibt application/json zurück, damit Blazor es direkt verarbeiten kann.
    [HttpGet("dynamic/{key}")]
    public async Task<IActionResult> GetGenericSetting(string key)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(x => x.Key == key);
        return Content(setting?.Value ?? "{}", "application/json");
    }

    // FIX: Endpunkt zum Speichern der dynamischen Listen (Status, Prioritäten, Vorlagen)
    [HttpPost("dynamic/{key}")]
    public async Task<ActionResult> SaveGenericSetting(string key, [FromBody] JsonElement value)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(x => x.Key == key);
        if (setting == null)
        {
            _context.SystemSettings.Add(new SystemSetting { Id = Guid.NewGuid(), Key = key, Value = value.GetRawText() });
        }
        else
        {
            setting.Value = value.GetRawText();
        }
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("masterdata")]
    public async Task<ActionResult<MasterDataDto>> GetMasterData()
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "MasterData");
        var dto = new MasterDataDto();

        if (setting != null)
        {
            try
            {
                // Versuche, die Settings zu laden.
                dto = JsonSerializer.Deserialize<MasterDataDto>(setting.Value) ?? new MasterDataDto();
            }
            catch
            {
                // Fallback: Falls sich die Struktur in der Datenbank geändert hat (z.B. alter String vs. neue List<T>),
                // fangen wir den Fehler ab und nutzen die Standardwerte aus dem neuen Dto.
            }
        }

        var admin = await _context.Persons.FirstOrDefaultAsync(p => p.SystemRole == "Global Admin");
        if (admin != null)
        {
            dto.AdminFirstName = admin.FirstName;
            dto.AdminLastName = admin.LastName;
            dto.AdminEmail = admin.Email ?? "";
        }
        return Ok(dto);
    }

    [HttpPost("masterdata")]
    public async Task<ActionResult> SaveMasterData([FromBody] MasterDataDto dto)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "MasterData");

        var jsonDto = new MasterDataDto
        {
            CompanyName = dto.CompanyName,
            CompanyPrefix = dto.CompanyPrefix,
            Domain = dto.Domain,
            Street = dto.Street,
            ZipCity = dto.ZipCity,
            State = dto.State,
            Country = dto.Country,
            TimeZoneId = dto.TimeZoneId,
            AssetTagPattern = dto.AssetTagPattern,
            BarcodeType = dto.BarcodeType,
            ShowTextBelowBarcode = dto.ShowTextBelowBarcode,
            QrErrorCorrection = dto.QrErrorCorrection,
            LabelWidth = dto.LabelWidth,
            LabelHeight = dto.LabelHeight,
            LabelTemplate = dto.LabelTemplate,
            IncludeLogoInQr = dto.IncludeLogoInQr,
            PrintAssetName = dto.PrintAssetName,
            PrintCompanyName = dto.PrintCompanyName,
            PrintSerialNumber = dto.PrintSerialNumber,
            PrintResolutionDpi = dto.PrintResolutionDpi,
            TicketDefaultFilter = dto.TicketDefaultFilter,
            TicketShowClosed = dto.TicketShowClosed,
            TicketStatusList = dto.TicketStatusList, // Nun strukturierte Listen (Name, Farbe, Icon)
            TicketPriorityList = dto.TicketPriorityList
        };

        string jsonString = JsonSerializer.Serialize(jsonDto);
        if (setting == null)
            _context.SystemSettings.Add(new SystemSetting { Id = Guid.NewGuid(), Key = "MasterData", Value = jsonString });
        else
            setting.Value = jsonString;

        // Admin-Account aktualisieren
        var admin = await _context.Persons.FirstOrDefaultAsync(p => p.SystemRole == "Global Admin");
        if (admin != null)
        {
            admin.FirstName = dto.AdminFirstName; admin.LastName = dto.AdminLastName; admin.Email = dto.AdminEmail;
            if (!string.IsNullOrWhiteSpace(dto.AdminPassword)) admin.PasswordHash = SecurityHelper.HashPassword(dto.AdminPassword);
        }

        // Setup-Status updaten
        var setupCompleteSetting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "IsSetupComplete");
        if (setupCompleteSetting != null) setupCompleteSetting.Value = "true";

        var initPw = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == "InitialSetup_Password");
        if (initPw != null) _context.SystemSettings.Remove(initPw);

        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("assettag-schema")]
    public async Task<ActionResult<object>> GetAssetTagSchema()
    {
        var s = await _context.SystemSettings.FirstOrDefaultAsync(x => x.Key == "MasterData");
        var dto = s != null ? JsonSerializer.Deserialize<MasterDataDto>(s.Value) ?? new MasterDataDto() : new MasterDataDto();
        return Ok(new { companyPrefix = dto.CompanyPrefix });
    }
}

// --- NEUE STRUKTUREN FÜR FARBEN & ICONS ---
public class TicketStatusItem
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#757575";
    public string Icon { get; set; } = "Adjust";
}

public class TicketPriorityItem
{
    public string Name { get; set; } = "";
    public string Color { get; set; } = "#333333";
    public string Icon { get; set; } = "Flag";
}

public class MasterDataDto
{
    public string CompanyName { get; set; } = "";
    public string CompanyPrefix { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Street { get; set; } = "";
    public string ZipCity { get; set; } = "";
    public string State { get; set; } = "";
    public string Country { get; set; } = "";
    public string TimeZoneId { get; set; } = "W. Europe Standard Time";
    public string AssetTagPattern { get; set; } = "{COMP}-{LOC}-{CAT}-{NUM:5}";
    public string BarcodeType { get; set; } = "Code128";
    public bool ShowTextBelowBarcode { get; set; } = true;
    public string QrErrorCorrection { get; set; } = "M";
    public int LabelWidth { get; set; } = 50;
    public int LabelHeight { get; set; } = 30;
    public string LabelTemplate { get; set; } = "Custom";
    public bool IncludeLogoInQr { get; set; } = false;
    public bool PrintAssetName { get; set; } = true;
    public bool PrintCompanyName { get; set; } = true;
    public bool PrintSerialNumber { get; set; } = false;
    public int PrintResolutionDpi { get; set; } = 300;
    public string AdminFirstName { get; set; } = "System";
    public string AdminLastName { get; set; } = "Administrator";
    public string AdminEmail { get; set; } = "admin@infradesk.local";
    public string AdminPassword { get; set; } = "";

    // Ticket Settings
    public string TicketDefaultFilter { get; set; } = "Alle";
    public bool TicketShowClosed { get; set; } = false;

    // Ersetzt die alten "OFFEN,AKTIV"-Strings durch strukturierte Listen
    public List<TicketStatusItem> TicketStatusList { get; set; } = new()
    {
        new TicketStatusItem { Name = "OFFEN", Color = "#2E7D32", Icon = "Adjust" },
        new TicketStatusItem { Name = "BEANTWORTET", Color = "#1976D2", Icon = "Reply" },
        new TicketStatusItem { Name = "AKTIV", Color = "#C62828", Icon = "PlayArrow" },
        new TicketStatusItem { Name = "GESCHLOSSEN", Color = "#757575", Icon = "CheckCircle" }
    };

    public List<TicketPriorityItem> TicketPriorityList { get; set; } = new()
    {
        new TicketPriorityItem { Name = "Niedrig", Color = "#757575", Icon = "ArrowDownward" },
        new TicketPriorityItem { Name = "Normal", Color = "#1976D2", Icon = "Remove" },
        new TicketPriorityItem { Name = "Hoch", Color = "#C62828", Icon = "ArrowUpward" },
        new TicketPriorityItem { Name = "Kritisch", Color = "#B71C1C", Icon = "Warning" }
    };
}