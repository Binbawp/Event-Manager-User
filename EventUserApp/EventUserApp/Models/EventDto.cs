namespace EventUserApp.Models;

/// <summary>
/// Represents event data uploaded by the Android admin application to Firebase.
/// JSON property names are matched case-insensitively during deserialization.
/// </summary>
public class EventDto
{
    public string? Name { get; set; }
    public string? Date { get; set; }          // Date in DD/MM/YYYY format.
    public string? Location { get; set; }
    public string? Organiser { get; set; }
    public string? Description { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    /// <summary>Session entries keyed by identifiers such as "s1" and "s2".</summary>
    public Dictionary<string, SessionDto?>? Sessions { get; set; }
}

/// <summary>Represents session data stored in Firebase.</summary>
public class SessionDto
{
    public string? Name { get; set; }
    public string? Time { get; set; }          // Time in HH:mm format.
    public string? Speaker { get; set; }
    public string? Room { get; set; }
    public string? Type { get; set; }
    public string? Description { get; set; }
}
