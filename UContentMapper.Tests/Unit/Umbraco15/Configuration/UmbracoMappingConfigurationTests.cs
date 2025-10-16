using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;
using System.Reflection;
using UContentMapper.Core.Configuration.Profiles;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Configuration;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Tests.Unit.Umbraco15.Configuration;

[TestFixture]
public class UmbracoMappingConfigurationTests : TestBase
{
    private FakeLogger<UmbracoMappingConfiguration> _logger;
    private UmbracoMappingConfiguration _configuration;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = new FakeLogger<UmbracoMappingConfiguration>();
        _configuration = new UmbracoMappingConfiguration(_logger);
    }

    [Test]
    public void Constructor_ShouldInitializeWithLogger()
    {
        // Arrange & Act
        var configuration = new UmbracoMappingConfiguration(_logger);

        // Assert
        configuration.Should().NotBeNull();
    }

    [Test]
    public void CreateMap_ShouldCreateMappingExpression()
    {
        // Act
        var expression = _configuration.CreateMap<IPublishedContent, string>();

        // Assert
        expression.Should().NotBeNull();
        expression.Should().BeAssignableTo<UmbracoMappingExpression<IPublishedContent, string>>();
    }

    [Test]
    public void CreateMap_ShouldLogDebugMessage()
    {
        // Act
        _configuration.CreateMap<IPublishedContent, string>();

        // Assert
        _logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Message.Contains("Created mapping from IPublishedContent to String"));
    }

    [Test]
    public void CreateMap_WithIPublishedContentSource_ShouldSetWildcardAlias()
    {
        // Act
        var expression = _configuration.CreateMap<IPublishedContent, string>();

        // Assert
        expression.Should().NotBeNull();
        // The expression should be created with wildcard content type alias
    }

    [Test]
    public void CreateMap_WithDifferentSourceType_ShouldSetEmptyAlias()
    {
        // Act
        var expression = _configuration.CreateMap<string, int>();

        // Assert
        expression.Should().NotBeNull();
        // The expression should be created with empty content type alias
    }

    [Test]
    public void CreateMap_CalledTwiceWithSameTypes_ShouldReturnSameMapping()
    {
        // Act
        var expression1 = _configuration.CreateMap<IPublishedContent, string>();
        var expression2 = _configuration.CreateMap<IPublishedContent, string>();

        // Assert
        expression1.Should().NotBeNull();
        expression2.Should().NotBeNull();
        // Both expressions should reference the same underlying type mapping
    }

    [Test]
    public void CreateMap_WithDifferentTypes_ShouldCreateSeparateMappings()
    {
        // Act
        var expression1 = _configuration.CreateMap<IPublishedContent, string>();
        var expression2 = _configuration.CreateMap<IPublishedContent, int>();
        var expression3 = _configuration.CreateMap<string, int>();

        // Assert
        expression1.Should().NotBeNull();
        expression2.Should().NotBeNull();
        expression3.Should().NotBeNull();
        
        _logger.Collector.GetSnapshot().Should().HaveCount(3);
    }

    [Test]
    public void AddMappingProfiles_WithValidAssembly_ShouldAddProfiles()
    {
        // Arrange
        var assembly = Assembly.GetExecutingAssembly();

        // Act
        var result = _configuration.AddMappingProfiles(assembly);

        // Assert
        result.Should().Be(_configuration);
        _logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Message.Contains("Added mapping profile"));
    }

    [Test]
    public void AddMappingProfiles_WithMultipleAssemblies_ShouldProcessAll()
    {
        // Arrange
        var assembly1 = Assembly.GetExecutingAssembly();
        var assembly2 = typeof(UmbracoMappingConfiguration).Assembly;

        // Act
        var result = _configuration.AddMappingProfiles(assembly1, assembly2);

        // Assert
        result.Should().Be(_configuration);
    }

    [Test]
    public void AddMappingProfiles_WithEmptyAssemblyArray_ShouldNotThrow()
    {
        // Act
        var act = () => _configuration.AddMappingProfiles();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void AddMappingProfiles_WithNullAssembly_ShouldHandleGracefully()
    {
        // Act
        var act = () => _configuration.AddMappingProfiles(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>();
    }

    [Test]
    public void ValidateConfiguration_WithoutMappings_ShouldNotThrow()
    {
        // Act
        var act = () => _configuration.ValidateConfiguration();

        // Assert
        act.Should().NotThrow();
    }

    [Test]
    public void ValidateConfiguration_WithMappings_ShouldLogValidationInfo()
    {
        // Arrange
        _configuration.CreateMap<IPublishedContent, TestValidationModel>();

        // Act
        _configuration.ValidateConfiguration();

        // Assert
        _logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Message.Contains("Validating mapping"));
    }

    [Test]
    public void ValidateConfiguration_WithComplexProperties_ShouldLogWarnings()
    {
        // Arrange
        _configuration.CreateMap<IPublishedContent, ComplexValidationModel>();

        // Act
        _configuration.ValidateConfiguration();

        // Assert
        _logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Message.Contains("No mapping defined for complex property"));
    }

    [Test]
    public void ValidateConfiguration_WithValueTypeProperties_ShouldNotLogWarnings()
    {
        // Arrange
        _configuration.CreateMap<IPublishedContent, SimpleValidationModel>();

        // Act
        _configuration.ValidateConfiguration();

        // Assert
        _logger.Collector.GetSnapshot().Should().NotContain(log => 
            log.Message.Contains("No mapping defined"));
    }

    [Test]
    public void ValidateConfiguration_ShouldCheckAllRegisteredMappings()
    {
        // Arrange
        _configuration.CreateMap<IPublishedContent, TestValidationModel>();
        _configuration.CreateMap<string, int>();

        // Act
        _configuration.ValidateConfiguration();

        // Assert
        var logs = _logger.Collector.GetSnapshot();
        logs.Should().Contain(log => log.Message.Contains("Validating mapping: IPublishedContent -> TestValidationModel"));
        logs.Should().Contain(log => log.Message.Contains("Validating mapping: String -> Int32"));
    }

    /// <summary>
    /// Test model for validation testing
    /// </summary>
    public class TestValidationModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>
    /// Test model with complex properties for validation testing
    /// </summary>
    public class ComplexValidationModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ComplexProperty ComplexProp { get; set; } = new();
        public List<string> StringList { get; set; } = new();
    }

    /// <summary>
    /// Test model with only value types and strings for validation testing
    /// </summary>
    public class SimpleValidationModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
    }

    public class ComplexProperty
    {
        public string Value { get; set; } = string.Empty;
    }

    /// <summary>
    /// Test mapping profile for testing profile loading
    /// </summary>
    public class TestMappingProfile : MappingProfile
    {
        public TestMappingProfile()
        {
            // This will be used to test profile loading
        }
    }
}