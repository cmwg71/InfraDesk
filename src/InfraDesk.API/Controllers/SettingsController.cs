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

    public SettingsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/settings/asset-lifecycle
    [HttpGet("asset-lifecycle")]
    public async Task<ActionResult<Dictionary<string, List<string>>>> GetAssetLifecycle()
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == "AssetLifecycle");

        if (setting == null)
        {
            // Standardwerte zurückgeben, wenn noch nichts in der DB steht
            return Ok(GetDefaultLifecycle());
        }

        try
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(setting.Value);
            return Ok(data);
        }
        catch
        {
            return BadRequest("Fehler beim Verarbeiten der Einstellungsdaten.");
        }
    }

    // POST: api/settings/asset-lifecycle
    // Damit kannst du später die Liste über ein Admin-Interface speichern
    [HttpPost("asset-lifecycle")]
    public async Task<IActionResult> SaveAssetLifecycle([FromBody] Dictionary<string, List<string>> config)
    {
        var json = JsonSerializer.Serialize(config);
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == "AssetLifecycle");

        if (setting == null)
        {
            _context.SystemSettings.Add(new SystemSetting { Key = "AssetLifecycle", Value = json });
        }
        else
        {
            setting.Value = json;
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    private Dictionary<string, List<string>> GetDefaultLifecycle()
    {
        return new Dictionary<string, List<string>>
        {
            { "1. Planung & Design", new() { "Bedarf gemeldet", "Genehmigt", "Geplant", "In Bestellung" } },
            { "2. Beschaffung & Vorbereitung", new() { "Erhalten", "Lagernd", "In Vorbereitung", "In Prüfung / QS" } },
            { "3. Implementierung", new() { "In Installation", "Reserviert", "In Rollout" } },
            { "4. Betriebsphase", new() { "Produktiv", "In Wartung", "Gestört", "In Reparatur", "Nicht Verfügbar", "Leihgabe" } },
            { "5. Außerbetriebnahme", new() { "Abkündigungsphase", "In Außerbetriebnahme", "Ausgemustert", "Archiviert", "Entsorgt", "Verkauft", "Verloren / Gestohlen" } }
        };
    }
}