using Microsoft.AspNetCore.Html;
using UContentMapper.Core.Mapping;
using UContentMapper.Umbraco15.Extensions;
using Umbraco.Cms.Core.Strings;

namespace UContentMapper.Umbraco15.Mapping
{
    public class UmbracoPropertyValueConverter : BasicPropertyValueConverter
    {
        public override bool CanConvert(object? value, Type targetType)
        {
            if (value is null) return true;

            var valueType = value.GetType();
            return base.CanConvert(value, targetType) ||
                   (targetType == typeof(IHtmlContent) && valueType.Equals(typeof(HtmlEncodedString)));
        }

        public override object? ConvertValue(object? value, Type targetType)
        {
            if (value is null) return null;

            var valueType = value.GetType();

            // If the value is already the right type, return it
            if (targetType.IsAssignableFrom(valueType))
            {
                return value;
            }

            // Rich text editors
            if (targetType == typeof(IHtmlContent) && valueType.Equals(typeof(HtmlEncodedString)))
            {
                var htmlEncodedString = (HtmlEncodedString)value;
                return htmlEncodedString.ToHtmlContent();
            }

            // Fall back to basic conversions
            return base.ConvertValue(value, targetType);
        }
    }
}