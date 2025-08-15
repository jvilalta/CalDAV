namespace CalDAV.Tests.Utils;

[TestFixture]
public class CalDAVXmlParserTests
{
    private const string SampleCalendarListXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"" xmlns:cs=""http://calendarserver.org/ns/"">
  <d:response>
    <d:href>/calendars/user/personal/</d:href>
    <d:propstat>
      <d:prop>
        <d:displayname>Personal Calendar</d:displayname>
        <d:resourcetype>
          <d:collection/>
          <c:calendar/>
        </d:resourcetype>
        <cs:getctag>""123456""</cs:getctag>
        <c:calendar-description>My personal events</c:calendar-description>
        <c:calendar-color>#FF5733</c:calendar-color>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
  <d:response>
    <d:href>/calendars/user/work/</d:href>
    <d:propstat>
      <d:prop>
        <d:displayname>Work Calendar</d:displayname>
        <d:resourcetype>
          <d:collection/>
          <c:calendar/>
        </d:resourcetype>
        <cs:getctag>""789012""</cs:getctag>
        <c:calendar-description>Work related events</c:calendar-description>
        <c:calendar-color>#0066CC</c:calendar-color>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
</d:multistatus>";

    private const string SampleEventXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/calendars/user/personal/event1.ics</d:href>
    <d:propstat>
      <d:prop>
        <d:getetag>""etag123""</d:getetag>
        <c:calendar-data>BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Test//Test 1.0//EN
BEGIN:VEVENT
UID:test-event-123
DTSTART:20240615T140000Z
DTEND:20240615T150000Z
SUMMARY:Test Meeting
DESCRIPTION:A test meeting
LOCATION:Conference Room A
ORGANIZER:mailto:organizer@example.com
ATTENDEE:mailto:attendee1@example.com
ATTENDEE:mailto:attendee2@example.com
END:VEVENT
END:VCALENDAR</c:calendar-data>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
</d:multistatus>";

    [Test]
    public void ParseCalendarList_WithValidXml_ShouldReturnCalendars()
    {
        // Act
        var calendars = CalDAVXmlParser.ParseCalendarList(SampleCalendarListXml);

        // Assert
        calendars.Should().HaveCount(2);
        
        var personalCalendar = calendars.First();
        personalCalendar.Url.Should().Be("/calendars/user/personal/");
        personalCalendar.DisplayName.Should().Be("Personal Calendar");
        personalCalendar.Description.Should().Be("My personal events");
        personalCalendar.CTag.Should().Be("123456");
        personalCalendar.Color.Should().Be("#FF5733");

        var workCalendar = calendars.Last();
        workCalendar.Url.Should().Be("/calendars/user/work/");
        workCalendar.DisplayName.Should().Be("Work Calendar");
        workCalendar.Description.Should().Be("Work related events");
        workCalendar.CTag.Should().Be("789012");
        workCalendar.Color.Should().Be("#0066CC");
    }

    [Test]
    public void ParseCalendarList_WithEmptyXml_ShouldReturnEmptyList()
    {
        // Arrange
        const string emptyXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"">
</d:multistatus>";

        // Act
        var calendars = CalDAVXmlParser.ParseCalendarList(emptyXml);

        // Assert
        calendars.Should().BeEmpty();
    }

    [Test]
    public void ParseCalendarList_WithNonCalendarResources_ShouldFilterOut()
    {
        // Arrange
        const string xmlWithNonCalendar = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/calendars/user/personal/</d:href>
    <d:propstat>
      <d:prop>
        <d:displayname>Personal Calendar</d:displayname>
        <d:resourcetype>
          <d:collection/>
          <c:calendar/>
        </d:resourcetype>
      </d:prop>
    </d:propstat>
  </d:response>
  <d:response>
    <d:href>/calendars/user/contacts/</d:href>
    <d:propstat>
      <d:prop>
        <d:displayname>Contacts</d:displayname>
        <d:resourcetype>
          <d:collection/>
        </d:resourcetype>
      </d:prop>
    </d:propstat>
  </d:response>
</d:multistatus>";

        // Act
        var calendars = CalDAVXmlParser.ParseCalendarList(xmlWithNonCalendar);

        // Assert
        calendars.Should().HaveCount(1);
        calendars.First().DisplayName.Should().Be("Personal Calendar");
    }

