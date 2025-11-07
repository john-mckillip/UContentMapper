using System.Reflection;
using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Core.Mapping
{
    public class PropertySetter(
        IPropertyValueConverter converter) : IPropertySetter
    {
        private readonly IPropertyValueConverter _converter = converter;

        public void SetPropertyValue<TModel>(TModel model, PropertyInfo property, object? value) where TModel : class
        {
            if (value is not null && _converter.CanConvert(value, property.PropertyType))
            {
                var convertedValue = _converter.ConvertValue(value, property.PropertyType);
                property.SetValue(model, convertedValue);
            }
        }
    }
}