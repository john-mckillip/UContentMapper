using FluentAssertions;
using UContentMapper.Core.Models.Content;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Models;

[TestFixture]
public class BaseContentModelTests : TestBase
{
    private TestableBaseContentModel _model;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _model = new TestableBaseContentModel();
    }

    [Test]
    public void BaseContentModel_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var model = new TestableBaseContentModel();

        // Assert
        model.Id.Should().Be(0);
        model.Key.Should().Be(Guid.Empty);
        model.Name.Should().Be(string.Empty);
        model.ContentTypeAlias.Should().Be(string.Empty);
        model.Url.Should().Be(string.Empty);
        model.AbsoluteUrl.Should().Be(string.Empty);
        model.CreateDate.Should().Be(DateTime.MinValue);
        model.UpdateDate.Should().Be(DateTime.MinValue);
        model.Level.Should().Be(0);
        model.SortOrder.Should().Be(0);
        model.IsVisible.Should().BeFalse();
        model.TemplateId.Should().BeNull();
    }

    [Test]
    public void BaseContentModel_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var id = 1001;
        var key = Guid.NewGuid();
        var name = "Test Content";
        var contentTypeAlias = "testPage";
        var url = "/test-page";
        var absoluteUrl = "https://example.com/test-page";
        var createDate = DateTime.UtcNow.Date;
        var updateDate = DateTime.UtcNow.Date.AddHours(1);
        var level = 2;
        var sortOrder = 5;
        var isVisible = true;
        var templateId = 1234;

        // Act
        _model.Id = id;
        _model.Key = key;
        _model.Name = name;
        _model.ContentTypeAlias = contentTypeAlias;
        _model.Url = url;
        _model.AbsoluteUrl = absoluteUrl;
        _model.CreateDate = createDate;
        _model.UpdateDate = updateDate;
        _model.Level = level;
        _model.SortOrder = sortOrder;
        _model.IsVisible = isVisible;
        _model.TemplateId = templateId;

        // Assert
        _model.Id.Should().Be(id);
        _model.Key.Should().Be(key);
        _model.Name.Should().Be(name);
        _model.ContentTypeAlias.Should().Be(contentTypeAlias);
        _model.Url.Should().Be(url);
        _model.AbsoluteUrl.Should().Be(absoluteUrl);
        _model.CreateDate.Should().Be(createDate);
        _model.UpdateDate.Should().Be(updateDate);
        _model.Level.Should().Be(level);
        _model.SortOrder.Should().Be(sortOrder);
        _model.IsVisible.Should().Be(isVisible);
        _model.TemplateId.Should().Be(templateId);
    }

    [Test]
    public void BaseContentModel_ShouldAllowNullTemplateId()
    {
        // Arrange & Act
        _model.TemplateId = null;

        // Assert
        _model.TemplateId.Should().BeNull();
    }

    [Test]
    public void BaseContentModel_ShouldHandleEmptyStrings()
    {
        // Arrange & Act
        _model.Name = "";
        _model.ContentTypeAlias = "";
        _model.Url = "";
        _model.AbsoluteUrl = "";

        // Assert
        _model.Name.Should().Be("");
        _model.ContentTypeAlias.Should().Be("");
        _model.Url.Should().Be("");
        _model.AbsoluteUrl.Should().Be("");
    }

    [Test]
    public void BaseContentModel_ShouldHandleMinMaxDateTimes()
    {
        // Arrange & Act
        _model.CreateDate = DateTime.MinValue;
        _model.UpdateDate = DateTime.MaxValue;

        // Assert
        _model.CreateDate.Should().Be(DateTime.MinValue);
        _model.UpdateDate.Should().Be(DateTime.MaxValue);
    }

    [Test]
    public void BaseContentModel_ShouldHandleNegativeValues()
    {
        // Arrange & Act
        _model.Id = -1;
        _model.Level = -1;
        _model.SortOrder = -1;
        _model.TemplateId = -1;

        // Assert
        _model.Id.Should().Be(-1);
        _model.Level.Should().Be(-1);
        _model.SortOrder.Should().Be(-1);
        _model.TemplateId.Should().Be(-1);
    }

    /// <summary>
    /// Testable implementation of BaseContentModel for testing purposes
    /// </summary>
    private class TestableBaseContentModel : BaseContentModel
    {
        // This class is needed because BaseContentModel is abstract
    }
}