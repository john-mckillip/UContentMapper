namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IPropertyMapper
    {
        bool CanMapProperty(string propertyAlias, Type destinationType);
        object MapProperty(string propertyAlias, object value, Type destinationType);
    }
}