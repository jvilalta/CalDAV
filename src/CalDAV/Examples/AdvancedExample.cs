using CalDAV;
using CalDAV.Models;
using CalDAV.Utils;

namespace CalDAV.Examples;

/// <summary>
/// Advanced example showing more CalDAV client features
/// </summary>
public class AdvancedExample
{
    public static async Task RunAsync()
    {
        Console.WriteLine("Advanced CalDAV Client Example");
        Console.WriteLine("==============================");

        // Get configuration from environment variables or use defaults
        var serverUrl = Environment.GetEnvironmentVariable("CALDAV_SERVER") ?? "https://your-server.com/caldav/";
        var username = Environment.GetEnvironmentVariable("CALDAV_USERNAME") ?? "your-username";
        var password = Environment.GetEnvironmentVariable("CALDAV_PASSWORD") ?? "your-password";

        Console.WriteLine($"Connecting to: {serverUrl}");
        Console.WriteLine($"Username: {username}");

        using var client = new CalDAVClient(serverUrl, username, password);

        try
        {
            // Test connection and initialize
            if (!await client.TestConnectionAsync())
            {
                Console.WriteLine("? Failed to connect to CalDAV server");
                return;
            }

            if (!await client.InitializeAsync())
            {
                Console.WriteLine("? Failed to initialize CalDAV client");
                return;
            }

            Console.WriteLine("? Successfully connected and initialized");

            // Demonstrate calendar operations
            await DemonstrateCalendarOperations(client);
            
            // Demonstrate event operations
            await DemonstrateEventOperations(client);
            
            // Demonstrate advanced event creation
            await DemonstrateAdvancedEventCreation(client);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner: {ex.InnerException.Message}");
            }
        }
    }

    private static async Task DemonstrateCalendarOperations(CalDAVClient client)
    {
        Console.WriteLine("\n?? Calendar Operations");
        Console.WriteLine("---------------------");

        var calendars = await client.GetCalendarsAsync();
        Console.WriteLine($"Found {calendars.Count} calendars:");

        foreach (var calendar in calendars)
        {
            Console.WriteLine($"  ?? {calendar.DisplayName}");
            Console.WriteLine($"     URL: {calendar.Url}");
            Console.WriteLine($"     Description: {calendar.Description ?? "No description"}");
            Console.WriteLine($"     Read-only: {calendar.IsReadOnly}");
            Console.WriteLine($"     Color: {calendar.Color ?? "Default"}");
            Console.WriteLine();
        }
    }

    private static async Task DemonstrateEventOperations(CalDAVClient client)
    {
        Console.WriteLine("\n?? Event Operations");
        Console.WriteLine("------------------");

        var calendars = await client.GetCalendarsAsync();
        if (calendars.Count == 0)
        {
            Console.WriteLine("No calendars available for event operations");
            return;
        }

        var targetCalendar = calendars.First();
        Console.WriteLine($"Working with calendar: {targetCalendar.DisplayName}");

        // Get events for the next 30 days
        var startDate = DateTime.Today;
        var endDate = DateTime.Today.AddDays(30);
        
        Console.WriteLine($"Fetching events from {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}...");
        var events = await client.GetEventsAsync(targetCalendar.Url, startDate, endDate);
        
        Console.WriteLine($"Found {events.Count} events:");
        foreach (var evt in events.Take(10)) // Show first 10 events
        {
            Console.WriteLine($"  ???  {evt.Summary}");
            Console.WriteLine($"      ?? {evt.StartTime:yyyy-MM-dd HH:mm} - {evt.EndTime:yyyy-MM-dd HH:mm}");
            Console.WriteLine($"      ?? {evt.Location ?? "No location"}");
            Console.WriteLine($"      ?? {evt.Uid}");
            if (!string.IsNullOrEmpty(evt.Description))
            {
                var shortDesc = evt.Description.Length > 50 
                    ? evt.Description.Substring(0, 50) + "..." 
                    : evt.Description;
                Console.WriteLine($"      ?? {shortDesc}");
            }
            Console.WriteLine();
        }
    }

    private static async Task DemonstrateAdvancedEventCreation(CalDAVClient client)
    {
        Console.WriteLine("\n? Advanced Event Creation");
        Console.WriteLine("-------------------------");

        var calendars = await client.GetCalendarsAsync();
        if (calendars.Count == 0)
        {
            Console.WriteLine("No calendars available for event creation");
            return;
        }

        var targetCalendar = calendars.First();
        Console.WriteLine($"Creating events in calendar: {targetCalendar.DisplayName}");

        // Create a simple event
        await CreateSimpleEvent(client, targetCalendar);
        
        // Create a complex event with attendees
        await CreateComplexEvent(client, targetCalendar);
        
        // Create a recurring event (basic example)
        await CreateRecurringEvent(client, targetCalendar);
    }

    private static async Task CreateSimpleEvent(CalDAVClient client, Calendar calendar)
    {
        try
        {
            Console.WriteLine("Creating simple event...");
            
            var simpleEventData = ICalendarGenerator.CreateSimpleEvent(
                "Simple Test Event",
                DateTime.Now.AddDays(1).Date.AddHours(14), // Tomorrow at 2 PM
                DateTime.Now.AddDays(1).Date.AddHours(15), // Tomorrow at 3 PM
                "This is a simple test event created by the C# CalDAV client",
                "Test Location"
            );

            var eventUrl = await client.CreateEventAsync(calendar.Url, simpleEventData);
            Console.WriteLine($"? Simple event created: {eventUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Failed to create simple event: {ex.Message}");
        }
    }

    private static async Task CreateComplexEvent(CalDAVClient client, Calendar calendar)
    {
        try
        {
            Console.WriteLine("Creating complex event with attendees...");
            
            var complexEvent = new CalendarEvent
            {
                Uid = Guid.NewGuid().ToString(),
                Summary = "Team Meeting - Project Review",
                Description = "Monthly project review meeting\\nAgenda:\\n- Project status\\n- Budget review\\n- Next steps",
                StartTime = DateTime.Now.AddDays(2).Date.AddHours(10), // Day after tomorrow at 10 AM
                EndTime = DateTime.Now.AddDays(2).Date.AddHours(11).AddMinutes(30), // 1.5 hours later
                Location = "Conference Room A, Building 1",
                Organizer = "mailto:manager@company.com",
                Attendees = new List<string>
                {
                    "mailto:developer1@company.com",
                    "mailto:developer2@company.com",
                    "mailto:designer@company.com"
                }
            };

            var complexEventData = ICalendarGenerator.GenerateEvent(complexEvent);
            var eventUrl = await client.CreateEventAsync(calendar.Url, complexEventData);
            Console.WriteLine($"? Complex event created: {eventUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Failed to create complex event: {ex.Message}");
        }
    }

    private static async Task CreateRecurringEvent(CalDAVClient client, Calendar calendar)
    {
        try
        {
            Console.WriteLine("Creating recurring event...");
            
            // Create a basic recurring event (weekly standup)
            var recurringEventData = $@"BEGIN:VCALENDAR
VERSION:2.0
PRODID:-//CalDAV Client//CalDAV Client 1.0//EN
CALSCALE:GREGORIAN
METHOD:PUBLISH
BEGIN:VEVENT
UID:{Guid.NewGuid()}
DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}
DTSTART:{DateTime.Now.AddDays(1).Date.AddHours(9):yyyyMMddTHHmmssZ}
DTEND:{DateTime.Now.AddDays(1).Date.AddHours(9).AddMinutes(30):yyyyMMddTHHmmssZ}
SUMMARY:Daily Standup
DESCRIPTION:Daily team standup meeting
LOCATION:Virtual - Zoom Room
RRULE:FREQ=DAILY;BYDAY=MO,TU,WE,TH,FR;COUNT=10
END:VEVENT
END:VCALENDAR";

            var eventUrl = await client.CreateEventAsync(calendar.Url, recurringEventData);
            Console.WriteLine($"? Recurring event created: {eventUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Failed to create recurring event: {ex.Message}");
        }
    }
}