// Dateipfad: src/InfraDesk.UI.Web/Program.cs
using InfraDesk.UI.Web.Components;
using InfraDesk.UI.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Blazor Server Komponenten mit Interaktivität hinzufügen
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2. MudBlazor Services registrieren (erforderlich für UI-Komponenten)
builder.Services.AddMudServices();

// 3. Eigene Services registrieren
builder.Services.AddScoped<ThemeService>();

// FIX: Der UpdateService fehlte in der Registrierung! 
// Wir registrieren ihn hier als Typed HttpClient.
builder.Services.AddHttpClient<UpdateService>();

// 4. Authentifizierung (IAM / RBAC) registrieren
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<InfraDeskAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<InfraDeskAuthStateProvider>());

// 5. HttpClient Registrierung für die Kommunikation mit der API
builder.Services.AddHttpClient("InfraDeskAPI", client =>
{
    // Die URL sollte dem Port deines API-Projekts entsprechen
    client.BaseAddress = new Uri("https://localhost:7274/");
});

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("InfraDeskAPI"));

var app = builder.Build();

// Middleware Pipeline konfigurieren
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// WICHTIG für .NET 8: Antiforgery muss vor den Komponenten stehen
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();