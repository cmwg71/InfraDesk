using InfraDesk.Core.Common;
namespace InfraDesk.Core.Entities;

public class Location : BaseEntity
{
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public string? Address { get; set; }
    public string? RoomNumber { get; set; }
    public Guid? ParentLocationId { get; set; }
    public Location? ParentLocation { get; set; }
}