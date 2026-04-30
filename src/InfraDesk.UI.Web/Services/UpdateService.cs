// Dateipfad: src/InfraDesk.UI.Web/Services/UpdateService.cs
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace InfraDesk.UI.Web.Services;

public class UpdateService
{
    private readonly HttpClient _httpClient;
    // Hier den echten GitHub-Pfad eintragen
    private const string GitHubApiUrl = "https://api.github.com/repos/cmwg71/InfraDesk/releases/latest";

    public UpdateService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<GitHubRelease?> GetLatestReleaseAsync()
    {
        try
        {
            // GitHub benötigt zwingend einen User-Agent Header
            var request = new HttpRequestMessage(HttpMethod.Get, GitHubApiUrl);
            request.Headers.Add("User-Agent", "InfraDesk-Update-Checker");

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GitHubRelease>();
            }
        }
        catch
        {
            // Fehler beim Abruf (z.B. Offline oder API-Limit erreicht)
        }
        return null;
    }
}

public class GitHubRelease
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