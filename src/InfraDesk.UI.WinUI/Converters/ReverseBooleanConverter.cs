using System;
using Microsoft.UI.Xaml.Data;

namespace InfraDesk.UI.WinUI.Converters;

/// <summary>
/// Konvertiert einen booleschen Wert in sein Gegenteil (true -> false und umgekehrt).
/// Wird für den Umschalter zwischen Karten- und Tabellenansicht benötigt.
/// </summary>
public class ReverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return value is bool boolValue ? !boolValue : false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is bool boolValue ? !boolValue : false;
    }
}