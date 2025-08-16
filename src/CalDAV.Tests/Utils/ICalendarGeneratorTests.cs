namespace CalDAV.Tests.Utils;

[TestFixture]
public class ICalendarGeneratorTests
{
    [Test]
    public void GenerateEvent_WithBasicEvent_ShouldReturnValidICalendar()
    {
        // Arrange
        var calendarEvent = new CalendarEvent
        {
            Uid = "test-uid-123",
            Summary = "Test Meeting",
            Description = "Test Description",
            StartTime = new DateTime(2024, 6, 15, 14, 0, 0, DateTimeKind.Utc),
            EndTime = new DateTime(2024, 6, 15, 15, 0, 0, DateTimeKind.Utc),
            Location = "Conference Room A",
            Organizer = "mailto:organizer@example.com"
        };

        // Act
        var iCal = ICalendarGenerator.GenerateEvent(calendarEvent);

        // Assert
        iCal.Should().StartWith("BEGIN:VCALENDAR");
        iCal.Should().Contain("END:VCALENDAR");
        iCal.Should().Contain("VERSION:2.0");
        iCal.Should().Contain("BEGIN:VEVENT");
        iCal.Should().Contain("END:VEVENT");
        iCal.Should().Contain("UID:test-uid-123");
        iCal.Should().Contain("SUMMARY:Test Meeting");
        iCal.Should().Contain("DESCRIPTION:Test Description");
        iCal.Should().Contain("DTSTART:20240615T140000Z");
        iCal.Should().Contain("DTEND:20240615T150000Z");
        iCal.Should().Contain("LOCATION:Conference Room A");
        iCal.Should().Contain("ORGANIZER:mailto:organizer@example.com");
    }

    [Test]
    public void GenerateEvent_WithEmptyUid_ShouldGenerateGuid()
    {
        // Arrange
        var calendarEvent = new CalendarEvent
        {
            Uid = "",
            Summary = "Test Event",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var iCal = ICalendarGenerator.GenerateEvent(calendarEvent);

        // Assert
        iCal.Should().Contain("UID:");
        // Extract the UID line and verify it's a valid GUID format
        var uidLine = iCal.Split('\n').FirstOrDefault(line => line.StartsWith("UID:"));
        uidLine.Should().NotBeNull();
        var uid = uidLine!.Substring(4).Trim();
        Guid.TryParse(uid, out _).Should().BeTrue();
    }

    [Test]
    public void GenerateEvent_WithAttendees_ShouldIncludeAllAttendees()
    {
        // Arrange
        var calendarEvent = new CalendarEvent
        {
            Uid = "test-uid",
            Summary = "Meeting with Attendees",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1),
            Attendees = new List<string>
            {
                "mailto:attendee1@example.com",
                "mailto:attendee2@example.com",
                "mailto:attendee3@example.com"
            }
        };

        // Act
        var iCal = ICalendarGenerator.GenerateEvent(calendarEvent);

        // Assert
        iCal.Should().Contain("ATTENDEE:mailto:attendee1@example.com");
        iCal.Should().Contain("ATTENDEE:mailto:attendee2@example.com");
        iCal.Should().Contain("ATTENDEE:mailto:attendee3@example.com");
    }

    [Test]
    public void CreateSimpleEvent_ShouldReturnValidICalendar()
    {
        // Arrange
        var startTime = new DateTime(2024, 7, 1, 10, 0, 0, DateTimeKind.Utc);
        var endTime = new DateTime(2024, 7, 1, 11, 0, 0, DateTimeKind.Utc);

        // Act
        var iCal = ICalendarGenerator.CreateSimpleEvent(
            "Simple Test Event",
            startTime,
            endTime,
            "This is a description",
            "Meeting Room B"
        );

        // Assert
        iCal.Should().StartWith("BEGIN:VCALENDAR");
        iCal.Should().Contain("END:VCALENDAR");
        iCal.Should().Contain("SUMMARY:Simple Test Event");
        iCal.Should().Contain("DTSTART:20240701T100000Z");
        iCal.Should().Contain("DTEND:20240701T110000Z");
        iCal.Should().Contain("DESCRIPTION:This is a description");
        iCal.Should().Contain("LOCATION:Meeting Room B");
    }

