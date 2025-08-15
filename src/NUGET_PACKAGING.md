# NuGet Package Creation Guide

This document provides information about creating and publishing the CalDAV.Client NuGet package.

## Package Information

- **Package ID**: CalDAV.Client
- **Version**: 1.0.0
- **Target Framework**: .NET 9.0
- **License**: MIT

## Building the Package

To create the NuGet package locally:

```bash
# Build the project
dotnet build --configuration Release

# Create the package
dotnet pack --configuration Release --output ./nupkg

# Or create and build in one step
dotnet pack --configuration Release --output ./nupkg CalDAV/CalDAV.csproj
```

### Using the Provided Scripts

**PowerShell (Recommended)**:
```powershell
# Basic package creation
.\package-caldav.ps1

# Create and push to NuGet.org
.\package-caldav.ps1 -Push -ApiKey YOUR_API_KEY

# Custom configuration
.\package-caldav.ps1 -Configuration Debug -OutputPath ./debug-packages
```

**Batch file**:
```cmd
package-caldav.bat
```

## Package Contents

The package includes:
- **CalDAV Client Library** - Main library for CalDAV operations
- **XML Documentation** - IntelliSense support
- **Symbol Package** (.snupkg) - Debugging support
- **README.md** - Documentation

## Known Build Warnings

The build process may show XML documentation warnings for missing comments on public members. These are informational and don't prevent package creation. To address them, add XML documentation comments to public properties and methods.

Example:
```csharp
/// <summary>
/// Gets or sets the CalDAV server URL
/// </summary>
public string ServerUrl { get; set; } = string.Empty;
```

## Publishing to NuGet.org

1. **Get an API Key**:
   - Go to https://www.nuget.org/account/apikeys
   - Create a new API key with "Push new packages and package versions" permission

2. **Push the Package**:
   ```bash
   dotnet nuget push ./nupkg/CalDAV.Client.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

3. **Push the Symbol Package**:
   ```bash
   dotnet nuget push ./nupkg/CalDAV.Client.1.0.0.snupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
   ```

## Local Testing

Before publishing, test the package locally:

1. **Create a local NuGet source**:
   ```bash
   # Add local folder as package source
   dotnet nuget add source ./nupkg --name local-packages
   ```

2. **Create a test project**:
   ```bash
   mkdir TestCalDAV
   cd TestCalDAV
   dotnet new console
   dotnet add package CalDAV.Client --source local-packages
   ```

## Version Management

Update version numbers in `CalDAV.csproj`:
- `<PackageVersion>` - NuGet package version
- `<AssemblyVersion>` - Assembly version
- `<FileVersion>` - File version

Follow semantic versioning (Major.Minor.Patch):
- **Major**: Breaking changes
- **Minor**: New features, backward compatible
- **Patch**: Bug fixes, backward compatible

## Package Validation

Before publishing, validate the package:

```bash
# Install dotnet-validate tool (if not already installed)
dotnet tool install --global dotnet-validate

# Validate the package
dotnet validate package ./nupkg/CalDAV.Client.1.0.0.nupkg
```

## Updating Package Metadata

Key metadata is defined in `CalDAV.csproj`:
- **Description**: Package description for NuGet
- **Tags**: Search tags for discoverability
- **Repository URLs**: Links to source code
- **License**: MIT license expression
- **Release Notes**: What's new in this version

## Examples Project

A separate `CalDAV.Examples` project is provided to demonstrate usage without being included in the NuGet package.

To run examples:
```bash
cd CalDAV.Examples
dotnet run
# or
dotnet run advanced
```

## Project Structure

```
src/
??? CalDAV/                     # Main library project (packaged)
?   ??? Models/                 # Data models
?   ??? Utils/                  # Utility classes
?   ??? CalDAVClient.cs         # Main client class
?   ??? CalDAV.csproj           # Project file with NuGet metadata
??? CalDAV.Examples/            # Example console application
?   ??? Program.cs              # Basic examples
?   ??? AdvancedExample.cs      # Advanced usage examples
?   ??? CalDAV.Examples.csproj  # Examples project file
??? CalDAV.Tests/               # Unit and integration tests
??? NUGET_PACKAGING.md          # This file
```

## Troubleshooting

**Build fails with missing dependencies**:
- Ensure .NET 9 SDK is installed
- Run `dotnet restore` before building

**Package push fails**:
- Verify API key has correct permissions
- Check if package version already exists on NuGet.org
- Ensure package passes validation

**Missing XML documentation warnings**:
- These are informational only and don't prevent packaging
- Add XML comments to public members to resolve them