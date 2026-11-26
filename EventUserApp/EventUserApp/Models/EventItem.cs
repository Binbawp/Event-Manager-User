using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using EventUserApp.Services;

namespace EventUserApp.Models;

/// <summary>
/// Represents an event in the application, including display properties and favourite status.
/// Property-change notifications keep all event views synchronized.
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

        // Convert session entries into a list ordered by start time and name.
        Sessions = (dto.Sessions?.Values ?? Enumerable.Empty<SessionDto?>())
            .OfType<SessionDto>()
            .Select(s => new SessionItem(s))
            .OrderBy(s => s.Time)
            .ThenBy(s => s.Name)
            .ToList();

        _isFavourite = FavouritesStore.Contains(id);   // Restore the persisted favourite status.
    }

    /// <summary>The stable Firebase identifier assigned by the admin application.</summary>
    public string Id { get; }
    public string Name { get; }
    public string Date { get; }
    public string Location { get; }
    public string Organiser { get; }
    public string Description { get; }
    public string ContactEmail { get; }
    public string ContactPhone { get; }
    public List<SessionItem> Sessions { get; }

    // Favourite status

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
            FavouritesStore.Set(Id, value);              // Persist the updated favourite status.
            OnPropertyChanged();
            OnPropertyChanged(nameof(FavouriteIcon));
        }
    }

    /// <summary>Returns the filled or outlined heart icon according to favourite status.</summary>
    public string FavouriteIcon => _isFavourite ? "\u2665" : "\u2661";

    // Display helpers

    /// <summary>Gets the parsed event date, or null when the stored date is invalid.</summary>
    public DateTime? DateValue =>
        DateTime.TryParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
            ? d : null;

    /// <summary>Gets the day number for the date badge, or "--" for an invalid date.</summary>
    public string DayText => DateValue?.ToString("dd", CultureInfo.InvariantCulture) ?? "--";

    /// <summary>Gets the abbreviated month name for the date badge.</summary>
    public string MonthText => DateValue?.ToString("MMM", CultureInfo.InvariantCulture).ToUpperInvariant() ?? "";

    /// <summary>Indicates whether the event date is earlier than the current date.</summary>
    public bool IsPast => DateValue.HasValue && DateValue.Value.Date < DateTime.Today;

    /// <summary>Gets the formatted event date and organiser text.</summary>
    public string SubtitleText => string.Join(" \u00B7 ", new[] { Date, Organiser }.Where(p => p.Length > 0));

    public string SessionCountText => Sessions.Count == 1 ? "1 session" : $"{Sessions.Count} sessions";
    public string SessionsTitle => $"Sessions ({Sessions.Count})";

    public bool HasLocation => Location.Length > 0;
    public bool HasOrganiser => Organiser.Length > 0;
    public bool HasDescription => Description.Length > 0;
    public bool HasEmail => ContactEmail.Length > 0;
    public bool HasPhone => ContactPhone.Length > 0;
    public bool HasNoSessions => Sessions.Count == 0;

    /// <summary>Determines whether the event name or date contains the search text.</summary>
    public bool Matches(string query) =>
        Name.Contains(query, StringComparison.OrdinalIgnoreCase)
        || Date.Contains(query, StringComparison.OrdinalIgnoreCase);

    /// <summary>Gets the formatted event summary shared through external applications.</summary>
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
