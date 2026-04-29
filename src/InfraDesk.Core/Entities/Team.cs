using InfraDesk.Core.Common;
namespace InfraDesk.Core.Entities;

public class Team : BaseEntity
{
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public Guid? LeadId { get; set; }
    public Person? Lead { get; set; }
}
