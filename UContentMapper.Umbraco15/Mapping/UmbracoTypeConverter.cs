using UContentMapper.Core.Abstractions.Configuration;

namespace UContentMapper.Umbraco15.Mapping
{
    public abstract class UmbracoTypeConverter<TSource, TDestination> : ITypeConverter<TSource, TDestination>
    {
        public abstract TDestination Convert(TSource source);
    }
}