using FluentAssertions;
using UContentMapper.Core.Models.Content;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Models;

[TestFixture]
public class BaseElementModelTests : TestBase
{
    private BaseElementModel _model;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _model = new BaseElementModel();
    }

    [Test]
    public void BaseElementModel_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var model = new BaseElementModel();

        // Assert
        model.Key.Should().Be(Guid.Empty);
        model.ContentTypeAlias.Should().Be(string.Empty);
    }

    [Test]
    public void BaseElementModel_ShouldAllowSettingKey()
    {
        // Arrange
        var key = Guid.NewGuid();

        // Act
        _model.Key = key;

        // Assert
        _model.Key.Should().Be(key);
    }

    [Test]
    public void BaseElementModel_ShouldAllowSettingContentTypeAlias()
    {
        // Arrange
        var contentTypeAlias = "testElement";

        // Act
        _model.ContentTypeAlias = contentTypeAlias;

        // Assert
        _model.ContentTypeAlias.Should().Be(contentTypeAlias);
    }

    [Test]
    public void BaseElementModel_ShouldHandleEmptyGuid()
    {
        // Arrange & Act
        _model.Key = Guid.Empty;

        // Assert
        _model.Key.Should().Be(Guid.Empty);
    }

    [Test]
    public void BaseElementModel_ShouldHandleEmptyContentTypeAlias()
    {
        // Arrange & Act
        _model.ContentTypeAlias = "";

        // Assert
        _model.ContentTypeAlias.Should().Be("");
    }

    [Test]
    public void BaseElementModel_ShouldAllowSettingBothProperties()
    {
        // Arrange
        var key = Guid.NewGuid();
        var contentTypeAlias = "complexElement";

        // Act
        _model.Key = key;
        _model.ContentTypeAlias = contentTypeAlias;

        // Assert
        _model.Key.Should().Be(key);
        _model.ContentTypeAlias.Should().Be(contentTypeAlias);
    }
}