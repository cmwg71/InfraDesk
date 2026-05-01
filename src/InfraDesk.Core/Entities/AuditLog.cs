// Dateipfad: src/InfraDesk.Core/Entities/AuditLog.cs
using InfraDesk.Core.Common;
using System;

namespace InfraDesk.Core.Entities;

/// <summary>
/// Revisionssichere Protokollierung aller Änderungen (WORM-Prinzip).
/// Entspricht der Anforderung T1_12.
/// </summary>
public class AuditLog : BaseEntity
{
    public Guid? UserId { get; set; }
    public Guid? TenantId { get; set; }

    // Name der Tabelle/Entität (z.B. "Asset")
    public string EntityName { get; set; } = string.Empty;

    // Primärschlüssel des betroffenen Datensatzes
    public string EntityId { get; set; } = string.Empty;

    // Aktion: "Create", "Update", "Delete"
    public string Action { get; set; } = string.Empty;

    // Alter vs. Neuer Wert als JSON
    public string ChangesJson { get; set; } = "{}";

    public DateTime Timestamp { get; set; } = DateTime.Now;
}