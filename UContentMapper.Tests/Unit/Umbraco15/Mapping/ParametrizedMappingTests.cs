using FluentAssertions;
using Microsoft.Extensions.Logging.Testing;
using Moq;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Tests.Fixtures;
using UContentMapper.Tests.Mocks;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Mapping;

namespace UContentMapper.Tests.Unit.Umbraco15.Mapping;

[TestFixture]
public class ParametrizedMappingTests : TestBase
{
    private Mock<IMappingConfiguration> _mappingConfigurationMock;
    private FakeLogger<UmbracoContentMapper<TestPageModel>> _logger;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _mappingConfigurationMock = CreateMock<IMappingConfiguration>();
        _logger = new FakeLogger<UmbracoContentMapper<TestPageModel>>();
    }

    [TestCaseSource(nameof(GetContentTypeAliasTestCases))]
    public void CanMap_WithDifferentContentTypeAliases_ShouldReturnExpectedResult(
        string modelContentTypeAlias, string contentContentTypeAlias, bool expectedResult)
    {
        // Arrange
        var mapper = CreateMapperForContentType<TestPageModel>(modelContentTypeAlias);
        var content = MockPublishedContent.WithContentTypeAlias(contentContentTypeAlias).Object;

        // Act
        var result = mapper.CanMap(content);

        // Assert
        result.Should().Be(expectedResult);
    }

    [TestCaseSource(nameof(GetTypeConversionTestCases))]
    public void Map_WithDifferentTypeConversions_ShouldConvertCorrectly(
        object sourceValue, Type targetType, object expectedValue, string propertyName)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TypeConversionTestModel>(_mappingConfigurationMock.Object,
            new FakeLogger<UmbracoContentMapper<TypeConversionTestModel>>());
        
        var properties = new Dictionary<string, object> { { propertyName.ToLowerInvariant(), sourceValue } };
        var content = MockPublishedContent.WithProperties(properties).Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        var property = typeof(TypeConversionTestModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedValue);
    }

    [TestCaseSource(nameof(GetBuiltInPropertyTestCases))]
    public void Map_WithBuiltInProperties_ShouldMapCorrectly(
        string propertyName, object sourceValue, object expectedValue)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TestPageModel>(_mappingConfigurationMock.Object, _logger);
        var content = CreateContentWithBuiltInProperty(propertyName, sourceValue);

        // Act
        var result = mapper.Map(content);

        // Assert
        var property = typeof(TestPageModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedValue);
    }

    [TestCaseSource(nameof(GetErrorScenarioTestCases))]
    public void Map_WithErrorScenarios_ShouldHandleGracefully(
        Dictionary<string, object> properties, string expectedErrorProperty)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TestPageModel>(_mappingConfigurationMock.Object, _logger);
        var content = MockPublishedContent.WithProperties(properties).Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Should().NotBeNull();
        _logger.Collector.GetSnapshot().Should().Contain(log => 
            log.Message.Contains("Error mapping property") && 
            log.Message.Contains(expectedErrorProperty));
    }

    [TestCaseSource(nameof(GetNullValueTestCases))]
    public void Map_WithNullValues_ShouldUseDefaultValues(
        string propertyName, object? sourceValue, object expectedDefaultValue)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TestPageModel>(_mappingConfigurationMock.Object, _logger);
        var properties = new Dictionary<string, object> { { propertyName.ToLowerInvariant(), sourceValue! } };
        var content = MockPublishedContent.WithProperties(properties).Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        var property = typeof(TestPageModel).GetProperty(propertyName);
        property.Should().NotBeNull();
        var actualValue = property!.GetValue(result);
        actualValue.Should().Be(expectedDefaultValue);
    }

    [TestCaseSource(nameof(GetComplexMappingTestCases))]
    public void Map_WithComplexScenarios_ShouldMapCorrectly(
        Dictionary<string, object> properties, Action<TestPageModel> assertAction)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TestPageModel>(_mappingConfigurationMock.Object, _logger);
        var content = MockPublishedContent
            .WithContentTypeAlias("testPage")
            .WithProperties(properties)
            .Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Should().NotBeNull();
        assertAction(result);
    }

    [TestCaseSource(nameof(GetInvalidSourceTestCases))]
    public void CanMap_WithInvalidSources_ShouldReturnFalse(object source)
    {
        // Arrange
        var mapper = new UmbracoContentMapper<TestPageModel>(_mappingConfigurationMock.Object, _logger);

        // Act
        var result = mapper.CanMap(source);

        // Assert
        result.Should().BeFalse();
    }

    [TestCaseSource(nameof(GetModelTypeTestCases))]
    public void Mapper_WithDifferentModelTypes_ShouldWorkCorrectly(Type modelType, string contentTypeAlias, bool shouldMap)
    {
        // Arrange
        var mapperType = typeof(UmbracoContentMapper<>).MakeGenericType(modelType);
        var loggerType = typeof(FakeLogger<>).MakeGenericType(mapperType);
        var logger = Activator.CreateInstance(loggerType);
        var mapper = Activator.CreateInstance(mapperType, _mappingConfigurationMock.Object, logger);
        
        var content = MockPublishedContent.WithContentTypeAlias(contentTypeAlias).Object;

        // Act
        var canMapMethod = mapperType.GetMethod("CanMap")!;
        var result = (bool)canMapMethod.Invoke(mapper, new object[] { content })!;

        // Assert
        result.Should().Be(shouldMap);
    }

    #region Test Case Sources

    public static IEnumerable<TestCaseData> GetContentTypeAliasTestCases()
    {
        yield return new TestCaseData("testPage", "testPage", true).SetName("Exact Match");
        yield return new TestCaseData("testPage", "differentPage", false).SetName("No Match");
        yield return new TestCaseData("*", "anyContentType", true).SetName("Wildcard Match");
        yield return new TestCaseData("testPage", "", false).SetName("Empty Content Type");
        yield return new TestCaseData("", "testPage", false).SetName("Empty Model Alias");
    }

    public static IEnumerable<TestCaseData> GetTypeConversionTestCases()
    {
        yield return new TestCaseData("42", typeof(int), 42, nameof(TypeConversionTestModel.IntValue)).SetName("String to Int");
        yield return new TestCaseData("true", typeof(bool), true, nameof(TypeConversionTestModel.BoolValue)).SetName("String to Bool True");
        yield return new TestCaseData("false", typeof(bool), false, nameof(TypeConversionTestModel.BoolValue)).SetName("String to Bool False");
        yield return new TestCaseData("12345678-1234-1234-1234-123456789012", typeof(Guid), 
            new Guid("12345678-1234-1234-1234-123456789012"), nameof(TypeConversionTestModel.GuidValue)).SetName("String to Guid");
        yield return new TestCaseData(DateTime.Parse("2023-01-01"), typeof(DateTime), 
            DateTime.Parse("2023-01-01"), nameof(TypeConversionTestModel.DateTimeValue)).SetName("DateTime to DateTime");
        yield return new TestCaseData("Test String", typeof(string), "Test String", nameof(TypeConversionTestModel.StringValue)).SetName("String to String");
    }

    public static IEnumerable<TestCaseData> GetBuiltInPropertyTestCases()
    {
        yield return new TestCaseData(nameof(TestPageModel.Id), 1001, 1001).SetName("Id Property");
        yield return new TestCaseData(nameof(TestPageModel.Name), "Test Name", "Test Name").SetName("Name Property");
        yield return new TestCaseData(nameof(TestPageModel.Level), 2, 2).SetName("Level Property");
        yield return new TestCaseData(nameof(TestPageModel.SortOrder), 5, 5).SetName("SortOrder Property");
        yield return new TestCaseData(nameof(TestPageModel.IsVisible), true, true).SetName("IsVisible Property");
    }

    public static IEnumerable<TestCaseData> GetErrorScenarioTestCases()
    {
        yield return new TestCaseData(
            new Dictionary<string, object> { { "categoryid", "not_a_number" } },
            "CategoryId"
        ).SetName("Invalid Int Conversion");
        
        yield return new TestCaseData(
            new Dictionary<string, object> { { "guidvalue", "invalid_guid" } },
            "GuidValue"
        ).SetName("Invalid Guid Conversion");
    }

    public static IEnumerable<TestCaseData> GetNullValueTestCases()
    {
        yield return new TestCaseData(nameof(TestPageModel.Title), null, string.Empty).SetName("Null String");
        yield return new TestCaseData(nameof(TestPageModel.CategoryId), null, 0).SetName("Null Int");
        yield return new TestCaseData(nameof(TestPageModel.IsPublished), null, false).SetName("Null Bool");
        yield return new TestCaseData(nameof(TestPageModel.PublishDate), null, null).SetName("Null Nullable DateTime");
    }

    public static IEnumerable<TestCaseData> GetComplexMappingTestCases()
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
        ).SetName("All Properties Mapping");

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
        ).SetName("Empty Values Mapping");
    }

    public static IEnumerable<TestCaseData> GetInvalidSourceTestCases()
    {
        yield return new TestCaseData(new object()).SetName("Generic Object");
        yield return new TestCaseData("string").SetName("String");
        yield return new TestCaseData(123).SetName("Integer");
        yield return new TestCaseData(null!).SetName("Null");
    }

    public static IEnumerable<TestCaseData> GetModelTypeTestCases()
    {
        yield return new TestCaseData(typeof(TestPageModel), "testPage", true).SetName("TestPageModel with testPage");
        yield return new TestCaseData(typeof(WildcardContentTypeModel), "anyContentType", true).SetName("WildcardModel with any type");
        yield return new TestCaseData(typeof(WrongSourceTypeModel), "testPage", false).SetName("WrongSourceTypeModel");
        yield return new TestCaseData(typeof(SimpleTestModel), "simpleContent", true).SetName("SimpleTestModel with simpleContent");
    }

    #endregion

    #region Helper Methods

    private UmbracoContentMapper<T> CreateMapperForContentType<T>(string contentTypeAlias) where T : class
    {
        // Create a custom mapper configuration that simulates the attribute behavior
        var mapper = new UmbracoContentMapper<T>(_mappingConfigurationMock.Object,
            new FakeLogger<UmbracoContentMapper<T>>());
        return mapper;
    }

    private object CreateContentWithBuiltInProperty(string propertyName, object value)
    {
        var mock = MockPublishedContent.Create();

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
                mock.Setup(x => x.IsVisible()).Returns((bool)value);
                break;
        }

        return mock.Object;
    }

    #endregion
}