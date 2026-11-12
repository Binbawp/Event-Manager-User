namespace EventUserApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // The design is dark-only, so ignore the phone's light/dark setting
        UserAppTheme = AppTheme.Dark;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
