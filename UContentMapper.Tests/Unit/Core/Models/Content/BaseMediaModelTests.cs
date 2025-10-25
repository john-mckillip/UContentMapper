using FluentAssertions;
using UContentMapper.Core.Models.Content;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Models.Content;

[TestFixture]
public class BaseMediaModelTests : TestBase
{
    private BaseMediaModel _model;

  [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _model = new BaseMediaModel
        {
            Name = "test.jpg",
            Url = "/media/test.jpg",
            Extension = ".jpg"
        };
    }

    [Test]
    public void BaseMediaModel_ShouldInitializeWithRequiredProperties()
    {
        // Arrange
        var name = "test.jpg";
        var url = "/media/test.jpg";
        var extension = ".jpg";

        // Act
        var model = new BaseMediaModel
        {
            Name = name,
            Url = url,
            Extension = extension
        };

        // Assert
        model.Name.Should().Be(name);
        model.Url.Should().Be(url);
        model.Extension.Should().Be(extension);
        model.Id.Should().Be(0);
        model.Key.Should().Be(Guid.Empty);
        model.Bytes.Should().Be(0);
        model.Width.Should().BeNull();
        model.Height.Should().BeNull();
    }

    [Test]
    public void BaseMediaModel_ShouldAllowSettingAllProperties()
    {
        // Arrange
        var id = 1001;
        var key = Guid.NewGuid();
        var name = "image.png";
        var url = "/media/image.png";
        var extension = ".png";
        var bytes = 1024;
        var width = 800;
        var height = 600;

        // Act
        _model.Id = id;
        _model.Key = key;
        _model.Name = name;
        _model.Url = url;
        _model.Extension = extension;
        _model.Bytes = bytes;
        _model.Width = width;
        _model.Height = height;

        // Assert
        _model.Id.Should().Be(id);
        _model.Key.Should().Be(key);
        _model.Name.Should().Be(name);
        _model.Url.Should().Be(url);
        _model.Extension.Should().Be(extension);
        _model.Bytes.Should().Be(bytes);
        _model.Width.Should().Be(width);
        _model.Height.Should().Be(height);
    }

    [Test]
    public void BaseMediaModel_ShouldAllowNullDimensions()
    {
        // Act
        _model.Width = null;
        _model.Height = null;

        // Assert
        _model.Width.Should().BeNull();
        _model.Height.Should().BeNull();
    }

    [Test]
    public void BaseMediaModel_ShouldHandleNegativeValues()
    {
        // Act
        _model.Id = -1;
        _model.Bytes = -1;
        _model.Width = -1;
        _model.Height = -1;

        // Assert
        _model.Id.Should().Be(-1);
        _model.Bytes.Should().Be(-1);
        _model.Width.Should().Be(-1);
        _model.Height.Should().Be(-1);
    }

    [Test]
    public void BaseMediaModel_ShouldHandleZeroValues()
    {
        // Act
        _model.Id = 0;
        _model.Bytes = 0;
        _model.Width = 0;
        _model.Height = 0;

        // Assert
        _model.Id.Should().Be(0);
        _model.Bytes.Should().Be(0);
        _model.Width.Should().Be(0);
        _model.Height.Should().Be(0);
    }

    [Test]
    public void BaseMediaModel_ShouldHandleEmptyStrings()
    {
        // Act
        _model.Name = string.Empty;
        _model.Url = string.Empty;
        _model.Extension = string.Empty;

        // Assert
        _model.Name.Should().BeEmpty();
        _model.Url.Should().BeEmpty();
        _model.Extension.Should().BeEmpty();
    }
}