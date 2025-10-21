namespace UContentMapper.Core.Abstractions.Mapping
{
    public interface IValueResolver<in TSource, out TValue>
    {
        TValue Resolve(TSource source);
    }
}