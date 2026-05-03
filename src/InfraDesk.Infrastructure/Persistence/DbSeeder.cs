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
        var tenant = new Tenant { Id = tenantId, Name = "InfraDesk Global Enterprise", Domain = "infradesk.local" };
        context.Tenants.Add(tenant);

        // --- 2. GLOBAL ADMIN ---
        string adminPassword = "Admin123!";
        var globalAdmin = new Person
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            FirstName = "System",
            LastName = "Administrator",
            Email = "admin@infradesk.local",
            PasswordHash = SecurityHelper.HashPassword(adminPassword),
            SystemRole = "Global Admin",
            AllowedTenantsJson = $"[\"{tenantId}\"]",
            Department = "IT Management"
        };
        context.Persons.Add(globalAdmin);

        // --- 3. PERSONEN POOL (Mitarbeiter & Agenten) ---
        var firstNames = new[] { "Michael", "Thomas", "Andreas", "Stefan", "Christian", "Martin", "Frank", "Markus", "Maria", "Sabine", "Petra", "Monika", "Barbara", "Gabriele", "Susanne", "Ursula", "Jan", "Erik", "Lukas", "Leon", "Sarah", "Laura", "Julia", "Lea", "David", "Felix", "Maximilian", "Anna", "Lisa", "Katharina" };
        var lastNames = new[] { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann", "Schäfer", "Koch", "Bauer", "Richter", "Klein", "Wolf", "Schröder", "Neumann", "Schwarz", "Zimmermann", "Braun", "Krüger", "Hofmann", "Hartmann", "Lange", "Schmitt", "Werner", "Krause", "Meier" };
        var departments = new[] { "HR", "Sales", "Marketing", "Finance", "Legal", "Engineering", "Support", "Logistics", "R&D" };

        // Standard-Passwort für alle generierten Test-User
        string defaultUserPasswordHash = SecurityHelper.HashPassword("Start123!");

        var personPool = new List<Person>();
        // Erzeuge 150 Mitarbeiter
        for (int i = 0; i < 150; i++)
        {
            var first = firstNames[rng.Next(firstNames.Length)];
            var last = lastNames[rng.Next(lastNames.Length)];
            var dept = departments[rng.Next(departments.Length)];
            // Ca. 10% sind IT-Support-Agenten
            var role = (i % 10 == 0) ? "CI Editor" : "Reader";

            var p = new Person
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FirstName = first,
                LastName = last,
                Email = $"{first.ToLower()}.{last.ToLower()}{i}@firma.local",
                Department = role == "CI Editor" ? "IT Support" : dept,
                SystemRole = role,
                PasswordHash = defaultUserPasswordHash,
                AllowedTenantsJson = $"[\"{tenantId}\"]"
            };
            personPool.Add(p);
        }
        context.Persons.AddRange(personPool);

        var supportAgents = personPool.Where(p => p.SystemRole == "CI Editor").ToList();
        supportAgents.Add(globalAdmin);

        // --- 4. STANDORTE (Hierarchie: Campus -> Etagen -> Räume / RZ -> Racks) ---
        var allLocations = new List<Location>();

        var locRZ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Datacenter FRA-1", Address = "Hanauer Landstraße 300, Frankfurt" };
        var locHQ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Headquarters", Address = "MainTower 1, Frankfurt" };
        allLocations.Add(locRZ);
        allLocations.Add(locHQ);

        // Racks im Rechenzentrum
        var racks = new List<Location>();
        for (int r = 1; r <= 20; r++)
        {
            var rack = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = $"Rack {r:D2}", ParentLocationId = locRZ.Id, ParentLocation = locRZ };
            racks.Add(rack);
            allLocations.Add(rack);
        }

        // Etagen und Räume im HQ
        var rooms = new List<Location>();
        for (int f = 1; f <= 5; f++)
        {
            var floor = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = $"Etage {f}", ParentLocationId = locHQ.Id, ParentLocation = locHQ };
            allLocations.Add(floor);
            for (int r = 1; r <= 15; r++)
            {
                var room = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = $"Raum {f}{r:D2}", RoomNumber = $"{f}{r:D2}", ParentLocationId = floor.Id, ParentLocation = floor };
                rooms.Add(room);
                allLocations.Add(room);
            }
        }
        context.Locations.AddRange(allLocations);

        // --- 5. SUBNETZE & IPAM ---
        var subnets = new List<Subnet>();
        for (int s = 1; s <= 10; s++)
        {
            subnets.Add(new Subnet
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = s <= 3 ? $"Server-VLAN {s * 10}" : $"Client-VLAN {s * 10}",
                NetworkAddress = $"10.10.{s}.0",
                CidrMask = 24,
                Gateway = $"10.10.{s}.1",
                VlanId = s * 10,
                IsFullScanEnabled = true
            });
        }
        context.Subnets.AddRange(subnets);

        var serverSubnets = subnets.Take(3).ToList();
        var clientSubnets = subnets.Skip(3).ToList();

        // --- 6. HERSTELLER ---
        var manufacturers = new List<Manufacturer> {
            new() { Id = Guid.NewGuid(), Name = "Dell Technologies" },
            new() { Id = Guid.NewGuid(), Name = "Cisco Systems" },
            new() { Id = Guid.NewGuid(), Name = "Hewlett Packard Enterprise" },
            new() { Id = Guid.NewGuid(), Name = "Apple" },
            new() { Id = Guid.NewGuid(), Name = "Microsoft" },
            new() { Id = Guid.NewGuid(), Name = "VMware" },
            new() { Id = Guid.NewGuid(), Name = "Palo Alto Networks" },
            new() { Id = Guid.NewGuid(), Name = "Lenovo" },
            new() { Id = Guid.NewGuid(), Name = "Fortinet" },
            new() { Id = Guid.NewGuid(), Name = "Oracle" },
            new() { Id = Guid.NewGuid(), Name = "Red Hat" }
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
        await context.SaveChangesAsync(); // Zwischendrin speichern, da wir IDs für Assets brauchen

        // --- 8. MASSEN-ASSETS (Mindestens 20 pro Kategorie) ---
        var assetsPool = new List<Asset>();
        var ipAddresses = new List<IpAddress>();

        string[] statuses = { "Produktiv", "Produktiv", "Produktiv", "Auf Lager", "Ausgemustert", "In Reparatur" };
        string[] cpus = { "Intel Core i5", "Intel Core i7", "Intel Xeon Gold", "AMD Ryzen 5", "AMD EPYC", "Apple M2", "Apple M3 Pro" };

        foreach (var typeName in allTypeNames)
        {
            // FÜR JEDEN TYP WERDEN 25 OBJEKTE ERZEUGT!
            for (int i = 1; i <= 25; i++)
            {
                var assetId = Guid.NewGuid();
                var assetOwner = personPool[rng.Next(personPool.Count)];
                var mfr = manufacturers[rng.Next(manufacturers.Count)];

                // Logische Zuweisung: IT/Infra -> Rack, Clients -> Räume
                bool isServerOrNet = typeName.Contains("Server") || typeName.Contains("Switch") || typeName.Contains("Firewall") || typeName.Contains("Storage") || typeName.Contains("Appliance");
                bool isVirtual = typeName.Contains("Software") || typeName.Contains("Betriebssystem") || typeName.Contains("Datenbank") || typeName.Contains("Cloud");

                Guid? assignedLocation = isVirtual ? null : (isServerOrNet ? racks[rng.Next(racks.Count)].Id : rooms[rng.Next(rooms.Count)].Id);
                Guid? assignedOwner = isServerOrNet || isVirtual ? supportAgents[rng.Next(supportAgents.Count)].Id : assetOwner.Id;

                var asset = new Asset
                {
                    Id = assetId,
                    TenantId = tenantId,
                    Name = $"{typeName} {i:D3}",
                    AssetTag = $"TAG-{typeName.Substring(0, Math.Min(3, typeName.Length)).ToUpper()}-{rng.Next(10000, 99999)}",
                    SerialNumber = $"SN-{rng.Next(100000, 999999)}-{Guid.NewGuid().ToString().Substring(0, 4).ToUpper()}",
                    AssetTypeId = typeMap[typeName],
                    ManufacturerId = mfr.Id,
                    OwnerId = assignedOwner,
                    LocationId = assignedLocation,
                    LifecycleStatus = statuses[rng.Next(statuses.Length)]
                };

                // Realistische dynamische Daten
                object dynamicData = isVirtual
                    ? new { CustomFields = new { Version = $"v{rng.Next(1, 15)}.{rng.Next(0, 5)}", Lizenzmodell = "Abo", Laufzeit_Ende = DateTime.UtcNow.AddMonths(rng.Next(1, 36)).ToString("yyyy-MM-dd") } }
                    : new { CustomFields = new { CPU = cpus[rng.Next(cpus.Length)], RAM_GB = rng.Next(1, 16) * 8, Disk_GB = rng.Next(1, 10) * 256, Garantie_Bis = DateTime.UtcNow.AddMonths(rng.Next(-12, 48)).ToString("yyyy-MM-dd") } };

                asset.DynamicDataJson = JsonSerializer.Serialize(dynamicData);

                context.Assets.Add(asset);
                assetsPool.Add(asset);

                // IPs nur für netzwerkfähige Geräte
                if (isServerOrNet || typeName is "Laptop" or "Desktop" or "Workstation")
                {
                    var assignedSubnet = isServerOrNet ? serverSubnets[rng.Next(serverSubnets.Count)] : clientSubnets[rng.Next(clientSubnets.Count)];
                    var ipSuffix = rng.Next(10, 250);
                    var ipString = assignedSubnet.NetworkAddress.Replace(".0", $".{ipSuffix}");

                    ipAddresses.Add(new IpAddress
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        SubnetId = assignedSubnet.Id,
                        AssignedAssetId = assetId,
                        Address = ipString,
                        Description = "Primäres Netzwerk-Interface",
                        MacAddress = $"00:1A:2B:{rng.Next(10, 99):X2}:{rng.Next(10, 99):X2}:{rng.Next(10, 99):X2}",
                        IpStatus = "Used"
                    });
                }
            }
        }
        context.IpAddresses.AddRange(ipAddresses);

        // --- 9. TICKETS & HELP-DESK-JOURNALE (Ca. 200 Tickets) ---
        var ticketTitles = new[] {
            "Software stürzt nach Update ab", "Brauche neuen Monitor", "VPN Verbindung extrem langsam",
            "WLAN im Gebäude B instabil", "Drucker zieht Papier schief ein", "Passwort für ERP-System vergessen",
            "Lizenz für Visio abgelaufen", "Neues iPhone für Geschäftsführung", "Firewall-Regel für Projekt X benötigt",
            "Server-Festplatte meldet SMART-Fehler", "Datenbank-Performance eingebrochen", "Maus defekt",
            "Zugriff auf Laufwerk Z verweigert", "Onboarding: Neuer Mitarbeiter", "VLAN-Routing fehlerhaft"
        };
        var tStatuses = new[] { "OFFEN", "OFFEN", "BEANTWORTET", "AKTIV", "AKTIV", "GESCHLOSSEN", "GESCHLOSSEN", "GESCHLOSSEN" };
        var tPriorities = new[] { "Niedrig", "Normal", "Normal", "Normal", "Hoch", "Kritisch" };
        var tTags = new[] { "Hardware", "VIP", "Netzwerk", "Software", "Bestellung", "Sicherheit", "Dringend" };
        var itilTypes = new[] { "Incident (Störung)", "Incident (Störung)", "Service Request (Serviceanfrage)", "Problem (Problem)", "Change (Änderung)" };

        var tickets = new List<Ticket>();
        var activities = new List<TicketActivity>();

        for (int i = 1; i <= 200; i++)
        {
            var requester = personPool[rng.Next(personPool.Count)];
            var supporter = supportAgents[rng.Next(supportAgents.Count)];

            // Zufälliges Asset des Requesters finden, um es ans Ticket zu hängen
            var reqAssets = assetsPool.Where(a => a.OwnerId == requester.Id).ToList();
            var linkedAssets = reqAssets.Count > 0 ? reqAssets.Take(1).ToList() : new List<Asset>();

            var createdAt = DateTime.UtcNow.AddDays(-rng.Next(1, 60)).AddMinutes(-rng.Next(10, 600));
            var ticketId = Guid.NewGuid();

            var ticket = new Ticket
            {
                Id = ticketId,
                TenantId = tenantId,
                TicketNumber = 1000 + i,
                Title = $"{ticketTitles[rng.Next(ticketTitles.Length)]}",
                Description = $"<p>Hallo IT-Team,</p><p>Ich habe folgendes Problem: Das System verhält sich seit heute Morgen merkwürdig. Es betrifft meinen regulären Arbeitsablauf.</p><p>Viele Grüße,<br>{requester.FirstName}</p>",
                Status = tStatuses[rng.Next(tStatuses.Length)],
                Priority = tPriorities[rng.Next(tPriorities.Length)],
                Category1 = itilTypes[rng.Next(itilTypes.Length)],
                Category2 = linkedAssets.Any() ? "Hardware" : "Software",
                Tags = $"{tTags[rng.Next(tTags.Length)]}, Auto-Gen",
                RequesterId = requester.Id,
                SupporterId = supporter.Id,
                AssignedAssets = linkedAssets,
                CreatedAt = createdAt,
                DueAt = createdAt.AddDays(rng.Next(1, 14)),
                EstimatedMinutes = rng.Next(15, 120),
                SpentMinutes = rng.Next(0, 90)
            };
            tickets.Add(ticket);

            // Generiere 2-5 Journal-Einträge pro Ticket
            int actCount = rng.Next(2, 6);
            for (int j = 0; j < actCount; j++)
            {
                bool isSupporter = j % 2 == 1; // Wechselnder Chat
                activities.Add(new TicketActivity
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticketId,
                    Timestamp = createdAt.AddHours(j * 4),
                    AuthorName = isSupporter ? $"{supporter.FirstName} {supporter.LastName}" : $"{requester.FirstName} {requester.LastName}",
                    Content = isSupporter
                        ? $"Hallo {requester.FirstName},<br/>wir haben das Ticket übernommen und prüfen die Logs. <br/><i>(Automatisch erfasste Zeit: {rng.Next(5, 15)} Min)</i>"
                        : "Gibt es hierzu schon ein Update? Ich benötige das System dringend.",
                    Type = "Comment",
                    IsPublic = true,
                    IsStaffAction = isSupporter,
                    ActivitySpentMinutes = isSupporter ? rng.Next(5, 30) : 0
                });
            }
        }
        context.Tickets.AddRange(tickets);
        context.TicketActivities.AddRange(activities);

        // --- 10. MASTER-TICKET VERKNÜPFUNGEN ---
        // Verknüpfe ein paar Tickets als Sub-Tickets
        var closedTickets = tickets.Where(t => t.Status == "GESCHLOSSEN").ToList();
        if (closedTickets.Count > 10)
        {
            var master = closedTickets[0];
            for (int i = 1; i <= 5; i++)
            {
                closedTickets[i].MasterTicketId = master.Id;
            }
        }

        // --- SPEICHERN ---
        await context.SaveChangesAsync();
    }
}