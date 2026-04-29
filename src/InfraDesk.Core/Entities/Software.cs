using InfraDesk.Core.Common;
namespace InfraDesk.Core.Entities;

public class Software : BaseEntity
{
    public Guid TenantId { get; set; }
    public required string Name { get; set; }
    public string? Version { get; set; }
    public Guid? ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }
}

public class License : BaseEntity
{
    public Guid TenantId { get; set; }
    public required string LicenseKey { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Quantity { get; set; }
    public Guid SoftwareId { get; set; }
    public Software? Software { get; set; }
}