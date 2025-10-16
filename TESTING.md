# UContentMapper Testing Guide

This document provides comprehensive information about the testing strategy, setup, and execution for the UContentMapper project.

## 📋 Table of Contents

- [Testing Overview](#testing-overview)
- [Test Structure](#test-structure)
- [Running Tests](#running-tests)
- [Coverage Requirements](#coverage-requirements)
- [Test Categories](#test-categories)
- [Continuous Integration](#continuous-integration)
- [Development Guidelines](#development-guidelines)

## 🧪 Testing Overview

The UContentMapper project employs a comprehensive testing strategy to ensure code quality, reliability, and maintainability. Our testing approach includes:

- **Unit Tests**: Test individual components in isolation
- **Integration Tests**: Test component interactions and API workflows
- **Parametrized Tests**: Test multiple scenarios with different inputs
- **Coverage Analysis**: Ensure 100% code coverage
- **Automated CI/CD**: Continuous testing on every commit and PR

### Testing Stack

- **Framework**: NUnit 4.2.2
- **Mocking**: Moq 4.20.72
- **Test Data**: AutoFixture 4.18.1
- **Assertions**: FluentAssertions 6.12.2
- **Coverage**: Coverlet with multiple output formats
- **CI/CD**: GitHub Actions with quality gates

## 📁 Test Structure

```
UContentMapper.Tests/
├── Unit/                           # Unit tests for individual components
│   ├── Core/                       # Tests for Core library
│   │   ├── Models/                 # Model tests
│   │   ├── Configuration/          # Configuration tests
│   │   └── Exceptions/            # Exception tests
│   └── Umbraco15/                 # Tests for Umbraco15 implementation
│       ├── Mapping/               # Mapping logic tests
│       └── Configuration/         # Umbraco configuration tests
├── Integration/                   # Integration tests
│   ├── ServiceCollectionExtensionsIntegrationTests.cs
│   └── FullMappingIntegrationTests.cs
├── Fixtures/                      # Test data and models
│   ├── TestModels.cs             # Test model definitions
│   └── TestDataBuilder.cs       # Test data factory
├── Mocks/                        # Mock implementations
│   └── MockPublishedContent.cs  # Umbraco mocks
└── TestHelpers/                  # Test utilities
    └── TestBase.cs              # Base test class
```

## 🚀 Running Tests

### Prerequisites

- .NET 9.0 SDK
- Visual Studio 2022 or VS Code (optional)

### Command Line Options

#### Option 1: Using Test Scripts (Recommended)

**Linux/macOS:**
```bash
# Run all tests with default settings
./scripts/test.sh

# Run with custom configuration and open report
./scripts/test.sh --configuration Debug --threshold 95 --verbose --open-report

# Show help
./scripts/test.sh --help
```

**Windows:**
```powershell
# Run all tests with default settings
.\scripts\test.ps1

# Run with custom configuration and open report
.\scripts\test.ps1 -Configuration Debug -Threshold 95 -Verbose -OpenReport

# Show help
.\scripts\test.ps1 -Help
```

#### Option 2: Using dotnet CLI

```bash
# Basic test run
dotnet test

# Test with coverage
dotnet test --collect:"XPlat Code Coverage" --settings coverlet.runsettings

# Test with specific configuration
dotnet test --configuration Release --verbosity detailed

# Generate coverage report
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura,opencover
```

#### Option 3: Using IDE

- **Visual Studio**: Use Test Explorer to run tests
- **VS Code**: Use .NET Test Explorer extension
- **Rider**: Use built-in test runner

### Coverage Report Generation

After running tests with coverage, generate HTML reports:

```bash
# Install ReportGenerator (one-time setup)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Generate HTML report
reportgenerator \
  -reports:"TestResults/**/coverage.cobertura.xml" \
  -targetdir:"TestResults/coveragereport" \
  -reporttypes:"Html;Badges;TextSummary"
```

Open `TestResults/coveragereport/index.html` in your browser to view the coverage report.

## 📊 Coverage Requirements

### Coverage Targets

- **Line Coverage**: 100%
- **Branch Coverage**: 100%
- **Method Coverage**: 100%

### Coverage Configuration

Coverage is configured in `coverlet.runsettings` and includes:

- **Included Assemblies**: UContentMapper.Core, UContentMapper.Umbraco15, UContentMapper.Extensions
- **Excluded Assemblies**: Test assemblies, System libraries, third-party packages
- **Excluded Files**: Test helpers, mocks, fixtures

### Coverage Enforcement

- Local builds enforce 90% coverage threshold
- CI/CD pipeline enforces 100% coverage
- Pull requests must maintain or improve coverage
- Coverage reports are automatically generated and commented on PRs

## 🏷️ Test Categories

### Unit Tests

**Purpose**: Test individual components in isolation

**Examples**:
- `BaseContentModelTests`: Tests base model properties and behavior
- `UmbracoContentMapperTests`: Tests mapping logic without external dependencies
- `TypePairTests`: Tests type pair equality and hashing

**Characteristics**:
- Fast execution (< 100ms per test)
- No external dependencies
- Use mocks for dependencies
- Focus on single responsibility

### Integration Tests

**Purpose**: Test component interactions and end-to-end workflows

**Examples**:
- `ServiceCollectionExtensionsIntegrationTests`: Tests DI container registration
- `FullMappingIntegrationTests`: Tests complete mapping workflows

**Characteristics**:
- Slower execution (< 1s per test)
- Test real component interactions
- Use actual implementations where possible
- Focus on user scenarios

### Parametrized Tests

**Purpose**: Test multiple scenarios with different inputs

**Examples**:
- `ParametrizedMappingTests`: Tests mapping with various content types and values
- Type conversion tests with multiple data types
- Content type alias matching tests

**Characteristics**:
- Data-driven testing
- Comprehensive scenario coverage
- Reduce test duplication
- Easy to add new test cases

## 🔄 Continuous Integration

### GitHub Actions Workflow

The CI/CD pipeline (`/.github/workflows/ci.yml`) includes:

1. **Test Stage**:
   - Run all tests with coverage
   - Generate coverage reports
   - Upload to Codecov
   - Comment coverage on PRs

2. **Build Stage**:
   - Build solution in Release mode
   - Package NuGet packages
   - Upload build artifacts

3. **Security Stage**:
   - Run security vulnerability scans
   - CodeQL analysis
   - Dependency audits

4. **Quality Gate**:
   - Verify all stages passed
   - Enforce quality standards
   - Block deployment if quality gate fails

5. **Deploy Stage** (main branch only):
   - Deploy packages to NuGet
   - Create GitHub releases

### Quality Gates

Tests must pass these quality gates:

- ✅ All unit tests pass
- ✅ All integration tests pass
- ✅ Code coverage ≥ 90% (CI) / 100% (target)
- ✅ No security vulnerabilities
- ✅ No build warnings in Release mode
- ✅ All code analysis rules pass

## 📝 Development Guidelines

### Writing Tests

#### Test Naming Convention

```csharp
[Test]
public void MethodName_WithSpecificCondition_ShouldExpectedBehavior()
{
    // Arrange
    
    // Act
    
    // Assert
}
```

#### Test Structure

Follow the **Arrange-Act-Assert** pattern:

```csharp
[Test]
public void Map_WithValidContent_ShouldReturnMappedModel()
{
    // Arrange
    var content = TestDataBuilder.CreatePublishedContentWithProperties();
    var mapper = new UmbracoContentMapper<TestPageModel>(_config, _logger);

    // Act
    var result = mapper.Map(content);

    // Assert
    result.Should().NotBeNull();
    result.Should().BeOfType<TestPageModel>();
    result.Title.Should().Be("Expected Title");
}
```

#### Using Test Fixtures

```csharp
[Test, AutoData]
public void Method_WithAutoGeneratedData_ShouldWork(string input, int value)
{
    // AutoFixture automatically generates test data
}

[TestCaseSource(nameof(GetTestCases))]
public void Method_WithParametrizedData_ShouldWork(string input, bool expected)
{
    // Test with multiple predefined scenarios
}
```

### Test Data Management

#### Using TestDataBuilder

```csharp
// Create test content with built-in properties
var content = TestDataBuilder.CreatePublishedContentWithBuiltInProperties();

// Create test content with custom properties
var content = TestDataBuilder.CreatePublishedContentWithProperties();

// Create expected model for assertions
var expected = TestDataBuilder.CreateExpectedTestPageModel();
```

#### Using Mocks

```csharp
// Create basic mock
var contentMock = MockPublishedContent.Create();

// Create mock with specific properties
var contentMock = MockPublishedContent
    .WithContentTypeAlias("testPage")
    .WithProperties(new Dictionary<string, object>
    {
        { "title", "Test Title" },
        { "description", "Test Description" }
    });
```

### Performance Guidelines

- Unit tests should execute in < 100ms
- Integration tests should execute in < 1s
- Use `[SetUp]` and `[TearDown]` for expensive operations
- Prefer mocks over real dependencies for unit tests
- Cache expensive test data where appropriate

### Coverage Guidelines

- Aim for 100% line coverage
- Focus on meaningful tests, not just coverage metrics
- Test both happy path and error scenarios
- Include edge cases and boundary conditions
- Test all public API methods and properties

### Code Quality

- Use meaningful test names that describe the scenario
- Keep tests simple and focused
- One assertion per test (when possible)
- Use descriptive variable names
- Add comments for complex test scenarios
- Follow the project's coding standards

## 🔍 Troubleshooting

### Common Issues

#### Tests Not Running
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore

# Rebuild solution
dotnet build --no-incremental
```

#### Coverage Not Generated
```bash
# Check if coverlet is installed
dotnet list package | grep coverlet

# Verify settings file
cat coverlet.runsettings

# Run with verbose logging
dotnet test --verbosity detailed
```

#### IDE Integration Issues
```bash
# Clear VS Code cache
rm -rf .vscode/

# Reload window in VS Code
Ctrl+Shift+P -> "Developer: Reload Window"
```

### Getting Help

- Check GitHub Issues for known problems
- Review test output for specific error messages
- Verify .NET SDK version compatibility
- Ensure all dependencies are properly restored

## 📈 Metrics and Reporting

### Coverage Reports

Coverage reports include:
- Line-by-line coverage visualization
- Branch coverage analysis
- Method coverage statistics
- Historical coverage trends
- Uncovered code identification

### Test Reports

Test reports provide:
- Test execution results
- Performance metrics
- Test duration analysis
- Failure analysis and stack traces
- Test trend analysis

### Quality Metrics

Quality metrics tracked:
- Code coverage percentage
- Test execution time
- Build success rate
- Security vulnerability count
- Code quality scores

---

For more information, see the project documentation or contact the development team.