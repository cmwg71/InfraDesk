using System;                            // Für Guid
using InfraDesk.Core.Common;             // Für BaseEntity (da diese im Unterordner 'Common' liegt)
// Hinweis: 'AssetType' und 'Manufacturer' brauchen kein 'using', 
// wenn sie im selben Namespace 'InfraDesk.Core.Entities' liegen.

namespace InfraDesk.Core.Entities;

public class Asset : BaseEntity
{
    public required string Name { get; set; }
    public string? SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }

    // Verknüpfungen
    public Guid AssetTypeId { get; set; }
    public AssetType? AssetType { get; set; } // Jetzt bekannt, da AssetType.cs existiert

    public Guid? ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }

    public string DynamicDataJson { get; set; } = "{}";
}