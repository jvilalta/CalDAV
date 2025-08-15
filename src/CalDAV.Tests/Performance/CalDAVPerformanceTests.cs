using System.Diagnostics;

namespace CalDAV.Tests.Performance;

[TestFixture]
[Category("Performance")]
public class CalDAVPerformanceTests
{
    [Test]
    public void ICalendarGenerator_CreateSimpleEvent_ShouldBefast()
    {
        // Arrange
        const int iterations = 1000;
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var iCal = ICalendarGenerator.CreateSimpleEvent(
                $"Event {i}",
                DateTime.Now.AddDays(i),
                DateTime.Now.AddDays(i).AddHours(1),
                $"Description for event {i}",
                $"Location {i}"
            );
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000, 
            $"Creating {iterations} events should take less than 1 second");
        
        TestContext.WriteLine($"Created {iterations} events in {stopwatch.ElapsedMilliseconds}ms");
        TestContext.WriteLine($"Average time per event: {(double)stopwatch.ElapsedMilliseconds / iterations:F2}ms");
    }

    [Test]
    public void CalDAVXmlParser_ParseCalendarList_ShouldBefast()
    {
        // Arrange
        const int iterations = 500;
        var xml = TestFixtures.SampleXml.MultiStatusWithCalendars;
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var calendars = CalDAVXmlParser.ParseCalendarList(xml);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            $"Parsing {iterations} calendar lists should take less than 1 second");
        
        TestContext.WriteLine($"Parsed {iterations} calendar lists in {stopwatch.ElapsedMilliseconds}ms");
        TestContext.WriteLine($"Average time per parse: {(double)stopwatch.ElapsedMilliseconds / iterations:F2}ms");
    }

    [Test]
    public void CalDAVXmlParser_ParseCalendarEvents_ShouldBefast()
    {
        // Arrange
        const int iterations = 500;
        var xml = TestFixtures.SampleXml.MultiStatusWithEvents;
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var events = CalDAVXmlParser.ParseCalendarEvents(xml);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            $"Parsing {iterations} event lists should take less than 1 second");
        
        TestContext.WriteLine($"Parsed {iterations} event lists in {stopwatch.ElapsedMilliseconds}ms");
        TestContext.WriteLine($"Average time per parse: {(double)stopwatch.ElapsedMilliseconds / iterations:F2}ms");
    }

    [Test]
    public void CalDAVXmlGenerator_GenerateCalendarReportXml_ShouldBefast()
    {
        // Arrange
        const int iterations = 1000;
        var startTime = DateTime.Now;
        var endTime = DateTime.Now.AddMonths(1);
        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var xml = CalDAVXmlGenerator.GenerateCalendarReportXml(startTime, endTime);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(500,
            $"Generating {iterations} XML reports should take less than 500ms");
        
        TestContext.WriteLine($"Generated {iterations} XML reports in {stopwatch.ElapsedMilliseconds}ms");
        TestContext.WriteLine($"Average time per generation: {(double)stopwatch.ElapsedMilliseconds / iterations:F2}ms");
    }

    [Test]
    public void ICalendarGenerator_GenerateEvent_WithComplexEvent_ShouldBefast()
    {
        // Arrange
        const int iterations = 500;
        var complexEvent = new CalendarEvent
        {
            Uid = "complex-event-performance-test",
            Summary = "Complex Performance Test Event with Long Summary",
            Description = "This is a very long description for a complex event that includes multiple lines\nand various special characters like commas, semicolons; and backslashes\\",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(2),
            Location = "Complex Location with Special Characters: Room A&B; Building 1, Floor 2\\Conference Center",
            Organizer = "mailto:complex.organizer@performance-test.example.com",
            Attendees = new List<string>
            {
                "mailto:attendee1@performance-test.example.com",
                "mailto:attendee2@performance-test.example.com",
                "mailto:attendee3@performance-test.example.com",
                "mailto:attendee4@performance-test.example.com",
                "mailto:attendee5@performance-test.example.com"
            }
        };

        var stopwatch = Stopwatch.StartNew();

        // Act
        for (int i = 0; i < iterations; i++)
        {
            var iCal = ICalendarGenerator.GenerateEvent(complexEvent);
        }

        stopwatch.Stop();

        // Assert
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000,
            $"Generating {iterations} complex events should take less than 1 second");
        
        TestContext.WriteLine($"Generated {iterations} complex events in {stopwatch.ElapsedMilliseconds}ms");
        TestContext.WriteLine($"Average time per complex event: {(double)stopwatch.ElapsedMilliseconds / iterations:F2}ms");
    }

    [Test]
    public void Memory_Usage_ShouldBeLow()
    {
        // Arrange
        var initialMemory = GC.GetTotalMemory(true);
        const int iterations = 100;

        // Act
        for (int i = 0; i < iterations; i++)
        {
            // Create events
            var eventData = ICalendarGenerator.CreateSimpleEvent(
                $"Memory Test Event {i}",
                DateTime.Now.AddDays(i),
                DateTime.Now.AddDays(i).AddHours(1)
            );

            // Parse XML
            var calendars = CalDAVXmlParser.ParseCalendarList(TestFixtures.SampleXml.MultiStatusWithCalendars);
            var events = CalDAVXmlParser.ParseCalendarEvents(TestFixtures.SampleXml.MultiStatusWithEvents);

            // Generate XML
            var xml = CalDAVXmlGenerator.GenerateCalendarReportXml();
        }

        // Force garbage collection and measure memory
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var finalMemory = GC.GetTotalMemory(false);
        var memoryIncrease = finalMemory - initialMemory;

        // Assert
        // Memory increase should be reasonable (less than 10MB for these operations)
        memoryIncrease.Should().BeLessThan(10 * 1024 * 1024, 
            "Memory usage should not increase significantly");

        TestContext.WriteLine($"Initial memory: {initialMemory / 1024:N0} KB");
        TestContext.WriteLine($"Final memory: {finalMemory / 1024:N0} KB");
        TestContext.WriteLine($"Memory increase: {memoryIncrease / 1024:N0} KB");
    }
}