// Dateipfad: src/InfraDesk.Infrastructure/Persistence/ApplicationDbContext.cs
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
    public DbSet<Subnet> Subnets { get; set; }
    public DbSet<IpAddress> IpAddresses { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. PostgreSQL JSONB Konfiguration
        modelBuilder.Entity<Asset>().Property(b => b.DynamicDataJson).HasColumnType("jsonb");

        // 2. Stammdaten-Beziehungen
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

        // 3. IPAM Beziehungen
        modelBuilder.Entity<IpAddress>()
            .HasOne(i => i.AssignedAsset)
            .WithMany(a => a.NetworkInterfaces)
            .HasForeignKey(i => i.AssignedAssetId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<IpAddress>()
            .HasOne(i => i.Subnet)
            .WithMany(s => s.IpAddresses)
            .HasForeignKey(i => i.SubnetId)
            .OnDelete(DeleteBehavior.Cascade);

        // 4. Systemeinstellungen (Eindeutiger Key für schnellen Zugriff)
        modelBuilder.Entity<SystemSetting>()
            .HasIndex(s => s.Key)
            .IsUnique();
    }
}