// Dateipfad: src/InfraDesk.UI.Web/Services/ThemeService.cs
using System;

namespace InfraDesk.UI.Web.Services;

public class ThemeService
{
    public ThemeConfig Config { get; private set; } = new();
    public event Action? OnThemeChanged;

    public void UpdateConfig(ThemeConfig newConfig)
    {
        Config = newConfig;
        NotifyChanged();
    }

    public void NotifyChanged() => OnThemeChanged?.Invoke();
}

public class ThemeConfig
{
    // Basis-Farben
    public string PrimaryColor { get; set; } = "#1976D2";
    public string SecondaryColor { get; set; } = "#424242";

    // Oberflächen & Hintergründe
    public string BackgroundColor { get; set; } = "#f5f5f5";
    public string SurfaceColor { get; set; } = "#ffffff";

    // Spezifische Bereiche (AppBar & Menü)
    public string AppbarColor { get; set; } = "#333333";
    public string AppbarText { get; set; } = "#ffffff";

    // Menüspalte (Drawer) - Hier lagen die fehlenden Verknüpfungen
    public string DrawerColor { get; set; } = "#ffffff";
    public string DrawerText { get; set; } = "#424242";
    public string DrawerIcon { get; set; } = "#1976D2";

    // Texte allgemein
    public string TextPrimary { get; set; } = "#424242";
    public string TextSecondary { get; set; } = "#757575";

    // Typografie
    public string FontFamily { get; set; } = "'Roboto', sans-serif";
    public int BaseFontSize { get; set; } = 14;

    // Status
    public bool IsDark { get; set; } = false;
}