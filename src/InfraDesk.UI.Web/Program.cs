// Dateipfad: src/InfraDesk.UI.Web/Program.cs
using InfraDesk.UI.Web.Components;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Fügt Razor Components für den Server-Modus hinzu
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. MudBlazor UI Services registrieren
builder.Services.AddMudServices();

// 2. HttpClient registrieren, um mit unserer bestehenden InfraDesk.API zu kommunizieren
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7274/") });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();