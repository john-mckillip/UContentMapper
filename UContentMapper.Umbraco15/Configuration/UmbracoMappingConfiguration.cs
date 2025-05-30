using Microsoft.Extensions.Logging;
using System.Reflection;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Configuration;
using UContentMapper.Core.Configuration.Profiles;
using UContentMapper.Core.Models.Metadata;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Umbraco15.Configuration
{
    /// <summary>
    /// Umbraco 15 specific implementation of mapping configuration
    /// </summary>
    public class UmbracoMappingConfiguration(ILogger<UmbracoMappingConfiguration> logger) : MappingConfigurationBase
    {
        private readonly ILogger<UmbracoMappingConfiguration> _logger = logger;

        public override IMappingExpression<TSource, TDestination> CreateMap<TSource, TDestination>()
        {
            var typePair = new TypePair(typeof(TSource), typeof(TDestination));

            // Create a new type mapping if it doesn't exist
            if (!TypeMaps.TryGetValue(typePair, out _))
            {
                var metadata = new TypeMappingMetadata
                {
                    SourceType = typeof(TSource),
                    DestinationType = typeof(TDestination),
                    ContentTypeAlias = typeof(TSource) == typeof(IPublishedContent) ? "*" : string.Empty
                };

                TypeMaps[typePair] = metadata;
                _logger.LogDebug("Created mapping from {SourceType} to {DestinationType}",
                    typeof(TSource).Name, typeof(TDestination).Name);
            }

            return new UmbracoMappingExpression<TSource, TDestination>(TypeMaps[typePair]);
        }

        public override IMappingConfiguration AddMappingProfiles(params Assembly[] assemblies)
        {
            var profileType = typeof(MappingProfile);

            foreach (var assembly in assemblies)
            {
                var profileTypes = assembly.GetTypes()
                    .Where(t => !t.IsAbstract && !t.IsInterface && profileType.IsAssignableFrom(t));

                foreach (var type in profileTypes)
                {
                    if (Activator.CreateInstance(type) is MappingProfile profile)
                    {
                        profile.Initialize(this);
                        _logger.LogInformation("Added mapping profile: {ProfileName}", type.Name);
                    }
                }
            }

            return this;
        }

        public override void ValidateConfiguration()
        {
            // Validate all the type mappings
            foreach (var (typePair, metadata) in TypeMaps)
            {
                _logger.LogDebug("Validating mapping: {SourceType} -> {DestinationType}",
                    typePair.SourceType.Name, typePair.DestinationType.Name);

                // Check for missing properties/mappings
                var destProperties = typePair.DestinationType.GetProperties()
                    .Where(p => p.CanWrite && p.GetIndexParameters().Length == 0);

                foreach (var prop in destProperties)
                {
                    var hasMappingDefined = metadata.PropertyMappings.Any(m => m.MemberName == prop.Name);

                    if (!hasMappingDefined && !prop.PropertyType.IsValueType && prop.PropertyType != typeof(string))
                    {
                        _logger.LogWarning("No mapping defined for complex property {PropertyName} on {DestinationType}",
                            prop.Name, typePair.DestinationType.Name);
                    }
                }
            }
        }
    }
}