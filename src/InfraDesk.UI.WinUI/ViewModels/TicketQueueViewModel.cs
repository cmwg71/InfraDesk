using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InfraDesk.Core.Entities;

namespace InfraDesk.UI.WinUI.ViewModels;

public partial class TicketQueueViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private List<Ticket> _allTickets = new(); // Cache für die Suche

    [ObservableProperty]
    private ObservableCollection<Ticket> _tickets = new();

    [ObservableProperty]
    private bool _isLoading;

    public TicketQueueViewModel()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7274/") };
        _ = LoadTicketsAsync();
    }

    [RelayCommand]
    public async Task LoadTicketsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Ticket>>("api/tickets");
            if (response != null)
            {
                _allTickets = response;
                UpdateDisplayList(_allTickets);
            }
        }
        catch { }
        finally { IsLoading = false; }
    }

    public void SearchTickets(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            UpdateDisplayList(_allTickets);
            return;
        }

        var filtered = _allTickets.Where(t =>
            (t.Title?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (t.Description?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

        UpdateDisplayList(filtered);
    }

    private void UpdateDisplayList(List<Ticket> list)
    {
        Tickets.Clear();
        foreach (var t in list) Tickets.Add(t);
    }
}