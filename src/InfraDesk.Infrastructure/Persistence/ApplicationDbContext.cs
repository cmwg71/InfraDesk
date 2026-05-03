// Dateipfad: src/InfraDesk.Infrastructure/Persistence/ApplicationDbContext.cs
using Microsoft.EntityFrameworkCore;
using InfraDesk.Core.Entities;
using System;

namespace InfraDesk.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    private readonly Guid _currentTenantId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        _currentTenantId = Guid.Empty;
    }

    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<SystemSetting> SystemSettings { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Software> Softwares { get; set; }
    public DbSet<License> Licenses { get; set; }
    public DbSet<Subnet> Subnets { get; set; }
    public DbSet<IpAddress> IpAddresses { get; set; }
    public DbSet<Person> Persons { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketActivity> TicketActivities { get; set; }
    public DbSet<AssetLink> AssetLinks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Asset>().Property(b => b.DynamicDataJson).HasColumnType("jsonb");

        // Ticket Relationen
        modelBuilder.Entity<Ticket>().HasOne(t => t.Requester).WithMany().HasForeignKey(t => t.RequesterId);
        modelBuilder.Entity<Ticket>().HasOne(t => t.Supporter).WithMany().HasForeignKey(t => t.SupporterId);
        modelBuilder.Entity<Ticket>().HasOne(t => t.Approver).WithMany().HasForeignKey(t => t.ApproverId);
        modelBuilder.Entity<Ticket>().HasMany(t => t.Activities).WithOne().HasForeignKey(a => a.TicketId).OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.MasterTicket)
            .WithMany(t => t.ChildTickets)
            .HasForeignKey(t => t.MasterTicketId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Ticket>().HasMany(t => t.Watchers).WithMany();
        modelBuilder.Entity<Ticket>().HasMany(t => t.AssignedAssets).WithMany();

        modelBuilder.Entity<Person>().HasOne(p => p.Team).WithMany().HasForeignKey(p => p.TeamId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Team>().HasOne(t => t.Lead).WithMany().HasForeignKey(t => t.LeadId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AssetLink>()
            .HasOne(al => al.SourceAsset)
            .WithMany()
            .HasForeignKey(al => al.SourceAssetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssetLink>()
            .HasOne(al => al.TargetAsset)
            .WithMany()
            .HasForeignKey(al => al.TargetAssetId)
            .OnDelete(DeleteBehavior.Cascade);

        // NEU: Hierarchie-Relation für AssetTypes
        modelBuilder.Entity<AssetType>()
            .HasOne(at => at.ParentAssetType)
            .WithMany()
            .HasForeignKey(at => at.ParentAssetTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Globale Mandanten-Filter
        if (_currentTenantId != Guid.Empty)
        {
            modelBuilder.Entity<Asset>().HasQueryFilter(a => a.TenantId == _currentTenantId);
            modelBuilder.Entity<Ticket>().HasQueryFilter(t => t.TenantId == _currentTenantId);
            modelBuilder.Entity<Person>().HasQueryFilter(p => p.TenantId == _currentTenantId);
            modelBuilder.Entity<AssetLink>().HasQueryFilter(al => al.TenantId == _currentTenantId);
        }

        modelBuilder.Entity<Asset>().HasIndex(a => a.AssetTag).IsUnique();
        modelBuilder.Entity<SystemSetting>().HasIndex(s => s.Key).IsUnique();
    }
}