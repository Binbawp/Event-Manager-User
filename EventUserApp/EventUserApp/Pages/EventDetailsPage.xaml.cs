using EventUserApp.Models;

namespace EventUserApp.Pages;

/// <summary>
/// Receives event details through Shell navigation and presents them on the details page.
/// </summary>
[QueryProperty(nameof(SelectedEvent), "Event")]
public partial class EventDetailsPage : ContentPage
{
    private EventItem? _selectedEvent;

    public EventDetailsPage()
    {
        InitializeComponent();
    }

    public EventItem? SelectedEvent
    {
        get => _selectedEvent;
        set
        {
            _selectedEvent = value;
            BindingContext = value;
        }
    }

    /// <summary>Opens the details page for the selected event.</summary>
    public static Task OpenAsync(EventItem item)
    {
        return Shell.Current.GoToAsync(nameof(EventDetailsPage),
            new ShellNavigationQueryParameters { { "Event", item } });
    }

    private void OnFavouriteClicked(object? sender, EventArgs e)
    {
        if (_selectedEvent != null)
        {
            _selectedEvent.IsFavourite = !_selectedEvent.IsFavourite;
        }
    }

    /// <summary>Opens the platform share sheet with the formatted event details.</summary>
    private async void OnShareClicked(object? sender, EventArgs e)
    {
        if (_selectedEvent == null)
        {
            return;
        }
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Title = "Share event",
            Text = _selectedEvent.ShareText
        });
    }
}
