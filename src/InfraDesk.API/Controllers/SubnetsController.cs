// Dateipfad: src/InfraDesk.API/Controllers/SubnetsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubnetsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SubnetsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/subnets
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Subnet>>> GetSubnets()
    {
        // Liefert alle verfügbaren Subnetze für das IPAM-Modul im Wizard zurück
        return await _context.Subnets
            .OrderBy(s => s.NetworkAddress)
            .ToListAsync();
    }
}