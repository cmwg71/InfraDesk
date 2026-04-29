using Microsoft.EntityFrameworkCore;
using InfraDesk.Core.Entities;

namespace InfraDesk.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Software> Softwares { get; set; }
    public DbSet<License> Licenses { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. JSONB Konfiguration für PostgreSQL
        modelBuilder.Entity<Asset>().Property(b => b.DynamicDataJson).HasColumnType("jsonb");

        // 2. Beziehung auflösen: Person <-> Team
        // Wir definieren hier zwei separate 1:n Beziehungen, um die 1:1 Mehrdeutigkeit zu vermeiden.

        modelBuilder.Entity<Person>()
            .HasOne(p => p.Team)
            .WithMany()
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Team>()
            .HasOne(t => t.Lead)
            .WithMany()
            .HasForeignKey(t => t.LeadId)
            .OnDelete(DeleteBehavior.Restrict);

        // --- MEGA SEED DATA ---

        // Mandant
        var tenantId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        modelBuilder.Entity<Tenant>().HasData(
            new Tenant { Id = tenantId, Name = "Musterfirma GmbH", Domain = "musterfirma.de" }
        );

        // Standorte
        var locBerlin = Guid.NewGuid();
        var locServerRoom = Guid.NewGuid();
        modelBuilder.Entity<Location>().HasData(
            new Location { Id = locBerlin, TenantId = tenantId, Name = "Hauptsitz Berlin", Address = "Musterstraße 1" },
            new Location { Id = locServerRoom, TenantId = tenantId, Name = "Serverraum 01", RoomNumber = "UG-01", ParentLocationId = locBerlin }
        );

        // IDs für Team und Admin vorab festlegen
        var teamItId = Guid.NewGuid();
        var adminId = Guid.NewGuid();

        // 3. Team SEED
        // WICHTIG: Wir lassen LeadId hier leer (null), um den zirkulären Bezug beim Seeding zu brechen.
        // In einer echten App würde der Leiter später über die UI zugewiesen.
        modelBuilder.Entity<Team>().HasData(
            new Team
            {
                Id = teamItId,
                TenantId = tenantId,
                Name = "IT-Administration",
                LeadId = null
            }
        );

        // 4. Person SEED
        modelBuilder.Entity<Person>().HasData(
            new Person
            {
                Id = adminId,
                TenantId = tenantId,
                FirstName = "Max",
                LastName = "Admin",
                Email = "m.admin@musterfirma.de",
                TeamId = teamItId
            }
        );

        // 5. Hersteller & Typen
        var dellId = Guid.NewGuid();
        var typeServer = Guid.NewGuid();
        var typeLaptop = Guid.NewGuid();
        modelBuilder.Entity<Manufacturer>().HasData(
            new Manufacturer { Id = dellId, Name = "Dell Technologies" }
        );
        modelBuilder.Entity<AssetType>().HasData(
            new AssetType { Id = typeServer, Name = "Server", IconKey = "ServerIcon" },
            new AssetType { Id = typeLaptop, Name = "Laptop", IconKey = "LaptopIcon" }
        );

        // 6. Assets
        var assetId = Guid.NewGuid();
        modelBuilder.Entity<Asset>().HasData(
            new Asset
            {
                Id = assetId,
                TenantId = tenantId,
                Name = "SRV-DC-01",
                SerialNumber = "DELL-12345",
                InventoryNumber = "INV-0001",
                AssetTypeId = typeServer,
                ManufacturerId = dellId,
                LocationId = locServerRoom,
                OwnerId = adminId,
                DynamicDataJson = "{\"CPU\": \"32 Cores\", \"RAM\": \"128GB\", \"OS\": \"Windows Server 2022\"}"
            }
        );

        // 7. Tickets
        modelBuilder.Entity<Ticket>().HasData(
            new Ticket
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Title = "Festplattentausch erforderlich",
                Status = "Open",
                Priority = "High",
                RequesterId = adminId,
                AssignedAssetId = assetId,
                Description = "Der Server meldet einen defekten Sektor auf Disk 0."
            }
        );
    }
}