using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using EventUserApp.Services;

namespace EventUserApp.Models;

/// <summary>
/// An event as the app uses it: the data from the cloud plus a few ready-made text properties
/// for the screens and the favourite flag. It notifies the UI when the favourite flag changes,
/// so the star updates on every screen that shows this event.
/// </summary>
public class EventItem : INotifyPropertyChanged
{
    private bool _isFavourite;

    public EventItem(string id, EventDto dto)
    {
        Id = id;
        Name = dto.Name ?? "";
        Date = dto.Date ?? "";
        Location = dto.Location ?? "";
        Organiser = dto.Organiser ?? "";
        Description = dto.Description ?? "";
        ContactEmail = dto.ContactEmail ?? "";
        ContactPhone = dto.ContactPhone ?? "";

        // Sessions come as a dictionary (s1, s2, ...): keep the values, ordered by start time
        Sessions = (dto.Sessions?.Values ?? Enumerable.Empty<SessionDto?>())
            .OfType<SessionDto>()
            .Select(s => new SessionItem(s))
            .OrderBy(s => s.Time)
            .ThenBy(s => s.Name)
            .ToList();

        _isFavourite = FavouritesStore.Contains(id);   // restore the saved favourite state
    }

    /// <summary>The Firebase key (a UUID created by the admin app). Stays the same between uploads.</summary>
    public string Id { get; }
    public string Name { get; }
    public string Date { get; }
    public string Location { get; }
    public string Organiser { get; }
    public string Description { get; }
    public string ContactEmail { get; }
    public string ContactPhone { get; }
    public List<SessionItem> Sessions { get; }

    // ---------------------------------------------------------------- favourites (feature h)

    public bool IsFavourite
    {
        get => _isFavourite;
        set
        {
            if (_isFavourite == value)
            {
                return;
            }
            _isFavourite = value;
            FavouritesStore.Set(Id, value);              // save on the phone
            OnPropertyChanged();
            OnPropertyChanged(nameof(FavouriteIcon));
        }
    }

    /// <summary>Filled heart when favourite, outline heart otherwise.</summary>
    public string FavouriteIcon => _isFavourite ? "\u2665" : "\u2661";

    // ---------------------------------------------------------------- display helpers

    /// <summary>The date as a DateTime for sorting; null if the text is not a valid DD/MM/YYYY.</summary>
    public DateTime? DateValue =>
        DateTime.TryParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
            ? d : null;

    /// <summary>Day of the month for the date badge, e.g. "12" ("--" if the date cannot be read).</summary>
    public string DayText => DateValue?.ToString("dd", CultureInfo.InvariantCulture) ?? "--";

    /// <summary>Short month for the date badge, e.g. "NOV".</summary>
    public string MonthText => DateValue?.ToString("MMM", CultureInfo.InvariantCulture).ToUpperInvariant() ?? "";

    /// <summary>True when the event date is before today (used by the "Past" filter).</summary>
    public bool IsPast => DateValue.HasValue && DateValue.Value.Date < DateTime.Today;

    /// <summary>"12/11/2026 · Greenwich Tech Society"</summary>
    public string SubtitleText => string.Join(" \u00B7 ", new[] { Date, Organiser }.Where(p => p.Length > 0));

    public string SessionCountText => Sessions.Count == 1 ? "1 session" : $"{Sessions.Count} sessions";
    public string SessionsTitle => $"Sessions ({Sessions.Count})";

    public bool HasLocation => Location.Length > 0;
    public bool HasOrganiser => Organiser.Length > 0;
    public bool HasDescription => Description.Length > 0;
    public bool HasEmail => ContactEmail.Length > 0;
    public bool HasPhone => ContactPhone.Length > 0;
    public bool HasNoSessions => Sessions.Count == 0;

    /// <summary>True if the search text appears in the event name or date (ignoring upper/lower case).</summary>
    public bool Matches(string query) =>
        Name.Contains(query, StringComparison.OrdinalIgnoreCase)
        || Date.Contains(query, StringComparison.OrdinalIgnoreCase);

    /// <summary>The text sent to other apps by the Share button.</summary>
    public string ShareText
    {
        get
        {
            var sb = new StringBuilder(Name).AppendLine();
            sb.AppendLine($"Date: {Date}");
            if (HasLocation) sb.AppendLine($"Location: {Location}");
            if (HasOrganiser) sb.AppendLine($"Organiser: {Organiser}");
            if (HasDescription) sb.AppendLine().AppendLine(Description);
            return sb.ToString().TrimEnd();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
