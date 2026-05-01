// Dateipfad: src/InfraDesk.API/Controllers/AssetsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAssets()
    {
        return await _context.Assets
            .Include(a => a.AssetType)
            .Include(a => a.Location)
            .Include(a => a.Manufacturer)
            .Include(a => a.Owner)
            .Include(a => a.NetworkInterfaces)
                .ThenInclude(nic => nic.Subnet)
            .OrderByDescending(a => a.Id)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Asset>> GetAsset(Guid id)
    {
        var asset = await _context.Assets
            .Include(a => a.AssetType)
            .Include(a => a.Manufacturer)
            .Include(a => a.Location)
            .Include(a => a.Owner)
            .Include(a => a.NetworkInterfaces)
                .ThenInclude(nic => nic.Subnet)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null) return NotFound();
        return asset;
    }

    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<AssetType>>> GetAssetTypes()
    {
        return await _context.AssetTypes.OrderBy(t => t.Name).ToListAsync();
    }

    // NEU: Automatischer Generator für garantiert eindeutige Asset Tags
    [HttpGet("generate-tag")]
    public async Task<ActionResult<string>> GenerateTag([FromQuery] string company, [FromQuery] Guid? locationId, [FromQuery] Guid? typeId)
    {
        // 1. Firmenkürzel (Max 3 Buchstaben)
        string comp = string.IsNullOrEmpty(company) ? "GIT" : company.Substring(0, Math.Min(3, company.Length)).ToUpper();

        // 2. Standortkürzel (Max 3 Buchstaben)
        string loc = "GLO"; // Global
        if (locationId.HasValue)
        {
            var location = await _context.Locations.FindAsync(locationId);
            if (location != null && location.Name.Length >= 3)
                loc = location.Name.Substring(0, 3).ToUpper();
        }

        // 3. Kategoriekürzel (Max 3 Buchstaben)
        string cat = "GEN"; // General
        if (typeId.HasValue)
        {
            var type = await _context.AssetTypes.FindAsync(typeId);
            if (type != null && type.Name.Length >= 3)
                cat = type.Name.Substring(0, 3).ToUpper();
        }

        string prefix = $"{comp}-{loc}-{cat}-"; // z.B. GIT-FRA-LAP-

        // 4. Garantiert eindeutige Nummer ermitteln (Datenbank-Lock Logik simuliert)
        var lastAsset = await _context.Assets
            .Where(a => a.AssetTag.StartsWith(prefix))
            .OrderByDescending(a => a.AssetTag)
            .FirstOrDefaultAsync();

        int nextNum = 1;
        if (lastAsset != null)
        {
            var parts = lastAsset.AssetTag.Split('-');
            if (parts.Length == 4 && int.TryParse(parts[3], out int parsedNum))
            {
                nextNum = parsedNum + 1;
            }
        }

        // Ergebnis z.B.: GIT-FRA-LAP-00001
        return Ok(new { Tag = $"{prefix}{nextNum:D5}" });
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> PostAsset(Asset asset)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant == null) return BadRequest("Kein aktiver Mandant gefunden.");

        asset.TenantId = tenant.Id;
        if (asset.Id == Guid.Empty) asset.Id = Guid.NewGuid();

        asset.AssetType = null;
        asset.Manufacturer = null;
        asset.Location = null;
        asset.Owner = null;

        // Sicherheitsprüfung: Ist der Tag wirklich eindeutig?
        if (await _context.Assets.AnyAsync(a => a.AssetTag == asset.AssetTag))
        {
            return BadRequest("Kollision: Dieses Asset-Tag existiert bereits. Bitte Formular aktualisieren.");
        }

        if (asset.NetworkInterfaces != null && asset.NetworkInterfaces.Any())
        {
            foreach (var nic in asset.NetworkInterfaces)
            {
                if (nic.Id == Guid.Empty) nic.Id = Guid.NewGuid();
                nic.TenantId = tenant.Id;
                nic.AssignedAssetId = asset.Id;
                nic.Subnet = null;
            }
        }

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsset(Guid id, Asset asset)
    {
        if (id != asset.Id) return BadRequest("ID Mismatch");

        asset.AssetType = null;
        asset.Manufacturer = null;
        asset.Location = null;
        asset.Owner = null;

        _context.Entry(asset).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Assets.Any(e => e.Id == id)) return NotFound();
            else throw;
        }

        return NoContent();
    }
}