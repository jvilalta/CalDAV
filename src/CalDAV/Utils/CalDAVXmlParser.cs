using System.Xml;
using CalDAV.Models;

namespace CalDAV.Utils;

/// <summary>
/// Utility class for parsing CalDAV XML responses
/// </summary>
public static class CalDAVXmlParser
{
    /// <summary>
    /// Parses calendar list from PROPFIND response
    /// </summary>
    public static List<Calendar> ParseCalendarList(string xmlResponse)
    {
        var calendars = new List<Calendar>();
        var doc = new XmlDocument();
        doc.LoadXml(xmlResponse);

        var namespaceManager = new XmlNamespaceManager(doc.NameTable);
        namespaceManager.AddNamespace("d", "DAV:");
        namespaceManager.AddNamespace("c", "urn:ietf:params:xml:ns:caldav");
        namespaceManager.AddNamespace("cs", "http://calendarserver.org/ns/");

        var responses = doc.SelectNodes("//d:response", namespaceManager);
        if (responses == null) return calendars;

        foreach (XmlNode response in responses)
        {
            var resourceType = response.SelectSingleNode(".//d:resourcetype/c:calendar", namespaceManager);
            if (resourceType == null) continue; // Not a calendar

            var calendar = new Calendar();
            
            var href = response.SelectSingleNode("d:href", namespaceManager);
            if (href != null) calendar.Url = href.InnerText;

            var displayName = response.SelectSingleNode(".//d:displayname", namespaceManager);
            if (displayName != null) calendar.DisplayName = displayName.InnerText;

            var description = response.SelectSingleNode(".//c:calendar-description", namespaceManager);
            if (description != null) calendar.Description = description.InnerText;

            var ctag = response.SelectSingleNode(".//cs:getctag", namespaceManager);
            if (ctag != null) calendar.CTag = ctag.InnerText.Trim('"');

            var color = response.SelectSingleNode(".//c:calendar-color", namespaceManager);
            if (color != null) calendar.Color = color.InnerText;

            calendars.Add(calendar);
        }

        return calendars;
    }

    /// <summary>
    /// Parses calendar events from REPORT response
    /// </summary>
    public static List<CalendarEvent> ParseCalendarEvents(string xmlResponse)
    {
        var events = new List<CalendarEvent>();
        var doc = new XmlDocument();
        doc.LoadXml(xmlResponse);

        var namespaceManager = new XmlNamespaceManager(doc.NameTable);
        namespaceManager.AddNamespace("d", "DAV:");
        namespaceManager.AddNamespace("c", "urn:ietf:params:xml:ns:caldav");

        var responses = doc.SelectNodes("//d:response", namespaceManager);
        if (responses == null) return events;

        foreach (XmlNode response in responses)
        {
            var calendarData = response.SelectSingleNode(".//c:calendar-data", namespaceManager);
            if (calendarData == null) continue;

            var calEvent = new CalendarEvent();
            
            var href = response.SelectSingleNode("d:href", namespaceManager);
            if (href != null) calEvent.Href = href.InnerText;

            var etag = response.SelectSingleNode(".//d:getetag", namespaceManager);
            if (etag != null) calEvent.ETag = etag.InnerText.Trim('"');

            calEvent.ICalendarData = calendarData.InnerText;
            ParseICalendarData(calEvent);

            events.Add(calEvent);
        }

        return events;
    }

    /// <summary>
    /// Parses basic iCalendar data into CalendarEvent properties
    /// </summary>
    private static void ParseICalendarData(CalendarEvent calEvent)
    {
        var lines = calEvent.ICalendarData.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (trimmedLine.StartsWith("UID:"))
                calEvent.Uid = trimmedLine.Substring(4);
            else if (trimmedLine.StartsWith("SUMMARY:"))
                calEvent.Summary = trimmedLine.Substring(8);
            else if (trimmedLine.StartsWith("DESCRIPTION:"))
                calEvent.Description = trimmedLine.Substring(12);
            else if (trimmedLine.StartsWith("LOCATION:"))
                calEvent.Location = trimmedLine.Substring(9);
            else if (trimmedLine.StartsWith("ORGANIZER:"))
                calEvent.Organizer = trimmedLine.Substring(10);
            else if (trimmedLine.StartsWith("DTSTART:"))
            {
                calEvent.StartTime = ParseICalendarDateTime(trimmedLine.Substring(8));
            }
            else if (trimmedLine.StartsWith("DTEND:"))
            {
                calEvent.EndTime = ParseICalendarDateTime(trimmedLine.Substring(6));
            }
            else if (trimmedLine.StartsWith("ATTENDEE:"))
            {
                calEvent.Attendees.Add(trimmedLine.Substring(9));
            }
        }
    }

    /// <summary>
    /// Parses iCalendar datetime format
    /// </summary>
    private static DateTime ParseICalendarDateTime(string dateTimeString)
    {
        // Handle UTC times (ending with Z)
        if (dateTimeString.EndsWith("Z"))
        {
            var dateOnly = dateTimeString.Substring(0, dateTimeString.Length - 1);
            if (DateTime.TryParseExact(dateOnly, "yyyyMMddTHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var utcTime))
            {
                return DateTime.SpecifyKind(utcTime, DateTimeKind.Utc);
            }
        }
        else
        {
            // Handle local times or other formats
            if (DateTime.TryParseExact(dateTimeString, "yyyyMMddTHHmmss", null, 
                System.Globalization.DateTimeStyles.None, out var localTime))
            {
                return localTime;
            }
        }

        return DateTime.MinValue;
    }

    /// <summary>
    /// Extracts href from XML response
    /// </summary>
    public static string ExtractHref(string xmlResponse, string xpath = "//d:href")
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlResponse);

        var namespaceManager = new XmlNamespaceManager(doc.NameTable);
        namespaceManager.AddNamespace("d", "DAV:");
        namespaceManager.AddNamespace("c", "urn:ietf:params:xml:ns:caldav");

        var hrefNode = doc.SelectSingleNode(xpath, namespaceManager);
        return hrefNode?.InnerText ?? string.Empty;
    }
}