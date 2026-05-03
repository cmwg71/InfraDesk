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
        var tenant = new Tenant { Id = tenantId, Name = "Öffentliche Verwaltung & IT-Betrieb", Domain = "verwaltung.local" };
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
            Department = "IT-Leitung"
        };
        context.Persons.Add(globalAdmin);

        // --- 3. TEAMS & ORGANISATION ---
        var teams = new List<Team> {
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Dezernat 1 - Hauptverwaltung" },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Amt für IT & Digitalisierung" },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Support L1-L3 (IT-Service)" },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Netzwerk & Security (Admins)" },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Datenschutz & SiBe" },
            new() { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Externe Consultants (MSP)" }
        };
        context.Teams.AddRange(teams);

        // --- 4. PERSONEN POOL ---
        var firstNames = new[] { "Michael", "Thomas", "Andreas", "Stefan", "Christian", "Martin", "Frank", "Markus", "Maria", "Sabine", "Petra", "Monika", "Barbara", "Gabriele", "Susanne", "Ursula", "Jan", "Erik", "Lukas", "Leon", "Sarah", "Laura", "Julia", "Lea", "David" };
        var lastNames = new[] { "Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner", "Becker", "Schulz", "Hoffmann", "Schäfer", "Koch", "Bauer", "Richter", "Klein", "Wolf", "Schröder", "Neumann", "Schwarz", "Zimmermann", "Braun" };

        string defaultUserPasswordHash = SecurityHelper.HashPassword("Start123!");
        var personPool = new List<Person>();
        for (int i = 0; i < 150; i++)
        {
            var team = teams[rng.Next(teams.Count)];
            string role = (team.Name.Contains("Support") || team.Name.Contains("Admins") || team.Name.Contains("MSP")) ? "CI Editor" : "Reader";
            if (i == 5) role = "Tenant Admin";

            personPool.Add(new Person
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                FirstName = firstNames[rng.Next(firstNames.Length)],
                LastName = lastNames[rng.Next(lastNames.Length)],
                Email = $"user.{i}@verwaltung.local",
                Department = team.Name,
                TeamId = team.Id,
                SystemRole = role,
                PasswordHash = defaultUserPasswordHash,
                AllowedTenantsJson = $"[\"{tenantId}\"]"
            });
        }
        context.Persons.AddRange(personPool);

        var supportAgents = personPool.Where(p => p.SystemRole == "CI Editor" || p.SystemRole == "Tenant Admin").ToList();
        supportAgents.Add(globalAdmin);

        // --- 5. STANDORTE, GEBÄUDE & RÄUME (Kompakte Geo-Hierarchie) ---
        var allLocations = new List<Location>();
        var locDE = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "DE" };
        var locHE = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Hessen", ParentLocationId = locDE.Id };
        var locFB = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Fuldabrück", ParentLocationId = locHE.Id };

        var locCampus = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Rathaus Campus", ParentLocationId = locFB.Id };
        var locHaupt = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Hauptgebäude", ParentLocationId = locCampus.Id };

        allLocations.AddRange(new[] { locDE, locHE, locFB, locCampus, locHaupt });

        var rooms = new List<Location>();
        for (int f = 0; f <= 3; f++)
        {
            var floor = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = f == 0 ? "EG" : $"{f}. OG", ParentLocationId = locHaupt.Id };
            allLocations.Add(floor);
            for (int r = 1; r <= 15; r++)
            {
                var room = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = $"Büro {f}{r:D2}", RoomNumber = $"{f}{r:D2}", ParentLocationId = floor.Id };
                rooms.Add(room); allLocations.Add(room);
            }
        }

        var locRZ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Rechenzentrum FRA-1", ParentLocationId = locHE.Id };
        var locServerraum = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Serverraum Alpha", ParentLocationId = locRZ.Id };
        var locKaltgang = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Kaltgang 01", ParentLocationId = locServerraum.Id };
        allLocations.AddRange(new[] { locRZ, locServerraum, locKaltgang });

        var racks = new List<Location>();
        for (int r = 1; r <= 20; r++)
        {
            var rack = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = $"Rack {r:D2}", ParentLocationId = locKaltgang.Id };
            racks.Add(rack); allLocations.Add(rack);
        }
        context.Locations.AddRange(allLocations);

        // --- 6. ASSET TYPEN (Detaillierte CMDB Taxonomie - 4 Stufen) ---
        var typeHierarchies = new[] {
            new { G="1. Geografische & Bauliche Struktur", Icon="LocationCity",
                  L1=new[]{
                      new { N="Geo-Hierarchie", L2=new[]{ new { N="Kontinent / Land", L3=new[]{"Bundesland / Kanton / Regierungsbezirk"} } } },
                      new { N="Kommune/Verwaltung", L2=new[]{ new { N="Stadt / Gemeinde", L3=new[]{"Stadtteil / Quartier / Gemarkung"} } } },
                      new { N="Liegenschaft", L2=new[]{ new { N="Campus / Areal", L3=new[]{"Dienststelle / Außenstelle / Werksgelände"} } } },
                      new { N="Gebäude", L2=new[]{ new { N="Hauptgebäude / Trakt", L3=new[]{"Etage / Stockwerk / Flügel"} } } },
                      new { N="Raumstruktur", L2=new[]{ new { N="Büro / Besprechungsraum", L3=new[]{"Bürgerbüro / Schalterhalle / Sitzungssaal"} } } },
                      new { N="Rechenzentrum", L2=new[]{ new { N="RZ-Raum / Serverraum", L3=new[]{"Kaltgang / Rack-Reihe / Rack-Position (HE)"} } } },
                      new { N="Sonderzonen", L2=new[]{ new { N="Archiv / Lager", L3=new[]{"Werkstatt / Labor / Technikzentrale"} } } }
                  }
            },
            new { G="2. IT-Infrastruktur & Hardware", Icon="Computer",
                  L1=new[]{
                      new { N="Workplace", L2=new[]{ new { N="Notebook / Laptop", L3=new[]{"Standard", "Workstation", "Rugged (Outdoor)"} }, new { N="Desktop-PC", L3=new[]{"Mini-PC", "All-in-One", "Thin Client"} } } },
                      new { N="Mobile Devices", L2=new[]{ new { N="Smartphone", L3=new[]{"Diensthandy", "VIP", "MDE-Scanner"} }, new { N="Tablet", L3=new[]{"iOS", "Android", "Windows-Tablet"} } } },
                      new { N="Compute (Server)", L2=new[]{ new { N="Physical Server", L3=new[]{"Rack-Server", "Blade-Server", "Mainframe"} }, new { N="Virtual Instance", L3=new[]{"Virtual Machine (VM)", "Container", "Host"} } } },
                      new { N="Storage & Backup", L2=new[]{ new { N="Zentralspeicher", L3=new[]{"SAN-Array", "NAS-Filer", "Tape-Library"} } } },
                      new { N="Printing/Imaging", L2=new[]{ new { N="Drucker / Kopierer", L3=new[]{"Multifunktionsgerät (MFP)", "Plotter", "Labelprinter"} } } },
                      new { N="Peripherie", L2=new[]{ new { N="AV / Video", L3=new[]{"Monitor", "Dockingstation", "Konferenzsystem"} } } }
                  }
            },
            new { G="3. Netzwerk & Kommunikation", Icon="Router",
                  L1=new[]{
                      new { N="Switching", L2=new[]{ new { N="LAN-Switch", L3=new[]{"Core", "Distribution", "Access (Edge)"} } } },
                      new { N="Routing", L2=new[]{ new { N="WAN-Router", L3=new[]{"Internet-Gateway", "SD-WAN", "Standort-VPN"} } } },
                      new { N="Wireless", L2=new[]{ new { N="WLAN", L3=new[]{"Access Point", "WLAN-Controller", "Bridge"} } } },
                      new { N="Security", L2=new[]{ new { N="Firewall", L3=new[]{"Next-Gen Firewall", "IPS-Appliance", "Proxy"} } } },
                      new { N="Voice/Telephony", L2=new[]{ new { N="Telefonie", L3=new[]{"IP-Telefon", "DECT-Basis", "PBX-Gateway"} } } },
                      new { N="Behördenfunk", L2=new[]{ new { N="BOS / Funk", L3=new[]{"Funkleitstelle", "Repeater", "Handfunkgerät"} } } }
                  }
            },
            new { G="4. Passive Infrastruktur & Facility Management", Icon="Power",
                  L1=new[]{
                      new { N="Power", L2=new[]{ new { N="USV / UPS", L3=new[]{"Zentral-Anlage", "Rack-USV", "Batterie-Modul"} }, new { N="Verteilung", L3=new[]{"PDU (Managed)", "Sicherungskasten", "Unterverteilung"} } } },
                      new { N="Klima", L2=new[]{ new { N="Kühlung", L3=new[]{"In-Row-Kühlung", "Klimaschrank", "Lüftung"} } } },
                      new { N="Verkabelung", L2=new[]{ new { N="Passiv-Netz", L3=new[]{"Patchpanel", "Spleißbox", "Bodentank"} } } },
                      new { N="Sicherheit", L2=new[]{ new { N="Überwachung", L3=new[]{"Zutrittsleser", "Kamera (CCTV)", "Brandmelder"} } } }
                  }
            },
            new { G="5. Organisation & Social Assets", Icon="Business",
                  L1=new[]{
                      new { N="Behörde / Firma", L2=new[]{ new { N="Hauptverwaltung", L3=new[]{"Dezernat", "Amt", "Fachbereich"} } } },
                      new { N="Interne Gruppen", L2=new[]{ new { N="IT-Teams", L3=new[]{"Support L1-L3", "Fach-Admins", "Projektteam"} } } },
                      new { N="Personal", L2=new[]{ new { N="Amtsträger / MA", L3=new[]{"Beamter", "Angestellter", "Externer Consultant"} } } },
                      new { N="Sonderrollen", L2=new[]{ new { N="Beauftragte", L3=new[]{"DSB (Datenschutz)", "IT-SiBe", "Personalrat"} } } },
                      new { N="Partner", L2=new[]{ new { N="Dienstleister", L3=new[]{"Managed Service Provider", "Wartungsfirma"} } } },
                      new { N="Hersteller", L2=new[]{ new { N="Vendor", L3=new[]{"Hardware-Hersteller", "Software-Publisher"} } } }
                  }
            },
            new { G="6. Fachverfahren & Logische CIs", Icon="AccountBalance",
                  L1=new[]{
                      new { N="E-Government", L2=new[]{ new { N="Online-Dienste", L3=new[]{"OZG-Verfahren", "Bürgerportal", "Payment"} } } },
                      new { N="Fachverfahren", L2=new[]{ new { N="Register", L3=new[]{"Meldewesen", "Kataster", "Kfz-Zulassung"} } } },
                      new { N="Hoheitliche IT", L2=new[]{ new { N="Spezial-Hardware", L3=new[]{"Siegel-Drucker", "Fingerabdruck-Terminal"} } } },
                      new { N="Lizenzwesen", L2=new[]{ new { N="Software-Asset", L3=new[]{"Kauflizenz", "Abonnement (SaaS)", "Rahmenvertrag"} } } }
                  }
            }
        };

        var allTypes = new List<AssetType>();
        var leafTypes = new List<AssetType>();

        // Personen-CI Leaf-ID direkt ermitteln, um nachher die Personen-Assets zu generieren
        Guid beamterTypeId = Guid.Empty;
        Guid amtTypeId = Guid.Empty;

        foreach (var p in typeHierarchies)
        {
            var pillarNode = new AssetType { Id = Guid.NewGuid(), Name = p.G, IconKey = p.Icon };
            allTypes.Add(pillarNode);

            foreach (var l1 in p.L1)
            {
                var l1Node = new AssetType { Id = Guid.NewGuid(), Name = l1.N, IconKey = "Folder", ParentAssetTypeId = pillarNode.Id };
                allTypes.Add(l1Node);

                foreach (var l2 in l1.L2)
                {
                    var l2Node = new AssetType { Id = Guid.NewGuid(), Name = l2.N, IconKey = p.Icon, ParentAssetTypeId = l1Node.Id };
                    allTypes.Add(l2Node);

                    foreach (var l3 in l2.L3)
                    {
                        var l3Node = new AssetType { Id = Guid.NewGuid(), Name = l3, IconKey = p.Icon, ParentAssetTypeId = l2Node.Id };
                        allTypes.Add(l3Node);
                        leafTypes.Add(l3Node);

                        if (l3 == "Beamter") beamterTypeId = l3Node.Id;
                        if (l3 == "Amt") amtTypeId = l3Node.Id;
                    }
                }
            }
        }
        context.AssetTypes.AddRange(allTypes);
        await context.SaveChangesAsync();

        // --- 7. MASSEN-ASSETS & IPAM (Auf L3 Ebene) ---
        var subnets = new List<Subnet>();
        for (int s = 1; s <= 15; s++)
        {
            subnets.Add(new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = $"VLAN {s * 10}", NetworkAddress = $"10.10.{s}.0", CidrMask = 24, Gateway = $"10.10.{s}.1", VlanId = s * 10, IsFullScanEnabled = true, LocationId = (s <= 5 ? locRZ.Id : locHaupt.Id) });
        }
        context.Subnets.AddRange(subnets);

        var assetsPool = new List<Asset>();
        var ipAddresses = new List<IpAddress>();
        string[] statuses = { "Produktiv", "Produktiv", "Auf Lager", "Ausgemustert" };

        // Normale IT-Assets generieren
        foreach (var leaf in leafTypes.Where(t => !t.Name.Contains("Beamter") && !t.Name.Contains("Amt") && !t.Name.Contains("Dezernat") && !t.Name.Contains("Support") && !t.Name.Contains("Land") && !t.Name.Contains("Stadt")))
        {
            for (int i = 1; i <= 20; i++)
            {
                var assetId = Guid.NewGuid();
                var owner = personPool[rng.Next(personPool.Count)];

                bool isServerOrNet = new[] { "Server", "Mainframe", "Switch", "Router", "Firewall", "Storage", "Array", "USV", "PDU", "Passiv", "Klima" }.Any(x => leaf.Name.Contains(x));
                bool isLogical = new[] { "VM", "Container", "Dienste", "Verfahren", "Portal", "Lizenz", "Vertrag" }.Any(x => leaf.Name.Contains(x));

                Guid? assignedLocation = isLogical ? null : (isServerOrNet ? racks[rng.Next(racks.Count)].Id : rooms[rng.Next(rooms.Count)].Id);
                Guid? assignedOwner = isLogical || isServerOrNet ? supportAgents[rng.Next(supportAgents.Count)].Id : owner.Id;

                var asset = new Asset
                {
                    Id = assetId,
                    TenantId = tenantId,
                    Name = $"{leaf.Name} {i:D3}",
                    AssetTag = $"TAG-{leaf.Name.Substring(0, Math.Min(3, leaf.Name.Length)).ToUpper()}-{rng.Next(10000, 99999)}",
                    SerialNumber = isLogical ? null : $"SN-{rng.Next(100000, 999999)}",
                    AssetTypeId = leaf.Id,
                    OwnerId = assignedOwner,
                    LocationId = assignedLocation,
                    LifecycleStatus = statuses[rng.Next(statuses.Length)]
                };

                asset.DynamicDataJson = JsonSerializer.Serialize(new { CustomFields = new { SystemIdent = "Auto-Generated", LetztePruefung = DateTime.UtcNow.ToString("yyyy-MM-dd") } });
                context.Assets.Add(asset);
                assetsPool.Add(asset);

                if (!isLogical && !leaf.Name.Contains("Software") && !leaf.Name.Contains("Register"))
                {
                    var subnet = subnets[rng.Next(subnets.Count)];
                    ipAddresses.Add(new IpAddress { Id = Guid.NewGuid(), TenantId = tenantId, SubnetId = subnet.Id, AssignedAssetId = assetId, Address = subnet.NetworkAddress.Replace(".0", $".{rng.Next(10, 250)}"), IpStatus = "Used" });
                }
            }
        }

        // --- 8. PERSONEN & TEAMS ALS ECHTE CIs GENERIEREN ---
        foreach (var t in teams)
        {
            var teamAsset = new Asset
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = $"Team-CI: {t.Name}",
                AssetTag = $"ORG-TEAM-{rng.Next(1000, 9999)}",
                AssetTypeId = amtTypeId,
                LifecycleStatus = "Produktiv",
                DynamicDataJson = "{}"
            };
            context.Assets.Add(teamAsset);
            assetsPool.Add(teamAsset);
        }
        foreach (var p in supportAgents)
        {
            var persAsset = new Asset
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = $"Person-CI: {p.FirstName} {p.LastName}",
                AssetTag = $"ORG-PERS-{rng.Next(1000, 9999)}",
                AssetTypeId = beamterTypeId,
                LifecycleStatus = "Produktiv",
                DynamicDataJson = "{}"
            };
            context.Assets.Add(persAsset);
            assetsPool.Add(persAsset);
        }

        context.IpAddresses.AddRange(ipAddresses);

        // --- 9. ASSET-BEZIEHUNGEN (Dependencies & Impact-Graph) ---
        var assetLinks = new List<AssetLink>();

        // Filtern der generierten Assets nach Typ-IDs für logische Verknüpfungen
        var pServers = assetsPool.Where(a => a.Name.Contains("Server") || a.Name.Contains("Mainframe")).ToList();
        var vms = assetsPool.Where(a => a.Name.Contains("Virtual Machine") || a.Name.Contains("Container")).ToList();
        var sw = assetsPool.Where(a => a.Name.Contains("Switch")).ToList();
        var routers = assetsPool.Where(a => a.Name.Contains("Router") || a.Name.Contains("Gateway")).ToList();
        var pdus = assetsPool.Where(a => a.Name.Contains("PDU") || a.Name.Contains("Unterverteilung")).ToList();
        var apps = assetsPool.Where(a => a.Name.Contains("Register") || a.Name.Contains("OZG-Verfahren") || a.Name.Contains("Payment")).ToList();

        // Regel 1: Virtuelle Maschinen laufen auf physikalischen Servern
        foreach (var vm in vms)
        {
            if (pServers.Any())
            {
                var host = pServers[rng.Next(pServers.Count)];
                assetLinks.Add(new AssetLink { Id = Guid.NewGuid(), TenantId = tenantId, SourceAssetId = vm.Id, TargetAssetId = host.Id, LinkType = "Läuft auf (Hosted by)" });
            }
        }

        // Regel 2: Server hängen an LAN-Switches
        foreach (var srv in pServers)
        {
            if (sw.Any())
            {
                var switchObj = sw[rng.Next(sw.Count)];
                assetLinks.Add(new AssetLink { Id = Guid.NewGuid(), TenantId = tenantId, SourceAssetId = srv.Id, TargetAssetId = switchObj.Id, LinkType = "Netzwerk Uplink zu" });
            }
        }

        // Regel 3: Switches hängen an WAN-Routern
        foreach (var switchObj in sw)
        {
            if (routers.Any())
            {
                var rtr = routers[rng.Next(routers.Count)];
                assetLinks.Add(new AssetLink { Id = Guid.NewGuid(), TenantId = tenantId, SourceAssetId = switchObj.Id, TargetAssetId = rtr.Id, LinkType = "Uplink zu" });
            }
        }

        // Regel 4: IT-Infrastruktur hängt an Strom-Verteilungen (PDUs)
        foreach (var hw in pServers.Concat(sw).Concat(routers))
        {
            if (pdus.Any())
            {
                var pdu = pdus[rng.Next(pdus.Count)];
                assetLinks.Add(new AssetLink { Id = Guid.NewGuid(), TenantId = tenantId, SourceAssetId = hw.Id, TargetAssetId = pdu.Id, LinkType = "Stromversorgung via" });
            }
        }

        // Regel 5: Fachverfahren nutzen virtuelle Maschinen
        foreach (var app in apps)
        {
            if (vms.Any())
            {
                var vm = vms[rng.Next(vms.Count)];
                assetLinks.Add(new AssetLink { Id = Guid.NewGuid(), TenantId = tenantId, SourceAssetId = app.Id, TargetAssetId = vm.Id, LinkType = "Gehostet auf" });
            }
        }

        context.Set<AssetLink>().AddRange(assetLinks);

        // --- 10. TICKETS & HELP-DESK-JOURNALE (300 Tickets) ---
        var ticketTitles = new[] {
            "Fachverfahren stürzt ab", "Kamera-Überwachung ausgefallen", "USV meldet Batterie-Fehler",
            "WLAN in Besprechungsraum schwach", "Neuer Kaltgang-Sensor benötigt", "Passwort für OZG-Portal vergessen",
            "Behördenfunk (BOS) rauscht", "Routing ins RZ unterbrochen", "Firewall-Regel für Register-Abfrage",
            "Storage-Array meldet SMART-Fehler", "PDU im Rack ausgefallen", "Siegel-Drucker defekt",
            "Druckerpapier staut sich", "Notebook Akku defekt", "Maus funktioniert nicht", "Monitor flackert",
            "VPN Zugang gesperrt", "Lizenz abgelaufen", "Neuer Mitarbeiter Onboarding", "Server Latenz zu hoch"
        };
        var tStatuses = new[] { "OFFEN", "OFFEN", "BEANTWORTET", "AKTIV", "AKTIV", "GESCHLOSSEN", "GESCHLOSSEN", "GESCHLOSSEN" };
        var tPriorities = new[] { "Niedrig", "Normal", "Normal", "Hoch", "Kritisch" };
        var itilTypes = new[] { "Incident (Störung)", "Incident (Störung)", "Service Request (Serviceanfrage)", "Problem (Problem)", "Change (Änderung)" };

        var tickets = new List<Ticket>();
        var activities = new List<TicketActivity>();

        for (int i = 1; i <= 300; i++)
        {
            var requester = personPool[rng.Next(personPool.Count)];
            var supporter = supportAgents[rng.Next(supportAgents.Count)];

            // Intelligente Verknüpfung mit 1-3 zufälligen Assets pro Ticket
            var linkedAssets = new List<Asset>();
            int numAssets = rng.Next(1, 4);
            for (int a = 0; a < numAssets; a++)
            {
                linkedAssets.Add(assetsPool[rng.Next(assetsPool.Count)]);
            }
            linkedAssets = linkedAssets.Distinct().ToList();

            var createdAt = DateTime.UtcNow.AddDays(-rng.Next(1, 90)).AddMinutes(-rng.Next(10, 600));
            var ticketId = Guid.NewGuid();

            var ticket = new Ticket
            {
                Id = ticketId,
                TenantId = tenantId,
                TicketNumber = 1000 + i,
                Title = $"{ticketTitles[rng.Next(ticketTitles.Length)]}",
                Description = $"<p>Meldung durch {requester.Department}: Das System verhält sich seit einiger Zeit merkwürdig. Wir bitten um Prüfung der zugeordneten Assets.</p>",
                Status = tStatuses[rng.Next(tStatuses.Length)],
                Priority = tPriorities[rng.Next(tPriorities.Length)],
                Category1 = itilTypes[rng.Next(itilTypes.Length)],
                RequesterId = requester.Id,
                SupporterId = supporter.Id,
                AssignedAssets = linkedAssets,
                CreatedAt = createdAt,
                DueAt = createdAt.AddDays(rng.Next(1, 14)),
                EstimatedMinutes = rng.Next(15, 120),
                SpentMinutes = rng.Next(0, 90),
                IsInternal = rng.Next(0, 10) > 8 // 10% sind interne Tickets
            };
            tickets.Add(ticket);

            // Generiere 1-5 Aktivitäten pro Ticket (Journalverlauf)
            int numActivities = rng.Next(1, 6);
            for (int j = 0; j < numActivities; j++)
            {
                bool isSupporter = j % 2 == 1;
                activities.Add(new TicketActivity
                {
                    Id = Guid.NewGuid(),
                    TicketId = ticketId,
                    Timestamp = createdAt.AddHours((j + 1) * 2),
                    AuthorName = isSupporter ? $"{supporter.FirstName} {supporter.LastName}" : $"{requester.FirstName} {requester.LastName}",
                    Content = isSupporter ? "Geprüft und im Monitoring-System validiert. Wir kümmern uns um die Lösung." : "Gibt es schon ein Update zu diesem Vorgang?",
                    Type = "Comment",
                    IsPublic = true,
                    IsStaffAction = isSupporter,
                    ActivitySpentMinutes = isSupporter ? rng.Next(5, 30) : 0
                });
            }
        }
        context.Tickets.AddRange(tickets);
        context.TicketActivities.AddRange(activities);

        await context.SaveChangesAsync();
    }
}