namespace CalDAV.Models;

/// <summary>
/// Represents a calendar event
/// </summary>
public class CalendarEvent
{
    public string Uid { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public List<string> Attendees { get; set; } = new();
    
    /// <summary>
    /// The raw iCalendar data
    /// </summary>
    public string ICalendarData { get; set; } = string.Empty;
    
    /// <summary>
    /// The ETag from the server for this event
    /// </summary>
    public string ETag { get; set; } = string.Empty;
    
    /// <summary>
    /// The URL/path to this event on the server
    /// </summary>
    public string Href { get; set; } = string.Empty;
}