namespace CalDAV.Tests.Models;

[TestFixture]
public class CalDAVCredentialsTests
{
    [Test]
    public void Constructor_ShouldSetAllProperties()
    {
        // Arrange
        const string serverUrl = "https://test.com/caldav/";
        const string username = "testuser";
        const string password = "testpass";

        // Act
        var credentials = new CalDAVCredentials(serverUrl, username, password);

        // Assert
        credentials.ServerUrl.Should().Be(serverUrl);
        credentials.Username.Should().Be(username);
        credentials.Password.Should().Be(password);
    }

    [Test]
    public void Constructor_WithEmptyValues_ShouldSetEmptyStrings()
    {
        // Arrange & Act
        var credentials = new CalDAVCredentials("", "", "");

        // Assert
        credentials.ServerUrl.Should().BeEmpty();
        credentials.Username.Should().BeEmpty();
        credentials.Password.Should().BeEmpty();
    }

    [Test]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var credentials = new CalDAVCredentials("", "", "");

        // Act
        credentials.ServerUrl = "https://new-server.com/caldav/";
        credentials.Username = "newuser";
        credentials.Password = "newpass";

        // Assert
        credentials.ServerUrl.Should().Be("https://new-server.com/caldav/");
        credentials.Username.Should().Be("newuser");
        credentials.Password.Should().Be("newpass");
    }

    [TestCase("https://caldav.example.com/", "user@example.com", "secret123")]
    [TestCase("http://localhost:5232/", "admin", "admin")]
    [TestCase("https://caldav.icloud.com/", "apple@icloud.com", "app-specific-password")]
    public void Constructor_WithVariousInputs_ShouldWorkCorrectly(string serverUrl, string username, string password)
    {
        // Act
        var credentials = new CalDAVCredentials(serverUrl, username, password);

        // Assert
        credentials.ServerUrl.Should().Be(serverUrl);
        credentials.Username.Should().Be(username);
        credentials.Password.Should().Be(password);
    }
}