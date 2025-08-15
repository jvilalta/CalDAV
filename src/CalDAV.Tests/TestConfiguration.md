# CalDAV Test Configuration

This file contains test configuration settings and guidance for running the test suite.

## Environment Variables for Integration Tests

Set these environment variables to run integration tests against a real CalDAV server:

```bash
# Required for integration tests
TEST_CALDAV_SERVER=https://your-caldav-server.com/caldav/
TEST_CALDAV_USERNAME=your-test-username
TEST_CALDAV_PASSWORD=your-test-password
```

## Running Tests

### Run all tests
```bash
dotnet test
```

### Run only unit tests (exclude integration tests)
```bash
dotnet test --filter "Category!=Integration"
```

### Run only integration tests
```bash
dotnet test --filter "Category=Integration"
```

### Run with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## Test Categories

- **Unit Tests**: Fast tests that don't require external dependencies
- **Integration Tests**: Tests that require a real CalDAV server

## Popular CalDAV Servers for Testing

### Radicale (Local Testing)
```bash
# Install and run Radicale locally
pip install radicale
python -m radicale --config ""

# Set environment variables
TEST_CALDAV_SERVER=http://localhost:5232/
TEST_CALDAV_USERNAME=testuser
TEST_CALDAV_PASSWORD=testpass
```

### Nextcloud (Docker)
```bash
# Run Nextcloud in Docker
docker run -d -p 8080:80 nextcloud

# Access at http://localhost:8080 and create a user
# Then set:
TEST_CALDAV_SERVER=http://localhost:8080/remote.php/dav/
TEST_CALDAV_USERNAME=your-nextcloud-user
TEST_CALDAV_PASSWORD=your-nextcloud-password
```

### Apple iCloud (Real Account)
```bash
# Use app-specific password
TEST_CALDAV_SERVER=https://caldav.icloud.com/
TEST_CALDAV_USERNAME=your-apple-id@icloud.com
TEST_CALDAV_PASSWORD=your-app-specific-password
```

### Google Calendar (Real Account)
```bash
# Use app-specific password
TEST_CALDAV_SERVER=https://apidata.googleusercontent.com/caldav/v2/
TEST_CALDAV_USERNAME=your-email@gmail.com
TEST_CALDAV_PASSWORD=your-app-specific-password
```

## Test Data

The test suite uses sample XML responses and iCalendar data defined in `TestFixtures.cs`. 
This allows unit tests to run without requiring a real CalDAV server.

## Troubleshooting

### Integration Tests Skipped
- Ensure environment variables are set correctly
- Test server connectivity manually
- Check authentication credentials

### SSL/TLS Issues
- For development servers with self-signed certificates, you may need to configure certificate validation

### Authentication Issues
- Use app-specific passwords for services like Google Calendar and iCloud
- Ensure the CalDAV service is enabled on your server