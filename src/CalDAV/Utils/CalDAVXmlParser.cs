using System.Xml;
using CalDAV.Models;
using Ical.Net;

namespace CalDAV.Utils;

/// <summary>
/// Utility class for parsing CalDAV XML responses
/// </summary>
public static class CalDAVXmlParser
{
    /// <summary>
    /// Parses calendar list from PROPFIND response
    /// </summary>
    public static List<Models.Calendar> ParseCalendarList(string xmlResponse)
    {
        var calendars = new List<Models.Calendar>();
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

            var calendar = new Models.Calendar();
            
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
    public static List<Models.CalendarEvent> ParseCalendarEvents(string xmlResponse)
    {
        var events = new List<Models.CalendarEvent>();
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

            var calEvent = new Models.CalendarEvent();
            
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
    /// Parses iCalendar data using Ical.Net library into CalendarEvent properties
    /// </summary>
    private static void ParseICalendarData(Models.CalendarEvent calEvent)
    {
        try
        {
            var calendar = Ical.Net.Calendar.Load(calEvent.ICalendarData);
            var vcalendarEvent = calendar.Events?.FirstOrDefault();
            
            if (vcalendarEvent == null) return;

            // Map properties from Ical.Net event to our CalendarEvent
            calEvent.Uid = vcalendarEvent.Uid ?? string.Empty;
            calEvent.Summary = vcalendarEvent.Summary ?? string.Empty;
            calEvent.Description = vcalendarEvent.Description ?? string.Empty;
            calEvent.Location = vcalendarEvent.Location ?? string.Empty;

            // Handle organizer
            if (vcalendarEvent.Organizer != null)
            {
                calEvent.Organizer = vcalendarEvent.Organizer.Value?.ToString() ?? string.Empty;
            }

            // Handle date/time properties - use Value property to get DateTime
            if (vcalendarEvent.Start != null)
            {
                calEvent.StartTime = vcalendarEvent.Start.Value;
            }

            if (vcalendarEvent.End != null)
            {
                calEvent.EndTime = vcalendarEvent.End.Value;
            }

            // Handle attendees
            calEvent.Attendees.Clear();
            if (vcalendarEvent.Attendees != null)
            {
                foreach (var attendee in vcalendarEvent.Attendees)
                {
                    if (attendee.Value != null)
                    {
                        calEvent.Attendees.Add(attendee.Value.ToString());
                    }
                }
            }
        }
        catch (Exception)
        {
            // If parsing fails, leave the properties as they are
            // The raw ICalendarData is still available for fallback
        }
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