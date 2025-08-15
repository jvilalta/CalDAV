# CalDAV.Tests

Comprehensive test suite for the CalDAV Client library using NUnit 4.

## Overview

This test project provides extensive coverage for the CalDAV client library, including:

- **Unit Tests**: Fast, isolated tests for individual components
- **Integration Tests**: Tests against real CalDAV servers
- **Performance Tests**: Benchmarks for critical operations
- **XML Parsing Tests**: Validation of CalDAV XML parsing logic
- **iCalendar Generation Tests**: Verification of RFC 5545 compliance

## Test Framework and Tools

- **NUnit 4.2.2**: Main testing framework
- **FluentAssertions 6.12.1**: Expressive assertion library
- **Moq 4.20.72**: Mocking framework for dependencies
- **Coverlet**: Code coverage collection
- **NUnit3TestAdapter**: Visual Studio test adapter

## Project Structure

```
CalDAV.Tests/
??? Models/                     # Tests for data models
?   ??? CalDAVCredentialsTests.cs
?   ??? CalendarEventTests.cs
?   ??? CalendarTests.cs
??? Utils/                      # Tests for utility classes
?   ??? CalDAVXmlGeneratorTests.cs
?   ??? CalDAVXmlParserTests.cs
?   ??? ICalendarGeneratorTests.cs
??? Integration/                # Integration tests
?   ??? CalDAVClientIntegrationTests.cs
??? Performance/                # Performance benchmarks
?   ??? CalDAVPerformanceTests.cs
??? CalDAVClientTests.cs        # Main client tests
??? TestFixtures.cs            # Test data and helpers
??? GlobalUsings.cs            # Global using statements
??? TestConfiguration.md       # Test setup guide
```

## Running Tests

### Prerequisites

1. .NET 9 SDK installed
2. (Optional) CalDAV server for integration tests

### Command Line

```bash
# Run all tests
dotnet test

# Run only unit tests (excludes integration and performance tests)
dotnet test --filter "Category!=Integration&Category!=Performance"

# Run only integration tests
dotnet test --filter "Category=Integration"

# Run only performance tests
dotnet test --filter "Category=Performance"

# Run with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

### Visual Studio

1. Open the solution in Visual Studio
2. Use Test Explorer to run individual tests or test categories
3. Right-click on test methods/classes to run specific tests

## Integration Test Setup

Integration tests require a real CalDAV server. Set these environment variables:

```bash
# Windows (Command Prompt)
set TEST_CALDAV_SERVER=https://your-server.com/caldav/
set TEST_CALDAV_USERNAME=your-username
set TEST_CALDAV_PASSWORD=your-password

# Windows (PowerShell)
$env:TEST_CALDAV_SERVER="https://your-server.com/caldav/"
$env:TEST_CALDAV_USERNAME="your-username"
$env:TEST_CALDAV_PASSWORD="your-password"

# Linux/macOS
export TEST_CALDAV_SERVER=https://your-server.com/caldav/
export TEST_CALDAV_USERNAME=your-username
export TEST_CALDAV_PASSWORD=your-password
```

If these variables are not set, integration tests will be skipped automatically.

### Recommended Test Servers

1. **Radicale (Local Development)**
   ```bash
   pip install radicale
   python -m radicale --config ""
   # Server runs on http://localhost:5232/
   ```

2. **Nextcloud Docker**
   ```bash
   docker run -d -p 8080:80 nextcloud
   # Access at http://localhost:8080/remote.php/dav/
   ```

3. **Production Servers**
   - iCloud: `https://caldav.icloud.com/`
   - Google: `https://apidata.googleusercontent.com/caldav/v2/`

## Test Categories

### Unit Tests
- ? Fast execution (< 1ms per test)
- ? No external dependencies
- ? High code coverage
- ? Isolated component testing

### Integration Tests
- ?? Requires real CalDAV server
- ?? Uses actual authentication
- ?? Creates/deletes real events
- ?? May be slower (network dependent)

### Performance Tests
- ? Benchmarks critical operations
- ?? Memory usage validation
- ?? Performance regression detection
- ?? Scalability testing

## Test Data and Fixtures

The `TestFixtures` class provides:

- **Sample XML responses** for various CalDAV operations
- **Sample calendar events** with different configurations
- **Sample calendar collections** for testing
- **Custom assertions** for CalDAV-specific validation

Example usage:
```csharp
[Test]
public void MyTest()
{
    // Use predefined test data
    var testEvent = TestFixtures.SampleEvents.CreateTestEvent();
    var sampleXml = TestFixtures.SampleXml.MultiStatusWithCalendars;
    
    // Use custom assertions
    testEvent.ShouldBeValidEvent();
    sampleXml.ShouldContainValidXmlDeclaration();
}
```

## Code Coverage

Generate and view code coverage reports:

```bash
# Generate coverage
dotnet test --collect:"XPlat Code Coverage"

# Install report generator (one time)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator -reports:"TestResults/*/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Open report
start coveragereport/index.html  # Windows
open coveragereport/index.html   # macOS
xdg-open coveragereport/index.html # Linux
```

## Continuous Integration

For CI/CD pipelines, use:

```yaml
# Example GitHub Actions step
- name: Run Tests
  run: |
    dotnet test --no-build --verbosity normal --collect:"XPlat Code Coverage" --results-directory ./coverage
    
- name: Run Integration Tests
  run: |
    dotnet test --no-build --filter "Category=Integration" --verbosity normal
  env:
    TEST_CALDAV_SERVER: ${{ secrets.TEST_CALDAV_SERVER }}
    TEST_CALDAV_USERNAME: ${{ secrets.TEST_CALDAV_USERNAME }}
    TEST_CALDAV_PASSWORD: ${{ secrets.TEST_CALDAV_PASSWORD }}
```

## Debugging Tests

### Failed Assertions
FluentAssertions provides detailed failure messages:
```
Expected calendar.DisplayName to be "Personal Calendar", but found "Work Calendar".
```

### Integration Test Issues
1. Check network connectivity to CalDAV server
2. Verify credentials are correct
3. Ensure CalDAV service is enabled
4. Check for firewall/proxy issues

### Performance Test Failures
Performance tests may fail on slower systems. Adjust timeout values if needed:
```csharp
stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000, // Increased from 1000
    "Operation should complete within reasonable time");
```

## Contributing

When adding new tests:

1. Follow the existing naming conventions
2. Use descriptive test method names
3. Include appropriate test categories
4. Add comprehensive assertions
5. Update this README if adding new test categories

## Best Practices

1. **Arrange-Act-Assert**: Structure tests clearly
2. **Descriptive Names**: Test methods should describe what they test
3. **Independent Tests**: Each test should be isolated
4. **Fast Unit Tests**: Keep unit tests under 1ms when possible
5. **Comprehensive Coverage**: Test both success and failure scenarios