using System.Linq.Expressions;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Models.Metadata;

namespace UContentMapper.Umbraco15.Configuration
{
    /// <summary>
    /// Umbraco 15 implementation of mapping expression
    /// </summary>
    public class UmbracoMappingExpression<TSource, TDestination>(TypeMappingMetadata mappingMetadata) : IMappingExpression<TSource, TDestination>
    {
        private readonly TypeMappingMetadata _mappingMetadata = mappingMetadata;

        public IMappingExpression<TSource, TDestination> ForMember<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            Action<IMemberConfigurationExpression<TSource, TMember>> memberOptions)
        {
            if (destinationMember.Body is MemberExpression memberExpression)
            {
                var memberName = memberExpression.Member.Name;
                var memberType = typeof(TMember);

                // Create a new configuration expression for this member
                var memberConfigExpression = new UmbracoMemberConfigurationExpression<TSource, TMember>(
                    _mappingMetadata, memberName, memberType);

                // Apply the configuration options
                memberOptions(memberConfigExpression);
            }

            return this;
        }

        public IMappingExpression<TSource, TDestination> Ignore<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember)
        {
            return ForMember(destinationMember, opt => opt.Ignore());
        }

        public IMappingExpression<TSource, TDestination> MapFromProperty<TMember>(
            Expression<Func<TDestination, TMember>> destinationMember,
            string propertyAlias)
        {
            return ForMember(destinationMember, opt => opt.MapFromProperty(propertyAlias));
        }

        public IMappingExpression<TSource, TDestination> ConvertUsing(Func<TSource, TDestination> converter)
        {
            return this;
        }

        public IMappingExpression<TSource, TDestination> ConvertUsing<TTypeConverter>()
            where TTypeConverter : ITypeConverter<TSource, TDestination>
        {
            // Register the type converter for this mapping
            return this;
        }
    }
}