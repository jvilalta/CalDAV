@echo off
REM Batch script to build and package the CalDAV.Client NuGet package

echo ===============================================
echo CalDAV.Client NuGet Package Builder
echo ===============================================

REM Create output directory if it doesn't exist
if not exist "nupkg" mkdir nupkg

REM Build the project
echo Building project...
dotnet build CalDAV/CalDAV.csproj --configuration Release
if %ERRORLEVEL% neq 0 (
    echo Build failed!
    exit /b 1
)
echo Build completed successfully!

REM Create the package
echo Creating NuGet package...
dotnet pack CalDAV/CalDAV.csproj --configuration Release --output ./nupkg
if %ERRORLEVEL% neq 0 (
    echo Package creation failed!
    exit /b 1
)

echo Package created successfully!
echo.
echo Created packages:
dir nupkg\CalDAV.Client.* /b

echo.
echo ===============================================
echo Package creation completed!
echo ===============================================
echo.
echo To push the package to NuGet.org:
echo   dotnet nuget push nupkg\CalDAV.Client.1.0.0.nupkg --api-key YOUR_API_KEY --source https://api.nuget.org/v3/index.json
echo.
echo To test locally, add the output folder as a package source:
echo   dotnet nuget add source %cd%\nupkg --name local-caldav
echo.
pause