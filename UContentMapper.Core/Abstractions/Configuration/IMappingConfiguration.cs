using System.Reflection;

namespace UContentMapper.Core.Abstractions.Configuration
{
    public interface IMappingConfiguration
    {
        IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>();
        void ValidateConfiguration();
        IMappingConfiguration AddMappingProfiles(params Assembly[] assemblies);
    }
}