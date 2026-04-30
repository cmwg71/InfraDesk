// Dateipfad: src/InfraDesk.Core/Entities/Subnet.cs
using InfraDesk.Core.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace InfraDesk.Core.Entities;

public class Subnet : BaseEntity
{
    [SetsRequiredMembers]
    public Subnet()
    {
        Name = null!;
        NetworkAddress = null!;
    }

    public Guid TenantId { get; set; }

    // z.B. "Server-Netz RZ1"
    public required string Name { get; set; }

    // z.B. "10.10.20.0"
    public required string NetworkAddress { get; set; }

    // z.B. 24 (für /24)
    public int CidrMask { get; set; }

    // z.B. "10.10.20.1"
    public string? Gateway { get; set; }

    public int? VlanId { get; set; }

    // Alle IPs, die zu diesem Subnetz gehören
    public ICollection<IpAddress> IpAddresses { get; set; } = new List<IpAddress>();
}