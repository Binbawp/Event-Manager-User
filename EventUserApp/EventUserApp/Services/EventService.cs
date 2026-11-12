using System.Text.Json;
using EventUserApp.Models;

namespace EventUserApp.Services;

/// <summary>
/// Loads the events published by the Android admin app from the Firebase Realtime Database.
/// The last successful download is saved in a file, so the app can still show events when the
/// phone is offline (extra feature).
/// One instance is shared by the whole app, so every screen sees the same EventItem objects.
/// </summary>
public class EventService
{
    /// <summary>Same database the admin app uploads to.</summary>
    private const string EventsUrl =
        "https://event-manager-ac75b-default-rtdb.asia-southeast1.firebasedatabase.app/events.json";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };

    private static string CacheFilePath => Path.Combine(FileSystem.AppDataDirectory, "events_cache.json");

    /// <summary>The events from the last load, earliest date first.</summary>
    public List<EventItem> Events { get; private set; } = new();

    /// <summary>True when the list came from the saved copy because the cloud could not be reached.</summary>
    public bool IsShowingCachedData { get; private set; }

    /// <summary>When the data shown was downloaded.</summary>
    public DateTime? LastUpdated { get; private set; }

    /// <summary>
    /// Downloads the events (or reads the saved copy if offline). Throws if there is no way to
    /// get any data, so the page can show an error message.
    /// </summary>
    public async Task LoadEventsAsync()
    {
        string json;
        try
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                throw new HttpRequestException("No internet connection.");
            }

            json = await _http.GetStringAsync(EventsUrl);
            await File.WriteAllTextAsync(CacheFilePath, json);   // keep a copy for offline use
            IsShowingCachedData = false;
            LastUpdated = DateTime.Now;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Offline, time-out or server refused: fall back to the saved copy if there is one
            if (!File.Exists(CacheFilePath))
            {
                throw;
            }
            json = await File.ReadAllTextAsync(CacheFilePath);
            IsShowingCachedData = true;
            LastUpdated = File.GetLastWriteTime(CacheFilePath);
        }

        Events = ParseEvents(json);
    }

    /// <summary>Turns the Firebase JSON (a dictionary of events keyed by id) into a sorted list.</summary>
    private static List<EventItem> ParseEvents(string json)
    {
        // An empty database is returned by Firebase as the text "null", which gives a null result here
        var dtos = JsonSerializer.Deserialize<Dictionary<string, EventDto?>>(json, JsonOptions);
        if (dtos == null)
        {
            return new List<EventItem>();
        }

        return dtos
            .Where(pair => pair.Value != null)
            .Select(pair => new EventItem(pair.Key, pair.Value!))
            .OrderBy(e => e.DateValue ?? DateTime.MaxValue)   // events with an unreadable date go last
            .ThenBy(e => e.Name)
            .ToList();
    }
}
