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
        FavouritesStore.Changed += RefreshList;   // un-starring an event removes it from this list at once
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
