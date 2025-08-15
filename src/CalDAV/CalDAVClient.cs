using System.Net.Http.Headers;
using System.Text;
using CalDAV.Models;
using CalDAV.Utils;

namespace CalDAV;

/// <summary>
/// CalDAV client for connecting to and interacting with CalDAV servers
/// </summary>
public class CalDAVClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly CalDAVCredentials _credentials;
    private string? _principalUrl;
    private string? _calendarHomeUrl;

    /// <summary>
    /// Creates a new CalDAV client instance
    /// </summary>
    /// <param name="serverUrl">The CalDAV server URL (e.g., https://cal.example.com/caldav/)</param>
    /// <param name="username">Username for authentication</param>
    /// <param name="password">Password for authentication</param>
    public CalDAVClient(string serverUrl, string username, string password)
    {
        _credentials = new CalDAVCredentials(serverUrl, username, password);
        _httpClient = new HttpClient();
        
        // Set up basic authentication
        var authValue = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authValue);
        
        // Set default headers for CalDAV
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CalDAV-Client-CSharp/1.0");
    }

    /// <summary>
    /// Tests the connection to the CalDAV server
    /// </summary>
    /// <returns>True if connection is successful, false otherwise</returns>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Options, _credentials.ServerUrl));
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Discovers and initializes the CalDAV service endpoints
    /// </summary>
    public async Task<bool> InitializeAsync()
    {
        try
        {
            // Step 1: Discover current user principal
            _principalUrl = await DiscoverCurrentUserPrincipalAsync();
            if (string.IsNullOrEmpty(_principalUrl))
                return false;

            // Step 2: Discover calendar home
            _calendarHomeUrl = await DiscoverCalendarHomeAsync(_principalUrl);
            return !string.IsNullOrEmpty(_calendarHomeUrl);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Gets the list of available calendars
    /// </summary>
    /// <returns>List of calendars</returns>
    public async Task<List<Calendar>> GetCalendarsAsync()
    {
        if (string.IsNullOrEmpty(_calendarHomeUrl))
        {
            await InitializeAsync();
        }

        if (string.IsNullOrEmpty(_calendarHomeUrl))
            throw new InvalidOperationException("Calendar home URL not discovered. Call InitializeAsync() first.");

        var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), _calendarHomeUrl);
        request.Headers.Add("Depth", "1");
        request.Content = new StringContent(CalDAVXmlGenerator.GenerateCalendarPropfindXml(), 
                                          Encoding.UTF8, "application/xml");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var xmlContent = await response.Content.ReadAsStringAsync();
        return CalDAVXmlParser.ParseCalendarList(xmlContent);
    }

    /// <summary>
    /// Gets events from a specific calendar
    /// </summary>
    /// <param name="calendarUrl">The URL of the calendar</param>
    /// <param name="startTime">Optional start time filter</param>
    /// <param name="endTime">Optional end time filter</param>
    /// <returns>List of calendar events</returns>
    public async Task<List<CalendarEvent>> GetEventsAsync(string calendarUrl, DateTime? startTime = null, DateTime? endTime = null)
    {
        var request = new HttpRequestMessage(new HttpMethod("REPORT"), calendarUrl);
        request.Headers.Add("Depth", "1");
        request.Content = new StringContent(CalDAVXmlGenerator.GenerateCalendarReportXml(startTime, endTime), 
                                          Encoding.UTF8, "application/xml");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var xmlContent = await response.Content.ReadAsStringAsync();
        return CalDAVXmlParser.ParseCalendarEvents(xmlContent);
    }

    /// <summary>
    /// Creates a new event in the specified calendar
    /// </summary>
    /// <param name="calendarUrl">The URL of the calendar</param>
    /// <param name="eventData">The iCalendar data for the event</param>
    /// <param name="eventUid">Optional UID for the event (will be generated if not provided)</param>
    /// <returns>The URL of the created event</returns>
    public async Task<string> CreateEventAsync(string calendarUrl, string eventData, string? eventUid = null)
    {
        eventUid ??= Guid.NewGuid().ToString();
        var eventUrl = $"{calendarUrl.TrimEnd('/')}/{eventUid}.ics";

        var request = new HttpRequestMessage(HttpMethod.Put, eventUrl);
        request.Content = new StringContent(eventData, Encoding.UTF8, "text/calendar");
        request.Headers.Add("If-None-Match", "*"); // Ensure we don't overwrite existing events

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        return eventUrl;
    }

    /// <summary>
    /// Updates an existing event
    /// </summary>
    /// <param name="eventUrl">The URL of the event to update</param>
    /// <param name="eventData">The updated iCalendar data</param>
    /// <param name="etag">The ETag of the event for optimistic concurrency</param>
    public async Task UpdateEventAsync(string eventUrl, string eventData, string? etag = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, eventUrl);
        request.Content = new StringContent(eventData, Encoding.UTF8, "text/calendar");
        
        if (!string.IsNullOrEmpty(etag))
        {
            request.Headers.Add("If-Match", $"\"{etag}\"");
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Deletes an event
    /// </summary>
    /// <param name="eventUrl">The URL of the event to delete</param>
    /// <param name="etag">The ETag of the event for optimistic concurrency</param>
    public async Task DeleteEventAsync(string eventUrl, string? etag = null)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, eventUrl);
        
        if (!string.IsNullOrEmpty(etag))
        {
            request.Headers.Add("If-Match", $"\"{etag}\"");
        }

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Discovers the current user principal URL
    /// </summary>
    private async Task<string?> DiscoverCurrentUserPrincipalAsync()
    {
        var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), _credentials.ServerUrl);
        request.Headers.Add("Depth", "0");
        request.Content = new StringContent(CalDAVXmlGenerator.GenerateCurrentUserPrincipalXml(), 
                                          Encoding.UTF8, "application/xml");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var xmlContent = await response.Content.ReadAsStringAsync();
        var principalPath = CalDAVXmlParser.ExtractHref(xmlContent, "//d:current-user-principal/d:href");
        
        if (string.IsNullOrEmpty(principalPath)) return null;

        return principalPath.StartsWith("http") ? principalPath : 
               new Uri(new Uri(_credentials.ServerUrl), principalPath).ToString();
    }

    /// <summary>
    /// Discovers the calendar home URL for the user
    /// </summary>
    private async Task<string?> DiscoverCalendarHomeAsync(string principalUrl)
    {
        var request = new HttpRequestMessage(new HttpMethod("PROPFIND"), principalUrl);
        request.Headers.Add("Depth", "0");
        request.Content = new StringContent(CalDAVXmlGenerator.GenerateCalendarHomeXml(), 
                                          Encoding.UTF8, "application/xml");

        var response = await _httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode) return null;

        var xmlContent = await response.Content.ReadAsStringAsync();
        var calendarHomePath = CalDAVXmlParser.ExtractHref(xmlContent, "//c:calendar-home-set/d:href");
        
        if (string.IsNullOrEmpty(calendarHomePath)) return null;

        return calendarHomePath.StartsWith("http") ? calendarHomePath : 
               new Uri(new Uri(_credentials.ServerUrl), calendarHomePath).ToString();
    }

    /// <summary>
    /// Disposes the HTTP client
    /// </summary>
    public void Dispose()
    {
        _httpClient?.Dispose();
    }
}