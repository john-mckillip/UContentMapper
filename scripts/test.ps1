# UContentMapper Test Execution Script (PowerShell)
# This script runs all tests with coverage reporting

param(
    [string]$Configuration = "Release",
    [int]$Threshold = 90,
    [switch]$Verbose,
    [switch]$OpenReport,
    [switch]$Help
)

if ($Help) {
    Write-Host "UContentMapper Test Execution Script"
    Write-Host "Usage: .\test.ps1 [OPTIONS]"
    Write-Host ""
    Write-Host "Options:"
    Write-Host "  -Configuration    Build configuration (Debug|Release) [default: Release]"
    Write-Host "  -Threshold        Coverage threshold percentage [default: 90]"
    Write-Host "  -Verbose          Enable verbose output"
    Write-Host "  -OpenReport       Open coverage report in browser after completion"
    Write-Host "  -Help             Show this help message"
    exit 0
}

Write-Host "🧪 UContentMapper Test Suite" -ForegroundColor Blue
Write-Host "============================" -ForegroundColor Blue

Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "Coverage Threshold: $Threshold%" -ForegroundColor Cyan
Write-Host ""

# Clean previous test results
Write-Host "🧹 Cleaning previous test results..." -ForegroundColor Yellow
if (Test-Path "TestResults") {
    Remove-Item -Recurse -Force "TestResults"
}
New-Item -ItemType Directory -Force -Path "TestResults" | Out-Null

# Restore dependencies
Write-Host "📦 Restoring dependencies..." -ForegroundColor Yellow
dotnet restore

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to restore dependencies" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Build solution
Write-Host "🔨 Building solution..." -ForegroundColor Yellow
dotnet build --no-restore --configuration $Configuration

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to build solution" -ForegroundColor Red
    exit $LASTEXITCODE
}

# Run tests with coverage
Write-Host "🧪 Running tests with coverage..." -ForegroundColor Yellow

$VerbosityLevel = if ($Verbose) { "detailed" } else { "normal" }

$TestArgs = @(
    "test"
    "--no-build"
    "--configuration", $Configuration
    "--settings", "coverlet.runsettings"
    "--collect:XPlat Code Coverage"
    "--results-directory", "./TestResults"
    "--logger", "trx;LogFileName=test-results.trx"
    "--logger", "console;verbosity=$VerbosityLevel"
    "--verbosity", $VerbosityLevel
    "/p:CollectCoverage=true"
    "/p:CoverletOutputFormat=cobertura,lcov,opencover"
    "/p:CoverletOutput=./TestResults/coverage"
    "/p:Threshold=$Threshold"
    "/p:ThresholdType=line,branch,method"
    "/p:ThresholdStat=total"
)

& dotnet @TestArgs

$TestExitCode = $LASTEXITCODE

# Generate coverage report if tests passed
if ($TestExitCode -eq 0) {
    Write-Host "📊 Generating coverage report..." -ForegroundColor Yellow
    
    # Check if reportgenerator is installed globally
    $ReportGeneratorExists = $null -ne (Get-Command "reportgenerator" -ErrorAction SilentlyContinue)
    
    if (-not $ReportGeneratorExists) {
        Write-Host "Installing ReportGenerator tool..." -ForegroundColor Yellow
        dotnet tool install -g dotnet-reportgenerator-globaltool
    }
    
    $ReportArgs = @(
        "-reports:TestResults/**/coverage.cobertura.xml"
        "-targetdir:TestResults/coveragereport"
        "-reporttypes:Html;Badges;TextSummary;Cobertura"
        "-verbosity:Info"
        "-title:UContentMapper Code Coverage"
    )
    
    & reportgenerator @ReportArgs
    
    # Display coverage summary
    Write-Host ""
    Write-Host "📈 Coverage Summary:" -ForegroundColor Green
    Write-Host "===================" -ForegroundColor Green
    
    if (Test-Path "TestResults/coveragereport/Summary.txt") {
        Get-Content "TestResults/coveragereport/Summary.txt"
    }
    
    # Open coverage report if requested
    if ($OpenReport) {
        $ReportPath = "TestResults/coveragereport/index.html"
        if (Test-Path $ReportPath) {
            Start-Process $ReportPath
        } else {
            Write-Host "Coverage report not found at: $ReportPath" -ForegroundColor Yellow
        }
    }
    
    Write-Host ""
    Write-Host "✅ All tests passed with sufficient coverage!" -ForegroundColor Green
    Write-Host "📄 Coverage Report: TestResults/coveragereport/index.html" -ForegroundColor Cyan
    Write-Host "📄 Test Results: TestResults/test-results.trx" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "❌ Tests failed or coverage threshold not met!" -ForegroundColor Red
}

exit $TestExitCode