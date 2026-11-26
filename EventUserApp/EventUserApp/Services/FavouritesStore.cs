using System.Text.Json;

namespace EventUserApp.Services;

/// <summary>
/// Persists the user's favourite event identifiers using MAUI Preferences.
/// Favourite selections remain available after application restarts and event updates.
/// </summary>
public static class FavouritesStore
{
    private const string PreferenceKey = "favourite_event_ids";
    private static HashSet<string>? _ids;

    /// <summary>Raised when an event is added to or removed from favourites.</summary>
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
            return new HashSet<string>();   // Reset preferences when the stored data is invalid.
        }
    }
}
