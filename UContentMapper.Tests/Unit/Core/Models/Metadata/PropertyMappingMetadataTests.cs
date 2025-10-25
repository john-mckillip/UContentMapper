using FluentAssertions;
using UContentMapper.Core.Models.Metadata;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Models.Metadata;

[TestFixture]
public class PropertyMappingMetadataTests : TestBase
{
    [Test]
    public void PropertyMappingMetadata_ShouldSetRequiredProperties()
    {
        // Arrange
        var propertyAlias = "testProperty";
        var memberName = "TestProperty";
        var memberType = typeof(string);
        var valueResolverType = typeof(object);

        // Act
        var metadata = new PropertyMappingMetadata
        {
            PropertyAlias = propertyAlias,
            MemberName = memberName,
            MemberType = memberType,
            ValueResolverType = valueResolverType
        };

        // Assert
        metadata.PropertyAlias.Should().Be(propertyAlias);
        metadata.MemberName.Should().Be(memberName);
        metadata.MemberType.Should().Be(memberType);
        metadata.ValueResolverType.Should().Be(valueResolverType);
        metadata.IsIgnored.Should().BeFalse();
    }

    [Test]
    public void PropertyMappingMetadata_ShouldAllowSettingIsIgnored()
    {
        // Arrange
        var metadata = new PropertyMappingMetadata
        {
            PropertyAlias = "test",
            MemberName = "Test",
            MemberType = typeof(string),
            ValueResolverType = typeof(object),
            IsIgnored = true
        };

        // Assert
        metadata.IsIgnored.Should().BeTrue();
    }

    [Test]
    public void PropertyMappingMetadata_ShouldAllowDifferentTypes()
    {
        // Arrange
        var propertyTypes = new[]
        {
            typeof(int),
            typeof(string),
            typeof(DateTime),
            typeof(bool),
            typeof(Guid),
            typeof(double),
            typeof(object)
        };

        foreach (var type in propertyTypes)
        {
            // Act
            var metadata = new PropertyMappingMetadata
            {
                PropertyAlias = "test",
                MemberName = "Test",
                MemberType = type,
                ValueResolverType = typeof(object)
            };

            // Assert
            metadata.MemberType.Should().Be(type);
        }
    }

    [Test]
    public void PropertyMappingMetadata_ShouldAllowEmptyStrings()
    {
        // Arrange & Act
        var metadata = new PropertyMappingMetadata
        {
            PropertyAlias = string.Empty,
            MemberName = string.Empty,
            MemberType = typeof(string),
            ValueResolverType = typeof(object)
        };

        // Assert
        metadata.PropertyAlias.Should().BeEmpty();
        metadata.MemberName.Should().BeEmpty();
    }

    [Test]
    public void PropertyMappingMetadata_ShouldSupportInheritedTypes()
    {
        // Arrange & Act
        var metadata = new PropertyMappingMetadata
        {
            PropertyAlias = "test",
            MemberName = "Test",
            MemberType = typeof(TestDerivedClass),
            ValueResolverType = typeof(TestDerivedResolver)
        };

        // Assert
        metadata.MemberType.Should().Be(typeof(TestDerivedClass));
        metadata.ValueResolverType.Should().Be(typeof(TestDerivedResolver));
    }

    private class TestBaseClass { }
    private class TestDerivedClass : TestBaseClass { }
    private class TestBaseResolver { }
    private class TestDerivedResolver : TestBaseResolver { }
}