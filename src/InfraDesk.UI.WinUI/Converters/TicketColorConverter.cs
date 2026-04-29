using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace InfraDesk.UI.WinUI.Converters;

public class TicketColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        string input = value?.ToString()?.ToLower() ?? "";

        // Logik für Priorität-Farben
        if (parameter?.ToString() == "priority")
        {
            return input switch
            {
                "high" => new SolidColorBrush(Color.FromArgb(255, 232, 17, 35)),   // Rot
                "medium" => new SolidColorBrush(Color.FromArgb(255, 255, 185, 0)), // Gelb
                "normal" => new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)), // Grün
                "low" => new SolidColorBrush(Color.FromArgb(255, 0, 120, 212)),    // Blau
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        // Logik für Status-Farben
        if (parameter?.ToString() == "status")
        {
            return input switch
            {
                "open" => new SolidColorBrush(Color.FromArgb(255, 16, 124, 16)),   // Grün
                "closed" => new SolidColorBrush(Color.FromArgb(255, 122, 122, 122)), // Grau
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        return new SolidColorBrush(Colors.Transparent);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
}