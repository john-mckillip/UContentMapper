using System.Linq.Expressions;
using UContentMapper.Core.Abstractions.Mapping;

namespace UContentMapper.Core.Abstractions.Configuration
{
    /// <summary>
    /// Provides configuration options for individual member mappings
    /// </summary>
    public interface IMemberConfigurationExpression<TSource, TDestination, TMember>
    {
        /// <summary>
        /// Maps from a specific source member
        /// </summary>
        void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember);

        /// <summary>
        /// Maps from an Umbraco property alias
        /// </summary>
        void MapFromProperty(string propertyAlias);

        /// <summary>
        /// Uses a custom value resolver
        /// </summary>
        void ResolveUsing<TResolver>() where TResolver : IValueResolver<TSource, TDestination, TMember>;

        /// <summary>
        /// Uses a custom conversion function
        /// </summary>
        void ConvertUsing(Func<TSource, TMember> converter);

        /// <summary>
        /// Ignores this member during mapping
        /// </summary>
        void Ignore();

        /// <summary>
        /// Sets a constant value for this member
        /// </summary>
        void UseValue(TMember value);

        /// <summary>
        /// Applies a condition for when this mapping should occur
        /// </summary>
        void Condition(Func<TSource, bool> condition);

        /// <summary>
        /// Specifies null value handling
        /// </summary>
        void NullSubstitute(TMember nullValue);
    }
}