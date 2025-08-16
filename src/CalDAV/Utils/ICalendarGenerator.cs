using CalDAV.Models;
using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using ICalEvent = Ical.Net.CalendarComponents.CalendarEvent;

namespace CalDAV.Utils;

/// <summary>
/// Utility class for generating iCalendar (RFC 5545) data using Ical.Net library
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
        var calendar = new Ical.Net.Calendar();
        calendar.ProductId = "-//CalDAV Client//CalDAV Client 1.0//EN";

        var calEvent = new ICalEvent
        {
            Uid = string.IsNullOrEmpty(calendarEvent.Uid) ? Guid.NewGuid().ToString() : calendarEvent.Uid,
            Start = new CalDateTime(calendarEvent.StartTime),
            End = new CalDateTime(calendarEvent.EndTime),
            Summary = calendarEvent.Summary ?? string.Empty,
            Description = calendarEvent.Description ?? string.Empty,
            Location = calendarEvent.Location ?? string.Empty,
            Created = new CalDateTime(DateTime.UtcNow),
            LastModified = new CalDateTime(DateTime.UtcNow)
        };

        // Handle organizer
        if (!string.IsNullOrEmpty(calendarEvent.Organizer))
        {
            calEvent.Organizer = new Organizer(calendarEvent.Organizer);
        }

        // Handle attendees
        foreach (var attendee in calendarEvent.Attendees)
        {
            if (!string.IsNullOrEmpty(attendee))
            {
                calEvent.Attendees.Add(new Attendee(attendee));
            }
        }

        calendar.Events.Add(calEvent);

        var serializer = new CalendarSerializer();
        return serializer.SerializeToString(calendar);
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
}