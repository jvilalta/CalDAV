using System.Text;

namespace CalDAV.Utils;

/// <summary>
/// Utility class for generating CalDAV XML requests
/// </summary>
public static class CalDAVXmlGenerator
{
    /// <summary>
    /// Generates XML for PROPFIND request to discover calendars
    /// </summary>
    public static string GenerateCalendarPropfindXml()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:propfind xmlns:d=""DAV:"" xmlns:cs=""http://calendarserver.org/ns/"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:prop>
    <d:displayname />
    <d:resourcetype />
    <cs:getctag />
    <c:calendar-description />
    <c:calendar-color />
    <c:supported-calendar-component-set />
  </d:prop>
</d:propfind>";
    }

    /// <summary>
    /// Generates XML for REPORT request to get calendar events
    /// </summary>
    public static string GenerateCalendarReportXml(DateTime? startTime = null, DateTime? endTime = null)
    {
        var start = startTime?.ToString("yyyyMMddTHHmmssZ") ?? "19700101T000000Z";
        var end = endTime?.ToString("yyyyMMddTHHmmssZ") ?? "20380119T031407Z";

        return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<c:calendar-query xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:prop>
    <d:getetag />
    <c:calendar-data />
  </d:prop>
  <c:filter>
    <c:comp-filter name=""VCALENDAR"">
      <c:comp-filter name=""VEVENT"">
        <c:time-range start=""{start}"" end=""{end}"" />
      </c:comp-filter>
    </c:comp-filter>
  </c:filter>
</c:calendar-query>";
    }

    /// <summary>
    /// Generates XML for current user principal discovery
    /// </summary>
    public static string GenerateCurrentUserPrincipalXml()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:propfind xmlns:d=""DAV:"">
  <d:prop>
    <d:current-user-principal />
  </d:prop>
</d:propfind>";
    }

    /// <summary>
    /// Generates XML for calendar home discovery
    /// </summary>
    public static string GenerateCalendarHomeXml()
    {
        return @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:propfind xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:prop>
    <c:calendar-home-set />
  </d:prop>
</d:propfind>";
    }
}