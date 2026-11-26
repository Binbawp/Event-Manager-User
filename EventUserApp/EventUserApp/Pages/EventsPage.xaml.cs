using EventUserApp.Models;
using EventUserApp.Services;

namespace EventUserApp.Pages;

public partial class EventsPage : ContentPage
{
    private readonly EventService _eventService;
    private bool _hasLoadedOnce;
    private bool _lastLoadFailed;

    /// <summary>Defines the date filter selected above the event list.</summary>
    private enum DateFilter { All, Upcoming, Past }
    private DateFilter _dateFilter = DateFilter.All;

    public EventsPage(EventService eventService)
    {
        InitializeComponent();
        _eventService = eventService;
    }

    /// <summary>Loads event data from the cloud when the page first appears.</summary>
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

    /// <summary>Triggers an event refresh through the refresh control.</summary>
    private void OnRefreshClicked(object? sender, EventArgs e)
    {
        refreshView.IsRefreshing = true;     // Raising the refresh state invokes the Refreshing event.
    }

    /// <param name="pulledToRefresh">Indicates whether refresh was initiated by pull-to-refresh.</param>
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
            // Preserve the currently displayed data and record the unsuccessful update.
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

    /// <summary>Updates the selected date filter and reapplies filtering to the event list.</summary>
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

    /// <summary>Applies the selected or unselected visual style to a filter chip.</summary>
    private static void SetChipStyle(Button chip, bool selected)
    {
        string key = selected ? "ChipSelected" : "Chip";
        if (Application.Current?.Resources.TryGetValue(key, out object? style) == true && style is Style s)
        {
            chip.Style = s;
        }
    }

    /// <summary>Applies the selected date filter and search text to the event list.</summary>
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

        // Configure the message displayed when the event list is empty.
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
            eventsList.SelectedItem = null;          // Allow the same event to be selected again later.
            await EventDetailsPage.OpenAsync(selected);
        }
    }
}
