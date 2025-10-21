namespace UContentMapper.Core.Abstractions.Mapping
{
    /// <summary>
    /// Defines a mechanism for resolving a value of type <typeparamref name="TValue"/> from a source object of type
    /// <typeparamref name="TSource"/>.
    /// </summary>
    /// <remarks>Implement this interface to provide custom logic for extracting or transforming data from a
    /// source object into a specific value.</remarks>
    /// <typeparam name="TSource">The type of the source object from which the value is resolved.</typeparam>
    /// <typeparam name="TValue">The type of the value to be resolved.</typeparam>
    public interface IValueResolver<in TSource, out TValue>
    {
        TValue Resolve(TSource source);
    }
}