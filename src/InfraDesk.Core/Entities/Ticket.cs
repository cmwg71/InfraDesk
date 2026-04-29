using InfraDesk.Core.Common;
namespace InfraDesk.Core.Entities;

public class Ticket : BaseEntity
{
    public Guid TenantId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "Open";
    public string Priority { get; set; } = "Medium";

    public Guid? RequesterId { get; set; }
    public Person? Requester { get; set; }

    public Guid? AssignedAssetId { get; set; }
    public Asset? AssignedAsset { get; set; }
}