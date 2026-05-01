// Dateipfad: src/InfraDesk.Core/Entities/IpAddress.cs
using InfraDesk.Core.Common;
using System;
using System.Diagnostics.CodeAnalysis;

namespace InfraDesk.Core.Entities;

public class IpAddress : BaseEntity
{
    [SetsRequiredMembers]
    public IpAddress()
    {
        Address = null!;
    }

    public Guid TenantId { get; set; }

    // Die eigentliche IP, z.B. "10.10.20.15"
    public required string Address { get; set; }

    public string? MacAddress { get; set; }
    public string? Description { get; set; }
    public bool IsReserved { get; set; }

    // --- NEU FÜR IPAM V2 ---
    public string IpStatus { get; set; } = "Free"; // Free, Used, Reserved, Rogue
    public DateTime? LastPingDate { get; set; }
    public string? DnsHostname { get; set; }

    // --- NEU: Auto-Discovery Bestätigung ---
    public bool RequiresManualReview { get; set; }

    // Verknüpfung zum übergeordneten IPAM-Subnetz
    public Guid SubnetId { get; set; }
    public Subnet? Subnet { get; set; }

    // DIE WICHTIGE VERKNÜPFUNG: Gehört diese IP zu einem Asset?
    public Guid? AssignedAssetId { get; set; }
    public Asset? AssignedAsset { get; set; }
}