    [Test]
    public void CreateSimpleEvent_WithNullOptionalParameters_ShouldHandleGracefully()
    {
        // Arrange
        var startTime = DateTime.UtcNow;
        var endTime = DateTime.UtcNow.AddHours(1);

        // Act
        var iCal = ICalendarGenerator.CreateSimpleEvent("Test Event", startTime, endTime);

        // Assert
        iCal.Should().Contain("SUMMARY:Test Event");
        // With Ical.Net, empty properties might still appear but with empty values
        // We check that the actual content is empty rather than the property not existing
        if (iCal.Contains("DESCRIPTION:"))
        {
            // If DESCRIPTION exists, it should be empty or just whitespace
            var descLine = iCal.Split('\n').FirstOrDefault(line => line.StartsWith("DESCRIPTION:"))?.Trim();
            descLine.Should().Be("DESCRIPTION:");
        }
        if (iCal.Contains("LOCATION:"))
        {
            // If LOCATION exists, it should be empty or just whitespace
            var locLine = iCal.Split('\n').FirstOrDefault(line => line.StartsWith("LOCATION:"))?.Trim();
            locLine.Should().Be("LOCATION:");
        }
    }

    [TestCase("Test, Event", "Test, Event")] // Ical.Net handles escaping internally
    [TestCase("Test; Event", "Test; Event")]
    [TestCase("Test\nEvent", "Test\nEvent")]
    [TestCase("Test\\Event", "Test\\Event")]
    public void GenerateEvent_ShouldHandleSpecialCharacters(string input, string expected)
    {
        // Arrange
        var calendarEvent = new CalendarEvent
        {
            Summary = input,
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var iCal = ICalendarGenerator.GenerateEvent(calendarEvent);

        // Assert - Ical.Net handles escaping automatically, so we test that the content is properly serialized
        iCal.Should().Contain("SUMMARY:");
        iCal.Should().Contain("BEGIN:VEVENT");
        iCal.Should().Contain("END:VEVENT");
        
        // Verify we can parse it back (round-trip test)
        var parsedEvents = CalDAVXmlParser.ParseCalendarEvents(
            $@"<?xml version=""1.0"" encoding=""utf-8""?>
<d:multistatus xmlns:d=""DAV:"" xmlns:c=""urn:ietf:params:xml:ns:caldav"">
  <d:response>
    <d:href>/test.ics</d:href>
    <d:propstat>
      <d:prop>
        <c:calendar-data>{iCal}</c:calendar-data>
      </d:prop>
    </d:propstat>
  </d:response>
</d:multistatus>");
        
        parsedEvents.Should().HaveCount(1);
        parsedEvents[0].Summary.Should().Be(input); // Should round-trip correctly
    }

    [Test]
    public void GenerateEvent_ShouldIncludeDTSTAMP()
    {
        // Arrange
        var calendarEvent = new CalendarEvent
        {
            Summary = "Test Event",
            StartTime = DateTime.UtcNow,
            EndTime = DateTime.UtcNow.AddHours(1)
        };

        // Act
        var iCal = ICalendarGenerator.GenerateEvent(calendarEvent);

        // Assert
        iCal.Should().Contain("DTSTAMP:");
        // Extract DTSTAMP and verify it's in the correct format
        var dtstampLine = iCal.Split('\n').FirstOrDefault(line => line.StartsWith("DTSTAMP:"));
        dtstampLine.Should().NotBeNull();
        dtstampLine.Should().MatchRegex(@"DTSTAMP:\d{8}T\d{6}Z");
    }
}