namespace EventUserApp.Models;

/// <summary>
/// One event exactly as the Android admin app uploads it to Firebase.
/// (DTO = data transfer object: it only mirrors the JSON, the app uses EventItem.)
/// JSON property names are matched case-insensitively, so "contactEmail" fills ContactEmail.
/// </summary>
public class EventDto
{
    public string? Name { get; set; }
    public string? Date { get; set; }          // DD/MM/YYYY
    public string? Location { get; set; }
    public string? Organiser { get; set; }
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    /// <summary>Keys are "s1", "s2", ...; the whole node is missing when an event has no sessions.</summary>
    public Dictionary<string, SessionDto?>? Sessions { get; set; }
}

/// <summary>One session exactly as stored in Firebase.</summary>
public class SessionDto
{
    public string? Name { get; set; }
    public string? Time { get; set; }          // HH:mm
    public string? Speaker { get; set; }
    public string? Room { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
}
