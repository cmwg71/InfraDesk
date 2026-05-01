// Dateipfad: src/InfraDesk.Worker/Program.cs
using InfraDesk.Worker;

var builder = Host.CreateApplicationBuilder(args);

// Konfiguration des HTTP-Clients für die Kommunikation mit dem InfraDesk Backend
builder.Services.AddHttpClient("InfraDeskAPI", client =>
{
    // HINWEIS: Bei Remote-Workern muss hier die IP/URL des Servers eingetragen werden.
    client.BaseAddress = new Uri("https://localhost:7274/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    // Ignoriert SSL-Zertifikatsfehler (Nur für lokale Entwicklung mit HTTPS)
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});

// Registrierung des eigentlichen Hintergrunddienstes
builder.Services.AddHostedService<DiscoveryWorker>();

var host = builder.Build();
host.Run();