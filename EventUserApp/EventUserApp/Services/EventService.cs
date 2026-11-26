using System.Text.Json;
using EventUserApp.Models;

namespace EventUserApp.Services;

/// <summary>
/// Retrieves event data from the Firebase Realtime Database and maintains a local offline cache.
/// A single service instance is shared across the application.
/// </summary>
public class EventService
{
    /// <summary>The Firebase database endpoint used by the admin application.</summary>
    private const string EventsUrl =
        "https://event-manager-ac75b-default-rtdb.asia-southeast1.firebasedatabase.app/events.json";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };

    private static string CacheFilePath => Path.Combine(FileSystem.AppDataDirectory, "events_cache.json");

    /// <summary>The loaded events, ordered by date from earliest to latest.</summary>
    public List<EventItem> Events { get; private set; } = new();

    /// <summary>Indicates whether the event list is from the local offline cache.</summary>
    public bool IsShowingCachedData { get; private set; }

    /// <summary>The date and time when the displayed data was retrieved or cached.</summary>
    public DateTime? LastUpdated { get; private set; }

    /// <summary>
    /// Downloads events from Firebase or loads the cached copy when network access is unavailable.
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
            await File.WriteAllTextAsync(CacheFilePath, json);   // Cache the latest successful download.
            IsShowingCachedData = false;
            LastUpdated = DateTime.Now;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Fall back to the cached copy when network access or the remote server is unavailable.
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

    /// <summary>Converts the Firebase JSON response into a sorted list of event items.</summary>
    private static List<EventItem> ParseEvents(string json)
    {
        // Firebase returns "null" when the event database has no entries.
        var dtos = JsonSerializer.Deserialize<Dictionary<string, EventDto?>>(json, JsonOptions);
        if (dtos == null)
        {
            return new List<EventItem>();
        }

        return dtos
            .Where(pair => pair.Value != null)
            .Select(pair => new EventItem(pair.Key, pair.Value!))
            .OrderBy(e => e.DateValue ?? DateTime.MaxValue)   // Place entries with invalid dates last.
            .ThenBy(e => e.Name)
            .ToList();
    }
}
