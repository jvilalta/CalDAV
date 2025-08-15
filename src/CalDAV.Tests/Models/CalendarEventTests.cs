namespace CalDAV.Tests.Models;

[TestFixture]
public class CalendarEventTests
{
    [Test]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var calendarEvent = new CalendarEvent();

        // Assert
        calendarEvent.Uid.Should().BeEmpty();
        calendarEvent.Summary.Should().BeEmpty();
        calendarEvent.Description.Should().BeEmpty();
        calendarEvent.StartTime.Should().Be(DateTime.MinValue);
        calendarEvent.EndTime.Should().Be(DateTime.MinValue);
        calendarEvent.Location.Should().BeEmpty();
        calendarEvent.Organizer.Should().BeEmpty();
        calendarEvent.Attendees.Should().NotBeNull().And.BeEmpty();
        calendarEvent.ICalendarData.Should().BeEmpty();
        calendarEvent.ETag.Should().BeEmpty();
        calendarEvent.Href.Should().BeEmpty();
    }

    [Test]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var calendarEvent = new CalendarEvent();
        var startTime = DateTime.Now;
        var endTime = startTime.AddHours(1);
        var attendees = new List<string> { "test1@example.com", "test2@example.com" };

        // Act
        calendarEvent.Uid = "test-uid-123";
        calendarEvent.Summary = "Test Meeting";
        calendarEvent.Description = "Test Description";
        calendarEvent.StartTime = startTime;
        calendarEvent.EndTime = endTime;
        calendarEvent.Location = "Conference Room A";
        calendarEvent.Organizer = "organizer@example.com";
        calendarEvent.Attendees = attendees;
        calendarEvent.ICalendarData = "BEGIN:VCALENDAR...";
        calendarEvent.ETag = "\"123456789\"";
        calendarEvent.Href = "/calendars/user/calendar/event.ics";

        // Assert
        calendarEvent.Uid.Should().Be("test-uid-123");
        calendarEvent.Summary.Should().Be("Test Meeting");
        calendarEvent.Description.Should().Be("Test Description");
        calendarEvent.StartTime.Should().Be(startTime);
        calendarEvent.EndTime.Should().Be(endTime);
        calendarEvent.Location.Should().Be("Conference Room A");
        calendarEvent.Organizer.Should().Be("organizer@example.com");
        calendarEvent.Attendees.Should().BeEquivalentTo(attendees);
        calendarEvent.ICalendarData.Should().Be("BEGIN:VCALENDAR...");
        calendarEvent.ETag.Should().Be("\"123456789\"");
        calendarEvent.Href.Should().Be("/calendars/user/calendar/event.ics");
    }

    [Test]
    public void Attendees_ShouldSupportAddingItems()
    {
        // Arrange
        var calendarEvent = new CalendarEvent();

        // Act
        calendarEvent.Attendees.Add("attendee1@example.com");
        calendarEvent.Attendees.Add("attendee2@example.com");

        // Assert
        calendarEvent.Attendees.Should().HaveCount(2);
        calendarEvent.Attendees.Should().Contain("attendee1@example.com");
        calendarEvent.Attendees.Should().Contain("attendee2@example.com");
    }

    [Test]
    public void Attendees_WhenReplaced_ShouldWorkCorrectly()
    {
        // Arrange
        var calendarEvent = new CalendarEvent();
        calendarEvent.Attendees.Add("original@example.com");
        var newAttendees = new List<string> { "new1@example.com", "new2@example.com" };

        // Act
        calendarEvent.Attendees = newAttendees;

        // Assert
        calendarEvent.Attendees.Should().BeEquivalentTo(newAttendees);
        calendarEvent.Attendees.Should().NotContain("original@example.com");
    }
}