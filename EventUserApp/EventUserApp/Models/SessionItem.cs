namespace EventUserApp.Models;

/// <summary>Represents a session displayed on the event details page.</summary>
public class SessionItem
{
    public SessionItem(SessionDto dto)
    {
        Name = dto.Name ?? "";
        Time = dto.Time ?? "";
        Speaker = dto.Speaker ?? "";
        Room = dto.Room ?? "";
        Type = dto.Type ?? "";
        Description = dto.Description ?? "";
    }

    public string Name { get; }
    public string Time { get; }
    public string Speaker { get; }
    public string Room { get; }
    public string Type { get; }
    public string Description { get; }

    /// <summary>Gets the formatted session time and type.</summary>
    public string MetaText => JoinNonEmpty(" \u00B7 ", Time, Type);

    /// <summary>Gets the formatted speaker and room details supplied for the session.</summary>
    public string PeopleText => JoinNonEmpty(" \u00B7 ",
        Speaker.Length > 0 ? $"Speaker: {Speaker}" : "",
        Room.Length > 0 ? $"Room: {Room}" : "");

    public bool HasType => Type.Length > 0;
    public bool HasPeople => PeopleText.Length > 0;
    public bool HasDescription => Description.Length > 0;

    private static string JoinNonEmpty(string separator, params string[] parts) =>
        string.Join(separator, parts.Where(p => p.Length > 0));
}
