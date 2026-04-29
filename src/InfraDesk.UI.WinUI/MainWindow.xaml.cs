using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using InfraDesk.UI.WinUI.Views;
using System;

namespace InfraDesk.UI.WinUI;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        this.InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);
        ContentFrame.Navigate(typeof(DashboardPage));
    }

    private void GlobalSearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        // Wenn wir auf der Ticket-Queue Seite sind, rufen wir dort die Suche auf
        if (ContentFrame.Content is TicketQueuePage page)
        {
            page.ViewModel.SearchTickets(args.QueryText);
        }
        if (ContentFrame.Content is AssetPage assetPage)
        {
            assetPage.ViewModel.SearchAssets(args.QueryText);
        }
    }

    private void RootNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer?.Tag is string tag)
        {
            Type pageType = tag == "TicketQueuePage" ? typeof(TicketQueuePage) : typeof(DashboardPage);
            ContentFrame.Navigate(pageType);
        }
    }
}