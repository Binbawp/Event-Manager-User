using EventUserApp.Models;

namespace EventUserApp.Views;

public partial class EventCard : ContentView
{
    public EventCard()
    {
        InitializeComponent();
    }

    /// <summary>Toggles the event's favourite status and synchronizes all displayed cards.</summary>
    private void OnStarClicked(object? sender, EventArgs e)
    {
        if (BindingContext is EventItem item)
        {
            item.IsFavourite = !item.IsFavourite;
        }
    }
}
