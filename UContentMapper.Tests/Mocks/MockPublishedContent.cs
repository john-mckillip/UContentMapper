using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Tests.Mocks;

/// <summary>
/// Mock implementation and builder for IPublishedContent
/// </summary>
public class MockPublishedContent
{
    public static Mock<IPublishedContent> Create()
    {
        var mock = new Mock<IPublishedContent>();
        var contentTypeMock = new Mock<IPublishedContentType>();
        var propertiesMock = new Mock<IEnumerable<IPublishedProperty>>();
        
        // Set up basic properties
        mock.Setup(x => x.Id).Returns(1001);
        mock.Setup(x => x.Key).Returns(Guid.NewGuid());
        mock.Setup(x => x.Name).Returns("Test Content");
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        mock.Setup(x => x.CreateDate).Returns(DateTime.UtcNow.AddDays(-1));
        mock.Setup(x => x.UpdateDate).Returns(DateTime.UtcNow);
        mock.Setup(x => x.Level).Returns(1);
        mock.Setup(x => x.SortOrder).Returns(0);
        mock.Setup(x => x.TemplateId).Returns(1234);
        mock.Setup(x => x.Properties).Returns(propertiesMock.Object);
        
        // Set up content type
        contentTypeMock.Setup(x => x.Alias).Returns("testContentType");
        contentTypeMock.Setup(x => x.Id).Returns(1100);
        
        // Set up extension methods (these require more complex setup)
        mock.Setup(x => x.Url(It.IsAny<string>(), It.IsAny<UrlMode>()))
            .Returns((string culture, UrlMode mode) => 
                mode == UrlMode.Absolute ? "https://example.com/test-content" : "/test-content");
        
        mock.Setup(x => x.IsVisible()).Returns(true);
        
        return mock;
    }

    public static Mock<IPublishedContent> WithId(int id)
    {
        var mock = Create();
        mock.Setup(x => x.Id).Returns(id);
        return mock;
    }

    public static Mock<IPublishedContent> WithName(string name)
    {
        var mock = Create();
        mock.Setup(x => x.Name).Returns(name);
        return mock;
    }

    public static Mock<IPublishedContent> WithContentTypeAlias(string alias)
    {
        var mock = Create();
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns(alias);
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        return mock;
    }

    public static Mock<IPublishedContent> WithProperty(string alias, object value)
    {
        var mock = Create();
        var propertyMock = new Mock<IPublishedProperty>();
        
        propertyMock.Setup(x => x.Alias).Returns(alias);
        propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(value != null);
        propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(value);
        
        mock.Setup(x => x.HasProperty(alias)).Returns(true);
        mock.Setup(x => x.Value(alias, It.IsAny<string>(), It.IsAny<object>())).Returns(value);
        mock.Setup(x => x.GetProperty(alias)).Returns(propertyMock.Object);
        
        return mock;
    }

    public static Mock<IPublishedContent> WithProperties(Dictionary<string, object> properties)
    {
        var mock = Create();
        
        foreach (var kvp in properties)
        {
            var propertyMock = new Mock<IPublishedProperty>();
            propertyMock.Setup(x => x.Alias).Returns(kvp.Key);
            propertyMock.Setup(x => x.HasValue(It.IsAny<string>(), It.IsAny<string>())).Returns(kvp.Value != null);
            propertyMock.Setup(x => x.GetValue(It.IsAny<string>(), It.IsAny<string>())).Returns(kvp.Value);
            
            mock.Setup(x => x.HasProperty(kvp.Key)).Returns(true);
            mock.Setup(x => x.Value(kvp.Key, It.IsAny<string>(), It.IsAny<string?>())).Returns(kvp.Value);
            mock.Setup(x => x.GetProperty(kvp.Key)).Returns(propertyMock.Object);
        }
        
        return mock;
    }
}

/// <summary>
/// Mock implementation for IPublishedElement
/// </summary>
public class MockPublishedElement
{
    public static Mock<IPublishedElement> Create()
    {
        var mock = new Mock<IPublishedElement>();
        var contentTypeMock = new Mock<IPublishedContentType>();
        
        mock.Setup(x => x.Key).Returns(Guid.NewGuid());
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        
        contentTypeMock.Setup(x => x.Alias).Returns("testElement");
        contentTypeMock.Setup(x => x.Id).Returns(2000);
        
        return mock;
    }

    public static Mock<IPublishedElement> WithContentTypeAlias(string alias)
    {
        var mock = Create();
        var contentTypeMock = new Mock<IPublishedContentType>();
        contentTypeMock.Setup(x => x.Alias).Returns(alias);
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        return mock;
    }
}

/// <summary>
/// Mock implementation for MediaWithCrops
/// </summary>
public class MockMediaWithCrops
{
    public static Mock<MediaWithCrops> Create()
    {
        var mock = new Mock<MediaWithCrops>();
        var contentMock = MockPublishedContent.Create();
        
        mock.Setup(x => x.Content).Returns(contentMock.Object);
        mock.Setup(x => x.Name).Returns("Test Image");
        
        return mock;
    }

    public static Mock<MediaWithCrops> WithContent(IPublishedContent content)
    {
        var mock = Create();
        mock.Setup(x => x.Content).Returns(content);
        return mock;
    }
}