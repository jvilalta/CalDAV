using System.Text;
using CalDAV.Models;

namespace CalDAV.Utils;

/// <summary>
/// Utility class for generating iCalendar (RFC 5545) data
/// </summary>
public static class ICalendarGenerator
{
    /// <summary>
    /// Generates iCalendar data for a calendar event
    /// </summary>
    /// <param name="calendarEvent">The calendar event to convert</param>
    /// <returns>iCalendar formatted string</returns>
    public static string GenerateEvent(CalendarEvent calendarEvent)
    {
        var sb = new StringBuilder();
        var now = DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ");
        
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//CalDAV Client//CalDAV Client 1.0//EN");
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{(string.IsNullOrEmpty(calendarEvent.Uid) ? Guid.NewGuid().ToString() : calendarEvent.Uid)}");
        sb.AppendLine($"DTSTAMP:{now}");
        sb.AppendLine($"DTSTART:{calendarEvent.StartTime.ToUniversalTime():yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTEND:{calendarEvent.EndTime.ToUniversalTime():yyyyMMddTHHmmssZ}");
        
        if (!string.IsNullOrEmpty(calendarEvent.Summary))
            sb.AppendLine($"SUMMARY:{EscapeText(calendarEvent.Summary)}");
        
        if (!string.IsNullOrEmpty(calendarEvent.Description))
            sb.AppendLine($"DESCRIPTION:{EscapeText(calendarEvent.Description)}");
        
        if (!string.IsNullOrEmpty(calendarEvent.Location))
            sb.AppendLine($"LOCATION:{EscapeText(calendarEvent.Location)}");
        
        if (!string.IsNullOrEmpty(calendarEvent.Organizer))
            sb.AppendLine($"ORGANIZER:{calendarEvent.Organizer}");
        
        foreach (var attendee in calendarEvent.Attendees)
        {
            sb.AppendLine($"ATTENDEE:{attendee}");
        }
        
        sb.AppendLine("END:VEVENT");
        sb.Append("END:VCALENDAR"); // Use Append instead of AppendLine for the last line
        
        return sb.ToString();
    }

    /// <summary>
    /// Creates a simple event with basic information
    /// </summary>
    /// <param name="summary">Event title/summary</param>
    /// <param name="startTime">Event start time</param>
    /// <param name="endTime">Event end time</param>
    /// <param name="description">Optional event description</param>
    /// <param name="location">Optional event location</param>
    /// <returns>iCalendar formatted string</returns>
    public static string CreateSimpleEvent(string summary, DateTime startTime, DateTime endTime, 
                                         string? description = null, string? location = null)
    {
        var calEvent = new CalendarEvent
        {
            Uid = Guid.NewGuid().ToString(),
            Summary = summary,
            StartTime = startTime,
            EndTime = endTime,
            Description = description ?? string.Empty,
            Location = location ?? string.Empty
        };
        
        return GenerateEvent(calEvent);
    }

    /// <summary>
    /// Escapes text for iCalendar format
    /// </summary>
    private static string EscapeText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        
        return text
            .Replace("\\", "\\\\")
            .Replace(",", "\\,")
            .Replace(";", "\\;")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }

    /// <summary>
    /// Wraps long lines according to iCalendar specification (75 characters)
    /// </summary>
    private static string WrapLines(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        
        var lines = input.Split('\n');
        var result = new StringBuilder();
        
        foreach (var line in lines)
        {
            if (line.Length <= 75)
            {
                result.AppendLine(line);
            }
            else
            {
                result.AppendLine(line.Substring(0, 75));
                var remaining = line.Substring(75);
                
                while (remaining.Length > 74) // 74 because we need space for the leading space
                {
                    result.AppendLine(" " + remaining.Substring(0, 74));
                    remaining = remaining.Substring(74);
                }
                
                if (remaining.Length > 0)
                {
                    result.AppendLine(" " + remaining);
                }
            }
        }
        
        return result.ToString().TrimEnd();
    }
}