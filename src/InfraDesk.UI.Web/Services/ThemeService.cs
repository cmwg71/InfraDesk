// Dateipfad: src/InfraDesk.UI.Web/Services/ThemeService.cs
using MudBlazor;
using System;
using System.Globalization;

namespace InfraDesk.UI.Web.Services;

/// <summary>
/// Konfigurationsklasse für die dynamische Design-Gestaltung (Themes).
/// </summary>
public class ThemeConfig
{
    public bool IsDark { get; set; } = false;
    public string PrimaryColor { get; set; } = "#004a99";
    public string SecondaryColor { get; set; } = "#5e6e73";
    public string BackgroundColor { get; set; } = "#f4f4f4";
    public string SurfaceColor { get; set; } = "#ffffff";
    public string AppbarColor { get; set; } = "#004a99";
    public string AppbarText { get; set; } = "#ffffff";
    public string DrawerColor { get; set; } = "#f0f0f0";
    public string DrawerText { get; set; } = "#333333";
    public string DrawerIcon { get; set; } = "#5e6e73";
    public string TextPrimary { get; set; } = "#333333";
    public string FontFamily { get; set; } = "Roboto";
    public int BaseFontSize { get; set; } = 14;
}

/// <summary>
/// Verwaltet das UI-Design und die Enterprise-Zeitlogik (UTC zu Lokal).
/// </summary>
public class ThemeService
{
    // Standard-Zeitzone (wird aus den Systemeinstellungen geladen)
    private string _systemTimeZoneId = "W. Europe Standard Time";

    // Konfiguration für Layouts und Settings-Seite
    public ThemeConfig Config { get; private set; } = new ThemeConfig();

    // Event, um Komponenten über Änderungen am Design zu benachrichtigen
    public event Action? OnThemeChanged;

    public void UpdateConfig(ThemeConfig config)
    {
        Config = config;
        OnThemeChanged?.Invoke();
    }

    public MudTheme CurrentTheme { get; } = new MudTheme()
    {
        PaletteLight = new PaletteLight()
        {
            Primary = "#004a99",      // Enterprise Blue
            Secondary = "#5e6e73",    // Slate Gray
            AppbarBackground = "#004a99",
            AppbarText = "#ffffff",
            Surface = "#ffffff",
            Background = "#f4f4f4",
            Error = "#c62828",
            Success = "#2e7d32",
            Warning = "#ff9800",
            Info = "#1976d2",
            Divider = "#ccc",
            TextPrimary = "#333333",
            DrawerBackground = "#f0f0f0",
            DrawerText = "#333333",
            DrawerIcon = "#5e6e73",
            ActionDefault = "#004a99"
        },
        LayoutProperties = new LayoutProperties()
        {
            DefaultBorderRadius = "4px"
        }
    };

    /// <summary>
    /// Formatiert ein Datum für die ITSM-Ansicht und rechnet UTC in die lokale Zeit um.
    /// </summary>
    public string FormatDate(DateTime? utcDate, string format = "dd.MM.yyyy HH:mm")
    {
        if (!utcDate.HasValue) return "---";

        try
        {
            var targetZone = TimeZoneInfo.FindSystemTimeZoneById(_systemTimeZoneId);
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcDate.Value, targetZone);

            return localTime.ToString(format, CultureInfo.GetCultureInfo("de-DE"));
        }
        catch
        {
            return utcDate.Value.ToLocalTime().ToString(format);
        }
    }

    public void SetSystemTimeZone(string timeZoneId)
    {
        if (!string.IsNullOrEmpty(timeZoneId))
        {
            _systemTimeZoneId = timeZoneId;
        }
    }
}