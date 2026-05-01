// Dateipfad: src/InfraDesk.Core/Entities/Ticket.cs
using InfraDesk.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class Ticket : BaseEntity
{
    [SetsRequiredMembers]
    public Ticket()
    {
        Title = null!;
    }

    public Guid TenantId { get; set; }
    public long TicketNumber { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public string Status { get; set; } = "OFFEN";
    public string Priority { get; set; } = "Normal";
    public bool IsInternal { get; set; }
    public bool CreatedViaEmail { get; set; }

    // --- Zeitmanagement & Termine ---
    public DateTime? DueAt { get; set; }
    public DateTime? DeferredUntil { get; set; }
    public DateTime? ReminderDate { get; set; }
    public string? ReminderNote { get; set; }

    // --- Aufwand (in Minuten) ---
    public int EstimatedMinutes { get; set; }
    public int SpentMinutes { get; set; }

    // --- Klassifizierung (ITIL & CMDB) ---
    public string? Category1 { get; set; }
    public string? Category2 { get; set; }
    public string? Category3 { get; set; }
    public int EscalationLevel { get; set; } = 1;

    public string? Tags { get; set; }

    // --- NEU: Master- / Sub-Ticket Logik ---
    public Guid? MasterTicketId { get; set; }
    public Ticket? MasterTicket { get; set; }
    public ICollection<Ticket> ChildTickets { get; set; } = new List<Ticket>();

    // --- Verknüpfungen & Rollen ---
    public Guid? RequesterId { get; set; }
    public Person? Requester { get; set; }

    public Guid? SupporterId { get; set; }
    public Person? Supporter { get; set; }

    public Guid? ApproverId { get; set; }
    public Person? Approver { get; set; }
    public string? ApprovalStatus { get; set; }

    // Interessierter Kreis (CC / Watcher)
    public ICollection<Person> Watchers { get; set; } = new List<Person>();

    // Mehrere verknüpfte Assets pro Ticket
    public ICollection<Asset> AssignedAssets { get; set; } = new List<Asset>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TicketActivity> Activities { get; set; } = new List<TicketActivity>();
}

public class TicketActivity : BaseEntity
{
    public Guid TicketId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string Type { get; set; } = "Comment";
    public bool IsPublic { get; set; } = true;
    public bool IsStaffAction { get; set; }
    public int ActivitySpentMinutes { get; set; }
}