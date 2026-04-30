// Dateipfad: src/InfraDesk.UI.Web/Services/UpdateService.cs
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace InfraDesk.UI.Web.Services;

public class UpdateService
{
    private readonly HttpClient _httpClient;
    private const string GitHubApiUrl = "https://api.github.com/repos/cmwg71/InfraDesk/releases/latest";

    public UpdateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Ruft das neueste Release von GitHub ab.
    /// Akzeptiert nun einen optionalen Token für private Repositories.
    /// </summary>
    public async Task<GitHubReleaseData?> GetLatestReleaseAsync(string? token = null)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, GitHubApiUrl);

            // GitHub API benötigt zwingend einen User-Agent
            request.Headers.Add("User-Agent", "InfraDesk-App");

            // Falls ein Token vorhanden ist (für private Repos), fügen wir ihn hinzu
            if (!string.IsNullOrWhiteSpace(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GitHubReleaseData>();
            }
        }
        catch (Exception)
        {
            // Fehlerbehandlung (z.B. Offline oder API-Limit)
        }
        return null;
    }
}

/// <summary>
/// DTO für die GitHub-Release-Antwort. 
/// Umbenannt in GitHubReleaseData, um Konflikte mit Razor-Tags zu vermeiden.
/// </summary>
public class GitHubReleaseData
{
    [JsonPropertyName("tag_name")]
    public string TagName { get; set; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; set; } = string.Empty;

    [JsonPropertyName("published_at")]
    public DateTime PublishedAt { get; set; }

    [JsonPropertyName("body")]
    public string ReleaseNotes { get; set; } = string.Empty;
}