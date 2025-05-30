using Microsoft.Extensions.DependencyInjection;
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
            services.AddSingleton<IMappingConfiguration, UmbracoMappingConfiguration>();

            // Register generic mappers
            services.AddTransient(typeof(IContentMapper<>), typeof(UmbracoContentMapper<>));

            // Register common type converters
            services.AddTransient<PublishedContentToUrlConverter>();
            services.AddTransient<PublishedContentToMediaWithCropsConverter>();
            services.AddTransient<MediaWithCropsToUrlConverter>();

            // Register value resolvers
            services.AddTransient<PublishedContentUrlResolver>();
            services.AddTransient(typeof(MediaPropertyResolver<>));
            services.AddTransient<MediaItemResolver>();

            // Configure the mapping profile from the Umbraco15 assembly
            services.AddSingleton<UmbracoMappingProfile>();
            services.AddSingleton(sp => {
                var config = sp.GetRequiredService<IMappingConfiguration>();
                config.AddMappingProfiles(typeof(UmbracoMappingProfile).Assembly);
                return config;
            });

            return services;
        }
    }
}