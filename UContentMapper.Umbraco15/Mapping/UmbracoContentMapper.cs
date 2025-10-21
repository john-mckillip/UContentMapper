using Microsoft.Extensions.Logging;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Core.Exceptions;
using UContentMapper.Core.Models.Attributes;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Extensions;

namespace UContentMapper.Umbraco15.Mapping
{
	/// <summary>
	/// Umbraco 15 implementation of content mapper
	/// </summary>
	public class UmbracoContentMapper<TModel> : IContentMapper<TModel> where TModel : class
	{
		private readonly ILogger<UmbracoContentMapper<TModel>> _logger;
		private readonly MapperConfigurationAttribute? _attribute;

		public UmbracoContentMapper(
			ILogger<UmbracoContentMapper<TModel>> logger)
		{
			_logger = logger;
			_attribute = typeof(TModel)
				.GetCustomAttributes(typeof(MapperConfigurationAttribute), true)
				.FirstOrDefault() as MapperConfigurationAttribute;
		}

		public bool CanMap(object source)
		{
			if (source is not IPublishedContent content)
			{
				return false;
			}

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
					_attribute.ContentTypeAlias != "*" &&
					content.ContentType.Alias != _attribute.ContentTypeAlias)
				{
					return false;
				}
			}

			return true;
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
				if (source is IPublishedContent content)
				{
					// Create a new instance of the model
					var model = Activator.CreateInstance<TModel>();

					// Map all properties from the source to the destination
					_mapProperties(content, model);

					return model;
				}

				throw new InvalidOperationException(
					$"Source object type {source.GetType().Name} is not supported for mapping");
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

		private void _mapProperties(IPublishedContent content, TModel model)
		{
			var modelProperties = typeof(TModel)
				.GetProperties()
				.Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)
				.ToList();

			foreach (var property in modelProperties)
			{
				try
				{
					// Try to get the property value based on property naming convention
					var propertyAlias = property.Name.ToLowerInvariant();

					// Try to map built-in properties first
					if (_mapBuiltInProperty(content, model, property))
					{
						continue;
					}

					// Try to map from published content property
					if (content.HasProperty(propertyAlias))
					{
						var value = content.GetProperty(propertyAlias)?.GetValue() ?? null;
						if (value is not null)
						{
							// Convert the value to the property type if needed
							var convertedValue = _convertValue(value, property.PropertyType);
							property.SetValue(model, convertedValue);
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

		private static bool _mapBuiltInProperty(IPublishedContent content, TModel model, System.Reflection.PropertyInfo property)
		{
			var propertyName = property.Name;

			// Map built-in properties
			switch (propertyName)
			{
				case "Id":
					_trySetValue(model, property, content.Id);
					return true;
				case "Key":
					_trySetValue(model, property, content.Key);
					return true;
				case "Name":
					_trySetValue(model, property, content.Name);
					return true;
				case "ContentTypeAlias":
					_trySetValue(model, property, content.ContentType.Alias);
					return true;
				case "Url":
					_trySetValue(model, property, content.Url());
					return true;
				case "AbsoluteUrl":
					_trySetValue(model, property, content.Url(mode: UrlMode.Absolute));
					return true;
				case "CreateDate":
					_trySetValue(model, property, content.CreateDate);
					return true;
				case "UpdateDate":
					_trySetValue(model, property, content.UpdateDate);
					return true;
				case "Level":
					_trySetValue(model, property, content.Level);
					return true;
				case "SortOrder":
					_trySetValue(model, property, content.SortOrder);
					return true;
				case "TemplateId":
					_trySetValue(model, property, content.TemplateId);
					return true;
				case "IsVisible":
					_trySetValue(model, property, content.IsVisible());
					return true;
			}

			return false;
		}

		private static void _trySetValue(TModel model, System.Reflection.PropertyInfo property, object? value)
		{
			if (value is not null)
			{
				var convertedValue = _convertValue(value, property.PropertyType);
				property.SetValue(model, convertedValue);
			}
		}

		private static object? _convertValue(object? value, Type targetType)
		{
			if (value is null)
			{
				return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
			}

			var valueType = value.GetType();

			// Handle common built-in type conversions
			if (targetType == typeof(string))
			{
				return value.ToString();
			}

			if (targetType == typeof(short) && short.TryParse(value.ToString(), out var shortValue))
			{
				return shortValue;
			}

			if (targetType == typeof(int) && int.TryParse(value.ToString(), out var intValue))
			{
				return intValue;
			}

			if (targetType == typeof(bool) && bool.TryParse(value.ToString(), out var boolValue))
			{
				return boolValue;
			}

			if (targetType == typeof(DateTime) && value is DateTime dateValue)
			{
				return dateValue;
			}

			if (targetType == typeof(Guid) && Guid.TryParse(value.ToString(), out var guidValue))
			{
				return guidValue;
			}

			// If the value is already the right type, return it
			if (targetType.IsAssignableFrom(valueType))
			{
				return value;
			}

			// For complex types, we might need to do more sophisticated mapping here
			// This would involve recursively mapping objects or handling collections

			return value;
		}
	}
}