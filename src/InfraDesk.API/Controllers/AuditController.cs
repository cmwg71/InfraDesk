using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AuditController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/audit/asset/{id}
    [HttpGet("asset/{id}")]
    public async Task<ActionResult<IEnumerable<object>>> GetAssetAuditLogs(Guid id)
    {
        // Da die AuditLogs-Tabelle (T1_12) im Fundament liegt, 
        // liefern wir hier vorerst strukturierte Daten für die UI-Entwicklung.
        // Später wird dies eine Abfrage auf _context.AuditLogs sein.

        var logs = new List<object>
        {
            new {
                timestamp = DateTime.Now.AddDays(-1).AddHours(-2),
                user = "Discovery-Worker",
                action = "Automatischer Scan",
                details = "CPU-Kerne von 16 auf 32 aktualisiert."
            },
            new {
                timestamp = DateTime.Now.AddDays(-3),
                user = "Max Admin",
                action = "Stammdaten-Änderung",
                details = "Standort von 'Lager' zu 'Serverraum 01' verschoben."
            },
            new {
                timestamp = DateTime.Now.AddDays(-10),
                user = "System",
                action = "Asset angelegt",
                details = "Initialer Import via CSV-Assistent."
            }
        };

        return Ok(logs);
    }
}