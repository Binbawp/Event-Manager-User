namespace EventUserApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Enforce the application's dark-only theme regardless of the device setting.
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
