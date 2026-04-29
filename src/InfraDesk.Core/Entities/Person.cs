using InfraDesk.Core.Common;
namespace InfraDesk.Core.Entities;

public class Person : BaseEntity
{
    public Guid TenantId { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public string? Department { get; set; }
    public Guid? TeamId { get; set; }
    public Team? Team { get; set; }
}