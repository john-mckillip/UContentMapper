using System.Reflection;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Models.Metadata;

namespace UContentMapper.Core.Configuration
{
    public abstract class MappingConfigurationBase : IMappingConfiguration
    {
        protected readonly IDictionary<TypePair, TypeMappingMetadata> TypeMaps = new Dictionary<TypePair, TypeMappingMetadata>();

        public abstract IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
        public abstract void ValidateConfiguration();
        public abstract IMappingConfiguration AddMappingProfiles(params Assembly[] assemblies);
    }
}