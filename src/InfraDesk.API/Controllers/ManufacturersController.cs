// Dateipfad: src/InfraDesk.API/Controllers/ManufacturersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ManufacturersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ManufacturersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Manufacturer>>> GetManufacturers()
    {
        return await _context.Manufacturers.OrderBy(m => m.Name).ToListAsync();
    }
}