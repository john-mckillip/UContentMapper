using Microsoft.Extensions.Logging;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Core.Exceptions;
using UContentMapper.Core.Models.Attributes;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace UContentMapper.Umbraco15.Mapping
{
    /// <summary>
    /// Umbraco 15 implementation of content mapper
    /// </summary>
    public class UmbracoContentMapper<TModel>(
        ILogger<UmbracoContentMapper<TModel>> logger,
        IModelPropertyService modelPropertyService,
        IPublishedPropertyMapper<TModel> propertyMapper) : IContentMapper<TModel> where TModel : class
    {
        private readonly ILogger<UmbracoContentMapper<TModel>> _logger = logger;
        private readonly MapperConfigurationAttribute? _attribute = typeof(TModel)
                .GetCustomAttributes(typeof(MapperConfigurationAttribute), true)
                .FirstOrDefault() as MapperConfigurationAttribute;
        private readonly IModelPropertyService _modelPropertyService = modelPropertyService;
        private readonly IPublishedPropertyMapper<TModel> _propertyMapper = propertyMapper;

        public bool CanMap(object source)
        {
            if (source is IPublishedContent content)
            {
                if (string.IsNullOrEmpty(content.ContentType.Alias))
                {
                    return false;
                }

                // If we have a content type alias specified in the attribute, check it matches
                if (_attribute is not null)
                {
                    if (_attribute.SourceType != source.GetType() &&
                        _attribute.SourceType != typeof(IPublishedContent))
                    {
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(_attribute.ContentTypeAlias) &&
                        _attribute.ContentTypeAlias is not "*" &&
                        content.ContentType.Alias != _attribute.ContentTypeAlias)
                    {
                        return false;
                    }
                }

                return true;
            }

            if (source is IPublishedElement element)
            {
                if (string.IsNullOrEmpty(element.ContentType.Alias)) 
                {
                    return false;
                }

                if (_attribute is not null)
                {
                    if (_attribute.SourceType != source.GetType() &&
                        _attribute.SourceType != typeof(IPublishedElement))
                    {
                        return false;
                    }

                    if (!string.IsNullOrWhiteSpace(_attribute.ContentTypeAlias) &&
                        _attribute.ContentTypeAlias is not "*" &&
                        element.ContentType.Alias != _attribute.ContentTypeAlias)
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public TModel Map(object source)
        {
            if (!CanMap(source))
            {
                throw new InvalidOperationException(
                    $"Cannot map object of type {source.GetType().Name} to {typeof(TModel).Name}");
            }

            try
            {
                var model = Activator.CreateInstance<TModel>();
                _propertyMapper.MapProperties(source, model);
                return model;
            }
            catch (Exception ex)
            {
                var sourceType = source.GetType().Name;
                var destinationType = typeof(TModel).Name;

                _logger.LogError(ex, "Error mapping {SourceType} to {DestinationType}",
                    sourceType, destinationType);

                throw new MappingException($"Error mapping {sourceType} to {destinationType}.");
            }
        }
    }
}