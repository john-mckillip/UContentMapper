using Moq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;

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
        contentTypeMock.Setup(x => x.Alias).Returns("testPage");
        contentTypeMock.Setup(x => x.Id).Returns(1100);
        
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
        var publishedPropertyTypeMock = new Mock<IPublishedPropertyType>();

        contentTypeMock.Setup(x => x.Alias).Returns(alias);
        mock.Setup(x => x.ContentType).Returns(contentTypeMock.Object);
        mock.Setup(x => x.ContentType.GetPropertyType(alias)).Returns(publishedPropertyTypeMock.Object);
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