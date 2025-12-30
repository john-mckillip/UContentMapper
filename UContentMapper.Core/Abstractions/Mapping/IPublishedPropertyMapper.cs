namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IPublishedPropertyMapper<in TModel> where TModel : class
    {
        void MapProperties(object source, TModel destination);
        bool IsBuiltInProperty(string propertyName);
    }
}