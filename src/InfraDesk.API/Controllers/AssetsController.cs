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
            .Include(a => a.Manufacturer)
            .Include(a => a.Location)
            .Include(a => a.Owner)
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
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null) return NotFound();
        return asset;
    }

    // NEU: Endpunkt für Asset-Beziehungen (Links)
    [HttpGet("{id}/links")]
    public async Task<ActionResult<IEnumerable<object>>> GetAssetLinks(Guid id)
    {
        // Basierend auf T1_03 (CMDB Relationships)
        // Vorläufige Mock-Daten für die Verknüpfungs-Ansicht
        var links = new List<object>
        {
            new { type = "Läuft auf", targetName = "VM-Host-Cluster-01", targetId = Guid.NewGuid() },
            new { type = "Verbunden mit", targetName = "Core-Switch-Stack", targetId = Guid.NewGuid() },
            new { type = "Backup-Ziel", targetName = "NAS-Backup-Vault", targetId = Guid.NewGuid() }
        };

        return Ok(links);
    }

    [HttpPost]
    public async Task<ActionResult<Asset>> PostAsset(Asset asset)
    {
        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsset(Guid id, Asset asset)
    {
        if (id != asset.Id) return BadRequest();
        _context.Entry(asset).State = EntityState.Modified;
        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Assets.Any(e => e.Id == id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsset(Guid id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset == null) return NotFound();
        _context.Assets.Remove(asset);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}