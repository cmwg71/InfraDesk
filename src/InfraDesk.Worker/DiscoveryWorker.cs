// Dateipfad: src/InfraDesk.Worker/DiscoveryWorker.cs
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Http.Json;
using ArpLookup;

namespace InfraDesk.Worker;

public class DiscoveryWorker : BackgroundService
{
    private readonly ILogger<DiscoveryWorker> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public DiscoveryWorker(ILogger<DiscoveryWorker> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("InfraDesk Discovery Worker (Ping & ArpLookup Engine) gestartet.");

        var client = _httpClientFactory.CreateClient("InfraDeskAPI");

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("=================================================");
            _logger.LogInformation("Starte neuen Discovery Zyklus: {time}", DateTimeOffset.Now);

            try
            {
                var subnets = await client.GetFromJsonAsync<List<SubnetDto>>("api/ipam/subnets", stoppingToken);

                if (subnets != null)
                {
                    foreach (var subnet in subnets)
                    {
                        if (!subnet.IsFullScanEnabled)
                        {
                            _logger.LogInformation("Überspringe Subnetz {Name} (Scan deaktiviert).", subnet.Name);
                            continue;
                        }

                        _logger.LogInformation("Lade IPs für Subnetz {Name} ({Network})...", subnet.Name, subnet.NetworkAddress);

                        var ips = await client.GetFromJsonAsync<List<IpDto>>($"api/ipam/subnets/{subnet.Id}/ips", stoppingToken);
                        if (ips == null || !ips.Any()) continue;

                        var results = new List<PingScanResultDto>();
                        var tasks = new List<Task>();
                        var semaphore = new SemaphoreSlim(15);

                        foreach (var ip in ips)
                        {
                            tasks.Add(Task.Run(async () =>
                            {
                                await semaphore.WaitAsync(stoppingToken);
                                try
                                {
                                    using var ping = new Ping();
                                    var reply = await ping.SendPingAsync(ip.Address, 2000);

                                    if (reply.Status == IPStatus.Success)
                                    {
                                        // NEU: Extrem saubere ARP-Abfrage via NuGet Package
                                        string? macString = null;
                                        if (IPAddress.TryParse(ip.Address, out var ipAddr))
                                        {
                                            var macObj = await Arp.LookupAsync(ipAddr);
                                            if (macObj != null)
                                            {
                                                // Konvertiere die Bytes in das Format 00:11:22:33:44:55
                                                macString = string.Join(":", macObj.GetAddressBytes().Select(b => b.ToString("X2")));
                                            }
                                        }

                                        lock (results)
                                        {
                                            results.Add(new PingScanResultDto
                                            {
                                                IpAddress = ip.Address,
                                                IsAlive = true,
                                                RoundtripTime = reply.RoundtripTime,
                                                MacAddress = macString
                                            });
                                        }
                                    }
                                }
                                catch { }
                                finally
                                {
                                    semaphore.Release();
                                }
                            }, stoppingToken));
                        }

                        await Task.WhenAll(tasks);

                        if (results.Any())
                        {
                            int macCount = results.Count(r => !string.IsNullOrEmpty(r.MacAddress));
                            _logger.LogInformation(">>> ERFOLG: {Count} Hosts online, davon {MacCount} mit erkannter MAC-Adresse! Übertrage...", results.Count, macCount);

                            var response = await client.PostAsJsonAsync($"api/ipam/subnets/{subnet.Id}/scan-results", results, stoppingToken);
                            if (!response.IsSuccessStatusCode) _logger.LogError("Fehler beim Speichern in API.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Verbindungsfehler zur API.");
            }

            _logger.LogInformation("Zyklus beendet. Nächster Scan in 2 Minuten...");
            _logger.LogInformation("=================================================");

            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }
}

public class SubnetDto { public Guid Id { get; set; } public string Name { get; set; } = ""; public string NetworkAddress { get; set; } = ""; public bool IsFullScanEnabled { get; set; } }
public class IpDto { public string Address { get; set; } = ""; }
public class PingScanResultDto { public string IpAddress { get; set; } = ""; public bool IsAlive { get; set; } public long RoundtripTime { get; set; } public string? MacAddress { get; set; } }