using System.Reflection;

namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IModelPropertyService
    {
        List<PropertyInfo> GetProperties<T>(T model) where T : class;
    }
}