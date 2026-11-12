using EventUserApp.Models;

namespace EventUserApp.Views;

public partial class EventCard : ContentView
{
    public EventCard()
    {
        InitializeComponent();
    }

    /// <summary>Toggles the favourite flag; the star on every screen updates by itself.</summary>
    private void OnStarClicked(object? sender, EventArgs e)
    {
        if (BindingContext is EventItem item)
        {
            item.IsFavourite = !item.IsFavourite;
        }
    }
}
