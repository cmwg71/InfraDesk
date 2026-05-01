// Dateipfad: src/InfraDesk.API/Controllers/TicketsController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using InfraDesk.Core.Entities;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TicketsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetTickets()
    {
        return await _context.Tickets
            .Include(t => t.Requester)
            .Include(t => t.Supporter)
            .Include(t => t.ChildTickets) // WICHTIG: Erforderlich für das Master-Icon in der Liste!
            .OrderByDescending(t => t.TicketNumber)
            .AsSplitQuery()
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Ticket>> GetTicket(Guid id)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Requester)
            .Include(t => t.Supporter)
            .Include(t => t.Approver)
            .Include(t => t.AssignedAssets)
            .Include(t => t.Activities)
            .Include(t => t.Watchers)
            .Include(t => t.ChildTickets) // Lade Sub-Tickets
            .AsSplitQuery() // Behebt die EF Core Warnung bei mehreren Listen-Includes
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();
        return ticket;
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant == null) return BadRequest("Kein aktiver Mandant gefunden.");

        if (ticket.Id == Guid.Empty) ticket.Id = Guid.NewGuid();
        if (ticket.TenantId == Guid.Empty) ticket.TenantId = tenant.Id;

        // Automatische, fortlaufende Ticketnummer generieren
        long maxNumber = await _context.Tickets.MaxAsync(t => (long?)t.TicketNumber) ?? 500;
        ticket.TicketNumber = maxNumber + 1;
        ticket.CreatedAt = DateTime.UtcNow;

        // Skalare Navigation-Properties nullen, um Tracking-Probleme beim Insert zu vermeiden
        ticket.Requester = null;
        ticket.Supporter = null;
        ticket.Approver = null;

        // Verknüpfte Assets sauber an den Context hängen
        if (ticket.AssignedAssets != null && ticket.AssignedAssets.Any())
        {
            var assetIds = ticket.AssignedAssets.Select(a => a.Id).ToList();
            ticket.AssignedAssets = await _context.Assets.Where(a => assetIds.Contains(a.Id)).ToListAsync();
        }

        // Watchers sauber an den Context hängen
        if (ticket.Watchers != null && ticket.Watchers.Any())
        {
            var watcherIds = ticket.Watchers.Select(w => w.Id).ToList();
            ticket.Watchers = await _context.Persons.Where(p => watcherIds.Contains(p.Id)).ToListAsync();
        }

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTicket(Guid id, Ticket ticket)
    {
        if (id != ticket.Id) return BadRequest("ID Mismatch");

        // Ticket laden OHNE Activities! Verhindert Concurrency Errors.
        var existingTicket = await _context.Tickets
            .Include(t => t.Watchers)
            .Include(t => t.AssignedAssets)
            .Include(t => t.ChildTickets)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTicket == null) return NotFound("Ticket nicht gefunden");

        string oldStatus = existingTicket.Status;

        _context.Entry(existingTicket).CurrentValues.SetValues(ticket);

        bool statusChanged = oldStatus != existingTicket.Status;
        var newActivitiesForCascade = new List<TicketActivity>();

        if (ticket.Activities != null && ticket.Activities.Any())
        {
            var incomingIds = ticket.Activities.Select(a => a.Id).ToList();

            var existingIds = await _context.TicketActivities
                .Where(a => incomingIds.Contains(a.Id))
                .Select(a => a.Id)
                .ToListAsync();

            var newActivities = ticket.Activities.Where(a => !existingIds.Contains(a.Id)).ToList();

            foreach (var newAct in newActivities)
            {
                newAct.TicketId = existingTicket.Id;
                _context.TicketActivities.Add(newAct);
                newActivitiesForCascade.Add(newAct);
            }
        }

        // --- KASKADIERUNGS-LOGIK FÜR MASTER-TICKET ---
        if (existingTicket.ChildTickets != null && existingTicket.ChildTickets.Any())
        {
            foreach (var child in existingTicket.ChildTickets)
            {
                if (statusChanged && (existingTicket.Status == "GESCHLOSSEN" || existingTicket.Status == "AKTIV"))
                {
                    child.Status = existingTicket.Status;
                    _context.TicketActivities.Add(new TicketActivity
                    {
                        Id = Guid.NewGuid(),
                        TicketId = child.Id,
                        AuthorName = "System",
                        Content = $"Status wurde automatisch durch das Master-Ticket (#{existingTicket.TicketNumber}) auf '{existingTicket.Status}' gesetzt.",
                        Type = "System",
                        IsPublic = true
                    });
                }

                foreach (var act in newActivitiesForCascade.Where(a => a.IsPublic && a.Type == "Comment"))
                {
                    _context.TicketActivities.Add(new TicketActivity
                    {
                        Id = Guid.NewGuid(),
                        TicketId = child.Id,
                        AuthorName = act.AuthorName,
                        Content = $"<strong>[Update via Master-Ticket #{existingTicket.TicketNumber}]</strong><br/>" + act.Content,
                        Type = "Comment",
                        IsPublic = true,
                        IsStaffAction = act.IsStaffAction
                    });
                }
            }
        }

        existingTicket.Watchers.Clear();
        if (ticket.Watchers != null && ticket.Watchers.Any())
        {
            var watcherIds = ticket.Watchers.Select(w => w.Id).ToList();
            var persons = await _context.Persons.Where(p => watcherIds.Contains(p.Id)).ToListAsync();
            foreach (var p in persons) existingTicket.Watchers.Add(p);
        }

        existingTicket.AssignedAssets.Clear();
        if (ticket.AssignedAssets != null && ticket.AssignedAssets.Any())
        {
            var assetIds = ticket.AssignedAssets.Select(a => a.Id).ToList();
            var assetsToAssign = await _context.Assets.Where(a => assetIds.Contains(a.Id)).ToListAsync();
            foreach (var a in assetsToAssign) existingTicket.AssignedAssets.Add(a);
        }

        try { await _context.SaveChangesAsync(); }
        catch (DbUpdateException ex) { return StatusCode(500, $"Datenbankfehler beim Speichern: {ex.InnerException?.Message ?? ex.Message}"); }

        return NoContent();
    }
}