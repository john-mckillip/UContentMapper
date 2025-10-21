using System.Linq.Expressions;

namespace UContentMapper.Core.Abstractions.Configuration
{
    /// <summary>
    /// Defines a mapping configuration between a source type and a destination type.
    /// </summary>
    /// <remarks>This interface provides methods to configure mappings between properties of the source and
    /// destination types, customize member mappings, ignore specific members, and define custom conversion
    /// logic.</remarks>
    /// <typeparam name="TSource">The type of the source object to map from.</typeparam>
    /// <typeparam name="TDestination">The type of the destination object to map to.</typeparam>
    public interface IMappingExpression<TSource, TDestination>
    {
        IMappingExpression<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Action<IMemberConfigurationExpression<TSource, TMember>> memberOptions);

        IMappingExpression<TSource, TDestination> Ignore<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember);

        IMappingExpression<TSource, TDestination> MapFromProperty<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            string propertyAlias);

        IMappingExpression<TSource, TDestination> ConvertUsing(Func<TSource, TDestination> converter);

        IMappingExpression<TSource, TDestination> ConvertUsing<TTypeConverter>()
            where TTypeConverter : ITypeConverter<TSource, TDestination>;
    }
}