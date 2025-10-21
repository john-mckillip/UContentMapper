namespace UContentMapper.Core.Abstractions.Mapping
{
    /// <summary>
    /// Defines a contract for mapping an object to a model of type <typeparamref name="TModel"/>.
    /// </summary>
    /// <typeparam name="TModel">The type of the model to which the source object will be mapped. Must be a reference type.</typeparam>
    public interface IContentMapper<out TModel> where TModel : class
    {
        TModel Map(object source);
        bool CanMap(object source);
    }
}