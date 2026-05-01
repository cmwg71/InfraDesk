// Dateipfad: src/InfraDesk.API/Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Common;
using System.Threading.Tasks;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuthController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest("E-Mail und Passwort sind erforderlich.");

        // Suche den Benutzer anhand der E-Mail in der DB
        var user = await _context.Persons.FirstOrDefaultAsync(p => p.Email == request.Email);

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            return Unauthorized("Ungültige E-Mail oder Passwort.");

        // Hash-Vergleich
        if (!SecurityHelper.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized("Ungültige E-Mail oder Passwort.");

        // Erfolgreicher Login: Rückgabe der Claims an das Blazor-Frontend
        return Ok(new
        {
            Success = true,
            Email = user.Email,
            Name = $"{user.FirstName} {user.LastName}",
            Role = user.SystemRole ?? "Reader",
            AllowedTenants = user.AllowedTenantsJson ?? "[]"
        });
    }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}