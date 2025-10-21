using System.Linq.Expressions;

namespace UContentMapper.Core.Abstractions.Configuration
{
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