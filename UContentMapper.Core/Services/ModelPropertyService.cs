using System.Collections.Concurrent;
using System.Reflection;
using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Core.Services
{
    public class ModelPropertyService : IModelPropertyService
    {
        // Thread-safe cache of property info lists keyed by type
        private static readonly ConcurrentDictionary<Type, List<PropertyInfo>> _propertyCache = new();

        public List<PropertyInfo> GetProperties<T>(T model) where T : class
        {
            var modelType = typeof(T);

            // Get properties from cache or compute if not present
            return _propertyCache.GetOrAdd(modelType, type =>
                [.. type.GetProperties().Where(p => p.CanWrite && p.GetIndexParameters().Length == 0)]);
        }
    }
}