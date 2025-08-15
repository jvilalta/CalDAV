namespace CalDAV.Tests.Models;

[TestFixture]
public class CalendarTests
{
    [Test]
    public void DefaultConstructor_ShouldInitializeWithDefaults()
    {
        // Act
        var calendar = new Calendar();

        // Assert
        calendar.Name.Should().BeEmpty();
        calendar.DisplayName.Should().BeEmpty();
        calendar.Description.Should().BeEmpty();
        calendar.Url.Should().BeEmpty();
        calendar.Color.Should().BeEmpty();
        calendar.IsReadOnly.Should().BeFalse();
        calendar.ETag.Should().BeEmpty();
        calendar.CTag.Should().BeEmpty();
    }

    [Test]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var calendar = new Calendar();

        // Act
        calendar.Name = "personal";
        calendar.DisplayName = "Personal Calendar";
        calendar.Description = "My personal events";
        calendar.Url = "/calendars/user/personal/";
        calendar.Color = "#FF5733";
        calendar.IsReadOnly = true;
        calendar.ETag = "\"987654321\"";
        calendar.CTag = "\"tag123\"";

        // Assert
        calendar.Name.Should().Be("personal");
        calendar.DisplayName.Should().Be("Personal Calendar");
        calendar.Description.Should().Be("My personal events");
        calendar.Url.Should().Be("/calendars/user/personal/");
        calendar.Color.Should().Be("#FF5733");
        calendar.IsReadOnly.Should().BeTrue();
        calendar.ETag.Should().Be("\"987654321\"");
        calendar.CTag.Should().Be("\"tag123\"");
    }

    [TestCase("work", "Work Calendar", "Work related events", "#0066CC", true)]
    [TestCase("personal", "Personal", "", "#FF3366", false)]
    [TestCase("", "", "", "", false)]
    public void Properties_WithVariousValues_ShouldWorkCorrectly(
        string name, string displayName, string description, string color, bool isReadOnly)
    {
        // Arrange
        var calendar = new Calendar();

        // Act
        calendar.Name = name;
        calendar.DisplayName = displayName;
        calendar.Description = description;
        calendar.Color = color;
        calendar.IsReadOnly = isReadOnly;

        // Assert
        calendar.Name.Should().Be(name);
        calendar.DisplayName.Should().Be(displayName);
        calendar.Description.Should().Be(description);
        calendar.Color.Should().Be(color);
        calendar.IsReadOnly.Should().Be(isReadOnly);
    }
}