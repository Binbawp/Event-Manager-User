using EventUserApp.Models;

namespace EventUserApp.Pages;

/// <summary>
/// Receives the tapped event through Shell navigation (Lecture 9, passing data):
/// the "Event" query parameter fills the SelectedEvent property, which becomes the page's data.
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

    /// <summary>Opens the details page for an event (used by both lists).</summary>
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

    /// <summary>Opens the phone's share sheet with the event's details as text.</summary>
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
