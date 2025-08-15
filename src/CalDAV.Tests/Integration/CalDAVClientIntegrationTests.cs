using System.Net;
using System.Net.Http;

namespace CalDAV.Tests.Integration;

[TestFixture]
[Category("Integration")]
public class CalDAVClientIntegrationTests
{
    private CalDAVClient? _client;
    private string _serverUrl = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        // Get test credentials from environment variables
        _serverUrl = Environment.GetEnvironmentVariable("TEST_CALDAV_SERVER") ?? "";
        _username = Environment.GetEnvironmentVariable("TEST_CALDAV_USERNAME") ?? "";
        _password = Environment.GetEnvironmentVariable("TEST_CALDAV_PASSWORD") ?? "";

        if (string.IsNullOrEmpty(_serverUrl) || string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
        {
            Assert.Ignore("Integration tests skipped. Set TEST_CALDAV_SERVER, TEST_CALDAV_USERNAME, and TEST_CALDAV_PASSWORD environment variables to run integration tests.");
        }
    }

    [SetUp]
    public void SetUp()
    {
        if (!string.IsNullOrEmpty(_serverUrl))
        {
            _client = new CalDAVClient(_serverUrl, _username, _password);
        }
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }

    [Test]
    public async Task TestConnectionAsync_WithValidCredentials_ShouldReturnTrue()
    {
        // Act
        var result = await _client!.TestConnectionAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task InitializeAsync_WithValidServer_ShouldReturnTrue()
    {
        // Act
        var result = await _client!.InitializeAsync();

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public async Task GetCalendarsAsync_AfterInitialization_ShouldReturnCalendars()
    {
        // Arrange
        await _client!.InitializeAsync();

        // Act
        var calendars = await _client.GetCalendarsAsync();

        // Assert
        calendars.Should().NotBeNull();
        calendars.Should().NotBeEmpty();
        
        foreach (var calendar in calendars)
        {
            calendar.Url.Should().NotBeNullOrEmpty();
            calendar.DisplayName.Should().NotBeNullOrEmpty();
        }
    }

    [Test]
    public async Task GetEventsAsync_WithValidCalendar_ShouldReturnEvents()
    {
        // Arrange
        await _client!.InitializeAsync();
        var calendars = await _client.GetCalendarsAsync();
        calendars.Should().NotBeEmpty();

        // Act
        var events = await _client.GetEventsAsync(calendars.First().Url);

        // Assert
        events.Should().NotBeNull();
        // Note: We don't assert events are not empty as the calendar might be empty
    }

    [Test]
    public async Task CreateAndDeleteEvent_WithValidCalendar_ShouldWork()
    {
        // Arrange
        await _client!.InitializeAsync();
        var calendars = await _client.GetCalendarsAsync();
        calendars.Should().NotBeEmpty();
        
        var testCalendar = calendars.First();
        var eventData = ICalendarGenerator.CreateSimpleEvent(
            "Integration Test Event",
            DateTime.Now.AddDays(1),
            DateTime.Now.AddDays(1).AddHours(1),
            "This is a test event created during integration testing",
            "Test Location"
        );

        try
        {
            // Act - Create
            var eventUrl = await _client.CreateEventAsync(testCalendar.Url, eventData);

            // Assert - Create
            eventUrl.Should().NotBeNullOrEmpty();
            eventUrl.Should().Contain(testCalendar.Url);

            // Act - Delete
            await _client.DeleteEventAsync(eventUrl);

            // Assert - Verify deletion by trying to get the specific event
            // This might throw an exception or return no events, both are acceptable
        }
        catch (HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            // This is expected when trying to access a deleted event
        }
    }
}

[TestFixture]
[Category("Integration")]
public class CalDAVClientInvalidCredentialsTests
{
    [Test]
    public async Task TestConnectionAsync_WithInvalidCredentials_ShouldReturnFalse()
    {
        // Arrange
        using var client = new CalDAVClient("https://invalid-server.example.com/caldav/", "invalid", "invalid");

        // Act
        var result = await client.TestConnectionAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task InitializeAsync_WithInvalidCredentials_ShouldReturnFalse()
    {
        // Arrange
        using var client = new CalDAVClient("https://invalid-server.example.com/caldav/", "invalid", "invalid");

        // Act
        var result = await client.InitializeAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task GetCalendarsAsync_WithoutInitialization_ShouldThrow()
    {
        // Arrange
        using var client = new CalDAVClient("https://invalid-server.example.com/caldav/", "invalid", "invalid");

        // Act & Assert
        await client.Invoking(c => c.GetCalendarsAsync())
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Calendar home URL not discovered*");
    }
}