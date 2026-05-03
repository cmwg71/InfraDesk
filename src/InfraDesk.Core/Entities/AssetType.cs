// Dateipfad: src/InfraDesk.Core/Entities/AssetType.cs
using InfraDesk.Core.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class AssetType : BaseEntity
{
    [SetsRequiredMembers]
    public AssetType()
    {
        Name = null!;
    }

    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? IconKey { get; set; }

    // NEU: Ermöglicht die n-stufige Baumstruktur für Kategorien
    public Guid? ParentAssetTypeId { get; set; }
    public AssetType? ParentAssetType { get; set; }
}