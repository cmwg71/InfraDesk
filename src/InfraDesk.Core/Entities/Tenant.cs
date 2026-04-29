using InfraDesk.Core.Common;

namespace InfraDesk.Core.Entities;

public class Tenant : BaseEntity
{
    public required string Name { get; set; }
    public string? Domain { get; set; }
}
