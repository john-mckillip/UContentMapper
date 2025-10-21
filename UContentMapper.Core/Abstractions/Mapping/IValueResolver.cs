namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IValueResolver<in TSource, in TDestination, out TValue>
    {
        TValue Resolve(TSource source, TDestination destination, string memberName);
    }
}