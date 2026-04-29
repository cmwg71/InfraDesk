using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using InfraDesk.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace InfraDesk.UI.WinUI.Views;

public sealed partial class AssetDetailPage : Page, INotifyPropertyChanged
{
    private readonly HttpClient _httpClient;
    
    // Fix für CS9035: Wir initialisieren das Objekt mit leeren Werten für alle 'required' Felder.
    // Dies hilft dem Compiler, falls er während der Generierung Dummy-Instanzen benötigt.
    public Asset SelectedAsset { get; private set; } = new Asset 
    { 
        Name = string.Empty, 
        TenantId = Guid.Empty, 
        AssetTypeId = Guid.Empty 
    };
    
    public List<Location> AvailableLocations { get; private set; } = new();
    public List<Person> AvailablePeople { get; private set; } = new();
    public List<dynamic> AssetHistory { get; private set; } = new();
    public List<dynamic> AssetLinks { get; private set; } = new();

    private Dictionary<string, string> _allAttributes = new();

    private bool _isEditMode = false;
    public bool IsEditMode 
    { 
        get => _isEditMode; 
        set 
        { 
            _isEditMode = value; 
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(IsReadOnlyMode)); 
        } 
    }
    public bool IsReadOnlyMode => !IsEditMode;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public AssetDetailPage()
    {
        this.InitializeComponent();
        _httpClient = new HttpClient { 
            BaseAddress = new Uri("https://localhost:7274/"),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    protected override async void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is Asset asset)
        {
            SelectedAsset = asset;
            if (!string.IsNullOrEmpty(asset.DynamicDataJson))
            {
                try { _allAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(asset.DynamicDataJson) ?? new(); }
                catch { _allAttributes = new(); }
            }
            
            try 
            {
                if (FindName("IsLoadingOverlay") is ProgressRing loader)
                {
                    loader.IsActive = true;
                    loader.Visibility = Visibility.Visible;
                }

                await Task.WhenAll(LoadMetadataAsync(), LoadHistoryAndLinksAsync());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Fehler beim Laden der Asset-Details: {ex.Message}");
            }
            finally 
            {
                if (FindName("IsLoadingOverlay") is ProgressRing loaderOff)
                {
                    loaderOff.IsActive = false;
                    loaderOff.Visibility = Visibility.Collapsed;
                }
                
                if (DetailNav != null && DetailNav.MenuItems.Count > 0)
                {
                    DetailNav.SelectedItem = DetailNav.MenuItems[0];
                }
            }
        }
    }

    private async Task LoadMetadataAsync()
    {
        try
        {
            AvailableLocations = await _httpClient.GetFromJsonAsync<List<Location>>("api/locations") ?? new();
            AvailablePeople = await _httpClient.GetFromJsonAsync<List<Person>>("api/persons") ?? new();
            
            OnPropertyChanged(nameof(AvailableLocations));
            OnPropertyChanged(nameof(AvailablePeople));
        }
        catch (Exception ex) { Debug.WriteLine($"Metadaten-Fehler: {ex.Message}"); }
    }

    private async Task LoadHistoryAndLinksAsync()
    {
        try
        {
            AssetHistory = await _httpClient.GetFromJsonAsync<List<dynamic>>($"api/audit/asset/{SelectedAsset.Id}") ?? new();
            AssetLinks = await _httpClient.GetFromJsonAsync<List<dynamic>>($"api/assets/{SelectedAsset.Id}/links") ?? new();
            
            OnPropertyChanged(nameof(AssetHistory));
            OnPropertyChanged(nameof(AssetLinks));
        }
        catch (Exception ex) { Debug.WriteLine($"Historie/Beziehungs-Fehler: {ex.Message}"); }
    }

    private void DetailNav_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        var tag = (args.SelectedItem as NavigationViewItem)?.Tag?.ToString();
        UpdateContent(tag);
    }

    private void UpdateContent(string? tag)
    {
        if (tag == "General")
        {
            DetailContentPresenter.ContentTemplate = (DataTemplate)Resources["GeneralTemplate"];
            DetailContentPresenter.Content = this; 
        }
        else if (tag == "Hardware")
        {
            var hwFields = _allAttributes.Where(x => !x.Key.ToLower().Contains("ip") && !x.Key.ToLower().Contains("mac"));
            DetailContentPresenter.ContentTemplate = null;
            DetailContentPresenter.Content = CreateAttributeList(hwFields);
        }
        else if (tag == "Network")
        {
            var netFields = _allAttributes.Where(x => x.Key.ToLower().Contains("ip") || x.Key.ToLower().Contains("mac"));
            DetailContentPresenter.ContentTemplate = null;
            DetailContentPresenter.Content = CreateAttributeList(netFields);
        }
        else if (tag == "History")
        {
            DetailContentPresenter.ContentTemplate = null;
            DetailContentPresenter.Content = CreateHistoryList();
        }
        else if (tag == "Links")
        {
            DetailContentPresenter.ContentTemplate = null;
            DetailContentPresenter.Content = CreateLinkList();
        }
    }

    private UIElement CreateHistoryList()
    {
        var sp = new StackPanel { Spacing = 10 };
        if (AssetHistory == null || !AssetHistory.Any()) return new TextBlock { Text = "Keine Historie vorhanden.", Opacity = 0.5 };
        
        foreach (var log in AssetHistory)
        {
            var border = new Border 
            { 
                Padding = new Thickness(16), 
                Background = (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["LayerFillColorDefaultBrush"], 
                CornerRadius = new CornerRadius(8), 
                BorderThickness = new Thickness(1), 
                BorderBrush = (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["CardStrokeColorDefaultBrush"] 
            };
            
            var content = new StackPanel { Spacing = 4 };
            content.Children.Add(new TextBlock { Text = $"{log.timestamp:g} - {log.user}", FontWeight = Microsoft.UI.Text.FontWeights.Bold, FontSize = 12 });
            content.Children.Add(new TextBlock { Text = $"{log.action}", Foreground = (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["SystemAccentColor"] });
            content.Children.Add(new TextBlock { Text = $"{log.details}", Opacity = 0.8, TextWrapping = TextWrapping.Wrap });
            border.Child = content;
            sp.Children.Add(border);
        }
        return sp;
    }

    private UIElement CreateLinkList()
    {
        var sp = new StackPanel { Spacing = 8 };
        if (AssetLinks == null || !AssetLinks.Any()) return new TextBlock { Text = "Keine aktiven Beziehungen.", Opacity = 0.5 };
        
        foreach (var link in AssetLinks)
        {
            var linkGrid = new Grid 
            { 
                Padding = new Thickness(12), 
                Background = (SolidColorBrush)Microsoft.UI.Xaml.Application.Current.Resources["LayerFillColorDefaultBrush"], 
                CornerRadius = new CornerRadius(4) 
            };
            
            linkGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            linkGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            linkGrid.Children.Add(new TextBlock { Text = link.type, Opacity = 0.6 });
            var target = new HyperlinkButton { Content = link.targetName };
            Grid.SetColumn(target, 1);
            linkGrid.Children.Add(target);
            sp.Children.Add(linkGrid);
        }
        return sp;
    }

    private StackPanel CreateAttributeList(IEnumerable<KeyValuePair<string, string>> items)
    {
        var sp = new StackPanel { Spacing = 12 };
        foreach (var item in items)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(180) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.Children.Add(new TextBlock { Text = item.Key, FontWeight = Microsoft.UI.Text.FontWeights.SemiBold, Opacity = 0.6 });
            var val = new TextBlock { Text = item.Value, IsTextSelectionEnabled = true };
            Grid.SetColumn(val, 1);
            grid.Children.Add(val);
            sp.Children.Add(grid);
        }
        return sp;
    }

    private void ToggleEdit_Click(object sender, RoutedEventArgs e)
    {
        IsEditMode = !IsEditMode;
        if (VisualStateManager.GoToState(this, IsEditMode ? "EditMode" : "ReadOnly", true))
        {
            UpdateContent((DetailNav.SelectedItem as NavigationViewItem)?.Tag?.ToString() ?? "General");
        }
    }

    private async void SaveAsset_Click(object sender, RoutedEventArgs e)
    {
        try { 
            var response = await _httpClient.PutAsJsonAsync($"api/assets/{SelectedAsset.Id}", SelectedAsset); 
            if (response.IsSuccessStatusCode)
            {
                ToggleEdit_Click(this, new RoutedEventArgs());
            }
        }
        catch (Exception ex) {
            Debug.WriteLine($"Fehler beim Speichern: {ex.Message}");
        }
    }

    private void GoBack_Click(object sender, RoutedEventArgs e)
    {
        if (Frame != null && Frame.CanGoBack)
        {
            Frame.GoBack();
        }
    }
}