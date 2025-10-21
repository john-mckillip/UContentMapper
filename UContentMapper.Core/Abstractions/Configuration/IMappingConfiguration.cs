using System.Reflection;

namespace UContentMapper.Core.Abstractions.Configuration
{
    /// <summary>
    /// Defines the configuration interface for creating and managing object mappings.
    /// </summary>
    /// <remarks>This interface provides methods to create mappings between source and destination types, 
    /// validate the mapping configuration, and add mapping profiles from assemblies.</remarks>
    public interface IMappingConfiguration
    {
        IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
        void ValidateConfiguration();
        IMappingConfiguration AddMappingProfiles(params Assembly[] assemblies);
    }
}