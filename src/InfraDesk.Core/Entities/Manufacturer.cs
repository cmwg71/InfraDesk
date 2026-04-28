using InfraDesk.Core.Common;

namespace InfraDesk.Core.Entities;

public class Manufacturer : BaseEntity
{
    public required string Name { get; set; }
    public string? SupportContact { get; set; }
}