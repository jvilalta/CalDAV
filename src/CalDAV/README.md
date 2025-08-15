# CalDAV Client for C#

A .NET 9 CalDAV client library that allows you to connect to CalDAV servers using a username, password, and server URL.

## Features

- **Connection Management**: Connect to CalDAV servers with username/password authentication
- **Calendar Discovery**: Automatically discover available calendars
- **Event Management**: Create, read, update, and delete calendar events
- **iCalendar Support**: Full support for iCalendar (RFC 5545) format
- **Async/Await**: Modern async programming patterns throughout

## Usage

### Basic Setup

```csharp
using CalDAV;

// Create a CalDAV client
var client = new CalDAVClient("https://your-server.com/caldav/", "username", "password");

// Test the connection
var isConnected = await client.TestConnectionAsync();
if (isConnected)
{
    Console.WriteLine("Connected successfully!");
}

// Initialize the client (discovers endpoints)
await client.InitializeAsync();
```

### Working with Calendars

```csharp
// Get list of available calendars
var calendars = await client.GetCalendarsAsync();

foreach (var calendar in calendars)
{
    Console.WriteLine($"Calendar: {calendar.DisplayName}");
    Console.WriteLine($"URL: {calendar.Url}");
    Console.WriteLine($"Description: {calendar.Description}");
}
```

### Working with Events

```csharp
// Get events from a calendar
var events = await client.GetEventsAsync(calendar.Url);

// Get events within a date range
var startDate = DateTime.Today;
var endDate = DateTime.Today.AddDays(30);
var filteredEvents = await client.GetEventsAsync(calendar.Url, startDate, endDate);

// Create a new event
var eventData = ICalendarGenerator.CreateSimpleEvent(
    "Meeting with Team",
    DateTime.Now.AddDays(1),
    DateTime.Now.AddDays(1).AddHours(1),
    "Weekly team meeting",
    "Conference Room A"
);

var eventUrl = await client.CreateEventAsync(calendar.Url, eventData);

// Update an existing event
await client.UpdateEventAsync(eventUrl, updatedEventData, etag);

// Delete an event
await client.DeleteEventAsync(eventUrl, etag);
```

### Creating Custom Events

```csharp
using CalDAV.Models;
using CalDAV.Utils;

// Create a custom event
var customEvent = new CalendarEvent
{
    Uid = Guid.NewGuid().ToString(),
    Summary = "Custom Event",
    Description = "This is a custom event",
    StartTime = DateTime.Now.AddDays(2),
    EndTime = DateTime.Now.AddDays(2).AddHours(2),
    Location = "Meeting Room B",
    Organizer = "mailto:organizer@example.com",
    Attendees = new List<string> 
    { 
        "mailto:attendee1@example.com", 
        "mailto:attendee2@example.com" 
    }
};

var iCalData = ICalendarGenerator.GenerateEvent(customEvent);
await client.CreateEventAsync(calendar.Url, iCalData);
```

## Supported CalDAV Servers

This client has been designed to work with standard CalDAV servers including:

- **Apple Calendar Server**
- **Google Calendar** (via CalDAV API)
- **Microsoft Exchange** (with CalDAV support)
- **Nextcloud/ownCloud**
- **Radicale**
- **Baikal**
- **Zimbra**

## Error Handling

```csharp
try
{
    var calendars = await client.GetCalendarsAsync();
    // Process calendars...
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Client not initialized: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"General error: {ex.Message}");
}
```

## Authentication

Currently supports HTTP Basic Authentication. For servers requiring other authentication methods, you may need to extend the client or handle authentication externally.

## Dependencies

- .NET 9.0
- System.Net.Http (built-in)
- System.Xml (built-in)

## Example Application

Run the included example application:

```bash
dotnet run
```

The example demonstrates:
- Connecting to a CalDAV server
- Discovering calendars
- Fetching events
- Creating new events

## Configuration

Update the server URL, username, and password in `Program.cs` or modify the code to accept them as command-line arguments or environment variables.

## License

This project is provided as-is for educational and development purposes.