using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Tests.Fixtures;
using UContentMapper.Tests.Mocks;
using UContentMapper.Tests.TestHelpers;
using UContentMapper.Umbraco15.Extensions;
using Umbraco.Extensions;

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
        result.ContentTypeAlias.Should().Be("testContentType");
        result.Url.Should().Be("/test-content");
        result.AbsoluteUrl.Should().Be("https://example.com/test-content");
        result.Level.Should().Be(2);
        result.SortOrder.Should().Be(5);
        result.TemplateId.Should().Be(9999);
        result.IsVisible.Should().BeTrue();
    }

    [Test]
    public void EndToEndMapping_WithCustomProperties_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TestPageModel>>();
        
        var properties = new Dictionary<string, object>
        {
            { "title", "Integration Test Title" },
            { "description", "Integration test description" },
            { "categoryid", 999 },
            { "ispublished", true },
            { "publishdate", DateTime.UtcNow.AddDays(-5) }
        };
        
        var content = MockPublishedContent
            .WithContentTypeAlias("testPage")
            .WithProperties(properties)
            .Object;

        // Act
        var result = mapper.Map(content);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be("Integration Test Title");
        result.Description.Should().Be("Integration test description");
        result.CategoryId.Should().Be(999);
        result.IsPublished.Should().BeTrue();
        result.PublishDate.Should().BeCloseTo(DateTime.UtcNow.AddDays(-5), TimeSpan.FromSeconds(1));
    }

    [Test]
    public void EndToEndMapping_TypeConversion_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<TypeConversionTestModel>>();
        
        var properties = new Dictionary<string, object>
        {
            { "stringvalue", "Test String Value" },
            { "intvalue", "42" },
            { "boolvalue", "true" },
            { "datetimevalue", DateTime.Parse("2023-01-01T10:30:00Z") },
            { "guidvalue", "12345678-1234-1234-1234-123456789012" },
            { "doublevalue", "3.14159" },
            { "decimalvalue", "999.99" },
            { "floatvalue", "2.718" },
            { "longvalue", "9223372036854775807" },
            { "shortvalue", "32767" }
        };
        
        var content = MockPublishedContent
            .WithContentTypeAlias("typeConversionTest")
            .WithProperties(properties)
            .Object;

        // Act
        var result = mapper.Map(content);

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
    }

    [Test]
    public void EndToEndMapping_WildcardContentType_ShouldMapSuccessfully()
    {
        // Arrange
        var mapper = _serviceProvider.GetRequiredService<IContentMapper<WildcardContentTypeModel>>();
        var content = MockPublishedContent
            .WithContentTypeAlias("anyContentType")
            .WithProperty("customproperty", "Custom Value")
            .Object;

        // Act
        var canMap = mapper.CanMap(content);
        var result = mapper.Map(content);

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
        var content = MockPublishedContent
            .WithContentTypeAlias("wrongContentType")
            .Object;

        // Act
        var canMap = mapper.CanMap(content);

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
        
        var pageContent = MockPublishedContent
            .WithContentTypeAlias("testPage")
            .WithProperty("title", "Page Title")
            .Object;
            
        var simpleContent = MockPublishedContent
            .WithContentTypeAlias("simpleContent")
            .WithProperty("email", "test@example.com")
            .Object;

        // Act
        var pageResult = pageMapper.Map(pageContent);
        var simpleResult = simpleMapper.Map(simpleContent);

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
        
        var properties = new Dictionary<string, object>
        {
            { "title", null! },
            { "description", "" },
            { "categoryid", null! },
            { "ispublished", null! }
        };
        
        var content = MockPublishedContent
            .WithContentTypeAlias("testPage")
            .WithProperties(properties)
            .Object;

        // Act
        var result = mapper.Map(content);

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
        
        var properties = new Dictionary<string, object>
        {
            { "title", "Valid Title" },
            { "categoryid", "not_a_number" }, // This should fail conversion
            { "description", "Valid Description" }
        };
        
        var content = MockPublishedContent
            .WithContentTypeAlias("testPage")
            .WithProperties(properties)
            .Object;

        // Act
        var result = mapper.Map(content);

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
        
        var content = MockPublishedContent.Create();
        content.Setup(x => x.Id).Returns(5000);
        content.Setup(x => x.Key).Returns(new Guid("11111111-2222-3333-4444-555555555555"));
        content.Setup(x => x.Name).Returns("Complex Test Page");
        content.Setup(x => x.CreateDate).Returns(new DateTime(2023, 1, 1));
        content.Setup(x => x.UpdateDate).Returns(new DateTime(2023, 6, 15));
        content.Setup(x => x.Level).Returns(3);
        content.Setup(x => x.SortOrder).Returns(10);
        content.Setup(x => x.TemplateId).Returns(7777);
        content.Setup(x => x.IsVisible()).Returns(false);
        
        var contentTypeMock = MockPublishedContent.Create().Object.ContentType;
        content.Setup(x => x.ContentType.Alias).Returns("testPage");
        content.Setup(x => x.ContentType).Returns(contentTypeMock);
        
        content.Setup(x => x.HasProperty("title")).Returns(true);
        content.Setup(x => x.Value("title", It.IsAny<string>(), It.IsAny<object>())).Returns("Complex Title");
        content.Setup(x => x.HasProperty("description")).Returns(true);
        content.Setup(x => x.Value("description", It.IsAny<string>(), It.IsAny<object>())).Returns("Complex Description");

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