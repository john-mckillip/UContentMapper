using FluentAssertions;
using UContentMapper.Core.Models.Content;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Models.Content;

[TestFixture]
public class BaseSeoModelTests : TestBase
{
    private TestSeoModel _model;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _model = new TestSeoModel();
    }

    [Test]
    public void BaseSeoModel_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var model = new TestSeoModel();

        // Assert
        model.MetaTitle.Should().BeNull();
        model.MetaDescription.Should().BeNull();
        model.MetaKeywords.Should().BeNull();
        model.OgTitle.Should().BeNull();
        model.OgDescription.Should().BeNull();
        model.OgImage.Should().BeNull();
        model.NoIndex.Should().BeFalse();
    }

    [Test]
    public void BaseSeoModel_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var metaTitle = "Test Title";
        var metaDescription = "Test Description";
        var metaKeywords = "test,keywords";
        var ogTitle = "OG Title";
        var ogDescription = "OG Description";
        var ogImage = new ImageModel { Src = "/test.jpg", Alt = "Test" };
        var noIndex = true;

        // Act
        _model.MetaTitle = metaTitle;
        _model.MetaDescription = metaDescription;
        _model.MetaKeywords = metaKeywords;
        _model.OgTitle = ogTitle;
        _model.OgDescription = ogDescription;
        _model.OgImage = ogImage;
        _model.NoIndex = noIndex;

        // Assert
        _model.MetaTitle.Should().Be(metaTitle);
        _model.MetaDescription.Should().Be(metaDescription);
        _model.MetaKeywords.Should().Be(metaKeywords);
        _model.OgTitle.Should().Be(ogTitle);
        _model.OgDescription.Should().Be(ogDescription);
        _model.OgImage.Should().Be(ogImage);
        _model.NoIndex.Should().Be(noIndex);
    }

    [Test]
    public void BaseSeoModel_ShouldAllowNullValues()
    {
        // Act
        _model.MetaTitle = null;
        _model.MetaDescription = null;
        _model.MetaKeywords = null;
        _model.OgTitle = null;
        _model.OgDescription = null;
        _model.OgImage = null;

        // Assert
        _model.MetaTitle.Should().BeNull();
        _model.MetaDescription.Should().BeNull();
        _model.MetaKeywords.Should().BeNull();
        _model.OgTitle.Should().BeNull();
        _model.OgDescription.Should().BeNull();
        _model.OgImage.Should().BeNull();
    }

    [Test]
    public void BaseSeoModel_ShouldAllowEmptyStrings()
    {
        // Act
        _model.MetaTitle = string.Empty;
        _model.MetaDescription = string.Empty;
        _model.MetaKeywords = string.Empty;
        _model.OgTitle = string.Empty;
        _model.OgDescription = string.Empty;

        // Assert
        _model.MetaTitle.Should().BeEmpty();
        _model.MetaDescription.Should().BeEmpty();
        _model.MetaKeywords.Should().BeEmpty();
        _model.OgTitle.Should().BeEmpty();
        _model.OgDescription.Should().BeEmpty();
    }

    [Test]
    public void BaseSeoModel_ShouldInheritBaseContentModelProperties()
    {
        // Arrange
        var name = "Test Page";
        var url = "/test-page";
        var key = Guid.NewGuid();

        // Act
        _model.Name = name;
        _model.Url = url;
        _model.Key = key;

        // Assert
        _model.Name.Should().Be(name);
        _model.Url.Should().Be(url);
        _model.Key.Should().Be(key);
    }

    [Test]
    public void BaseSeoModel_OgImageShouldAllowFullConfiguration()
    {
        // Arrange
        var ogImage = new ImageModel
        {
            Src = "/test.jpg",
            Alt = "Test Image",
            Width = 1200,
            Height = 630
        };

        // Act
        _model.OgImage = ogImage;

        // Assert
        _model.OgImage.Should().NotBeNull();
        _model.OgImage!.Src.Should().Be(ogImage.Src);
        _model.OgImage.Alt.Should().Be(ogImage.Alt);
        _model.OgImage.Width.Should().Be(ogImage.Width);
        _model.OgImage.Height.Should().Be(ogImage.Height);
    }

    /// <summary>
    /// Test implementation of BaseSeoModel for testing
    /// </summary>
    private class TestSeoModel : BaseSeoModel
    {
        // This class is needed because BaseSeoModel is abstract
    }
}