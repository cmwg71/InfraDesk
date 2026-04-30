// Dateipfad: src/InfraDesk.Core/Entities/Asset.cs
using InfraDesk.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class Asset : BaseEntity
{
    [SetsRequiredMembers]
    public Asset()
    {
        Name = null!;
    }

    public Guid TenantId { get; set; }

    public required string Name { get; set; }

    // NEU: Eindeutiger Asset-Tag für die physische Kennzeichnung (Barcode/QR)
    public string? AssetTag { get; set; }

    public string? SerialNumber { get; set; }
    public string? InventoryNumber { get; set; }

    public string LifecycleStatus { get; set; } = "Produktiv";

    public Guid AssetTypeId { get; set; }
    public AssetType? AssetType { get; set; }

    public Guid? ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }

    public Guid? LocationId { get; set; }
    public Location? Location { get; set; }

    public Guid? OwnerId { get; set; }
    public Person? Owner { get; set; }

    public string DynamicDataJson { get; set; } = "{}";

    public ICollection<IpAddress> NetworkInterfaces { get; set; } = new List<IpAddress>();
}