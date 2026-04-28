using InfraDesk.Core.Common;

namespace InfraDesk.Core.Entities;

public class AssetType : BaseEntity
{
    public required string Name { get; set; } // z.B. "Server", "Laptop", "Switch"
    public string? Description { get; set; }
    public string? IconKey { get; set; } // Für die Anzeige in der UI
}