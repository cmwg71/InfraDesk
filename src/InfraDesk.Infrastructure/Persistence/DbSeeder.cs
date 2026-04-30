// Dateipfad: src/InfraDesk.Infrastructure/Persistence/DbSeeder.cs
using InfraDesk.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace InfraDesk.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Assets.AnyAsync()) return;

        var rnd = new Random();
        var tenantId = Guid.NewGuid();

        context.Tenants.Add(new Tenant { Id = tenantId, Name = "Musterfirma Enterprise GmbH", Domain = "musterfirma.local" });

        var locs = new List<Location>
        {
            new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Hauptsitz Berlin", Address = "Musterstr. 1" },
            new Location { Id = Guid.NewGuid(), TenantId = tenantId, Name = "RZ Frankfurt", Address = "Cloud-Park 42" }
        };
        context.Locations.AddRange(locs);

        var teamIt = new Team { Id = Guid.NewGuid(), TenantId = tenantId, Name = "IT Operations" };
        context.Teams.Add(teamIt);

        var persons = new List<Person>();
        for (int i = 0; i < 10; i++)
        {
            persons.Add(new Person { Id = Guid.NewGuid(), TenantId = tenantId, FirstName = "Max", LastName = $"Admin {i}", TeamId = teamIt.Id });
        }
        context.Persons.AddRange(persons);

        var manDell = new Manufacturer { Id = Guid.NewGuid(), Name = "Dell Technologies" };
        var manCisco = new Manufacturer { Id = Guid.NewGuid(), Name = "Cisco Systems" };
        context.Manufacturers.AddRange(manDell, manCisco);

        var typeServer = new AssetType { Id = Guid.NewGuid(), Name = "Server", IconKey = "Server" };
        var typeWorkstation = new AssetType { Id = Guid.NewGuid(), Name = "Workstation", IconKey = "Computer" };
        var typeSwitch = new AssetType { Id = Guid.NewGuid(), Name = "Switch", IconKey = "Router" };
        context.AssetTypes.AddRange(typeServer, typeWorkstation, typeSwitch);

        // --- NEU: IPAM SUBNETZE ---
        var subServer = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Server-Netz RZ1", NetworkAddress = "10.10.20.0", CidrMask = 24, Gateway = "10.10.20.1" };
        var subClient = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Client-Netz Berlin", NetworkAddress = "10.10.30.0", CidrMask = 24, Gateway = "10.10.30.1" };
        var subSwitch = new Subnet { Id = Guid.NewGuid(), TenantId = tenantId, Name = "Switch-Mgmt", NetworkAddress = "10.10.10.0", CidrMask = 24, Gateway = "10.10.10.1" };
        context.Subnets.AddRange(subServer, subClient, subSwitch);

        var assets = new List<Asset>();
        var ips = new List<IpAddress>();

        // 1. Server generieren (mit IPAM Verknüpfung)
        for (int i = 10; i <= 60; i++)
        {
            var assetId = Guid.NewGuid();
            var isWin = rnd.Next(2) == 0;

            // HINWEIS: IP und MAC sind NICHT mehr im JSON!
            var json = new
            {
                General = new { OS = isWin ? "Windows Server 2022" : "Ubuntu 22.04 LTS", Uptime = $"{rnd.Next(1, 100)} days" },
                Hardware = new { Processor = "Intel Xeon Gold", Memory = "64 GB" },
                Applications = new[] { new { Name = "Action1 Agent", Version = "5.2", Publisher = "Action1" } }
            };

            assets.Add(new Asset
            {
                Id = assetId,
                TenantId = tenantId,
                AssetTypeId = typeServer.Id,
                ManufacturerId = manDell.Id,
                LocationId = locs[1].Id,
                OwnerId = persons[0].Id,
                Name = isWin ? $"S-DC{i:D2}" : $"L-WEB{i:D2}",
                InventoryNumber = $"INV-SRV-{1000 + i}",
                DynamicDataJson = JsonSerializer.Serialize(json)
            });

            // IP Adresse relational anlegen und verknüpfen
            ips.Add(new IpAddress { Id = Guid.NewGuid(), TenantId = tenantId, Address = $"10.10.20.{i}", MacAddress = $"BC:24:11:F0:38:{i:D2}", SubnetId = subServer.Id, AssignedAssetId = assetId });
        }

        // 2. Switches generieren (mit IPAM Verknüpfung)
        for (int i = 10; i <= 60; i++)
        {
            var assetId = Guid.NewGuid();
            var json = new { General = new { Firmware = "Cisco IOS XE 17.03" }, Hardware = new { Ports = 48, Uplink = "10G SFP+" } };

            assets.Add(new Asset
            {
                Id = assetId,
                TenantId = tenantId,
                AssetTypeId = typeSwitch.Id,
                ManufacturerId = manCisco.Id,
                LocationId = locs[0].Id,
                Name = $"SW-CORE-{i:D2}",
                InventoryNumber = $"INV-NET-{3000 + i}",
                DynamicDataJson = JsonSerializer.Serialize(json)
            });

            ips.Add(new IpAddress { Id = Guid.NewGuid(), TenantId = tenantId, Address = $"10.10.10.{i}", MacAddress = $"00:1A:2B:3C:4D:{i:D2}", SubnetId = subSwitch.Id, AssignedAssetId = assetId });
        }

        context.Assets.AddRange(assets);
        context.IpAddresses.AddRange(ips);
        await context.SaveChangesAsync();
    }
}