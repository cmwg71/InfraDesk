// Dateipfad: src/InfraDesk.API/Controllers/AssetTypesController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/assets/types")]
public class AssetTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AssetTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AssetType>>> GetTypes()
    {
        return await _context.AssetTypes.OrderBy(a => a.Name).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<AssetType>> PostType(AssetType assetType)
    {
        if (assetType.Id == Guid.Empty) assetType.Id = Guid.NewGuid();
        assetType.CreatedAt = DateTime.UtcNow;

        _context.AssetTypes.Add(assetType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTypes), new { id = assetType.Id }, assetType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutType(Guid id, AssetType assetType)
    {
        if (id != assetType.Id) return BadRequest("ID stimmt nicht überein.");

        var existing = await _context.AssetTypes.FindAsync(id);
        if (existing == null) return NotFound();

        existing.Name = assetType.Name;
        existing.Description = assetType.Description;
        existing.IconKey = assetType.IconKey;
        existing.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteType(Guid id)
    {
        var assetType = await _context.AssetTypes.FindAsync(id);
        if (assetType == null) return NotFound();

        // Schutz: Keine Löschung, wenn noch Assets in dieser Kategorie existieren!
        bool isUsed = await _context.Assets.AnyAsync(a => a.AssetTypeId == id);
        if (isUsed)
        {
            return BadRequest("Diese Kategorie kann nicht gelöscht werden, da ihr noch Geräte/Assets zugewiesen sind.");
        }

        _context.AssetTypes.Remove(assetType);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}