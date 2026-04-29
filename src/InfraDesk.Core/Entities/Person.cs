using InfraDesk.Core.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class Person : BaseEntity
{
    [SetsRequiredMembers]
    public Person() 
    { 
        // Person hat zwei erforderliche Felder!
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
}