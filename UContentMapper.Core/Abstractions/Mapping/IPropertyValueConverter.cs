namespace UContentMapper.Core.Abstractions.Mapping
{
    /// <summary>
    /// Defines methods for converting property values to a specified target type.
    /// </summary>
    /// <remarks>This interface is typically used to implement custom value conversion logic for scenarios
    /// where property values need to be transformed or validated before being used in a specific context.</remarks>
    public interface IPropertyValueConverter
    {
        /// <summary>
        /// Converts the specified value to the specified target type.
        /// </summary>
        /// <param name="value">The value to be converted. Can be <see langword="null"/>.</param>
        /// <param name="targetType">The type to which the value should be converted. Must not be <see langword="null"/>.</param>
        /// <returns>The converted value as an object of the specified target type, or <see langword="null"/> if the input value
        /// is <see langword="null"/>.</returns>
        object? ConvertValue(object? value, Type targetType);
        /// <summary>
        /// Determines whether the specified value can be converted to the specified target type.
        /// </summary>
        /// <param name="value">The value to evaluate for conversion. Can be <see langword="null"/>.</param>
        /// <param name="targetType">The type to which the value is being evaluated for conversion. Cannot be <see langword="null"/>.</param>
        /// <returns><see langword="true"/> if the value can be converted to the specified target type; otherwise, <see
        /// langword="false"/>.</returns>
        bool CanConvert(object? value, Type targetType);
    }
}