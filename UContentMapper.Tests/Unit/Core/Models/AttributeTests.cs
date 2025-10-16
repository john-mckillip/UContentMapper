using FluentAssertions;
using UContentMapper.Core.Models.Attributes;
using UContentMapper.Tests.TestHelpers;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Tests.Unit.Core.Models;

[TestFixture]
public class AttributeTests : TestBase
{
    [Test]
    public void MapperConfigurationAttribute_ShouldAllowSettingSourceType()
    {
        // Arrange & Act
        var attribute = new MapperConfigurationAttribute
        {
            SourceType = typeof(IPublishedContent),
            ContentTypeAlias = "testPage"
        };

        // Assert
        attribute.SourceType.Should().Be(typeof(IPublishedContent));
    }

    [Test]
    public void MapperConfigurationAttribute_ShouldAllowSettingContentTypeAlias()
    {
        // Arrange & Act
        var attribute = new MapperConfigurationAttribute
        {
            SourceType = typeof(IPublishedContent),
            ContentTypeAlias = "testPage"
        };

        // Assert
        attribute.ContentTypeAlias.Should().Be("testPage");
    }

    [Test]
    public void MapperConfigurationAttribute_ShouldAllowWildcardContentTypeAlias()
    {
        // Arrange & Act
        var attribute = new MapperConfigurationAttribute
        {
            SourceType = typeof(IPublishedContent),
            ContentTypeAlias = "*"
        };

        // Assert
        attribute.ContentTypeAlias.Should().Be("*");
    }

    [Test]
    public void MapperConfigurationAttribute_ShouldAllowEmptyContentTypeAlias()
    {
        // Arrange & Act
        var attribute = new MapperConfigurationAttribute
        {
            SourceType = typeof(IPublishedContent),
            ContentTypeAlias = ""
        };

        // Assert
        attribute.ContentTypeAlias.Should().Be("");
    }

    [Test]
    public void MapperConfigurationAttribute_ShouldWorkWithDifferentSourceTypes()
    {
        // Arrange & Act
        var attribute = new MapperConfigurationAttribute
        {
            SourceType = typeof(string),
            ContentTypeAlias = "test"
        };

        // Assert
        attribute.SourceType.Should().Be(typeof(string));
    }

    [Test]
    public void IgnoreMapAttribute_ShouldBeCreatable()
    {
        // Arrange & Act
        var attribute = new IgnoreMapAttribute();

        // Assert
        attribute.Should().NotBeNull();
        attribute.Should().BeOfType<IgnoreMapAttribute>();
    }

    [Test]
    public void MapFromAttribute_ShouldAllowSettingPropertyName()
    {
        // Arrange
        var propertyName = "customPropertyName";

        // Act
        var attribute = new MapFromAttribute(propertyName);

        // Assert
        attribute.PropertyName.Should().Be(propertyName);
    }

    [Test]
    public void MapFromAttribute_ShouldHandleEmptyPropertyName()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute("");

        // Assert
        attribute.PropertyName.Should().Be("");
    }

    [Test]
    public void MapFromAttribute_ShouldHandleNullPropertyName()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute(null!);

        // Assert
        attribute.PropertyName.Should().BeNull();
    }

    [Test]
    public void AttributeUsage_MapperConfigurationAttribute_ShouldOnlyAllowClass()
    {
        // Arrange
        var attributeType = typeof(MapperConfigurationAttribute);

        // Act
        var attributeUsage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(attributeType, typeof(AttributeUsageAttribute))!;

        // Assert
        attributeUsage.Should().NotBeNull();
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Class);
    }

    [Test]
    public void AttributeUsage_IgnoreMapAttribute_ShouldOnlyAllowProperty()
    {
        // Arrange
        var attributeType = typeof(IgnoreMapAttribute);

        // Act
        var attributeUsage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(attributeType, typeof(AttributeUsageAttribute))!;

        // Assert
        attributeUsage.Should().NotBeNull();
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Property);
    }

    [Test]
    public void AttributeUsage_MapFromAttribute_ShouldOnlyAllowProperty()
    {
        // Arrange
        var attributeType = typeof(MapFromAttribute);

        // Act
        var attributeUsage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(attributeType, typeof(AttributeUsageAttribute))!;

        // Assert
        attributeUsage.Should().NotBeNull();
        attributeUsage.ValidOn.Should().Be(AttributeTargets.Property);
    }
}