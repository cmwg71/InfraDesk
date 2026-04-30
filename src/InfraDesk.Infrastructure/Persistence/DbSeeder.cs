// Dateipfad: src/InfraDesk.Infrastructure/Persistence/DbSeeder.cs
using InfraDesk.Core.Entities;
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
        if (await context.Assets.AnyAsync()) return;

        var tenantId = Guid.NewGuid();
        var tenant = new Tenant { Id = tenantId, Name = "Grams IT Global Enterprise", Domain = "grams-it.com" };
        context.Tenants.Add(tenant);

        // --- 1. STANDORTE (Physisch) ---
        var locRZ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "RZ-FRA-01 (Tier 4)", Address = "Hanauer Landstraße 300" };
        var locHQ = new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "HQ Frankfurt", Address = "MainTower 1" };
        context.Locations.AddRange(locRZ, locHQ);

        // --- 2. IPAM SUBNETZE ---
        var subMgmt = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "OOB-Management", NetworkAddress = "10.0.0.0", CidrMask = 24, Gateway = "10.0.0.1", VlanId = 10 };
        var subProd = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Prod-Server", NetworkAddress = "10.0.20.0", CidrMask = 24, Gateway = "10.0.20.1", VlanId = 20 };
        var subClient = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Client-WiFi", NetworkAddress = "10.0.100.0", CidrMask = 22, Gateway = "10.0.100.1", VlanId = 100 };
        context.Subnets.AddRange(subMgmt, subProd, subClient);

        // --- 3. HERSTELLER PORTFOLIO ---
        var manufacturers = new List<Manufacturer> {
            new() { Id = Guid.NewGuid(), Name = "Dell Technologies" },
            new() { Id = Guid.NewGuid(), Name = "Cisco Systems" },
            new() { Id = Guid.NewGuid(), Name = "Hewlett Packard Enterprise" },
            new() { Id = Guid.NewGuid(), Name = "IBM" },
            new() { Id = Guid.NewGuid(), Name = "Apple" },
            new() { Id = Guid.NewGuid(), Name = "Microsoft" },
            new() { Id = Guid.NewGuid(), Name = "Amazon Web Services" },
            new() { Id = Guid.NewGuid(), Name = "SAP SE" },
            new() { Id = Guid.NewGuid(), Name = "VMware" },
            new() { Id = Guid.NewGuid(), Name = "Palo Alto Networks" },
            new() { Id = Guid.NewGuid(), Name = "NetApp" },
            new() { Id = Guid.NewGuid(), Name = "APC by Schneider" },
            new() { Id = Guid.NewGuid(), Name = "Oracle" },
            new() { Id = Guid.NewGuid(), Name = "Bechtle AG" },
            new() { Id = Guid.NewGuid(), Name = "T-Systems" }
        };
        context.Manufacturers.AddRange(manufacturers);

        // --- 4. ASSET TYPEN (Vollständiger Scope inkl. neue Lokation/Org/Partner) ---
        var typeMap = new Dictionary<string, Guid>();
        string[] allTypeNames = { 
            // 1. Hardware
            "Laptop", "Desktop", "Workstation", "Tablet", "Smartphone",
            "Rack-Server", "Blade-Center", "Mainframe", "Backup-Appliance",
            "Rack", "PDU", "USV-Anlage", "Klimagerät",
            "SAN-Storage", "NAS-System", "Tape-Library", "NVMe-Array",
            "Netzwerkdrucker", "Multifunktionsgerät", "Dockingstation", "Monitor",
            // 2. Netzwerk
            "Core-Switch", "Access-Switch", "Router", "WLAN-Controller", "Access-Point",
            "Firewall", "Load-Balancer", "VPN-Gateway", "IDS-IPS-System",
            "WAN-Leitung", "Internet-Breakout", "SFP-Modul",
            // 3. Virtualisierung & Cloud
            "Hypervisor-Host", "VM-Cluster", "Resource-Pool",
            "Cloud-Instanz", "S3-Bucket", "Entra-ID-Tenant", "Cloud-VNET",
            "Kubernetes-Cluster", "K8s-Node", "K8s-Namespace", "Docker-Host",
            // 4. Software & OS
            "Betriebssystem", "Active-Directory-Domain", "DNS-DHCP-Server", "PKI-Dienst",
            "MS-SQL-Server", "Oracle-DB", "PostgreSQL-DB", "SAP-HANA",
            // 5. Apps
            "Exchange-Server", "SharePoint", "ERP-System", "CRM-System", "Webserver", "Message-Broker",
            "EDR-Software", "Backup-Software",
            // 6. Services & Lizenzen
            "Business-Service", "Software-Lizenz", "Wartungsvertrag", "SSL-Zertifikat",
            
            // NEU: 7. Lokations-Hierarchie
            "Land", "Bundesland", "Gemeinde", "Stadt", "Ortsteil", "Adresse", "Gebäude", "Etage", "Stockwerk", "Raum", "Stellplatz",
            
            // NEU: 8. Personen & Rollen
            "Person", "Kontakt", "Vorgesetzter", "Dienststellenleitung", "Standortleiter", "C-Level",
            
            // NEU: 9. Organisation
            "Gruppe", "Team", "Kostenstelle",
            
            // NEU: 10. Externe Partner & Lieferanten
            "Lieferant", "Vendor", "Dienstleister", "MSP", "Wartungspartner", "Hersteller"
        };

        foreach (var name in allTypeNames)
        {
            var id = Guid.NewGuid();
            context.AssetTypes.Add(new AssetType { Id = id, Name = name, IconKey = name });
            typeMap.Add(name, id);
        }
        await context.SaveChangesAsync();

        var assets = new List<Asset>();
        var ipAddresses = new List<IpAddress>();
        var rng = new Random();

        // --- 5. MASSEN-SEEDING (Dynamische Payloads je nach Kategorie) ---
        foreach (var typeName in allTypeNames)
        {
            // Generiere 3 bis 6 Instanzen pro Sub-Typ, um in Summe > 300 Assets zu haben
            int count = rng.Next(3, 7);
            for (int i = 1; i <= count; i++)
            {
                var assetId = Guid.NewGuid();
                var asset = new Asset
                {
                    Id = assetId,
                    TenantId = tenantId,
                    Name = $"{typeName.Replace("-", " ")} {i:D2}",
                    AssetTag = $"TAG-{typeName.Substring(0, Math.Min(3, typeName.Length)).ToUpper()}-{rng.Next(1000, 9999)}",
                    SerialNumber = $"SN-{rng.Next(100000, 999999)}",
                    AssetTypeId = typeMap[typeName],
                    LocationId = typeName.Contains("Server") || typeName.Contains("Switch") ? locRZ.Id : locHQ.Id,
                    LifecycleStatus = "Produktiv"
                };

                // JSON-Payload Logik (Spezifisch für die Ansicht in AssetDetails.razor)
                object dynamicData;

                if (typeName is "Land" or "Bundesland" or "Gemeinde" or "Stadt" or "Ortsteil" or "Adresse" or "Gebäude" or "Etage" or "Stockwerk" or "Raum" or "Stellplatz")
                {
                    dynamicData = new { Location = new { Country = "Deutschland", State = "Hessen", City = "Frankfurt a.M.", Building = "MainTower 1", Floor = $"Etage {rng.Next(1, 40)}", Room = $"Raum {rng.Next(100, 999)}", Rack = $"Rack 0{rng.Next(1, 9)}" } };
                }
                else if (typeName is "Person" or "Kontakt" or "Vorgesetzter" or "Dienststellenleitung" or "Standortleiter" or "C-Level")
                {
                    string[] titles = { "CIO", "CTO", "Head of IT", "Senior Admin", "Consultant" };
                    dynamicData = new { Organization = new { User = $"Mitarbeiter {i}", Manager = "Dr. Thomas Müller", CLevel = titles[rng.Next(titles.Length)], Team = "Executive Board", CostCenter = $"CC-{rng.Next(1000, 9999)}" } };
                }
                else if (typeName is "Gruppe" or "Team" or "Kostenstelle")
                {
                    dynamicData = new { Organization = new { Team = $"{typeName} Alpha", Manager = "Projektleiter", CostCenter = $"CC-{rng.Next(1000, 9999)}" } };
                }
                else if (typeName is "Lieferant" or "Vendor" or "Dienstleister" or "MSP" or "Wartungspartner" or "Hersteller")
                {
                    string[] msps = { "T-Systems", "Bechtle AG", "Cancom", "Computacenter" };
                    dynamicData = new { Partners = new { Vendor = msps[rng.Next(msps.Length)], MSP = "Local IT-Service GmbH", MaintenancePartner = "Dell ProSupport 24/7" } };
                }
                else if (typeName is "Rack-Server" or "Blade-Center" or "Mainframe" or "Backup-Appliance")
                {
                    dynamicData = new { System = new { Model = "Enterprise Gen10", Chassis = "2U Rackmount" }, Hardware = new { CPU = "2x Intel Xeon Platinum", RAM = "512 GB DDR4" }, Organization = new { Team = "Datacenter Ops" }, Partners = new { MaintenancePartner = "CarePack 24/7" } };
                }
                else if (typeName is "Laptop" or "Desktop" or "Workstation" or "Tablet" or "Smartphone")
                {
                    dynamicData = new { System = new { Model = "Latitude 7000 Series", Chassis = "Portable" }, Hardware = new { CPU = "Intel Core i7", RAM = "32 GB" }, Organization = new { User = $"User {rng.Next(100, 999)}", CostCenter = "CC-Client-IT" } };
                }
                else if (typeName is "SAN-Storage" or "NAS-System" or "Tape-Library" or "NVMe-Array")
                {
                    dynamicData = new { Storage = new { Capacity = $"{rng.Next(50, 500)} TB", Protocol = "iSCSI / FCP", Used = $"{rng.Next(10, 90)}%" }, System = new { Model = "All-Flash Array" } };
                }
                else if (typeName is "USV-Anlage" or "Klimagerät" or "PDU" or "Rack")
                {
                    dynamicData = new { Datacenter = new { Load = $"{rng.Next(20, 80)}%", BatteryStatus = "Healthy", RemainingTime = $"{rng.Next(15, 60)} min" }, Location = new { Room = "Serverraum 1.04" } };
                }
                else if (typeName.Contains("Cloud") || typeName.Contains("S3") || typeName.Contains("Tenant") || typeName.Contains("VNET"))
                {
                    string[] providers = { "AWS", "Microsoft Azure", "Google Cloud" };
                    dynamicData = new { Cloud = new { Provider = providers[rng.Next(providers.Length)], Region = "eu-central-1", InstanceType = "m5.large / D2s_v3" }, Organization = new { CostCenter = "CC-Cloud-Ops" } };
                }
                else if (typeName.Contains("Switch") || typeName.Contains("Router") || typeName.Contains("Firewall") || typeName.Contains("Access-Point") || typeName.Contains("Modul"))
                {
                    dynamicData = new { Network = new { Ports = "48x 10G", Firmware = $"v{rng.Next(7, 12)}.{rng.Next(1, 9)}", Uplink = "4x 100G" }, Partners = new { Vendor = "Cisco Partner DE" } };
                }
                else if (typeName is "Software-Lizenz" or "Wartungsvertrag" or "SSL-Zertifikat")
                {
                    dynamicData = new { Contract = new { Type = "Enterprise Agreement", Seats = $"{rng.Next(10, 1000)}", Renewal = DateTime.Now.AddMonths(rng.Next(1, 36)).ToString("yyyy-MM-dd") } };
                }
                else if (typeName.Contains("Service") || typeName.Contains("System") || typeName.Contains("Server") || typeName.Contains("Software"))
                {
                    dynamicData = new { Service = new { Criticality = "Mission Critical", SLA = "99.99%", Team = "Application Management" } };
                }
                else
                {
                    dynamicData = new { General = new { Note = "Standard Asset" } };
                }

                asset.DynamicDataJson = JsonSerializer.Serialize(dynamicData);
                assets.Add(asset);

                // Netzwerk-Konfiguration für IP-fähige Geräte (1:n Beziehung)
                if (typeName is "Rack-Server" or "Blade-Center" or "Laptop" or "Desktop" or "Workstation" or "Core-Switch" or "Access-Switch" or "Router" or "Firewall" or "Access-Point" or "Cloud-Instanz" or "Hypervisor-Host" or "Docker-Host")
                {
                    ipAddresses.Add(new IpAddress
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        SubnetId = subProd.Id,
                        AssignedAssetId = assetId,
                        Address = $"10.0.20.{rng.Next(2, 254)}",
                        Description = "Primary NIC",
                        MacAddress = $"00:50:56:{rng.Next(10, 99):X2}:{rng.Next(10, 99):X2}:{rng.Next(10, 99):X2}"
                    });
                }
            }
        }

        context.Assets.AddRange(assets);
        context.IpAddresses.AddRange(ipAddresses);
        await context.SaveChangesAsync();
    }
}