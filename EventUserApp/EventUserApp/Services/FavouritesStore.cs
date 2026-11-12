using System.Text.Json;

namespace EventUserApp.Services;

/// <summary>
/// Remembers which events the user marked as favourite (feature h).
/// The ids are saved on the phone with MAUI Preferences (a small key/value store), so favourites
/// survive closing the app and re-uploads from the admin app, because the event id never changes.
/// </summary>
public static class FavouritesStore
{
    private const string PreferenceKey = "favourite_event_ids";
    private static HashSet<string>? _ids;

    /// <summary>Raised whenever a favourite is added or removed (the Favourites tab listens to it).</summary>
    public static event Action? Changed;

    private static HashSet<string> Ids => _ids ??= Load();

    public static bool Contains(string eventId) => Ids.Contains(eventId);

    public static void Set(string eventId, bool isFavourite)
    {
        bool changed = isFavourite ? Ids.Add(eventId) : Ids.Remove(eventId);
        if (!changed)
        {
            return;
        }
        Preferences.Default.Set(PreferenceKey, JsonSerializer.Serialize(Ids));
        Changed?.Invoke();
    }

    private static HashSet<string> Load()
    {
        string json = Preferences.Default.Get(PreferenceKey, "");
        if (json.Length == 0)
        {
            return new HashSet<string>();
        }
        try
        {
            return JsonSerializer.Deserialize<HashSet<string>>(json) ?? new HashSet<string>();
        }
        catch (JsonException)
        {
            return new HashSet<string>();   // damaged data: start with no favourites
        }
    }
}
