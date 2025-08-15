# PowerShell script to build and package the CalDAV.Client NuGet package

param(
    [string]$Configuration = "Release",
    [string]$OutputPath = "./nupkg",
    [switch]$SkipBuild,
    [switch]$Push,
    [string]$ApiKey = "",
    [string]$Source = "https://api.nuget.org/v3/index.json"
)

# Colors for output
$ErrorColor = "Red"
$SuccessColor = "Green"
$InfoColor = "Cyan"
$WarningColor = "Yellow"

function Write-Info {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $InfoColor
}

function Write-Success {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $SuccessColor
}

function Write-Error {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $ErrorColor
}

function Write-Warning {
    param([string]$Message)
    Write-Host $Message -ForegroundColor $WarningColor
}

Write-Info "==============================================="
Write-Info "CalDAV.Client NuGet Package Builder"
Write-Info "==============================================="

# Ensure output directory exists
if (!(Test-Path $OutputPath)) {
    Write-Info "Creating output directory: $OutputPath"
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Build project if not skipping
if (!$SkipBuild) {
    Write-Info "Building project in $Configuration configuration..."
    $buildResult = dotnet build CalDAV/CalDAV.csproj --configuration $Configuration
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed!"
        exit 1
    }
    Write-Success "Build completed successfully!"
}

# Create package
Write-Info "Creating NuGet package..."
$packResult = dotnet pack CalDAV/CalDAV.csproj --configuration $Configuration --output $OutputPath --no-build:$SkipBuild
if ($LASTEXITCODE -ne 0) {
    Write-Error "Package creation failed!"
    exit 1
}

Write-Success "Package created successfully!"

# List created packages
Write-Info "Created packages:"
Get-ChildItem -Path $OutputPath -Filter "CalDAV.Client.*" | ForEach-Object {
    Write-Host "  - $($_.Name)" -ForegroundColor White
}

# Push package if requested
if ($Push) {
    if ([string]::IsNullOrEmpty($ApiKey)) {
        Write-Error "API key is required for pushing packages. Use -ApiKey parameter."
        exit 1
    }
    
    Write-Info "Pushing package to $Source..."
    
    # Find the main package file
    $packageFile = Get-ChildItem -Path $OutputPath -Filter "CalDAV.Client.*.nupkg" | Where-Object { $_.Name -notlike "*.symbols.*" } | Select-Object -First 1
    if (!$packageFile) {
        Write-Error "Could not find main package file to push!"
        exit 1
    }
    
    Write-Info "Pushing $($packageFile.Name)..."
    $pushResult = dotnet nuget push $packageFile.FullName --api-key $ApiKey --source $Source
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Package push failed!"
        exit 1
    }
    
    # Push symbol package if it exists
    $symbolPackage = Get-ChildItem -Path $OutputPath -Filter "CalDAV.Client.*.snupkg" | Select-Object -First 1
    if ($symbolPackage) {
        Write-Info "Pushing symbol package $($symbolPackage.Name)..."
        $symbolPushResult = dotnet nuget push $symbolPackage.FullName --api-key $ApiKey --source $Source
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Symbol package push failed, but main package was pushed successfully."
        } else {
            Write-Success "Symbol package pushed successfully!"
        }
    }
    
    Write-Success "Package pushed successfully!"
}

Write-Info "==============================================="
Write-Success "Package creation completed!"
Write-Info "==============================================="

if (!$Push) {
    Write-Info "To push the package to NuGet.org, run:"
    Write-Host "  .\package-caldav.ps1 -Push -ApiKey YOUR_API_KEY" -ForegroundColor White
    Write-Info ""
    Write-Info "To test locally, add the output folder as a package source:"
    Write-Host "  dotnet nuget add source $((Resolve-Path $OutputPath).Path) --name local-caldav" -ForegroundColor White
}