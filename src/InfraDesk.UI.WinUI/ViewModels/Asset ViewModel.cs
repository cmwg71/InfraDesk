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

public partial class AssetViewModel : ObservableObject
{
    private readonly HttpClient _httpClient;
    private List<Asset> _allAssets = new();

    [ObservableProperty]
    private ObservableCollection<Asset> _assets = new();

    [ObservableProperty]
    private bool _isLoading;

    public AssetViewModel()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7274/") };
        _ = LoadAssetsAsync();
    }

    [RelayCommand]
    public async Task LoadAssetsAsync()
    {
        IsLoading = true;
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Asset>>("api/assets");
            if (response != null)
            {
                _allAssets = response;
                UpdateDisplayList(_allAssets);
            }
        }
        catch (Exception)
        {
            // Fehlerbehandlung
        }
        finally
        {
            IsLoading = false;
        }
    }

    public void SearchAssets(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            UpdateDisplayList(_allAssets);
            return;
        }

        var filtered = _allAssets.Where(a =>
            (a.Name?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (a.SerialNumber?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (a.InventoryNumber?.Contains(query, StringComparison.OrdinalIgnoreCase) ?? false)).ToList();

        UpdateDisplayList(filtered);
    }

    private void UpdateDisplayList(List<Asset> list)
    {
        Assets.Clear();
        foreach (var item in list)
        {
            Assets.Add(item);
        }
    }
}