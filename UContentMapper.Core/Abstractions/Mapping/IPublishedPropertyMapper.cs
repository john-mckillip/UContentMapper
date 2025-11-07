namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IPublishedPropertyMapper<TModel> where TModel : class
    {
        void MapProperties(object source, TModel destination);
        bool IsBuiltInProperty(string propertyName);
    }
}