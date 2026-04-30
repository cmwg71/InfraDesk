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

    // GET: api/assets
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

    // GET: api/assets/{id}
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

    // GET: api/assets/types
    // NEU: Wird für den Wizard (Schritt 1) benötigt, um alle Vorlagen zu laden.
    [HttpGet("types")]
    public async Task<ActionResult<IEnumerable<AssetType>>> GetAssetTypes()
    {
        return await _context.AssetTypes
            .OrderBy(t => t.Name)
            .ToListAsync();
    }

    // POST: api/assets
    // NEU: Speichert das im Wizard erstellte Asset inkl. der IPAM-Schnittstellen in der Datenbank.
    [HttpPost]
    public async Task<ActionResult<Asset>> PostAsset(Asset asset)
    {
        // Für diese Demo weisen wir dem neuen Asset den ersten (bzw. einzigen) Mandanten zu.
        // In einem Produktionssystem kommt diese ID aus dem JWT-Token des eingeloggten Users.
        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant == null) return BadRequest("Kein aktiver Mandant gefunden.");

        asset.TenantId = tenant.Id;
        if (asset.Id == Guid.Empty) asset.Id = Guid.NewGuid();

        // WICHTIG für EF Core: Navigations-Eigenschaften auf null setzen, 
        // da wir nur die IDs übergeben und nicht versehentlich neue Hersteller/Typen anlegen wollen.
        asset.AssetType = null;
        asset.Manufacturer = null;
        asset.Location = null;
        asset.Owner = null;

        // Netzwerkschnittstellen (IPAM) korrekt verknüpfen
        if (asset.NetworkInterfaces != null && asset.NetworkInterfaces.Any())
        {
            foreach (var nic in asset.NetworkInterfaces)
            {
                if (nic.Id == Guid.Empty) nic.Id = Guid.NewGuid();
                nic.TenantId = tenant.Id;
                nic.AssignedAssetId = asset.Id;
                nic.Subnet = null; // Verhindert EF-Core Update-Schleifen beim Subnetz
            }
        }

        _context.Assets.Add(asset);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAsset), new { id = asset.Id }, asset);
    }

    // PUT: api/assets/{id}
    // NEU: Speichert Aktualisierungen (aus der Detailansicht heraus)
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsset(Guid id, Asset asset)
    {
        if (id != asset.Id) return BadRequest("ID Mismatch");

        // Navigations-Eigenschaften nullen, um Trackings-Konflikte im EF Core zu vermeiden
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
            if (!AssetExists(id)) return NotFound();
            else throw;
        }

        return NoContent();
    }

    // GET: api/assets/{id}/links
    // Dient als Dummy/Platzhalter, damit die Detailansicht keine 404 Fehler wirft.
    [HttpGet("{id}/links")]
    public async Task<ActionResult<IEnumerable<object>>> GetAssetLinks(Guid id)
    {
        // Später wird dies aus einer AssetLinks-Tabelle gelesen (T1_03 CMDB-Beziehungen).
        var dummyLinks = new List<object>();
        return Ok(dummyLinks);
    }

    private bool AssetExists(Guid id)
    {
        return _context.Assets.Any(e => e.Id == id);
    }
}