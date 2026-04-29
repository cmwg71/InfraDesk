using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using InfraDesk.UI.WinUI.ViewModels;

namespace InfraDesk.UI.WinUI.Views;

/// <summary>
/// Die Code-Behind Klasse für die Asset-Übersicht.
/// WICHTIG: Muss 'partial' sein, damit InitializeComponent gefunden wird.
/// </summary>
public sealed partial class AssetPage : Page
{
    public AssetViewModel ViewModel { get; }

    public AssetPage()
    {
        // Initialisiere das ViewModel für die Datenbindung
        ViewModel = new AssetViewModel();

        // Diese Methode wird vom XAML-Compiler automatisch generiert
        this.InitializeComponent();
    }
}