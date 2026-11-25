using EventUserApp.Models;
using EventUserApp.Services;

namespace EventUserApp.Pages;

public partial class EventsPage : ContentPage
{
    private readonly EventService _eventService;
    private bool _hasLoadedOnce;
    private bool _lastLoadFailed;

    /// <summary>Which chip is selected above the list.</summary>
    private enum DateFilter { All, Upcoming, Past }
    private DateFilter _dateFilter = DateFilter.All;

    public EventsPage(EventService eventService)
    {
        InitializeComponent();
        _eventService = eventService;
    }

    /// <summary>The first time the page appears, load the events from the cloud.</summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (!_hasLoadedOnce)
        {
            _hasLoadedOnce = true;
            await LoadEventsAsync(pulledToRefresh: false);
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadEventsAsync(pulledToRefresh: true);
    }

    /// <summary>The round refresh button in the header: starts the same reload as pulling the list down.</summary>
    private void OnRefreshClicked(object? sender, EventArgs e)
    {
        refreshView.IsRefreshing = true;     // this raises the Refreshing event above
    }

    /// <param name="pulledToRefresh">true when the user pulled the list down (it shows its own spinner)</param>
    private async Task LoadEventsAsync(bool pulledToRefresh)
    {
        if (!pulledToRefresh)
        {
            busyIndicator.IsVisible = true;
            busyIndicator.IsRunning = true;
        }

        try
        {
            await _eventService.LoadEventsAsync();
            _lastLoadFailed = false;
        }
        catch (Exception ex)
        {
            // No internet and no saved copy, or the server refused: keep whatever is already shown
            _lastLoadFailed = true;
            System.Diagnostics.Debug.WriteLine($"Loading events failed: {ex.Message}");
        }
        finally
        {
            busyIndicator.IsRunning = false;
            busyIndicator.IsVisible = false;
            refreshView.IsRefreshing = false;
        }

        UpdateStatusBanner();
        ApplyFilter();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        ApplyFilter();
    }

    /// <summary>The user tapped All / Upcoming / Past: remember it, restyle the chips and refilter.</summary>
    private void OnChipClicked(object? sender, EventArgs e)
    {
        _dateFilter = sender == chipUpcoming ? DateFilter.Upcoming
                    : sender == chipPast ? DateFilter.Past
                    : DateFilter.All;

        SetChipStyle(chipAll, _dateFilter == DateFilter.All);
        SetChipStyle(chipUpcoming, _dateFilter == DateFilter.Upcoming);
        SetChipStyle(chipPast, _dateFilter == DateFilter.Past);
        ApplyFilter();
    }

    /// <summary>Yellow chip when selected, dark chip otherwise.</summary>
    private static void SetChipStyle(Button chip, bool selected)
    {
        string key = selected ? "ChipSelected" : "Chip";
        if (Application.Current?.Resources.TryGetValue(key, out object? style) == true && style is Style s)
        {
            chip.Style = s;
        }
    }

    /// <summary>Shows only the events that match the selected chip and contain the search text in name or date.</summary>
    private void ApplyFilter()
    {
        string query = searchBar.Text?.Trim() ?? "";

        IEnumerable<EventItem> source = _dateFilter switch
        {
            DateFilter.Upcoming => _eventService.Events.Where(ev => !ev.IsPast),
            DateFilter.Past => _eventService.Events.Where(ev => ev.IsPast),
            _ => _eventService.Events
        };

        List<EventItem> items = query.Length == 0
            ? source.ToList()
            : source.Where(ev => ev.Matches(query)).ToList();
        eventsList.ItemsSource = items;

        // What the list says when it has nothing to show
        if (_lastLoadFailed && _eventService.Events.Count == 0)
        {
            eventsList.EmptyView = "Could not load events.\nCheck your internet connection and pull down to try again.";
        }
        else if (query.Length > 0 || _dateFilter != DateFilter.All)
        {
            eventsList.EmptyView = "No events match your search.";
        }
        else
        {
            eventsList.EmptyView = "No events have been published yet.";
        }
    }

    private void UpdateStatusBanner()
    {
        if (_lastLoadFailed)
        {
            statusLabel.Text = "Could not update the events. Pull down to try again.";
            statusLabel.IsVisible = true;
        }
        else if (_eventService.IsShowingCachedData)
        {
            statusLabel.Text = $"You are offline. Showing events saved on {_eventService.LastUpdated:g}.";
            statusLabel.IsVisible = true;
        }
        else
        {
            statusLabel.IsVisible = false;
        }
    }

    private async void OnEventSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is EventItem selected)
        {
            eventsList.SelectedItem = null;          // so the same event can be tapped again later
            await EventDetailsPage.OpenAsync(selected);
        }
    }
}
