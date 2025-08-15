namespace CalDAV.Tests;

/// <summary>
/// Helper class for setting up test data and configurations
/// </summary>
public static class TestFixtures
{
    public static class SampleXml
    {
        public const string MultiStatusWithCalendars = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"" xmlns:cs=""http://calendarserver.org/ns/"">
  <d:response>
    <d:href>/calendars/testuser/personal/</d:href>
    <d:propstat>
      <d:prop>
        <d:displayname>Personal Calendar</d:displayname>
        <d:resourcetype>
          <d:collection/>
          <c:calendar/>
        </d:resourcetype>
        <cs:getctag>""12345""</cs:getctag>
        <c:calendar-description>Personal events and appointments</c:calendar-description>
        <c:calendar-color>#FF5733</c:calendar-color>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
  <d:response>
    <d:href>/calendars/testuser/work/</d:href>
    <d:propstat>
      <d:prop>
        <d:displayname>Work Calendar</d:displayname>
        <d:resourcetype>
          <d:collection/>
          <c:calendar/>
        </d:resourcetype>
        <cs:getctag>""67890""</cs:getctag>
        <c:calendar-description>Work meetings and deadlines</c:calendar-description>
        <c:calendar-color>#0066CC</c:calendar-color>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
</d:multistatus>";

        public const string MultiStatusWithEvents = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/calendars/testuser/personal/meeting1.ics</d:href>
    <d:propstat>
      <d:prop>
        <d:getetag>""abc123""</d:getetag>
        <c:calendar-data>BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Test//Test 1.0//EN
BEGIN:VEVENT
UID:meeting-001
DTSTART:20240701T090000Z
DTEND:20240701T100000Z
SUMMARY:Team Standup
DESCRIPTION:Daily team standup meeting
LOCATION:Conference Room A
ORGANIZER:mailto:manager@company.com
ATTENDEE:mailto:dev1@company.com
ATTENDEE:mailto:dev2@company.com
END:VEVENT
END:VCALENDAR</c:calendar-data>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
  <d:response>
    <d:href>/calendars/testuser/personal/appointment1.ics</d:href>
    <d:propstat>
      <d:prop>
        <d:getetag>""def456""</d:getetag>
        <c:calendar-data>BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//Test//Test 1.0//EN
BEGIN:VEVENT
UID:appointment-001
DTSTART:20240702T140000Z
DTEND:20240702T150000Z
SUMMARY:Doctor Appointment
DESCRIPTION:Annual checkup
LOCATION:Medical Center
END:VEVENT
END:VCALENDAR</c:calendar-data>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
</d:multistatus>";

        public const string CurrentUserPrincipal = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"">
  <d:response>
    <d:href>/caldav/</d:href>
    <d:propstat>
      <d:prop>
        <d:current-user-principal>
          <d:href>/principals/testuser/</d:href>
        </d:current-user-principal>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
</d:multistatus>";

        public const string CalendarHomeSet = @"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/principals/testuser/</d:href>
    <d:propstat>
      <d:prop>
        <c:calendar-home-set>
          <d:href>/calendars/testuser/</d:href>
        </c:calendar-home-set>
      </d:prop>
      <d:status>HTTP/1.1 200 OK</d:status>
    </d:propstat>
  </d:response>
</d:multistatus>";
    }

    public static class SampleEvents
    {
        public static CalendarEvent CreateTestEvent() => new()
        {
            Uid = "test-event-12345",
            Summary = "Test Meeting",
            Description = "This is a test meeting for unit testing",
            StartTime = new DateTime(2024, 7, 15, 14, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2024, 7, 15, 15, 30, 0, DateTimeKind.Utc),
            Location = "Test Conference Room",
            Organizer = "mailto:organizer@test.com",
            Attendees = new List<string>
            {
                "mailto:attendee1@test.com",
                "mailto:attendee2@test.com"
            }
        };

        public static CalendarEvent CreateAllDayEvent() => new()
        {
            Uid = "all-day-event-001",
            Summary = "All Day Event",
            Description = "This is an all-day event",
            StartTime = new DateTime(2024, 7, 20, 0, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2024, 7, 20, 23, 59, 59, DateTimeKind.Utc),
            Location = "Everywhere"
        };

        public static CalendarEvent CreateRecurringEvent() => new()
        {
            Uid = "recurring-event-001",
            Summary = "Weekly Team Meeting",
            Description = "Weekly recurring team meeting",
            StartTime = new DateTime(2024, 7, 22, 10, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2024, 7, 22, 11, 0, 0, DateTimeKind.Utc),
            Location = "Meeting Room B",
            Organizer = "mailto:manager@company.com"
        };
    }

    public static class SampleCalendars
    {
        public static Calendar CreatePersonalCalendar() => new()
        {
            Name = "personal",
            DisplayName = "Personal Calendar",
            Description = "Personal events and appointments",
            Url = "/calendars/testuser/personal/",
            Color = "#FF5733",
            IsReadOnly = false,
            ETag = "\"etag-personal-123\"",
            CTag = "\"ctag-personal-456\""
        };

        public static Calendar CreateWorkCalendar() => new()
        {
            Name = "work",
            DisplayName = "Work Calendar",
            Description = "Work meetings and deadlines",
            Url = "/calendars/testuser/work/",
            Color = "#0066CC",
            IsReadOnly = false,
            ETag = "\"etag-work-789\"",
            CTag = "\"ctag-work-012\""
        };

        public static Calendar CreateReadOnlyCalendar() => new()
        {
            Name = "shared",
            DisplayName = "Shared Calendar",
            Description = "Shared team calendar",
            Url = "/calendars/shared/team/",
            Color = "#00CC66",
            IsReadOnly = true,
            ETag = "\"etag-shared-345\"",
            CTag = "\"ctag-shared-678\""
        };
    }
}

/// <summary>
/// Custom assertions for CalDAV objects
/// </summary>
public static class CalDAVAssertions
{
    public static void ShouldBeValidCalendar(this Calendar calendar)
    {
        calendar.Should().NotBeNull();
        calendar.Url.Should().NotBeNullOrEmpty();
        calendar.DisplayName.Should().NotBeNullOrEmpty();
    }

    public static void ShouldBeValidEvent(this CalendarEvent calendarEvent)
    {
        calendarEvent.Should().NotBeNull();
        calendarEvent.Uid.Should().NotBeNullOrEmpty();
        calendarEvent.Summary.Should().NotBeNullOrEmpty();
        calendarEvent.StartTime.Should().BeBefore(calendarEvent.EndTime);
    }

    public static void ShouldBeValidICalendar(this string iCalendarData)
    {
        iCalendarData.Should().NotBeNullOrEmpty();
        iCalendarData.Should().StartWith("BEGIN:VCALENDAR");
        iCalendarData.Should().EndWith("END:VCALENDAR");
        iCalendarData.Should().Contain("VERSION:2.0");
        iCalendarData.Should().Contain("BEGIN:VEVENT");
        iCalendarData.Should().Contain("END:VEVENT");
    }

    public static void ShouldContainValidXmlDeclaration(this string xmlContent)
    {
        xmlContent.Should().NotBeNullOrEmpty();
        xmlContent.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    }
}