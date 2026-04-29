using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace InfraDesk.UI.WinUI.Converters;

/// <summary>
/// Konvertiert einen Double-Wert (z. B. Breite) in ein GridLength-Objekt.
/// Wird benötigt, um die Logo-Spalte dynamisch an das Menü zu binden.
/// </summary>
public class DoubleToGridLengthConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is double doubleValue)
        {
            return new GridLength(doubleValue);
        }
        return new GridLength(320); // Fallback-Breite
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is GridLength gridLength)
        {
            return gridLength.Value;
        }
        return 320.0;
    }
}