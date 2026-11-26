using EventUserApp.Pages;

namespace EventUserApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register the detail screen as a navigation route outside the tab layout.
        Routing.RegisterRoute(nameof(EventDetailsPage), typeof(EventDetailsPage));
    }
}
