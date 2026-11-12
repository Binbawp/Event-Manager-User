using EventUserApp.Pages;

namespace EventUserApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // The details page is not a tab, so its route is registered here
        Routing.RegisterRoute(nameof(EventDetailsPage), typeof(EventDetailsPage));
    }
}
