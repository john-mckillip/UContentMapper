using FluentAssertions;
using System.Globalization;
using UContentMapper.Core.Configuration;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Configuration;

[TestFixture]
public class MapperConfigurationOptionsTests : TestBase
{
    private MapperConfigurationOptions _options;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _options = new MapperConfigurationOptions();
    }

    [Test]
    public void Constructor_ShouldInitializeDefaultValues()
    {
        // Arrange & Act
        var options = new MapperConfigurationOptions();

        // Assert
        options.EnableAttributeMapping.Should().BeTrue();
        options.EnablePropertyCache.Should().BeTrue();
        options.AutoMapUnmatchedProperties.Should().BeTrue();
        options.DefaultCulture.Should().Be(CultureInfo.CurrentCulture);
    }

    [Test]
    public void EnableAttributeMapping_ShouldAllowModification()
    {
        // Act
        _options.EnableAttributeMapping = false;

        // Assert
        _options.EnableAttributeMapping.Should().BeFalse();
    }

    [Test]
    public void EnablePropertyCache_ShouldAllowModification()
    {
        // Act
        _options.EnablePropertyCache = false;

        // Assert
        _options.EnablePropertyCache.Should().BeFalse();
    }

    [Test]
    public void AutoMapUnmatchedProperties_ShouldAllowModification()
    {
        // Act
        _options.AutoMapUnmatchedProperties = false;

        // Assert
        _options.AutoMapUnmatchedProperties.Should().BeFalse();
    }

    [Test]
    public void DefaultCulture_ShouldAllowModification()
    {
        // Arrange
        var invariantCulture = CultureInfo.InvariantCulture;

        // Act
        _options.DefaultCulture = invariantCulture;

        // Assert
        _options.DefaultCulture.Should().Be(invariantCulture);
    }

    [Test]
    public void DefaultCulture_ShouldAllowSettingToNull()
    {
        // Act
        _options.DefaultCulture = null!;

        // Assert
        _options.DefaultCulture.Should().BeNull();
    }

    [Test]
    public void MultiplePropertiesModification_ShouldMaintainIndependentValues()
    {
        // Arrange
        var invariantCulture = CultureInfo.InvariantCulture;

        // Act
        _options.EnableAttributeMapping = false;
        _options.EnablePropertyCache = true;
        _options.AutoMapUnmatchedProperties = false;
        _options.DefaultCulture = invariantCulture;

        // Assert
        _options.EnableAttributeMapping.Should().BeFalse();
        _options.EnablePropertyCache.Should().BeTrue();
        _options.AutoMapUnmatchedProperties.Should().BeFalse();
        _options.DefaultCulture.Should().Be(invariantCulture);
    }
}