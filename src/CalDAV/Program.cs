using CalDAV;
using CalDAV.Utils;
using CalDAV.Examples;

namespace CalDAV;

/// <summary>
/// Example program demonstrating how to use the CalDAV client
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("CalDAV Client for C#");
        Console.WriteLine("===================");

        // Check if user wants to run advanced example
        if (args.Length > 0 && args[0].ToLower() == "advanced")
        {
            await AdvancedExample.RunAsync();
            return;
        }

        // Basic example
        await RunBasicExample();
    }

    static async Task RunBasicExample()
    {
        Console.WriteLine("Basic Example");
        Console.WriteLine("============");

        // Get configuration from environment variables or prompt user
        var serverUrl = Environment.GetEnvironmentVariable("CALDAV_SERVER");
        var username = Environment.GetEnvironmentVariable("CALDAV_USERNAME");
        var password = Environment.GetEnvironmentVariable("CALDAV_PASSWORD");

        // If not provided via environment variables, use defaults or prompt
        if (string.IsNullOrEmpty(serverUrl))
        {
            Console.WriteLine("Set environment variables for automatic connection:");
            Console.WriteLine("  CALDAV_SERVER=https://your-server.com/caldav/");
            Console.WriteLine("  CALDAV_USERNAME=your-username");
            Console.WriteLine("  CALDAV_PASSWORD=your-password");
            Console.WriteLine();
            Console.WriteLine("Or run with 'dotnet run advanced' for the advanced example");
            Console.WriteLine();
            
            // Use example values for demo
            serverUrl = "https://your-caldav-server.com/caldav/";
            username = "your-username";
            password = "your-password";
            
            Console.WriteLine("Using example values (will likely fail to connect):");
            Console.WriteLine($"Server: {serverUrl}");
            Console.WriteLine($"Username: {username}");
            Console.WriteLine("Password: [hidden]");
            Console.WriteLine();
        }

        try
        {
            // Create CalDAV client
            using var client = new CalDAVClient(serverUrl, username, password);

            // Test connection
            Console.WriteLine("Testing connection...");
            var isConnected = await client.TestConnectionAsync();
            if (!isConnected)
            {
                Console.WriteLine("? Failed to connect to CalDAV server.");
                Console.WriteLine("   Please check your server URL, username, and password.");
                Console.WriteLine("   Make sure the server supports CalDAV.");
                return;
            }
            Console.WriteLine("? Connected successfully!");

            // Initialize the client (discover endpoints)
            Console.WriteLine("Initializing client...");
            var initialized = await client.InitializeAsync();
            if (!initialized)
            {
                Console.WriteLine("? Failed to initialize CalDAV client.");
                Console.WriteLine("   The server may not support required CalDAV features.");
                return;
            }
            Console.WriteLine("? Client initialized!");

            // Get list of calendars
            Console.WriteLine("\nFetching calendars...");
            var calendars = await client.GetCalendarsAsync();
            
            Console.WriteLine($"Found {calendars.Count} calendar(s):");
            foreach (var calendar in calendars)
            {
                Console.WriteLine($"  ?? {calendar.DisplayName} ({calendar.Url})");
                if (!string.IsNullOrEmpty(calendar.Description))
                    Console.WriteLine($"     Description: {calendar.Description}");
            }

            // If we have calendars, demonstrate working with events
            if (calendars.Count > 0)
            {
                var firstCalendar = calendars[0];
                Console.WriteLine($"\nWorking with calendar: {firstCalendar.DisplayName}");

                // Get events from the first calendar
                Console.WriteLine("Fetching events...");
                var events = await client.GetEventsAsync(firstCalendar.Url);
                Console.WriteLine($"Found {events.Count} event(s):");
                
                foreach (var evt in events.Take(5)) // Show first 5 events
                {
                    Console.WriteLine($"  ???  {evt.Summary}");
                    Console.WriteLine($"      ?? Start: {evt.StartTime}");
                    Console.WriteLine($"      ?? End: {evt.EndTime}");
                    if (!string.IsNullOrEmpty(evt.Location))
                        Console.WriteLine($"      ?? Location: {evt.Location}");
                    Console.WriteLine();
                }

                // Example: Create a new event
                Console.WriteLine("Creating a new test event...");
                var newEventData = ICalendarGenerator.CreateSimpleEvent(
                    "Test Event from C# CalDAV Client",
                    DateTime.Now.AddDays(1),
                    DateTime.Now.AddDays(1).AddHours(1),
                    "This is a test event created by the C# CalDAV client",
                    "Virtual Meeting"
                );

                try
                {
                    var eventUrl = await client.CreateEventAsync(firstCalendar.Url, newEventData);
                    Console.WriteLine($"? Event created successfully at: {eventUrl}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"? Failed to create event: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("No calendars found. You may need to create a calendar first.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"? Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("Example completed!");
        Console.WriteLine("To run the advanced example: dotnet run advanced");
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Reads password from console without displaying characters
    /// </summary>
    private static string ReadPassword()
    {
        var password = "";
        ConsoleKeyInfo key;
        
        do
        {
            key = Console.ReadKey(true);
            
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        }
        while (key.Key != ConsoleKey.Enter);
        
        Console.WriteLine();
        return password;
    }
}