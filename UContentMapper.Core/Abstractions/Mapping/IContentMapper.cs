namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IContentMapper<out TModel> where TModel : class
    {
        TModel Map(object source);
        bool CanMap(object source);
    }
}