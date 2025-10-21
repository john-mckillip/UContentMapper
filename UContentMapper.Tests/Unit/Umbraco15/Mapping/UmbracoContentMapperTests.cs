using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Tests.Fixtures;
using UContentMapper.Tests.Mocks;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Tests.Unit.Umbraco15.Mapping;

[TestFixture]
public class UmbracoContentMapperTests : TestBase
{
    private Mock<IMappingConfiguration> _mappingConfigurationMock;
    private FakeLogger<UmbracoContentMapper<TestPageModel>> _logger;
    private UmbracoContentMapper<TestPageModel> _mapper;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _mappingConfigurationMock = CreateMock<IMappingConfiguration>();
        _logger = new FakeLogger<UmbracoContentMapper<TestPageModel>>();
        _mapper = new UmbracoContentMapper<TestPageModel>(_logger);
    }

    [Test]
    public void Constructor_ShouldInitializeWithDependencies()
    {
        // Arrange & Act
        var mapper = new UmbracoContentMapper<TestPageModel>(_logger);

        // Assert
        mapper.Should().NotBeNull();
    }

    [Test]
    public void CanMap_WithIPublishedContent_ShouldReturnTrue()
    {
        // Arrange
        var content = MockPublishedContent.WithContentTypeAlias("testPage").Object;

        // Act
        var result = _mapper.CanMap(content);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void CanMap_WithNonIPublishedContent_ShouldReturnFalse()
    {
        // Arrange
        var notContent = new object();

        // Act
        var result = _mapper.CanMap(notContent);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanMap_WithMatchingContentTypeAlias_ShouldReturnTrue()
    {
        // Arrange
        var content = MockPublishedContent.WithContentTypeAlias("testPage").Object;
        var mapper = new UmbracoContentMapper<TestPageModel>(_logger);

        // Act
        var result = mapper.CanMap(content);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void CanMap_WithNonMatchingContentTypeAlias_ShouldReturnFalse()
    {
        // Arrange
        var content = MockPublishedContent.WithContentTypeAlias("differentPage").Object;
        var mapper = new UmbracoContentMapper<TestPageModel>(_logger);

        // Act
        var result = mapper.CanMap(content);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void CanMap_WithWildcardContentType_ShouldReturnTrue()
    {
        // Arrange
        var content = MockPublishedContent.WithContentTypeAlias("anyContentType").Object;
        var mapper = new UmbracoContentMapper<WildcardContentTypeModel>(new FakeLogger<UmbracoContentMapper<WildcardContentTypeModel>>());

        // Act
        var result = mapper.CanMap(content);

        // Assert
        result.Should().BeTrue();
    }

    [Test]
    public void CanMap_WithWrongSourceType_ShouldReturnFalse()
    {
        // Arrange
        var content = MockPublishedContent.WithContentTypeAlias("testPage").Object;
        var mapper = new UmbracoContentMapper<WrongSourceTypeModel>(new FakeLogger<UmbracoContentMapper<WrongSourceTypeModel>>());

        // Act
        var result = mapper.CanMap(content);

        // Assert
        result.Should().BeFalse();
    }

    [Test]
    public void Map_WithValidContent_ShouldReturnMappedModel()
    {
        // Arrange
        var content = TestDataBuilder.CreatePublishedContentWithBuiltInProperties();

        // Act
        var result = _mapper.Map(content);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeOfType<TestPageModel>();
        result.Id.Should().Be(content.Id);
        result.Key.Should().Be(content.Key);
        result.Name.Should().Be(content.Name);
        result.ContentTypeAlias.Should().Be(content.ContentType.Alias);
    }

    [Test]
    public void Map_WithInvalidSource_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var invalidSource = new object();

        // Act
        var act = () => _mapper.Map(invalidSource);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot map object of type Object to TestPageModel");
    }

    [TestCaseSource(typeof(TestDataBuilder), nameof(TestDataBuilder.GetBuiltInPropertyTestCases))]
    public void Map_ShouldMapBuiltInProperties(IPublishedContent content, string propertyName, object expectedValue)
    {
        // Act
        var result = _mapper.Map(content);

        // Assert
        var property = typeof(TestPageModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void Map_WithContentProperties_ShouldMapCustomProperties()
    {
        // Arrange
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
        var mock = MockPublishedContent.Create();

        // Set up properties
        var properties = new Dictionary<string, object>
        {
            { "title", "Test Page Title" },
            { "description", "Test Description" },
            { "categoryid", 123 },
            { "ispublished", true },
            { "publishdate", DateTime.UtcNow.AddDays(-5) }
        };

        foreach (var prop in properties)
        {
            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(x => x.Alias).Returns(prop.Key);
            propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value != null);
            propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value);

            mock.Setup(x => x.GetProperty(prop.Key)).Returns(propertyMock.Object);
            mock.Setup(x => x.ContentType.GetPropertyType(prop.Key)).Returns(publishedPropertyTypeMock.Object);
        }

        // Act
        var result = _mapper.Map(mock.Object);

        // Assert
        result.Title.Should().Be("Test Page Title");
        result.Description.Should().Be("Test Description");
        result.CategoryId.Should().Be(123);
        result.IsPublished.Should().BeTrue();
    }

    [Test]
    public void Map_WithNullPropertyValues_ShouldSkipProperties()
    {
        // Arrange
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
        var mock = MockPublishedContent.Create();

        // Set up properties
        var titlePropertyMock = new Mock<IPublishedProperty>();
        var titleAlias = "title";
        titlePropertyMock.Setup(x => x.Alias).Returns(titleAlias);
        titlePropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        titlePropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(string.Empty);

        mock.Setup(x => x.GetProperty(titleAlias)).Returns(titlePropertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(titleAlias)).Returns(publishedPropertyTypeMock.Object);

        var descriptionPropertyMock = new Mock<IPublishedProperty>();
        var descriptionAlias = "description";
        descriptionPropertyMock.Setup(x => x.Alias).Returns(descriptionAlias);
        descriptionPropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        descriptionPropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("Valid Description");

        mock.Setup(x => x.GetProperty(descriptionAlias)).Returns(descriptionPropertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(descriptionAlias)).Returns(publishedPropertyTypeMock.Object);

        // Act
        var result = _mapper.Map(mock.Object);

        // Assert
        result.Title.Should().Be(string.Empty); // Default value
        result.Description.Should().Be("Valid Description");
    }

    [Test]
    public void Map_WithMissingProperties_ShouldUseDefaultValues()
    {
        // Arrange
        var content = MockPublishedContent.Create();

        // Act
        var result = _mapper.Map(content.Object);

        // Assert
        result.Title.Should().Be(string.Empty);
        result.Description.Should().Be(string.Empty);
        result.CategoryId.Should().Be(0);
        result.IsPublished.Should().BeFalse();
    }

    [TestCaseSource(typeof(TestDataBuilder), nameof(TestDataBuilder.GetTypeConversionTestCases))]
    public void Map_ShouldConvertTypes(object sourceValue, Type targetType, object expectedValue)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TypeConversionTestModel>(new FakeLogger<UmbracoContentMapper<TypeConversionTestModel>>());

        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
        var propertyName = GetPropertyNameForType(targetType);
        var mock = MockPublishedContent.Create();

        // Set up property
        var propertyMock = new Mock<IPublishedProperty>();
        propertyMock.Setup(x => x.Alias).Returns(propertyName.ToLowerInvariant());
        propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(sourceValue);

        mock.Setup(x => x.GetProperty(propertyName.ToLowerInvariant())).Returns(propertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(propertyName.ToLowerInvariant())).Returns(publishedPropertyTypeMock.Object);

        // Act
        var result = mapper.Map(mock.Object);

        // Assert
        var property = typeof(TypeConversionTestModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedValue);
    }

    [Test]
    public void Map_WithReadOnlyProperties_ShouldSkipReadOnlyProperties()
    {
        // Arrange
        var mapper = new UmbracoContentMapper<ReadOnlyPropertiesTestModel>(new FakeLogger<UmbracoContentMapper<ReadOnlyPropertiesTestModel>>());
        var content = MockPublishedContent.Create().Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Should().NotBeNull();
        result.ReadOnlyProperty.Should().Be("ReadOnly"); // Should remain unchanged
    }

    [Test]
    public void Map_WithIndexerProperties_ShouldSkipIndexerProperties()
    {
        // Arrange
        var mapper = new UmbracoContentMapper<IndexerPropertiesTestModel>(new FakeLogger<UmbracoContentMapper<IndexerPropertiesTestModel>>());
        var content = MockPublishedContent.Create().Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Should().NotBeNull();
        // Test should not throw exception when encountering indexer properties
    }

    [Test]
    public void Map_WithPropertyMappingException_ShouldLogWarningAndContinue()
    {
        // Arrange
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
        var mock = MockPublishedContent.Create();

        // Set up properties - one valid, one that will cause conversion error
        var titlePropertyMock = new Mock<IPublishedProperty>();
        var titleAlias = "title";
        titlePropertyMock.Setup(x => x.Alias).Returns(titleAlias);
        titlePropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        titlePropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("Valid Title");

        mock.Setup(x => x.GetProperty(titleAlias)).Returns(titlePropertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(titleAlias)).Returns(publishedPropertyTypeMock.Object);

        var categoryIdPropertyMock = new Mock<IPublishedProperty>();
        var categoryIdnAlias = "categoryid";
        categoryIdPropertyMock.Setup(x => x.Alias).Returns(categoryIdnAlias);
        categoryIdPropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        categoryIdPropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("invalid_number");

        mock.Setup(x => x.GetProperty(categoryIdnAlias)).Returns(categoryIdPropertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(categoryIdnAlias)).Returns(publishedPropertyTypeMock.Object);

        // Act
        var result = _mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Valid Title"); // Should still map valid properties
        result.CategoryId.Should().Be(0); // Should use default value for failed conversion
        
        // Should log warning for failed property mapping
        _logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Level == LogLevel.Warning && 
            log.Message.Contains("Error mapping property"));
    }

    [Test]
    public void Map_WithComplexMappingError_ShouldLogErrorAndThrow()
    {
        // Arrange
        var content = MockPublishedContent.Create().Object;
        var abstractLogger = new FakeLogger<UmbracoContentMapper<AbstractTestModel>>();
        
        // Create a mapper that will fail during activation (simulate error)
        var mapper = new UmbracoContentMapper<AbstractTestModel>(abstractLogger);

        // Act
        var act = () => mapper.Map(content);

        // Assert
        act.Should().Throw<Exception>();
        abstractLogger.Collector.GetSnapshot().Should().Contain(log => 
            log.Level == LogLevel.Error && 
            log.Message.Contains("Error mapping"));
    }

    private static string GetPropertyNameForType(Type type)
    {
        return type.Name switch
        {
            nameof(String) => nameof(TypeConversionTestModel.StringValue),
            nameof(Int32) => nameof(TypeConversionTestModel.IntValue),
            nameof(Boolean) => nameof(TypeConversionTestModel.BoolValue),
            nameof(DateTime) => nameof(TypeConversionTestModel.DateTimeValue),
            nameof(Guid) => nameof(TypeConversionTestModel.GuidValue),
            _ => nameof(TypeConversionTestModel.StringValue)
        };
    }

    /// <summary>
    /// Abstract test model to trigger activation errors
    /// </summary>
    public abstract class AbstractTestModel
    {
        public int Id { get; set; }
    }
}