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
    public void MapFromAttribute_ShouldStorePropertyAlias()
    {
        // Arrange
        const string propertyAlias = "customProperty";

        // Act
        var attribute = new MapFromAttribute(propertyAlias);

        // Assert
        attribute.PropertyAlias.Should().Be(propertyAlias);
    }

    [Test]
    public void MapFromAttribute_ShouldInitializeWithDefaultRecursiveValue()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute("testProperty");

        // Assert
        attribute.Recursive.Should().BeFalse();
    }

    [Test]
    public void MapFromAttribute_ShouldAllowSettingRecursive()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute("testProperty") { Recursive = true };

        // Assert
        attribute.Recursive.Should().BeTrue();
    }

    [Test]
    public void MapFromAttribute_ShouldHandleEmptyPropertyAlias()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute(string.Empty);

        // Assert
        attribute.PropertyAlias.Should().BeEmpty();
    }

    [Test]
    public void MapFromAttribute_ShouldHandleNullPropertyAlias()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute(null!);

        // Assert
        attribute.PropertyAlias.Should().BeNull();
    }

    [Test]
    public void MapFromAttribute_ShouldBeDecoratedWithAttributeUsageAttribute()
    {
        // Arrange
        var attributeType = typeof(MapFromAttribute);

        // Act
        var usage = (AttributeUsageAttribute)Attribute.GetCustomAttribute(attributeType, typeof(AttributeUsageAttribute))!;

        // Assert
        usage.Should().NotBeNull();
        usage.ValidOn.Should().Be(AttributeTargets.Property);
        usage.AllowMultiple.Should().BeFalse();
        usage.Inherited.Should().BeTrue();
    }

    [Test]
    public void MapFromAttribute_ShouldInheritFromAttribute()
    {
        // Arrange & Act
        var attribute = new MapFromAttribute("test");

        // Assert
        attribute.Should().BeAssignableTo<Attribute>();
    }
}