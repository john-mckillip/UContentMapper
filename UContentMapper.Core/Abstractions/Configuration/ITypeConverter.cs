namespace UContentMapper.Core.Abstractions.Configuration
{
    public interface ITypeConverter<in TSource, out TDestination>
    {
        TDestination Convert(TSource source);
    }
}