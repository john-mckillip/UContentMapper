using System.Reflection;

namespace UContentMapper.Core.Abstractions.Mapping
{
    /// <summary>
    /// Defines a method for setting the value of a specified property on a given model instance.
    /// </summary>
    /// <remarks>This interface is designed to provide a generic mechanism for assigning values to properties
    /// of objects dynamically. It is particularly useful in scenarios involving reflection or dynamic property
    /// manipulation.</remarks>
    public interface IPropertySetter
    {
        /// <summary>
        /// Sets the specified value to the model property.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="model"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        void SetPropertyValue<TModel>(TModel model, PropertyInfo property, object? value) where TModel : class;
    }
}