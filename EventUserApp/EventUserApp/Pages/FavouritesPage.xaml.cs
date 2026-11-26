using EventUserApp.Models;
using EventUserApp.Services;

namespace EventUserApp.Pages;

public partial class FavouritesPage : ContentPage
{
    private readonly EventService _eventService;

    public FavouritesPage(EventService eventService)
    {
        InitializeComponent();
        _eventService = eventService;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FavouritesStore.Changed += RefreshList;   // Refresh immediately when a favourite is removed.
        RefreshList();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        FavouritesStore.Changed -= RefreshList;
    }

    private void RefreshList()
    {
        favouritesList.ItemsSource = _eventService.Events.Where(ev => ev.IsFavourite).ToList();
    }

    private async void OnEventSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is EventItem selected)
        {
            favouritesList.SelectedItem = null;
            await EventDetailsPage.OpenAsync(selected);
        }
    }
}
