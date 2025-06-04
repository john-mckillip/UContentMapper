using Microsoft.Extensions.DependencyInjection;
using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Umbraco15.Mapping
{
    /// <summary>
    /// Factory for creating content mappers
    /// </summary>
    public class MapperFactory(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        /// <summary>
        /// Creates a content mapper for the specified model type
        /// </summary>
        public IContentMapper<TModel> CreateMapper<TModel>() where TModel : class
        {
            return _serviceProvider.GetRequiredService<IContentMapper<TModel>>();
        }

        /// <summary>
        /// Creates a content mapper for the specified model type
        /// </summary>
        public IContentMapper<object> CreateMapperForType(Type modelType)
        {
            var mapperType = typeof(IContentMapper<>).MakeGenericType(modelType);
            return (IContentMapper<object>)_serviceProvider.GetRequiredService(mapperType);
        }
    }
}