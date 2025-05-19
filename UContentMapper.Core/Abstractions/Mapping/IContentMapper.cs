namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IContentMapper<TModel> where TModel : class
    {
        TModel Map(object source);
        bool CanMap(object source);
    }
}