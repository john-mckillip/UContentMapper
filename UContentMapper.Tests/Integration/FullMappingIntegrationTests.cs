using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Tests.Fixtures;
using UContentMapper.Tests.Mocks;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Extensions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Strings;

namespace UContentMapper.Tests.Integration;

[TestFixture]
public class FullMappingIntegrationTests : TestBase
{
    private IServiceProvider _serviceProvider;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddUContentMapper();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public override void TearDown()
    {
        base.TearDown();
        (_serviceProvider as IDisposable)?.Dispose();
    }

    [Test]
    public void EndToEndMapping_SimpleModel_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        var content = TestDataBuilder.CreatePublishedContentWithBuiltInProperties();

        // Act
        var canMap = mapper.CanMap(content);
        var result = mapper.Map(content);

        // Assert
        canMap.Should().BeTrue();
        result.Should().NotBeNull();
        result.Id.Should().Be(1001);
        result.Key.Should().Be(new Guid("12345678-1234-1234-1234-123456789012"));
        result.Name.Should().Be("Test Content Name");
        result.ContentTypeAlias.Should().Be("testPage");
        result.Level.Should().Be(2);
        result.SortOrder.Should().Be(5);
        result.TemplateId.Should().Be(9999);
    }

    [Test]
    public void EndToEndMapping_WithCustomProperties_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        
        // Create content mock with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var publishedValueFallbackMock = new Mock<PublishedValueFallback>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Set up content type alias
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);

        // Set up properties
        var properties = new Dictionary<string, object>
        {
            { "title", "Integration Test Title" },
            { "mainbody", new HtmlEncodedString("{<p>This is some test html from a WYSIWYG editor.</p>}") },
            { "description", "Integration test description" },
            { "categoryid", 999 },
            { "ispublished", true },
            { "publishdate", DateTime.UtcNow.AddDays(-5) }
        };
        
        foreach (var prop in properties)
        {
            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(x => x.Alias).Returns(prop.Key);
            propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value is not null);
            propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value);
            
            mock.Setup(x => x.GetProperty(prop.Key)).Returns(propertyMock.Object);
            mock.Setup(x => x.ContentType.GetPropertyType(prop.Key)).Returns(publishedPropertyTypeMock.Object);
        }

        // Act
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Integration Test Title");
        result.Description.Should().Be("Integration test description");
        result.MainBody.Should().NotBe(null);
        result.MainBody?.ToString().Should().Be("{<p>This is some test html from a WYSIWYG editor.</p>}");
        result.CategoryId.Should().Be(999);
        result.IsPublished.Should().BeTrue();
        result.PublishDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-5), TimeSpan.FromSeconds(1));
    }

    [Test]
    public void EndToEndMapping_TypeConversion_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TypeConversionTestModel>>();
        
        // Create content mock with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Set up content type alias
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("typeConversionTest");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);

        // Set up properties
        var properties = new Dictionary<string, object>
        {
            { "stringvalue", "Test String Value" },
            { "intvalue", 42 },
            { "boolvalue", true },
            { "datetimevalue", DateTime.Parse("2023-01-01T10:30:00Z") },
            { "guidvalue", "12345678-1234-1234-1234-123456789012" },
            { "doublevalue", 3.14159 },
            { "decimalvalue", 999.99m },
            { "floatvalue", 2.718F },
            { "longvalue", 9223372036854775807 },
            { "shortvalue", 32767 },
            { "nullablehtmlcontentvalue", new HtmlEncodedString("{<p>This is some test html from a WYSIWYG editor.</p>}")}
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
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        result.StringValue.Should().Be("Test String Value");
        result.IntValue.Should().Be(42);
        result.BoolValue.Should().BeTrue();
        result.DateTimeValue.Should().Be(DateTime.Parse("2023-01-01T10:30:00Z"));
        result.GuidValue.Should().Be(new Guid("12345678-1234-1234-1234-123456789012"));
        result.DoubleValue.Should().BeApproximately(3.14159, 0.00001);
        result.DecimalValue.Should().Be(999.99m);
        result.FloatValue.Should().BeApproximately(2.718f, 0.001f);
        result.LongValue.Should().Be(9223372036854775807);
        result.ShortValue.Should().Be(32767);
        result.NullableHtmlContentValue.Should().NotBe(null);
        result.NullableHtmlContentValue?.ToString().Should().Be("{<p>This is some test html from a WYSIWYG editor.</p>}");
    }

    [Test]
    public void EndToEndMapping_WildcardContentType_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<WildcardContentTypeModel>>();
        
        // Create content mock with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Set up content type alias
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("anyContentType");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        // Set up custom property
        var propertyMock = new Mock<IPublishedProperty>();
        propertyMock.Setup(x => x.Alias).Returns("customproperty");
        propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("Custom Value");
        
        mock.Setup(x => x.GetProperty("customproperty")).Returns(propertyMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType("customproperty")).Returns(publishedPropertyTypeMock.Object);

        // Act
        var canMap = mapper.CanMap(mock.Object);
        var result = mapper.Map(mock.Object);

        // Assert
        canMap.Should().BeTrue();
        result.Should().NotBeNull();
        result.CustomProperty.Should().Be("Custom Value");
    }

    [Test]
    public void EndToEndMapping_InvalidContentType_ShouldNotMap()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        
        // Create content mock with properties
        var mock = MockPublishedContent.Create();
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("wrongContentType");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);

        // Act
        var canMap = mapper.CanMap(mock.Object);

        // Assert
        canMap.Should().BeFalse();
    }

    [Test]
    public void EndToEndMapping_InvalidSource_ShouldThrowException()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        var invalidSource = new object();

        // Act
        var canMap = mapper.CanMap(invalidSource);
        var act = () => mapper.Map(invalidSource);

        // Assert
        canMap.Should().BeFalse();
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Cannot map object of type Object to TestPageModel");
    }

    [Test]
    public void EndToEndMapping_MultipleMappers_ShouldWorkIndependently()
    {
        // Arrange
        var pageMapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        var simpleMapper = _serviceProvider.GetRequiredService<IContentMapper<SimpleTestModel>>();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Create page content
        var pageMock = MockPublishedContent.Create();
        var pageContentTypeMock = new Mock<IPublishedContentType>();
        pageContentTypeMock.Setup(x => x.Alias).Returns("testPage");
        pageMock.Setup(x => x.ContentType).Returns(pageContentTypeMock.Object);
        pageMock.Setup(x => x.ContentType.GetPropertyType("title")).Returns(publishedPropertyTypeMock.Object);

        // Set up title property
        var titlePropertyMock = new Mock<IPublishedProperty>();
        titlePropertyMock.Setup(x => x.Alias).Returns("title");
        titlePropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        titlePropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("Page Title");
        
        pageMock.Setup(x => x.GetProperty("title")).Returns(titlePropertyMock.Object);
            
        // Create simple content
        var simpleMock = MockPublishedContent.Create();
        var simpleContentTypeMock = new Mock<IPublishedContentType>();
        simpleContentTypeMock.Setup(x => x.Alias).Returns("simpleContent");
        simpleMock.Setup(x => x.ContentType).Returns(simpleContentTypeMock.Object);
        simpleMock.Setup(x => x.ContentType.GetPropertyType("email")).Returns(publishedPropertyTypeMock.Object);

        // Set up email property
        var emailPropertyMock = new Mock<IPublishedProperty>();
        emailPropertyMock.Setup(x => x.Alias).Returns("email");
        emailPropertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        emailPropertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("test@example.com");
        
        simpleMock.Setup(x => x.GetProperty("email")).Returns(emailPropertyMock.Object);

        // Act
        var pageResult = pageMapper.Map(pageMock.Object);
        var simpleResult = simpleMapper.Map(simpleMock.Object);

        // Assert
        pageResult.Should().NotBeNull();
        pageResult.Should().BeOfType<TestPageModel>();
        pageResult.Title.Should().Be("Page Title");

        simpleResult.Should().NotBeNull();
        simpleResult.Should().BeOfType<SimpleTestModel>();
        simpleResult.Email.Should().Be("test@example.com");
    }

    [Test]
    public void EndToEndMapping_NullAndEmptyValues_ShouldHandleGracefully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        
        // Create content mock with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        
        // Set up content type alias
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        // Set up properties with null/empty values
        var properties = new Dictionary<string, object?>
        {
            { "title", null! },
            { "description", "" },
            { "categoryid", null! },
            { "ispublished", null! }
        };
        
        foreach (var prop in properties)
        {
            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(x => x.Alias).Returns(prop.Key);
            propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(false); // All are null or empty
            propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value);
            
            mock.Setup(x => x.GetProperty(prop.Key)).Returns(propertyMock.Object);
        }

        // Act
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(string.Empty); // Default value for string
        result.Description.Should().Be("");
        result.CategoryId.Should().Be(0); // Default value for int
        result.IsPublished.Should().BeFalse(); // Default value for bool
    }

    [Test]
    public void EndToEndMapping_ErrorConditions_ShouldLogAndContinue()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        
        // Create content mock with properties
        var mock = MockPublishedContent.Create();
        var fallbackMock = new Mock<IPublishedValueFallback>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        // Set up content type alias
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);

        // Set up properties - one valid, one that will cause conversion error
        var properties = new Dictionary<string, object>
        {
            { "title", "Valid Title" },
            { "categoryid", "not_a_number" }, // This should fail conversion
            { "description", "Valid Description" }
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
        var result = mapper.Map(mock.Object);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Valid Title"); // Should map successfully
        result.CategoryId.Should().Be(0); // Should use default value on conversion failure
        result.Description.Should().Be("Valid Description"); // Should continue mapping other properties
    }

    [Test]
    public void EndToEndMapping_ComplexScenario_ShouldMapAllSupportedProperties()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();
        
        var content = MockPublishedContent.Create();
        content.Setup(x => x.Id).Returns(5000);
        content.Setup(x => x.Key).Returns(new Guid("11111111-2222-3333-4444-555555555555"));
        content.Setup(x => x.Name).Returns("Complex Test Page");
        content.Setup(x => x.CreateDate).Returns(new DateTime(2023, 1, 1));
        content.Setup(x => x.UpdateDate).Returns(new DateTime(2023, 6, 15));
        content.Setup(x => x.Level).Returns(3);
        content.Setup(x => x.SortOrder).Returns(10);
        content.Setup(x => x.TemplateId).Returns(7777);
        
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        content.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        // Setup properties
        var titlePropMock = new Mock<IPublishedProperty>();
        titlePropMock.Setup(x => x.Alias).Returns("title");
        titlePropMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        titlePropMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("Complex Title");
        
        var descPropMock = new Mock<IPublishedProperty>();
        descPropMock.Setup(x => x.Alias).Returns("description");
        descPropMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        descPropMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns("Complex Description");
        
        content.Setup(x => x.GetProperty("title")).Returns(titlePropMock.Object);
        content.Setup(x => x.GetProperty("description")).Returns(descPropMock.Object);

        content.Setup(x => x.ContentType.GetPropertyType("title")).Returns(publishedPropertyTypeMock.Object);
        content.Setup(x => x.ContentType.GetPropertyType("description")).Returns(publishedPropertyTypeMock.Object);

        // Act
        var result = mapper.Map(content.Object);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(5000);
        result.Key.Should().Be(new Guid("11111111-2222-3333-4444-555555555555"));
        result.Name.Should().Be("Complex Test Page");
        result.ContentTypeAlias.Should().Be("testPage");
        result.CreateDate.Should().Be(new DateTime(2023, 1, 1));
        result.UpdateDate.Should().Be(new DateTime(2023, 6, 15));
        result.Level.Should().Be(3);
        result.SortOrder.Should().Be(10);
        result.TemplateId.Should().Be(7777);
        result.IsVisible.Should().BeFalse();
        result.Title.Should().Be("Complex Title");
        result.Description.Should().Be("Complex Description");
    }
}