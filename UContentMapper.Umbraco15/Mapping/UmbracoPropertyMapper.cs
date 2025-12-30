using Microsoft.Extensions.Logging;
using System.Reflection;
using UContentMapper.Core.Abstractions.Mapping;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
    public class UmbracoPropertyMapper<TModel>(
        ILogger<UmbracoPropertyMapper<TModel>> logger,
        IModelPropertyService modelPropertyService,
        IPropertyValueConverter converter) : IPublishedPropertyMapper<TModel> where TModel : class
    {
        private readonly ILogger<UmbracoPropertyMapper<TModel>> _logger = logger;
        private readonly IModelPropertyService _modelPropertyService = modelPropertyService;
        private readonly IPropertyValueConverter _converter = converter;

        public void MapProperties(object source, TModel destination)
        {
            var modelProperties = _modelPropertyService.GetProperties(destination);

            switch (source)
            {
                case IPublishedContent content:
                    _mapPublishedContentProperties(content, destination, modelProperties);
                    break;
                case IPublishedElement element:
                    _mapPublishedElementProperties(element, destination, modelProperties);
                    break;
                default:
                    throw new ArgumentException($"Unsupported source type: {source.GetType().Name}", nameof(source));
            }
        }

        public bool IsBuiltInProperty(string propertyName) => propertyName switch
        {
            "Id" or "Key" or "Name" or "ContentTypeAlias" or "Url" or
            "AbsoluteUrl" or "CreateDate" or "UpdateDate" or "Level" or
            "SortOrder" or "TemplateId" or "IsVisible" => true,
            _ => false
        };

        #region Helper Methods

        private void _mapPublishedContentProperties(IPublishedContent content, TModel model, List<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                try
                {
                    var propertyAlias = property.Name.ToLowerInvariant();

                    // Try to map built-in properties first
                    if (IsBuiltInProperty(property.Name))
                    {
                        _mapBuiltInPublishedContentProperty(content, model, property);
                        continue;
                    }

                    // Try to map from published content property
                    if (content.HasProperty(propertyAlias))
                    {
                        var value = content.GetProperty(propertyAlias)?.GetValue() ?? null;
                        if (value is not null)
                        {
                            _setPropertyValue(model, property, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error mapping property {PropertyName} for {ContentType}",
                        property.Name, content.ContentType.Alias);
                }
            }
        }

        private void _mapPublishedElementProperties(IPublishedElement element, TModel model, List<PropertyInfo> properties)
        {
            foreach (var property in properties)
            {
                try
                {
                    var propertyAlias = property.Name.ToLowerInvariant();

                    // Try to map built-in properties first
                    if (IsBuiltInProperty(property.Name))
                    {
                        _mapBuiltInPublishedElementProperty(element, model, property);
                        continue;
                    }

                    if (element.HasProperty(propertyAlias))
                    {
                        var value = element.GetProperty(propertyAlias)?.GetValue() ?? null;
                        if (value is not null)
                        {
                            _setPropertyValue(model, property, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error mapping property {PropertyName} for {ContentType}",
                        property.Name, element.ContentType.Alias);
                }
            }
        }

        private void _mapBuiltInPublishedElementProperty(IPublishedElement element, TModel model,  PropertyInfo property)
        {
            var propertyName = property.Name;

            object? value = propertyName switch
            {
                "Key" => element.Key,
                "ContentTypeAlias" => element.ContentType.Alias,
                _ => null
            };

            if (value is not null)
            {
                _setPropertyValue(model, property, value);
            }
        }

        private void _mapBuiltInPublishedContentProperty(IPublishedContent content, TModel model, PropertyInfo property)
        {
            var propertyName = property.Name;

            object? value = propertyName switch
            {
                "Id" => content.Id,
                "Key" => content.Key,
                "Name" => content.Name,
                "ContentTypeAlias" => content.ContentType.Alias,
                "Url" => content.Url(),
                "AbsoluteUrl" => content.Url(mode: UrlMode.Absolute),
                "CreateDate" => content.CreateDate,
                "UpdateDate" => content.UpdateDate,
                "Level" => content.Level,
                "SortOrder" => content.SortOrder,
                "TemplateId" => content.TemplateId,
                "IsVisible" => content.IsVisible(),
                _ => null
            };

            if (value is not null)
            {
                _setPropertyValue(model, property, value);
            }
        }

        private void _setPropertyValue(TModel model, PropertyInfo property, object? value)
        {
            if (value is not null)
            {
                var convertedValue = _converter.ConvertValue(value, property.PropertyType);
                property.SetValue(model, convertedValue);
            }
        }

        #endregion
    }
}