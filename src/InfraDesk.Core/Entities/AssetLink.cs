// Dateipfad: src/InfraDesk.Core/Entities/AssetLink.cs
using InfraDesk.Core.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

/// <summary>
/// Bildet die CMDB-Beziehungen zwischen Assets ab (z.B. VM läuft auf Host, Switch verbunden mit Router).
/// Dient als Basis für den späteren Impact-Graphen.
/// </summary>
public class AssetLink : BaseEntity
{
    [SetsRequiredMembers]
    public AssetLink()
    {
        LinkType = null!;
    }

    public Guid TenantId { get; set; }

    // Das Quell-Objekt der Beziehung (z.B. VM)
    public Guid SourceAssetId { get; set; }
    public Asset? SourceAsset { get; set; }

    // Das Ziel-Objekt der Beziehung (z.B. Host-Server)
    public Guid TargetAssetId { get; set; }
    public Asset? TargetAsset { get; set; }

    // Art der Beziehung (z.B. "Läuft auf", "Verbunden mit", "Mit Strom versorgt von")
    public required string LinkType { get; set; }
}