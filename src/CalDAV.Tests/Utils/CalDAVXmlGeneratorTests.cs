namespace CalDAV.Tests.Utils;

[TestFixture]
public class CalDAVXmlGeneratorTests
{
    [Test]
    public void GenerateCalendarPropfindXml_ShouldReturnValidXml()
    {
        // Act
        var xml = CalDAVXmlGenerator.GenerateCalendarPropfindXml();

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xml.Should().Contain("<d:propfind");
        xml.Should().Contain("xmlns:d=\"DAV:\"");
        xml.Should().Contain("xmlns:cs=\"http://calendarserver.org/ns/\"");
        xml.Should().Contain("xmlns:c=\"urn:ietf:params:xml:ns:caldav\"");
        xml.Should().Contain("<d:displayname />");
        xml.Should().Contain("<d:resourcetype />");
        xml.Should().Contain("<cs:getctag />");
        xml.Should().Contain("<c:calendar-description />");
        xml.Should().Contain("<c:calendar-color />");
        xml.Should().Contain("<c:supported-calendar-component-set />");
        xml.Should().Contain("</d:propfind>");
    }

    [Test]
    public void GenerateCalendarReportXml_WithNoTimeRange_ShouldReturnValidXml()
    {
        // Act
        var xml = CalDAVXmlGenerator.GenerateCalendarReportXml();

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xml.Should().Contain("<c:calendar-query");
        xml.Should().Contain("xmlns:d=\"DAV:\"");
        xml.Should().Contain("xmlns:c=\"urn:ietf:params:xml:ns:caldav\"");
        xml.Should().Contain("<d:getetag />");
        xml.Should().Contain("<c:calendar-data />");
        xml.Should().Contain("<c:comp-filter name=\"VCALENDAR\">");
        xml.Should().Contain("<c:comp-filter name=\"VEVENT\">");
        xml.Should().Contain("start=\"19700101T000000Z\"");
        xml.Should().Contain("end=\"20380119T031407Z\"");
        xml.Should().Contain("</c:calendar-query>");
    }

    [Test]
    public void GenerateCalendarReportXml_WithTimeRange_ShouldUseProvidedDates()
    {
        // Arrange
        var startTime = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endTime = new DateTime(2024, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        var xml = CalDAVXmlGenerator.GenerateCalendarReportXml(startTime, endTime);

        // Assert
        xml.Should().Contain("start=\"20240101T000000Z\"");
        xml.Should().Contain("end=\"20241231T235959Z\"");
    }

    [Test]
    public void GenerateCalendarReportXml_WithOnlyStartTime_ShouldUseStartTimeAndDefaultEnd()
    {
        // Arrange
        var startTime = new DateTime(2024, 6, 15, 12, 30, 0, DateTimeKind.Utc);

        // Act
        var xml = CalDAVXmlGenerator.GenerateCalendarReportXml(startTime);

        // Assert
        xml.Should().Contain("start=\"20240615T123000Z\"");
        xml.Should().Contain("end=\"20380119T031407Z\""); // Default end time
    }

    [Test]
    public void GenerateCurrentUserPrincipalXml_ShouldReturnValidXml()
    {
        // Act
        var xml = CalDAVXmlGenerator.GenerateCurrentUserPrincipalXml();

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xml.Should().Contain("<d:propfind xmlns:d=\"DAV:\">");
        xml.Should().Contain("<d:current-user-principal />");
        xml.Should().Contain("</d:propfind>");
    }

    [Test]
    public void GenerateCalendarHomeXml_ShouldReturnValidXml()
    {
        // Act
        var xml = CalDAVXmlGenerator.GenerateCalendarHomeXml();

        // Assert
        xml.Should().NotBeNullOrEmpty();
        xml.Should().Contain("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        xml.Should().Contain("<d:propfind");
        xml.Should().Contain("xmlns:d=\"DAV:\"");
        xml.Should().Contain("xmlns:c=\"urn:ietf:params:xml:ns:caldav\"");
        xml.Should().Contain("<c:calendar-home-set />");
        xml.Should().Contain("</d:propfind>");
    }

    [Test]
    public void AllGeneratedXml_ShouldBeWellFormed()
    {
        // Arrange & Act
        var xmlDocuments = new[]
        {
            CalDAVXmlGenerator.GenerateCalendarPropfindXml(),
            CalDAVXmlGenerator.GenerateCalendarReportXml(),
            CalDAVXmlGenerator.GenerateCurrentUserPrincipalXml(),
            CalDAVXmlGenerator.GenerateCalendarHomeXml()
        };

        // Assert
        foreach (var xml in xmlDocuments)
        {
            // This should not throw an exception if XML is well-formed
            var xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xml);
            xmlDoc.DocumentElement.Should().NotBeNull();
        }
    }
}