using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Tests.Fixtures;
using UContentMapper.Tests.Mocks;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Tests.Unit.Umbraco15.Mapping;

[TestFixture]
public class ParametrizedMappingTests : TestBase
{
    private FakeLogger<UmbracoContentMapper<TestPageModel>> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _logger = new FakeLogger<UmbracoContentMapper<TestPageModel>>();
    }

    [TestCaseSource(nameof(_getContentTypeAliasTestCases))]
    public void CanMap_WithDifferentContentTypeAliases_ShouldReturnExpectedResult(
        string modelContentTypeAlias, string contentContentTypeAlias, bool expectedResult)
    {
        // Arrange
        var mapper = _createMapperForContentType<TestPageModel>(modelContentTypeAlias);
        var content = MockPublishedContent.WithContentTypeAlias(contentContentTypeAlias).Object;

        // Act
        var result = mapper.CanMap(content);

        // Assert
        result.Should().Be(expectedResult);
    }

    [TestCaseSource(nameof(_getTypeConversionTestCases))]
    public void Map_WithDifferentTypeConversions_ShouldConvertCorrectly(
        object sourceValue, object expectedValue, string propertyName)
    {
        // Arrange
        var mapper = _createMapper<TypeConversionTestModel>(propertyName, sourceValue);
        
        // Create mock with IPublishedValueFallback
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var propertyMock = new Mock<IPublishedProperty>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Setup property
        propertyMock.Setup(x => x.Alias).Returns(propertyName.ToLowerInvariant());
        propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(sourceValue != null);
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

    [TestCaseSource(nameof(_getBuiltInPropertyTestCases))]
    public void Map_WithBuiltInProperties_ShouldMapCorrectly(
        string propertyName, object sourceValue, object expectedValue)
    {
        // Arrange
        var mapper = _createMapper<TestPageModel>(propertyName, sourceValue);
        var content = _createContentWithBuiltInProperty(propertyName, sourceValue);

        // Act
        var result = mapper.Map(content);

        // Assert
        var property = typeof(TestPageModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedValue);
    }

    [TestCaseSource(nameof(_getErrorScenarioTestCases))]
    public void Map_WithErrorScenarios_ShouldHandleGracefully(
        Dictionary<string, object> properties, string expectedErrorProperty)
    {
        // Arrange
        var (mapper, logger)  = _createMapperWithLogger<TestPageModel>(properties);
        
        // Create mock content with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Setup properties
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
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        logger.Collector.GetSnapshot().Should().Contain(log =>
            log.Message.Contains("Error mapping property", StringComparison.OrdinalIgnoreCase) &&
            log.Message.Contains(expectedErrorProperty, StringComparison.OrdinalIgnoreCase));
    }

    [TestCaseSource(nameof(_getNullValueTestCases))]
    public void Map_WithNullValues_ShouldUseDefaultValues(
        string propertyName, object? sourceValue, object expectedDefaultValue)
    {
        // Arrange
        var mapper = _createMapper<TestPageModel>();
        
        // Create mock content with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var propertyMock = new Mock<IPublishedProperty>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Setup property
        propertyMock.Setup(x => x.Alias).Returns(propertyName.ToLowerInvariant());
        propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false); // No value
        propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(sourceValue);
        
        mock.Setup(x => x.GetProperty(propertyName.ToLowerInvariant())).Returns(propertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(propertyName.ToLowerInvariant())).Returns(publishedPropertyTypeMock.Object);

        // Act
        var result = mapper.Map(mock.Object);

        // Assert
        var property = typeof(TestPageModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedDefaultValue);
    }

    [TestCaseSource(nameof(_getComplexMappingTestCases))]
    public void Map_WithComplexScenarios_ShouldMapCorrectly(
        Dictionary<string, object> properties, Action<TestPageModel> assertAction)
    {
        // Arrange
        var mapper = _createMapper<TestPageModel>(properties);
        
        // Create mock content with properties and content type
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var contentTypeMock = new Mock<IPublishedContentType>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        // Setup properties
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
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        assertAction(result);
    }

    [TestCaseSource(nameof(_getInvalidSourceTestCases))]
    public void CanMap_WithInvalidSources_ShouldReturnFalse(object source)
    {
        // Arrange
        var mapper = _createMapper<TestPageModel>();

        // Act
        var result = mapper.CanMap(source);

        // Assert
        result.Should().BeFalse();
    }

    [TestCaseSource(nameof(_getModelTypeTestCases))]
    public void Mapper_WithDifferentModelTypes_ShouldWorkCorrectly(Type modelType, string contentTypeAlias, bool shouldMap)
    {
        // Arrange
        var mapperType = typeof(UmbracoContentMapper<>).MakeGenericType(modelType);

        var loggerType = typeof(FakeLogger<>).MakeGenericType(mapperType);
        var collector = new FakeLogCollector();
        var logger = Activator.CreateInstance(loggerType, [collector]);

        var modelPropertyServiceType = typeof(IModelPropertyService);
        var propertyMapperType = typeof(IPublishedPropertyMapper<>).MakeGenericType(modelType);

        var propertyMapperMockType = typeof(Mock<>).MakeGenericType(propertyMapperType);
        var propertyMapperMock = Activator.CreateInstance(propertyMapperMockType);

        // Find the correct 'Object' property by type
        var objectProperty = propertyMapperMockType
            .GetProperties()
            .First(p => p.Name == "Object" && p.PropertyType == propertyMapperType);

        var propertyMapper = objectProperty.GetValue(propertyMapperMock);

        // Pass all three arguments to the constructor
        var mapper = Activator.CreateInstance(mapperType, logger, propertyMapper);

        var content = MockPublishedContent.WithContentTypeAlias(contentTypeAlias).Object;

        // Act
        var canMapMethod = mapperType.GetMethod("CanMap")!;
        var result = (bool)canMapMethod.Invoke(mapper, [content])!;

        // Assert
        result.Should().Be(shouldMap);
    }

    #region Test Case Sources

    private static IEnumerable<TestCaseData> _getContentTypeAliasTestCases()
    {
        yield return new TestCaseData("testPage", "testPage", true);
        yield return new TestCaseData("testPage", "differentPage", false);
        yield return new TestCaseData("*", "anyContentType", false);
        yield return new TestCaseData("testPage", "", false);
        yield return new TestCaseData("", "", false);
    }

    private static IEnumerable<TestCaseData> _getTypeConversionTestCases()
    {
        yield return new TestCaseData(42, 42, nameof(TypeConversionTestModel.IntValue));
        yield return new TestCaseData(true, true, nameof(TypeConversionTestModel.BoolValue));
        yield return new TestCaseData(false, false, nameof(TypeConversionTestModel.BoolValue));
        yield return new TestCaseData(Guid.Parse("ccc61176-74e7-443c-2a7f-08da87726a29"), 
            Guid.Parse("ccc61176-74e7-443c-2a7f-08da87726a29"), nameof(TypeConversionTestModel.GuidValue));
        yield return new TestCaseData(DateTime.Parse("2023-01-01"),
            DateTime.Parse("2023-01-01"), nameof(TypeConversionTestModel.DateTimeValue));
        yield return new TestCaseData("Test String", "Test String", nameof(TypeConversionTestModel.StringValue));
    }

    private static IEnumerable<TestCaseData> _getBuiltInPropertyTestCases()
    {
        yield return new TestCaseData(nameof(TestPageModel.Id), 1001, 1001);
        yield return new TestCaseData(nameof(TestPageModel.Name), "Test Name", "Test Name");
        yield return new TestCaseData(nameof(TestPageModel.Level), 2, 2);
        yield return new TestCaseData(nameof(TestPageModel.SortOrder), 5, 5);
    }

    private static IEnumerable<TestCaseData> _getErrorScenarioTestCases()
    {
        yield return new TestCaseData(
            new Dictionary<string, object> { { "categoryid", "not_a_number" } },
            "CategoryId"
        );
    }

    private static IEnumerable<TestCaseData> _getNullValueTestCases()
    {
        yield return new TestCaseData(nameof(TestPageModel.Title), null, string.Empty);
        yield return new TestCaseData(nameof(TestPageModel.CategoryId), null, 0);
        yield return new TestCaseData(nameof(TestPageModel.IsPublished), null, false);
        yield return new TestCaseData(nameof(TestPageModel.PublishDate), null, null);
    }

    private static IEnumerable<TestCaseData> _getComplexMappingTestCases()
    {
        yield return new TestCaseData(
            new Dictionary<string, object>
            {
                { "title", "Complex Title" },
                { "description", "Complex Description" },
                { "categoryid", 999 },
                { "ispublished", true }
            },
            new Action<TestPageModel>(model =>
            {
                model.Title.Should().Be("Complex Title");
                model.Description.Should().Be("Complex Description");
                model.CategoryId.Should().Be(999);
                model.IsPublished.Should().BeTrue();
            })
        );

        yield return new TestCaseData(
            new Dictionary<string, object>
            {
                { "title", "" },
                { "categoryid", 0 },
                { "ispublished", false }
            },
            new Action<TestPageModel>(model =>
            {
                model.Title.Should().Be("");
                model.CategoryId.Should().Be(0);
                model.IsPublished.Should().BeFalse();
            })
        );
    }

    private static IEnumerable<TestCaseData> _getInvalidSourceTestCases()
    {
        yield return new TestCaseData(new object());
        yield return new TestCaseData("string");
        yield return new TestCaseData(123);
        yield return new TestCaseData(null!);
    }

    private static IEnumerable<TestCaseData> _getModelTypeTestCases()
    {
        yield return new TestCaseData(typeof(TestPageModel), "testPage", true);
        yield return new TestCaseData(typeof(WildcardContentTypeModel), "anyContentType", true);
        yield return new TestCaseData(typeof(WrongSourceTypeModel), "testPage", false);
        yield return new TestCaseData(typeof(SimpleTestModel), "simpleContent", true);
    }

    #endregion

    #region Helper Methods

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
                prop?.SetValue(destination, sourceValue);
            });

        return new UmbracoContentMapper<T>(
            logger,
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
            propertyMapperMock.Object);

        return (mapper, logger);
    }

    private UmbracoContentMapper<T> _createMapperForContentType<T>(string contentTypeAlias) where T : class
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
            propertyMapperMock.Object);
    }

    private static object _createContentWithBuiltInProperty(string propertyName, object value)
    {
        var mock = MockPublishedContent.Create();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
        var publishedElementMock = new Mock<IPublishedElement>();

        switch (propertyName)
        {
            case nameof(TestPageModel.Id):
                mock.Setup(x => x.Id).Returns((int)value);
                break;
            case nameof(TestPageModel.Name):
                mock.Setup(x => x.Name).Returns((string)value);
                break;
            case nameof(TestPageModel.Level):
                mock.Setup(x => x.Level).Returns((int)value);
                break;
            case nameof(TestPageModel.SortOrder):
                mock.Setup(x => x.SortOrder).Returns((int)value);
                break;
            case nameof(TestPageModel.IsVisible):
                var propertyMock = new Mock<IPublishedProperty>();
                var alias = "isvisible";
                propertyMock.Setup(x => x.Alias).Returns(alias);
                propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
                propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

                publishedElementMock.Setup(x => x.GetProperty(It.IsAny<string>())).Returns(propertyMock.Object);

                mock.Setup(x => x.GetProperty(alias)).Returns(propertyMock.Object);
                mock.Setup(x => x.ContentType.GetPropertyType(alias)).Returns(publishedPropertyTypeMock.Object);
                break;
        }

        return mock.Object;
    }

    #endregion
}