namespace UContentMapper.Core.Abstractions.Configuration
{
    /// <summary>
    /// Defines a mechanism for converting an object of type <typeparamref name="TSource"/>  to an object of type
    /// <typeparamref name="TDestination"/>.
    /// </summary>
    /// <typeparam name="TSource">The type of the source object to be converted.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object resulting from the conversion.</typeparam>
    public interface ITypeConverter<in TSource, out TDestination>
    {
        TDestination Convert(TSource source);
    }
}