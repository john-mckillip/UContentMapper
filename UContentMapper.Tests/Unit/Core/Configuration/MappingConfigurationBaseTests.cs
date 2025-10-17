using FluentAssertions;
using System.Reflection;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Configuration;
using UContentMapper.Core.Models.Metadata;
using UContentMapper.Tests.TestHelpers;

namespace UContentMapper.Tests.Unit.Core.Configuration;

[TestFixture]
public class MappingConfigurationBaseTests : TestBase
{
    private TestableMappingConfiguration _configuration;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _configuration = new TestableMappingConfiguration();
    }

    [Test]
    public void MappingConfigurationBase_ShouldInitializeEmptyTypeMaps()
    {
        // Arrange & Act
        var configuration = new TestableMappingConfiguration();

        // Assert
        configuration.GetTypeMaps().Should().NotBeNull();
        configuration.GetTypeMaps().Should().BeEmpty();
    }

    [Test]
    public void CreateMap_ShouldBeAbstractMethod()
    {
        // Arrange
        var method = typeof(MappingConfigurationBase).GetMethod(nameof(MappingConfigurationBase.CreateMap));

        // Act & Assert
        method.Should().NotBeNull();
        method!.IsAbstract.Should().BeTrue();
    }

    [Test]
    public void ValidateConfiguration_ShouldBeAbstractMethod()
    {
        // Arrange
        var method = typeof(MappingConfigurationBase).GetMethod(nameof(MappingConfigurationBase.ValidateConfiguration));

        // Act & Assert
        method.Should().NotBeNull();
        method!.IsAbstract.Should().BeTrue();
    }

    [Test]
    public void AddMappingProfiles_ShouldBeAbstractMethod()
    {
        // Arrange
        var method = typeof(MappingConfigurationBase).GetMethod(nameof(MappingConfigurationBase.AddMappingProfiles));

        // Act & Assert
        method.Should().NotBeNull();
        method!.IsAbstract.Should().BeTrue();
    }

    [Test]
    public void TypeMaps_ShouldBeAccessibleToImplementations()
    {
        // Arrange
        var typePair = new TypePair(typeof(string), typeof(int));
        var metadata = new TypeMappingMetadata
        {
            SourceType = typeof(string),
            DestinationType = typeof(int),
            ContentTypeAlias = string.Empty
        };

        // Act
        _configuration.AddTypeMap(typePair, metadata);

        // Assert
        _configuration.GetTypeMaps().Should().ContainKey(typePair);
        _configuration.GetTypeMaps()[typePair].Should().Be(metadata);
    }

    [Test]
    public void TypeMaps_ShouldAllowMultipleEntries()
    {
        // Arrange
        var typePair1 = new TypePair(typeof(string), typeof(int));
        var typePair2 = new TypePair(typeof(double), typeof(bool));
        var metadata1 = new TypeMappingMetadata { SourceType = typeof(string), DestinationType = typeof(int), ContentTypeAlias = string.Empty };
        var metadata2 = new TypeMappingMetadata { SourceType = typeof(double), DestinationType = typeof(bool), ContentTypeAlias = string.Empty };

        // Act
        _configuration.AddTypeMap(typePair1, metadata1);
        _configuration.AddTypeMap(typePair2, metadata2);

        // Assert
        _configuration.GetTypeMaps().Should().HaveCount(2);
        _configuration.GetTypeMaps().Should().ContainKey(typePair1);
        _configuration.GetTypeMaps().Should().ContainKey(typePair2);
    }

    [Test]
    public void TypeMaps_ShouldAllowOverwritingExistingEntry()
    {
        // Arrange
        var typePair = new TypePair(typeof(string), typeof(int));
        var metadata1 = new TypeMappingMetadata { SourceType = typeof(string), DestinationType = typeof(int), ContentTypeAlias = string.Empty };
        var metadata2 = new TypeMappingMetadata { SourceType = typeof(string), DestinationType = typeof(int), ContentTypeAlias = string.Empty };

        // Act
        _configuration.AddTypeMap(typePair, metadata1);
        _configuration.AddTypeMap(typePair, metadata2);

        // Assert
        _configuration.GetTypeMaps().Should().HaveCount(1);
        _configuration.GetTypeMaps()[typePair].Should().Be(metadata2);
    }

    [Test]
    public void TypeMaps_ShouldHandleNullMetadata()
    {
        // Arrange
        var typePair = new TypePair(typeof(string), typeof(int));

        // Act
        _configuration.AddTypeMap(typePair, null!);

        // Assert
        _configuration.GetTypeMaps().Should().ContainKey(typePair);
        _configuration.GetTypeMaps()[typePair].Should().BeNull();
    }

    /// <summary>
    /// Testable implementation of MappingConfigurationBase for testing purposes
    /// </summary>
    private class TestableMappingConfiguration : MappingConfigurationBase
    {
        public IDictionary<TypePair, TypeMappingMetadata> GetTypeMaps() => TypeMaps;

        public void AddTypeMap(TypePair typePair, TypeMappingMetadata metadata)
        {
            TypeMaps[typePair] = metadata;
        }

        public override IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            throw new NotImplementedException("This is a test implementation");
        }

        public override void ValidateConfiguration()
        {
            throw new NotImplementedException("This is a test implementation");
        }

        public override IMappingConfiguration AddMappingProfiles(params Assembly[] assemblies)
        {
            throw new NotImplementedException("This is a test implementation");
        }
    }
}