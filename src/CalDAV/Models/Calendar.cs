namespace CalDAV.Models;

/// <summary>
/// Represents a calendar collection on the CalDAV server
/// </summary>
public class Calendar
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsReadOnly { get; set; }
    public string ETag { get; set; } = string.Empty;
    public string CTag { get; set; } = string.Empty;
}