    [Test]
    public void ParseCalendarEvents_WithValidXml_ShouldReturnEvents()
    {
        // Act
        var events = CalDAVXmlParser.ParseCalendarEvents(SampleEventXml);

        // Assert
        events.Should().HaveCount(1);
        
        var calEvent = events.First();
        calEvent.Href.Should().Be("/calendars/user/personal/event1.ics");
        calEvent.ETag.Should().Be("etag123");
        calEvent.Uid.Should().Be("test-event-123");
        calEvent.Summary.Should().Be("Test Meeting");
        calEvent.Description.Should().Be("A test meeting");
        calEvent.Location.Should().Be("Conference Room A");
        calEvent.Organizer.Should().Be("mailto:organizer@example.com");
        calEvent.StartTime.Should().Be(new DateTime(2024, 6, 15, 14, 0, 0, DateTimeKind.Utc));
        calEvent.EndTime.Should().Be(new DateTime(2024, 6, 15, 15, 0, 0, DateTimeKind.Utc));
        calEvent.Attendees.Should().HaveCount(2);
        calEvent.Attendees.Should().Contain("mailto:attendee1@example.com");
        calEvent.Attendees.Should().Contain("mailto:attendee2@example.com");
    }

    [Test]
    public void ParseCalendarEvents_WithEmptyXml_ShouldReturnEmptyList()
    {
        // Arrange
        const string emptyXml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"">
</d:multistatus>";

        // Act
        var events = CalDAVXmlParser.ParseCalendarEvents(emptyXml);

        // Assert
        events.Should().BeEmpty();
    }

    [Test]
    public void ExtractHref_WithValidXml_ShouldReturnHref()
    {
        // Arrange
        const string xmlWithHref = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"">
  <d:response>
    <d:href>/principals/user/</d:href>
  </d:response>
</d:multistatus>";

        // Act
        var href = CalDAVXmlParser.ExtractHref(xmlWithHref);

        // Assert
        href.Should().Be("/principals/user/");
    }

    [Test]
    public void ExtractHref_WithCustomXPath_ShouldReturnCorrectHref()
    {
        // Arrange
        const string xmlWithMultipleHrefs = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/principals/user/</d:href>
    <d:propstat>
      <d:prop>
        <c:calendar-home-set>
          <d:href>/calendars/user/</d:href>
        </c:calendar-home-set>
      </d:prop>
    </d:propstat>
  </d:response>
</d:multistatus>";

        // Act
        var href = CalDAVXmlParser.ExtractHref(xmlWithMultipleHrefs, "//c:calendar-home-set/d:href");

        // Assert
        href.Should().Be("/calendars/user/");
    }

    [Test]
    public void ExtractHref_WithNoMatchingElement_ShouldReturnEmptyString()
    {
        // Arrange
        const string xmlWithoutHref = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"">
  <d:response>
    <d:status>HTTP/1.1 200 OK</d:status>
  </d:response>
</d:multistatus>";

        // Act
        var href = CalDAVXmlParser.ExtractHref(xmlWithoutHref);

        // Assert
        href.Should().BeEmpty();
    }

    [Test]
    public void ParseCalendarEvents_WithMalformedICalendar_ShouldHandleGracefully()
    {
        // Arrange
        const string xmlWithMalformedICal = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/calendars/user/personal/event1.ics</d:href>
    <d:propstat>
      <d:prop>
        <d:getetag>""etag123""</d:getetag>
        <c:calendar-data>INVALID ICALENDAR DATA</c:calendar-data>
      </d:prop>
    </d:propstat>
  </d:response>
</d:multistatus>";

        // Act
        var events = CalDAVXmlParser.ParseCalendarEvents(xmlWithMalformedICal);

        // Assert
        events.Should().HaveCount(1);
        var calEvent = events.First();
        calEvent.ICalendarData.Should().Be("INVALID ICALENDAR DATA");
        calEvent.Href.Should().Be("/calendars/user/personal/event1.ics");
        calEvent.ETag.Should().Be("etag123");
        // Other properties should be empty/default since parsing failed
        calEvent.Uid.Should().BeEmpty();
        calEvent.Summary.Should().BeEmpty();
    }
}