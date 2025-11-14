using Moq;
using UContentMapper.Tests.Mocks;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Tests.Fixtures;

/// <summary>
/// Builder for creating test data scenarios
/// </summary>
public class TestDataBuilder
{
    public static IPublishedContent CreateBasicPublishedContent()
    {
        return MockPublishedContent.Create().Object;
    }

    public static IPublishedContent CreatePublishedContentWithProperties()
    {
        var mock = MockPublishedContent.Create();

        var properties = new Dictionary<string, object>
        {
            { "title", "Test Page Title" },
            { "description", "This is a test page description" },
            { "categoryid", 123 },
            { "ispublished", true },
            { "publishdate", DateTime.UtcNow.AddDays(-2) },
            { "tags", new List<string> { "tag1", "tag2", "tag3" } }
        };

        // Setup properties individually
        foreach (var prop in properties)
        {
            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(x => x.Alias).Returns(prop.Key);
            propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value != null);
            propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(prop.Value);
            
            mock.Setup(x => x.GetProperty(prop.Key)).Returns(propertyMock.Object);
        }
        
        // Setup content type
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        return mock.Object;
    }

    public static IPublishedContent CreatePublishedContentWithBuiltInProperties()
    {
        var mock = MockPublishedContent.Create();
        
        mock.Setup(x => x.Id).Returns(1001);
        mock.Setup(x => x.Key).Returns(Guid.Parse("12345678-1234-1234-1234-123456789012"));
        mock.Setup(x => x.Name).Returns("Test Content Name");
        mock.Setup(x => x.CreateDate).Returns(new DateTime(2023, 1, 1, 10, 0, 0));
        mock.Setup(x => x.UpdateDate).Returns(new DateTime(2023, 6, 15, 14, 30, 0));
        mock.Setup(x => x.Level).Returns(2);
        mock.Setup(x => x.SortOrder).Returns(5);
        mock.Setup(x => x.TemplateId).Returns(9999);
        mock.Setup(x => x.ContentType.Alias).Returns("testPage");

        return mock.Object;
    }

    public static IPublishedContent CreatePublishedContentWithEmptyStringContentTypeAlias()
    {
        var mock = MockPublishedContent.Create();

        mock.Setup(x => x.ContentType.Alias)
            .Returns(string.Empty);

        return mock.Object;
    }

    public static IPublishedElement CreatePublishedElementWithEmptyStringContentTypeAlias()
    {
        var mock = MockPublishedElement.Create();

        mock.Setup(x => x.ContentType.Alias)
            .Returns(string.Empty);

        return mock.Object;
    }

    public static IPublishedContent CreatePublishedContentForTypeConversion()
    {
        var mock = MockPublishedContent.Create();
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        var properties = new Dictionary<string, object>
        {
            { "stringvalue", "Test String" },
            { "intvalue", 42 },
            { "boolvalue", true },
            { "datetimevalue", DateTime.UtcNow },
            { "guidvalue", "12345678-1234-1234-1234-123456789012" },
            { "doublevalue", 3.14159 },
            { "decimalvalue", 999.99m },
            { "floatvalue", 2.718F },
            { "longvalue", 9223372036854775807 },
            { "shortvalue", 32767 },
            { "nullableintvalue", "null" },
            { "nullableboolvalue", "null" },
            { "nullabledatetimevalue", DateTime.UtcNow.AddDays(-1) },
            { "nullableguidvalue", "87654321-4321-4321-4321-210987654321" }
        };

        // Setup properties individually
        foreach (var prop in properties)
        {
            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(x => x.Alias).Returns(prop.Key);
            propertyMock.
                Setup(x => x.HasValue(
                    It.IsAny<string>(), 
                    It.IsAny<string>())).
                Returns(prop.Value is not null);
            propertyMock
                .Setup(x => x.GetValue(
                    It.IsAny<string>(), 
                    It.IsAny<string>()))
                .Returns(prop.Value);
            
            mock.Setup(x => x.GetProperty(prop.Key)).Returns(propertyMock.Object);
            mock.Setup(x => x.ContentType.GetPropertyType(prop.Key)).Returns(publishedPropertyTypeMock.Object);
        }
        
        // Setup content type
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns("typeConversionTest");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        return mock.Object;
    }

    public static IEnumerable<TestCaseData> GetTypeConversionTestCases()
    {
        yield return new TestCaseData(42, typeof(int), 42);
        yield return new TestCaseData(true, typeof(bool), true);
        yield return new TestCaseData(false, typeof(bool), false);
        yield return new TestCaseData(Guid.Parse("12345678-1234-1234-1234-123456789012"), typeof(Guid), 
            Guid.Parse("12345678-1234-1234-1234-123456789012"));
        yield return new TestCaseData(DateTime.Parse("2023-01-01"), typeof(DateTime), 
            DateTime.Parse("2023-01-01"));
        yield return new TestCaseData("Test", typeof(string), "Test");
        yield return new TestCaseData(null, typeof(string), string.Empty);
        yield return new TestCaseData(null, typeof(int), 0);
    }

    public static IEnumerable<TestCaseData> GetContentTypeAliasTestCases()
    {
        yield return new TestCaseData("testPage", "testPage", true);
        yield return new TestCaseData("testPage", "differentPage", false);
        yield return new TestCaseData("*", "anyContentType", true);
        yield return new TestCaseData("testPage", "", false);
    }

    public static IEnumerable<TestCaseData> GetBuiltInPropertyTestCases()
    {
        var content = CreatePublishedContentWithBuiltInProperties();
        
        yield return new TestCaseData(content, nameof(TestPageModel.Id), content.Id);
        yield return new TestCaseData(content, nameof(TestPageModel.Key), content.Key);
        yield return new TestCaseData(content, nameof(TestPageModel.Name), content.Name);
        yield return new TestCaseData(content, nameof(TestPageModel.ContentTypeAlias), content.ContentType.Alias);
        yield return new TestCaseData(content, nameof(TestPageModel.CreateDate), content.CreateDate);
        yield return new TestCaseData(content, nameof(TestPageModel.UpdateDate), content.UpdateDate);
        yield return new TestCaseData(content, nameof(TestPageModel.Level), content.Level);
        yield return new TestCaseData(content, nameof(TestPageModel.SortOrder), content.SortOrder);
        yield return new TestCaseData(content, nameof(TestPageModel.TemplateId), content.TemplateId);
    }

    public static IEnumerable<TestCaseData> GetMappingFailureTestCases()
    {
        yield return new TestCaseData(new object(), typeof(TestPageModel), "Cannot map object");
        yield return new TestCaseData(null, typeof(TestPageModel), "source");
    }

    public static TestPageModel CreateExpectedTestPageModel()
    {
        return new TestPageModel
        {
            Id = 1001,
            Key = new Guid("12345678-1234-1234-1234-123456789012"),
            Name = "Test Content Name",
            ContentTypeAlias = "testPage",
            Url = "/test-content",
            AbsoluteUrl = "https://example.com/test-content",
            CreateDate = new DateTime(2023, 1, 1, 10, 0, 0),
            UpdateDate = new DateTime(2023, 6, 15, 14, 30, 0),
            Level = 2,
            SortOrder = 5,
            TemplateId = 9999,
            IsVisible = true,
            Title = "Test Page Title",
            Description = "This is a test page description",
            CategoryId = 123,
            IsPublished = true,
            PublishDate = DateTime.UtcNow.AddDays(-2)
        };
    }
}