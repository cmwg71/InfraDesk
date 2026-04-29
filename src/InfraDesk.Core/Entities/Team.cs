using InfraDesk.Core.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class Team : BaseEntity
{
    [SetsRequiredMembers]
    public Team() 
    { 
        // Der Null-Forgiving-Operator (!null) sagt dem Compiler, 
        // dass er hier keine CS8618-Warnung werfen soll.
        Name = null!; 
    } 

    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public Guid? LeadId { get; set; }
    public Person? Lead { get; set; }
}