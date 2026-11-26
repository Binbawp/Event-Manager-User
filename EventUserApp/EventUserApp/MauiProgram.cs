using EventUserApp.Pages;
using EventUserApp.Services;
using Microsoft.Extensions.Logging;

namespace EventUserApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Register application services and pages for dependency injection.
        builder.Services.AddSingleton<EventService>();
        builder.Services.AddTransient<EventsPage>();
        builder.Services.AddTransient<FavouritesPage>();
        builder.Services.AddTransient<EventDetailsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
