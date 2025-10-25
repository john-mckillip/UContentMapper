namespace UContentMapper.Core.Abstractions.Mapping
{
    /// <summary>
    /// Defines a factory for creating content mappers that map data to specific model types.
    /// </summary>
    /// <remarks>This interface provides methods to create strongly-typed content mappers for a given model
    /// type. It supports both generic and non-generic approaches to accommodate various use cases.</remarks>
    public interface IContentMapperFactory
    {
        /// <summary>
        /// Creates and returns an instance of a content mapper for the specified model type.
        /// </summary>
        /// <typeparam name="TModel">The type of the model that the content mapper will handle. Must be a reference type.</typeparam>
        /// <returns>An instance of <see cref="IContentMapper{TModel}"/> configured for the specified model type.</returns>
        IContentMapper<TModel> CreateMapper<TModel>() where TModel : class;
        /// <summary>
        /// Creates an instance of an <see cref="IContentMapper{T}"/> for the specified model type.
        /// </summary>
        /// <param name="modelType">The type of the model for which the content mapper is created. This parameter cannot be <see
        /// langword="null"/>.</param>
        /// <returns>An <see cref="IContentMapper{T}"/> instance capable of mapping content for the specified model type.</returns>
        IContentMapper<object> CreateMapperForType(Type modelType);
    }
}