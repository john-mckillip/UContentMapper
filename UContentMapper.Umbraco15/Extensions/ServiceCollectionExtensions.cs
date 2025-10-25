using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Umbraco15.Configuration;
using UContentMapper.Umbraco15.Mapping;

namespace UContentMapper.Umbraco15.Extensions
{
    /// <summary>
    /// Extension methods for configuring UContentMapper services
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds UContentMapper services for Umbraco 15
        /// </summary>
        public static IServiceCollection AddUContentMapper(this IServiceCollection services)
        {
            // Register the main mapping configuration
            services.TryAddSingleton<IMappingConfiguration, UmbracoMappingConfiguration>();

            // Register generic mappers
            services.TryAddTransient(typeof(IContentMapper<>), typeof(UmbracoContentMapper<>));
            services.TryAddTransient<IContentMapperFactory, ContentMapperFactory>();

            // Register common type converters
            services.TryAddTransient<PublishedContentToUrlConverter>();
            services.TryAddTransient<MediaWithCropsToUrlConverter>();

            // Register value resolvers
            services.TryAddTransient<PublishedContentUrlResolver>();
            services.TryAddTransient(typeof(MediaPropertyResolver<>));

            // Configure the mapping profile from the Umbraco15 assembly
            services.TryAddSingleton(serviceProvider =>
            {
                var config = serviceProvider.GetRequiredService<IMappingConfiguration>();
                var profile = new UmbracoMappingProfile();
                profile.Initialize(config);
                return profile;
            });

            return services;
        }
    }
}