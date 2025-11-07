using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Core.Mapping
{
    public class BasicPropertyValueConverter : IPropertyValueConverter
    {
        public virtual bool CanConvert(object? value, Type targetType)
        {
            if (value is null) return true;

            var valueType = value.GetType();
            return targetType.IsAssignableFrom(valueType) ||
                   targetType == typeof(string) ||
                   targetType == typeof(short) ||
                   targetType == typeof(int) ||
                   targetType == typeof(bool) ||
                   targetType == typeof(DateTime) ||
                   targetType == typeof(Guid);
        }

        public virtual object? ConvertValue(object? value, Type targetType)
        {
            if (value is null)
            {
                return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
            }

            var valueType = value.GetType();

            // If the value is already the right type, return it
            if (targetType.IsAssignableFrom(valueType))
            {
                return value;
            }

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

            return value;
        }
    }
}