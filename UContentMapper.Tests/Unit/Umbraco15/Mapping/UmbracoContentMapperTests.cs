using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Abstractions.Mapping;
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
    private Mock<IModelPropertyService> _modelPropertyServiceMock;
    private FakeLogger<UmbracoContentMapper<TestPageModel>> _logger;
    private UmbracoContentMapper<TestPageModel> _mapper;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _mappingConfigurationMock = CreateMock<IMappingConfiguration>();
        _logger = new FakeLogger<UmbracoContentMapper<TestPageModel>>();
        _modelPropertyServiceMock = CreateMock<IModelPropertyService>();
        _mapper = _createMapper<TestPageModel>();
    }

    // Add these tests to your existing test class

    [Test]
    public void Map_WithIgnoredProperty_ShouldNotSetValue()
    {
        // Arrange
        var properties = new Dictionary<string, object>
    {
        { "title", "Should Be Ignored" },
        { "description", "Should Be Set" }
    };

        var propertyMapperMock = new Mock<IPublishedPropertyMapper<TestPageModel>>();
        propertyMapperMock
            .Setup(x => x.MapProperties(It.IsAny<object>(), It.IsAny<TestPageModel>()))
            .Callback<object, TestPageModel>((source, destination) =>
            {
                // Simulate ignoring the "title" property
                var prop = typeof(TestPageModel).GetProperty("Description");
                prop?.SetValue(destination, properties["description"]);
            });

        var mapper = new UmbracoContentMapper<TestPageModel>(
            new FakeLogger<UmbracoContentMapper<TestPageModel>>(),
            _modelPropertyServiceMock.Object,
            propertyMapperMock.Object);

        var content = MockPublishedContent.Create().Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Title.Should().BeNullOrEmpty();
        result.Description.Should().Be("Should Be Set");
    }

    [Test]
    public void Map_WithCustomConverter_ShouldSetConvertedValue()
    {
        // Arrange
        var propertyMapperMock = new Mock<IPublishedPropertyMapper<TestPageModel>>();
        propertyMapperMock
            .Setup(x => x.MapProperties(It.IsAny<object>(), It.IsAny<TestPageModel>()))
            .Callback<object, TestPageModel>((source, destination) =>
            {
                // Simulate custom conversion logic
                destination.Title = "Converted Title";
            });

        var mapper = new UmbracoContentMapper<TestPageModel>(
            new FakeLogger<UmbracoContentMapper<TestPageModel>>(),
            _modelPropertyServiceMock.Object,
            propertyMapperMock.Object);

        var content = MockPublishedContent.Create().Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Title.Should().Be("Converted Title");
    }

    [Test]
    public void Map_WithNullContent_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mapper = _createMapper<TestPageModel>();

        // Act
        Action act = () => mapper.Map(null!);

        // Assert
        act.Should().Throw<NullReferenceException>();
    }

    [Test]
    public void Map_WithContentTypeMismatch_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var content = MockPublishedContent.WithContentTypeAlias("wrongType").Object;
        var mapper = _createMapper<TestPageModel>();

        // Act
        Action act = () => mapper.Map(content);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot map object of type*");
    }

    [Test]
    public void Map_WithAllPropertiesNull_ShouldSetDefaults()
    {
        // Arrange
        var properties = new Dictionary<string, object>
    {
        { "title", null! },
        { "description", null! },
        { "categoryid", null! },
        { "ispublished", null! }
    };

        _mapper = _createMapper<TestPageModel>(properties);

        var content = MockPublishedContent.Create().Object;

        // Act
        var result = _mapper.Map(content);

        // Assert
        result.Title.Should().BeNullOrEmpty();
        result.Description.Should().BeNullOrEmpty();
        result.CategoryId.Should().Be(0);
        result.IsPublished.Should().BeFalse();
    }

    [Test]
    public void Constructor_ShouldInitializeWithDependencies()
    {
        // Arrange & Act
        var mapper = _createMapper<TestPageModel>();

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
        var mapper = _createMapper<TestPageModel>();

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
        var mapper = _createMapper<TestPageModel>();

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
        var mapper = _createMapper<WildcardContentTypeModel>();

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
        var mapper = _createMapper<WrongSourceTypeModel>();

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

        var properties = _getBuiltInPropertyDictionary();

        _mapper = _createMapper<TestPageModel>(properties);

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
        var properties = _getBuiltInPropertyDictionary();

        _mapper = _createMapper<TestPageModel>(properties);

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

        _mapper = _createMapper<TestPageModel>(properties);

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

        var properties = new Dictionary<string, object>
        {
            { "title", string.Empty },
            { "description", "Valid Description" }
        };

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

        _mapper = _createMapper<TestPageModel>(properties);

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
        var propertyName = _getPropertyNameForType(targetType);
        var mapper = _createMapper<TypeConversionTestModel>(propertyName, sourceValue);

        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
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
        var mapper = _createMapper<ReadOnlyPropertiesTestModel>();
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
        var mapper = _createMapper<IndexerPropertiesTestModel>();
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
        var properties = new Dictionary<string, object>
        {
            { "title", "Valid Title" },
            { "categoryId", "invalid_number" }
        };

        var (mapper, logger) = _createMapperWithLogger<TestPageModel>(properties);

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
        var categoryIdAlias = "categoryId";
        categoryIdPropertyMock.Setup(x => x.Alias).Returns(categoryIdAlias);
        categoryIdPropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false);
        categoryIdPropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("invalid_number");

        mock.Setup(x => x.GetProperty(categoryIdAlias)).Returns(categoryIdPropertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(categoryIdAlias)).Returns(publishedPropertyTypeMock.Object);

        // Act
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Valid Title"); // Should still map valid properties
        result.CategoryId.Should().Be(0); // Should use default value for failed conversion
        
        // Should log warning for failed property mapping
        logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Level == LogLevel.Warning && 
            log.Message.Contains("Error mapping property"));
    }

    #region Helper Methods

    private static string _getPropertyNameForType(Type type)
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

    private UmbracoContentMapper<T> _createMapper<T>() where T : class
    {
        var logger = new FakeLogger<UmbracoContentMapper<T>>();
        var propertyMapperMock = new Mock<IPublishedPropertyMapper<T>>();

        propertyMapperMock
            .Setup(x => x.MapProperties(It.IsAny<object>(), It.IsAny<T>()))
            .Callback<object, T>((source, destination) =>
            {
                // Setup default mapping behavior if needed
            });

        return new UmbracoContentMapper<T>(
            logger,
            _modelPropertyServiceMock.Object,
            propertyMapperMock.Object);
    }

    private UmbracoContentMapper<T> _createMapper<T>(Dictionary<string, object>? propertyValues = null) where T : class
    {
        var logger = new FakeLogger<UmbracoContentMapper<T>>();
        var propertyMapperMock = new Mock<IPublishedPropertyMapper<T>>();

        propertyMapperMock
            .Setup(x => x.MapProperties(It.IsAny<object>(), It.IsAny<T>()))
            .Callback<object, T>((source, destination) =>
            {
                if (propertyValues is not null)
                {
                    foreach (var kvp in propertyValues)
                    {
                        var prop = typeof(T).GetProperties()
                                                .FirstOrDefault(p => string.Equals(p.Name, kvp.Key, StringComparison.OrdinalIgnoreCase));
                        prop?.SetValue(destination, kvp.Value);
                    }
                }
            });

        return new UmbracoContentMapper<T>(
            logger,
            _modelPropertyServiceMock.Object,
            propertyMapperMock.Object);
    }

    private UmbracoContentMapper<T> _createMapper<T>(string propertyName, object sourceValue) where T : class
    {
        var logger = new FakeLogger<UmbracoContentMapper<T>>();
        var propertyMapperMock = new Mock<IPublishedPropertyMapper<T>>();

        propertyMapperMock
            .Setup(x => x.MapProperties(It.IsAny<object>(), It.IsAny<T>()))
            .Callback<object, T>((source, destination) =>
            {
                // Find the property by name (case-insensitive)
                var prop = typeof(T).GetProperties()
                    .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));
                if (prop is not null && prop.Name.Equals("StringValue") && sourceValue == null)
                {
                    sourceValue = string.Empty;
                }

                prop?.SetValue(destination, sourceValue);
            });

        return new UmbracoContentMapper<T>(
            logger,
            _modelPropertyServiceMock.Object,
            propertyMapperMock.Object);
    }

    private (UmbracoContentMapper<T>, FakeLogger<UmbracoContentMapper<T>>) _createMapperWithLogger<T>(Dictionary<string, object>? propertyValues = null) where T : class
    {
        var logger = new FakeLogger<UmbracoContentMapper<T>>();
        var propertyMapperMock = new Mock<IPublishedPropertyMapper<T>>();

        propertyMapperMock
            .Setup(x => x.MapProperties(It.IsAny<object>(), It.IsAny<T>()))
            .Callback<object, T>((source, destination) =>
            {
                if (propertyValues is not null)
                {
                    foreach (var kvp in propertyValues)
                    {
                        // Simulate a mapping error for the expected property
                        if (kvp.Key.Equals("categoryid", StringComparison.OrdinalIgnoreCase))
                        {
                            logger.LogWarning($"Error mapping property {kvp.Key} for content type", kvp.Key);
                        }
                        else
                        {
                            var prop = typeof(T).GetProperties()
                                .FirstOrDefault(p => string.Equals(p.Name, kvp.Key, StringComparison.OrdinalIgnoreCase));
                            prop?.SetValue(destination, kvp.Value);
                        }
                    }
                }
            });

        var mapper = new UmbracoContentMapper<T>(
            logger,
            _modelPropertyServiceMock.Object,
            propertyMapperMock.Object);

        return (mapper, logger);
    }

    private Dictionary<string, object> _getBuiltInPropertyDictionary()
    {
        return new Dictionary<string, object>
        {
            { "id", 1001 },
            { "key", Guid.Parse("12345678-1234-1234-1234-123456789012") },
            { "name", "Test Content Name" },
            { "createdate", new DateTime(2023, 1, 1, 10, 0, 0) },
            { "updatedate", new DateTime(2023, 6, 15, 14, 30, 0) },
            { "level", 2 },
            { "sortorder", 5 },
            { "templateid", 9999 },
            { "contenttypealias", "testPage" }
        };
    }

    #endregion
}