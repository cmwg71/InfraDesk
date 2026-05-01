// Dateipfad: src/InfraDesk.Infrastructure/Persistence/DbSeeder.cs
using InfraDesk.Core.Entities;
using InfraDesk.Core.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace InfraDesk.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Falls bereits Daten vorhanden sind, Seed überspringen
        if (await context.Assets.AnyAsync()) return;

        var rng = new Random();

        // --- 1. MANDANT (TENANT) ---
        var tenantId = Guid.NewGuid();
        var tenant = new Tenant { Id = tenantId, Name = "Grams IT Global Enterprise", Domain = "grams-it.com" };
        context.Tenants.Add(tenant);

        // --- 2. INITIALER SYSTEM-ADMINISTRATOR ---
        var adminId = Guid.NewGuid();
        var admin = new Person
        {
            Id = adminId,
            TenantId = tenantId,
            FirstName = "System",
            LastName = "Administrator",
            Email = "admin@infradesk.local",
            SystemRole = "Global Admin",
            PasswordHash = SecurityHelper.HashPassword("Admin123!"),
            AllowedTenantsJson = JsonSerializer.Serialize(new[] { new { Id = tenantId, Name = tenant.Name } })
        };
        context.Persons.Add(admin);

        // --- 3. ANONYMISIERTER PERSONEN POOL (Zufallsnamen) ---
        var firstNames = new[] {
            "Michael", "Thomas", "Andreas", "Stefan", "Christian", "Martin", "Frank", "Markus",
            "Maria", "Sabine", "Petra", "Monika", "Barbara", "Gabriele", "Susanne", "Ursula",
            "Jan", "Erik", "Lukas", "Leon", "Sarah", "Laura", "Julia", "Lea"
        };
        var lastNames = new[] {
            "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker",
            "Schulz", "Hoffmann", "Schäfer", "Koch", "Bauer", "Richter", "Klein", "Wolf",
            "Schröder", "Neumann", "Schwarz", "Zimmermann", "Braun", "Krüger", "Hofmann"
        };

        var personPool = new List<Person>();
        // Erstelle 25 zufällige Mitarbeiter
        for (int i = 0; i < 25; i++)
        {
            var first = firstNames[rng.Next(firstNames.Length)];
            var last = lastNames[rng.Next(lastNames.Length)];

            // Verhindere exakte Duplikate im Seed durch Index-Anhang falls nötig
            var p = new Person
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FirstName = first,
                LastName = last,
                Email = $"{first.ToLower()}.{last.ToLower()}.{i}@grams-it.com",
                SystemRole = "CI Editor"
            };
            context.Persons.Add(p);
            personPool.Add(p);
        }

        // --- 4. STANDORTE ---
        var locRZ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "RZ-FRA-01 (Tier 4)", Address = "Hanauer Landstraße 300" };
        var locHQ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "HQ Frankfurt", Address = "MainTower 1" };
        context.Locations.AddRange(locRZ, locHQ);

        // --- 5. IPAM SUBNETZE ---
        var subMgmt = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "OOB-Management", NetworkAddress = "10.0.0.0", CidrMask = 24, Gateway = "10.0.0.1", VlanId = 10 };
        var subProd = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Prod-Server", NetworkAddress = "10.0.20.0", CidrMask = 24, Gateway = "10.0.20.1", VlanId = 20 };
        var subClient = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Client-WiFi", NetworkAddress = "10.0.100.0", CidrMask = 22, Gateway = "10.0.100.1", VlanId = 100 };
        context.Subnets.AddRange(subMgmt, subProd, subClient);

        // --- 6. HERSTELLER ---
        var manufacturers = new List<Manufacturer> {
            new() { Id = Guid.NewGuid(), Name = "Dell Technologies" },
            new() { Id = Guid.NewGuid(), Name = "Cisco Systems" },
            new() { Id = Guid.NewGuid(), Name = "Hewlett Packard Enterprise" },
            new() { Id = Guid.NewGuid(), Name = "Apple" },
            new() { Id = Guid.NewGuid(), Name = "Microsoft" },
            new() { Id = Guid.NewGuid(), Name = "VMware" },
            new() { Id = Guid.NewGuid(), Name = "Palo Alto Networks" }
        };
        context.Manufacturers.AddRange(manufacturers);

        // --- 7. ASSET TYPEN ---
        var typeMap = new Dictionary<string, Guid>();
        string[] allTypeNames = {
            "Laptop", "Desktop", "Workstation", "Tablet", "Smartphone",
            "Rack-Server", "Blade-Center", "Mainframe", "Backup-Appliance",
            "Rack", "USV-Anlage", "SAN-Storage", "NAS-System",
            "Core-Switch", "Access-Switch", "Router", "Firewall",
            "Cloud-Instanz", "Betriebssystem", "Datenbank-Instanz"
        };

        foreach (var name in allTypeNames)
        {
            var id = Guid.NewGuid();
            context.AssetTypes.Add(new AssetType { Id = id, Name = name, IconKey = name });
            typeMap.Add(name, id);
        }
        await context.SaveChangesAsync();

        var assetsPool = new List<Asset>();
        var ipAddresses = new List<IpAddress>();

        // --- 8. MASSEN-SEEDING CMDB ---
        foreach (var typeName in allTypeNames)
        {
            int count = rng.Next(2, 6);
            for (int i = 1; i <= count; i++)
            {
                var assetId = Guid.NewGuid();
                var assetOwner = personPool[rng.Next(personPool.Count)];
                var asset = new Asset
                {
                    Id = assetId,
                    TenantId = tenantId,
                    Name = $"{typeName.Replace("-", " ")} {i:D2}",
                    AssetTag = $"TAG-{typeName.Substring(0, Math.Min(3, typeName.Length)).ToUpper()}-{rng.Next(1000, 9999)}",
                    SerialNumber = $"SN-{rng.Next(100000, 999999)}",
                    AssetTypeId = typeMap[typeName],
                    OwnerId = assetOwner.Id,
                    LocationId = typeName.Contains("Server") || typeName.Contains("Switch") ? locRZ.Id : locHQ.Id,
                    LifecycleStatus = "Produktiv"
                };

                object dynamicData = new { General = new { Note = "Generiertes Test-Asset", LastScan = DateTime.UtcNow.AddDays(-rng.Next(0, 5)).ToString("g") } };
                asset.DynamicDataJson = JsonSerializer.Serialize(dynamicData);

                context.Assets.Add(asset);
                assetsPool.Add(asset);

                if (typeName is "Rack-Server" or "Laptop" or "Core-Switch" or "Firewall")
                {
                    ipAddresses.Add(new IpAddress
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        SubnetId = subProd.Id,
                        AssignedAssetId = assetId,
                        Address = $"10.0.20.{rng.Next(2, 254)}",
                        Description = "Primäres Interface",
                        MacAddress = $"00:50:56:{rng.Next(10, 99):X2}:{rng.Next(10, 99):X2}:{rng.Next(10, 99):X2}"
                    });
                }
            }
        }

        // --- 9. TICKET SYSTEM ---
        var ticketTitles = new[] {
            "Mängelmelder | Zu viele Einträge", "Beschaffung | Laptops + Docking Station",
            "VPN Verbindung bricht ab", "WLAN im RZ instabil", "Drucker im 3. OG defekt",
            "Exchange Online Sync Fehler", "Passwort Reset angefordert", "Software-Lizenz abgelaufen",
            "Neuer User anlegen", "Firewall-Regel Anpassung", "Dienstleister Zugriff gewähren"
        };
        var statuses = new[] { "OFFEN", "BEANTWORTET", "AKTIV", "GESCHLOSSEN" };
        var priorities = new[] { "Niedrig", "Normal", "Hoch" };
        var tags = new[] { "Hardware, Serverausfall", "Dringend, VIP", "Netzwerk", "Software", "Bestellung" };

        var itilTypes = new[] {
            "Incident (Störung)",
            "Service Request (Serviceanfrage)",
            "Problem (Problem)",
            "Change (Änderung)"
        };

        for (int i = 1; i <= 100; i++)
        {
            var requester = personPool[rng.Next(personPool.Count)];
            var supporter = i % 5 == 0 ? admin : personPool[rng.Next(personPool.Count)];

            var requesterAssets = assetsPool.Where(a => a.OwnerId == requester.Id).ToList();
            var linkedAssets = requesterAssets.Count > 0 ? requesterAssets.Take(rng.Next(1, 3)).ToList() : new List<Asset>();

            var createdAt = DateTime.UtcNow.AddDays(-rng.Next(1, 120));
            var ticketId = Guid.NewGuid();

            var ticket = new Ticket
            {
                Id = ticketId,
                TenantId = tenantId,
                TicketNumber = 500 + i,
                Title = $"{ticketTitles[rng.Next(ticketTitles.Length)]}",
                Description = $"Automatisches Ticket #{500 + i}. Systemgeneriert für Testzwecke.",
                Status = statuses[rng.Next(statuses.Length)],
                Priority = priorities[rng.Next(priorities.Length)],

                Category1 = itilTypes[rng.Next(itilTypes.Length)],
                Category2 = linkedAssets.Any() ? "Hardware" : "Software",
                Category3 = linkedAssets.FirstOrDefault()?.AssetType?.Name ?? "Allgemein",

                Tags = tags[rng.Next(tags.Length)],

                RequesterId = requester.Id,
                SupporterId = supporter.Id,
                AssignedAssets = linkedAssets,
                CreatedAt = createdAt,
                DueAt = createdAt.AddDays(7)
            };
            context.Tickets.Add(ticket);

            int actCount = rng.Next(1, 6);
            for (int j = 0; j < actCount; j++)
            {
                bool isSupporterEntry = j % 2 == 1;
                context.TicketActivities.Add(new TicketActivity
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticketId,
                    Timestamp = createdAt.AddHours(j * 8),
                    AuthorName = isSupporterEntry ? $"{supporter.FirstName} {supporter.LastName}" : $"{requester.FirstName} {requester.LastName}",
                    Content = isSupporterEntry ? "Wir bearbeiten Ihre Anfrage. Bitte gedulden Sie sich ein wenig." : "Ich benötige dringend Hilfe bei diesem Vorgang.",
                    Type = "Comment",
                    IsPublic = true
                });
            }
        }

        context.IpAddresses.AddRange(ipAddresses);
        await context.SaveChangesAsync();
    }
}