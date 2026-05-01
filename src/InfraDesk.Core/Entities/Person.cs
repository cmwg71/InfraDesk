// Dateipfad: src/InfraDesk.Core/Entities/Person.cs
using InfraDesk.Core.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class Person : BaseEntity
{
    [SetsRequiredMembers]
    public Person()
    {
        FirstName = null!;
        LastName = null!;
    }

    public Guid TenantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }

    // --- NEUE AUTHENTIFIZIERUNGS-FELDER ---

    // Gespeichertes Passwort (niemals Klartext!)
    public string? PasswordHash { get; set; }

    // RBAC Rolle (z.B. "Global Admin", "CI Editor")
    public string? SystemRole { get; set; }

    // JSON-Liste der Mandanten, auf die dieser User zugreifen darf
    public string? AllowedTenantsJson { get; set; }
}