// Dateipfad: src/InfraDesk.Core/Entities/SystemSetting.cs
using InfraDesk.Core.Common;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class SystemSetting : BaseEntity
{
    [SetsRequiredMembers]
    public SystemSetting()
    {
        Key = null!;
        Value = null!;
    }

    // Eindeutiger Schlüssel, z.B. "AssetLifecycle"
    public required string Key { get; set; }

    // Der Wert als JSON-String
    public required string Value { get; set; }

    public string? Description { get; set; }
}