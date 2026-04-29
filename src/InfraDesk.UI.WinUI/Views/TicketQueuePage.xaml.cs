using Microsoft.UI.Xaml.Controls;
using InfraDesk.UI.WinUI.ViewModels;

namespace InfraDesk.UI.WinUI.Views;

public sealed partial class TicketQueuePage : Page
{
    // Diese Eigenschaft ist zwingend für {x:Bind}
    public TicketQueueViewModel ViewModel { get; }

    public TicketQueuePage()
    {
        ViewModel = new TicketQueueViewModel();
        this.InitializeComponent();
    }
}