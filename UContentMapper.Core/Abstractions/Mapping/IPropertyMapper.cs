namespace UContentMapper.Core.Abstractions.Mapping
{
    /// <summary>
    /// Defines methods for mapping property values to a specified destination type.
    /// </summary>
    /// <remarks>This interface is designed to facilitate the conversion of property values, identified by
    /// their alias, into a specified destination type. Implementations of this interface can be used to handle custom
    /// property mapping logic in scenarios such as data transformation or object serialization.</remarks>
    public interface IPropertyMapper
    {
        bool CanMapProperty(string propertyAlias, Type destinationType);
        object MapProperty(string propertyAlias, object value, Type destinationType);
    }
}