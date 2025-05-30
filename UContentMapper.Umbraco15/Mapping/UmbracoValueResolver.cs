using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Umbraco15.Mapping
{
    public abstract class UmbracoValueResolver<TSource, TDestination, TValue>
        : IValueResolver<TSource, TDestination, TValue>
    {
        public abstract TValue Resolve(TSource source, TDestination destination, string memberName);
    }
}