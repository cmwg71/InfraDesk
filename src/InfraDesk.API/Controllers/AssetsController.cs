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
            .Include(a => a.Owner)
            // NEU: Lade alle zugewiesenen Netzwerkkarten inkl. deren Subnetz direkt mit!
            .Include(a => a.NetworkInterfaces)
                .ThenInclude(nic => nic.Subnet)
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
            // NEU: Auch in der Detailansicht laden
            .Include(a => a.NetworkInterfaces)
                .ThenInclude(nic => nic.Subnet)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (asset == null) return NotFound();
        return asset;
    }
}