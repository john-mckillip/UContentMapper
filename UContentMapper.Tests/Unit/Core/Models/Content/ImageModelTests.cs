using FluentAssertions;
using UContentMapper.Core.Models.Content;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Models.Content;

[TestFixture]
public class ImageModelTests : TestBase
{
    private ImageModel _model;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _model = new ImageModel
        {
            Src = "/media/test.jpg",
            Alt = "Test Image"
        };
    }

    [Test]
    public void ImageModel_ShouldInitializeWithRequiredProperties()
    {
        // Arrange
        var src = "/media/image.jpg";
        var alt = "Test Image";

        // Act
        var model = new ImageModel
        {
            Src = src,
            Alt = alt
        };

        // Assert
        model.Src.Should().Be(src);
        model.Alt.Should().Be(alt);
        model.Width.Should().BeNull();
        model.Height.Should().BeNull();
    }

    [Test]
    public void ImageModel_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var src = "/media/new.jpg";
        var alt = "New Image";
        var width = 1024;
        var height = 768;

        // Act
        _model.Src = src;
        _model.Alt = alt;
        _model.Width = width;
        _model.Height = height;

        // Assert
        _model.Src.Should().Be(src);
        _model.Alt.Should().Be(alt);
        _model.Width.Should().Be(width);
        _model.Height.Should().Be(height);
    }

    [Test]
    public void ImageModel_ShouldAllowNullDimensions()
    {
        // Act
        _model.Width = null;
        _model.Height = null;

        // Assert
        _model.Width.Should().BeNull();
        _model.Height.Should().BeNull();
    }

    [Test]
    public void ImageModel_ShouldHandleEmptyStrings()
    {
        // Act
        _model.Src = string.Empty;
        _model.Alt = string.Empty;

        // Assert
        _model.Src.Should().BeEmpty();
        _model.Alt.Should().BeEmpty();
    }

    [Test]
    public void ImageModel_ShouldHandleZeroDimensions()
    {
        // Act
        _model.Width = 0;
        _model.Height = 0;

        // Assert
        _model.Width.Should().Be(0);
        _model.Height.Should().Be(0);
    }

    [Test]
    public void ImageModel_ShouldHandleNegativeDimensions()
    {
        // Act
        _model.Width = -1;
        _model.Height = -1;

        // Assert
        _model.Width.Should().Be(-1);
        _model.Height.Should().Be(-1);
    }

    [Test]
    public void ImageModel_ShouldHandleLargeDimensions()
    {
        // Act
        _model.Width = int.MaxValue;
        _model.Height = int.MaxValue;

        // Assert
        _model.Width.Should().Be(int.MaxValue);
        _model.Height.Should().Be(int.MaxValue);
    }
}