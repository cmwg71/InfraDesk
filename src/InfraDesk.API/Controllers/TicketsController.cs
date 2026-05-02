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
            .Include(t => t.ChildTickets)
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
            .Include(t => t.Watchers)
            .Include(t => t.AssignedAssets)
            .Include(t => t.ChildTickets)
            .AsSplitQuery()
            .FirstOrDefaultAsync(t => t.Id == id);

        if (ticket == null) return NotFound();

        ticket.Activities = await _context.TicketActivities
            .Where(a => a.TicketId == id)
            .OrderBy(a => a.Timestamp)
            .ToListAsync();

        return ticket;
    }

    // NEU: Liefert die neusten echten Journal-Einträge für das Dashboard
    [HttpGet("recent-activities")]
    public async Task<ActionResult<IEnumerable<object>>> GetRecentActivities()
    {
        var activities = await _context.TicketActivities
            .OrderByDescending(a => a.Timestamp)
            .Where(a => a.IsPublic) // Nur öffentliche/sichtbare für das Dashboard
            .Take(15)
            .Join(_context.Tickets, a => a.TicketId, t => t.Id, (a, t) => new {
                TicketId = a.TicketId,
                TicketNumber = t.TicketNumber,
                TicketTitle = t.Title,
                AuthorName = a.AuthorName,
                Content = a.Content,
                Timestamp = a.Timestamp,
                Type = a.Type
            })
            .ToListAsync();

        return Ok(activities);
    }

    [HttpPost]
    public async Task<ActionResult<Ticket>> PostTicket(Ticket ticket)
    {
        var tenant = await _context.Tenants.FirstOrDefaultAsync();
        if (tenant == null) return BadRequest("Kein aktiver Mandant gefunden.");

        ticket.TenantId = tenant.Id;
        if (ticket.Id == Guid.Empty) ticket.Id = Guid.NewGuid();

        var maxTicketNumber = await _context.Tickets.MaxAsync(t => (long?)t.TicketNumber) ?? 1000;
        ticket.TicketNumber = maxTicketNumber + 1;
        ticket.CreatedAt = DateTime.UtcNow;

        // Beziehungen entkoppeln, um PK-Konflikte zu vermeiden
        ticket.Requester = null;
        ticket.Supporter = null;
        ticket.Approver = null;

        var assignedAssets = new List<Asset>();
        if (ticket.AssignedAssets != null)
        {
            foreach (var a in ticket.AssignedAssets)
            {
                var trackedAsset = await _context.Assets.FindAsync(a.Id);
                if (trackedAsset != null) assignedAssets.Add(trackedAsset);
            }
        }
        ticket.AssignedAssets = assignedAssets;

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTicket), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutTicket(Guid id, Ticket ticket)
    {
        if (id != ticket.Id) return BadRequest();

        var existingTicket = await _context.Tickets
            .Include(t => t.Watchers)
            .Include(t => t.AssignedAssets)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTicket == null) return NotFound();

        // Stammdaten aktualisieren
        existingTicket.Title = ticket.Title;
        existingTicket.Description = ticket.Description;
        existingTicket.Status = ticket.Status;
        existingTicket.Priority = ticket.Priority;
        existingTicket.Category1 = ticket.Category1;
        existingTicket.Category2 = ticket.Category2;
        existingTicket.Category3 = ticket.Category3;
        existingTicket.Tags = ticket.Tags;
        existingTicket.RequesterId = ticket.RequesterId;
        existingTicket.SupporterId = ticket.SupporterId;
        existingTicket.ApproverId = ticket.ApproverId;
        existingTicket.ApprovalStatus = ticket.ApprovalStatus;
        existingTicket.EscalationLevel = ticket.EscalationLevel;
        existingTicket.SpentMinutes = ticket.SpentMinutes;
        existingTicket.EstimatedMinutes = ticket.EstimatedMinutes;
        existingTicket.DueAt = ticket.DueAt;
        existingTicket.MasterTicketId = ticket.MasterTicketId;
        existingTicket.IsInternal = ticket.IsInternal;

        // Beziehungen synchronisieren
        existingTicket.Watchers.Clear();
        if (ticket.Watchers != null)
        {
            foreach (var w in ticket.Watchers)
            {
                var person = await _context.Persons.FindAsync(w.Id);
                if (person != null) existingTicket.Watchers.Add(person);
            }
        }

        existingTicket.AssignedAssets.Clear();
        if (ticket.AssignedAssets != null)
        {
            foreach (var a in ticket.AssignedAssets)
            {
                var asset = await _context.Assets.FindAsync(a.Id);
                if (asset != null) existingTicket.AssignedAssets.Add(asset);
            }
        }

        // NEUE Aktivitäten (Journal) anfügen
        if (ticket.Activities != null)
        {
            foreach (var newAct in ticket.Activities.Where(a => a.Id != Guid.Empty && !_context.TicketActivities.Any(dbA => dbA.Id == a.Id)))
            {
                newAct.TicketId = existingTicket.Id;
                _context.TicketActivities.Add(newAct);
            }
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTicket(Guid id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        _context.Tickets.Remove(ticket);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}