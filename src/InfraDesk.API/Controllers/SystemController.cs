// Dateipfad: src/InfraDesk.API/Controllers/SystemController.cs
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Net.Http.Headers;
using System.Text.Json;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class SystemController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IWebHostEnvironment _env;
    private const string GitHubApiUrl = "https://api.github.com/repos/cmwg71/InfraDesk/releases/latest";

    public SystemController(IHttpClientFactory httpClientFactory, IWebHostEnvironment env)
    {
        _httpClientFactory = httpClientFactory;
        _env = env;
    }

    /// <summary>
    /// GET /api/v1/system/version
    /// Liefert die aktuell installierte Server-Version dynamisch aus der Assembly.
    /// </summary>
    [HttpGet("version")]
    public IActionResult GetVersion()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0.0";
        return Ok(new { Version = version, Environment = _env.EnvironmentName });
    }

    /// <summary>
    /// GET /api/v1/system/check-update
    /// Prüft gegen den GitHub-Release-Channel. 
    /// Der Server fungiert hier als Proxy, um User-Agent Probleme im Client zu vermeiden.
    /// </summary>
    [HttpGet("check-update")]
    public async Task<IActionResult> CheckUpdate()
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, GitHubApiUrl);

            // GitHub API benötigt zwingend einen User-Agent
            request.Headers.Add("User-Agent", "InfraDesk-Backend-Service");

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }

            return StatusCode((int)response.StatusCode, "GitHub API konnte nicht erreicht werden.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Interner Fehler beim Update-Check: {ex.Message}");
        }
    }

    /// <summary>
    /// GET /api/v1/system/download/setup.exe
    /// Streamt den lokal auf dem Server liegenden Installer an den Client.
    /// </summary>
    [HttpGet("download/setup.exe")]
    public IActionResult DownloadSetup()
    {
        // Pfad zum Installer im Root-Verzeichnis oder einem 'downloads' Ordner
        var filePath = Path.Combine(_env.ContentRootPath, "downloads", "InfraDesk_Setup.exe");

        if (!System.IO.File.Exists(filePath))
        {
            // Fallback für Entwicklung / Erste Einrichtung
            return NotFound("Der Installer 'InfraDesk_Setup.exe' wurde noch nicht auf dem Server hinterlegt.");
        }

        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, "application/octet-stream", "InfraDesk_Setup.exe");
    }
}