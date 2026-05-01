// Dateipfad: src/InfraDesk.UI.Web/Services/InfraDeskAuthStateProvider.cs
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace InfraDesk.UI.Web.Services;

/// <summary>
/// Verwaltet den Login-Status für die Blazor-App. 
/// Nutzt ProtectedLocalStorage, damit der Login auch nach F5 (Reload) erhalten bleibt.
/// </summary>
public class InfraDeskAuthStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedLocalStorage _localStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public InfraDeskAuthStateProvider(ProtectedLocalStorage localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            // Versuch, die verschlüsselte Session aus dem Browser-Speicher zu lesen
            var result = await _localStorage.GetAsync<string>("user_session");
            if (result.Success && !string.IsNullOrEmpty(result.Value))
            {
                var session = JsonSerializer.Deserialize<UserSession>(result.Value);
                if (session != null)
                {
                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, session.Name),
                        new Claim(ClaimTypes.Email, session.Email),
                        new Claim(ClaimTypes.Role, session.Role),
                        new Claim("AllowedTenants", session.AllowedTenants)
                    }, "InfraDeskAuth");

                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
        }
        catch
        {
            // Kann beim Prerendering passieren, wenn JS noch nicht verfügbar ist
        }

        return new AuthenticationState(_anonymous);
    }

    // WICHTIG: Rückgabetyp muss Task sein, nicht void!
    public async Task MarkUserAsAuthenticated(string email, string role, string name, string allowedTenantsJson)
    {
        var session = new UserSession { Email = email, Role = role, Name = name, AllowedTenants = allowedTenantsJson };

        // Speichert die Daten verschlüsselt im Browser
        await _localStorage.SetAsync("user_session", JsonSerializer.Serialize(session));

        var identity = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, name),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim("AllowedTenants", allowedTenantsJson)
        }, "InfraDeskAuth");

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    // WICHTIG: Rückgabetyp muss Task sein, nicht void!
    public async Task MarkUserAsLoggedOut()
    {
        await _localStorage.DeleteAsync("user_session");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private class UserSession
    {
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public string Name { get; set; } = "";
        public string AllowedTenants { get; set; } = "";
    }
}