using Microsoft.EntityFrameworkCore;
using InfraDesk.Core.Entities;

namespace InfraDesk.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Das sind die "Ankerpunkte" für die Datenbank. 
    // Jedes DbSet hier wird später eine Tabelle in PostgreSQL.
    public DbSet<Asset> Assets { get; set; }
    public DbSet<AssetType> AssetTypes { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Spezielle Konfiguration für PostgreSQL JSONB (die flexiblen Felder)
        modelBuilder.Entity<Asset>()
            .Property(b => b.DynamicDataJson)
            .HasColumnType("jsonb");

        // Optional: Namen von Asset-Typen (z.B. "Server") eindeutig machen
        modelBuilder.Entity<AssetType>()
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}