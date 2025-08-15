using System.Net;
using System.Net.Http;

namespace CalDAV.Tests;

[TestFixture]
public class CalDAVClientTests
{
    private Mock<HttpMessageHandler> _mockHttpHandler = null!;
    private HttpClient _httpClient = null!;
    private CalDAVClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _mockHttpHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpHandler.Object);
        
        // We need to use reflection to set the private HttpClient field
        // This is a simplified approach for testing - in a real scenario,
        // you might want to refactor CalDAVClient to accept HttpClient as a dependency
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
        _httpClient?.Dispose();
    }

    [Test]
    public void Constructor_ShouldSetCredentials()
    {
        // Arrange & Act
        _client = new CalDAVClient("https://test.com/caldav/", "testuser", "testpass");

        // Assert
        _client.Should().NotBeNull();
        // Note: We can't easily test the private credentials without exposing them
        // In a real implementation, you might want to add a method to validate this
    }

    [Test]
    public void Constructor_WithEmptyParameters_ShouldNotThrow()
    {
        // Arrange & Act
        _client = new CalDAVClient("", "", "");

        // Assert
        _client.Should().NotBeNull();
    }

    [Test]
    public void Dispose_ShouldNotThrow()
    {
        // Arrange
        _client = new CalDAVClient("https://test.com/caldav/", "user", "pass");

        // Act & Assert
        _client.Invoking(c => c.Dispose()).Should().NotThrow();
    }

    [Test]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        // Arrange
        _client = new CalDAVClient("https://test.com/caldav/", "user", "pass");

        // Act & Assert
        _client.Invoking(c => c.Dispose()).Should().NotThrow();
        _client.Invoking(c => c.Dispose()).Should().NotThrow();
    }

    [TestCase("https://caldav.example.com/", "user@example.com", "password123")]
    [TestCase("http://localhost:5232/", "admin", "admin")]
    [TestCase("https://caldav.icloud.com/", "apple@icloud.com", "app-password")]
    public void Constructor_WithVariousValidInputs_ShouldWork(string serverUrl, string username, string password)
    {
        // Act
        _client = new CalDAVClient(serverUrl, username, password);

        // Assert
        _client.Should().NotBeNull();
    }
}

[TestFixture]
public class CalDAVClientAsyncTests
{
    private CalDAVClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _client = new CalDAVClient("https://test-server.example.com/caldav/", "testuser", "testpass");
    }

    [TearDown]
    public void TearDown()
    {
        _client?.Dispose();
    }

    [Test]
    public async Task TestConnectionAsync_WithInvalidServer_ShouldReturnFalse()
    {
        // Act
        var result = await _client.TestConnectionAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task InitializeAsync_WithInvalidServer_ShouldReturnFalse()
    {
        // Act
        var result = await _client.InitializeAsync();

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public async Task GetCalendarsAsync_WithoutInitialization_ShouldThrowInvalidOperationException()
    {
        // Act & Assert
        await _client.Invoking(c => c.GetCalendarsAsync())
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Calendar home URL not discovered*");
    }

    [Test]
    public async Task CreateEventAsync_WithInvalidCalendarUrl_ShouldThrow()
    {
        // Arrange
        var eventData = ICalendarGenerator.CreateSimpleEvent(
            "Test Event", 
            DateTime.Now, 
            DateTime.Now.AddHours(1)
        );

        // Act & Assert
        await _client.Invoking(c => c.CreateEventAsync("invalid-url", eventData))
            .Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task UpdateEventAsync_WithInvalidEventUrl_ShouldThrow()
    {
        // Arrange
        var eventData = ICalendarGenerator.CreateSimpleEvent(
            "Updated Event", 
            DateTime.Now, 
            DateTime.Now.AddHours(1)
        );

        // Act & Assert
        await _client.Invoking(c => c.UpdateEventAsync("invalid-url", eventData))
            .Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task DeleteEventAsync_WithInvalidEventUrl_ShouldThrow()
    {
        // Act & Assert
        await _client.Invoking(c => c.DeleteEventAsync("invalid-url"))
            .Should().ThrowAsync<Exception>();
    }
}