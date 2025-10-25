using Microsoft.Extensions.DependencyInjection;
using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Umbraco15.Mapping
{
    public class ContentMapperFactory(
        IServiceProvider serviceProvider) : IContentMapperFactory
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public IContentMapper<TModel> CreateMapper<TModel>() where TModel : class
        {
            return _serviceProvider.GetRequiredService<IContentMapper<TModel>>();
        }

        public IContentMapper<object> CreateMapperForType(Type modelType)
        {
            var mapperType = typeof(IContentMapper<>).MakeGenericType(modelType);
            return (IContentMapper<object>)_serviceProvider.GetRequiredService(mapperType);
        }
    }
}