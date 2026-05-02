// Dateipfad: src/InfraDesk.API/Controllers/SearchController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InfraDesk.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InfraDesk.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public SearchController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SearchResultDto>>> Search([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q) || q.Length < 2) return Ok(new List<SearchResultDto>());

        var results = new List<SearchResultDto>();
        var query = q.ToLower();

        // 1. Assets (Sicherer Check ohne JSONB ToLower Absturz)
        var assets = await _context.Assets
            .Include(a => a.AssetType)
            .Include(a => a.Owner)
            .Where(a => a.Name.ToLower().Contains(query)
                     || (a.AssetTag != null && a.AssetTag.ToLower().Contains(query))
                     || (a.SerialNumber != null && a.SerialNumber.ToLower().Contains(query)))
            .Take(20)
            .ToListAsync();

        results.AddRange(assets.Select(a => new SearchResultDto
        {
            Id = a.Id,
            Type = "Asset",
            Title = a.Name,
            Subtitle = $"Tag: {a.AssetTag} | Besitzer: {a.Owner?.LastName}",
            Url = $"/assets/{a.Id}",
            Status = a.LifecycleStatus ?? "---",
            Icon = "Computer"
        }));

        // 2. Tickets
        var tickets = await _context.Tickets
            .Include(t => t.Requester)
            .Where(t => t.Title.ToLower().Contains(query)
                     || t.TicketNumber.ToString().Contains(query))
            .Take(20)
            .ToListAsync();

        results.AddRange(tickets.Select(t => new SearchResultDto
        {
            Id = t.Id,
            Type = "Ticket",
            Title = $"#{t.TicketNumber} - {t.Title}",
            Subtitle = $"Kontakt: {t.Requester?.LastName}",
            Url = $"/tickets/{t.Id}",
            Status = t.Status ?? "---",
            Icon = "ConfirmationNumber"
        }));

        return Ok(results.OrderBy(r => r.Type).ToList());
    }
}

public class SearchResultDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
}