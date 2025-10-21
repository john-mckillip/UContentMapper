using System.Linq.Expressions;
using UContentMapper.Core.Abstractions.Configuration;
using UContentMapper.Core.Abstractions.Mapping;
using UContentMapper.Core.Models.Metadata;

namespace UContentMapper.Umbraco15.Configuration
{
    /// <summary>
    /// Umbraco 15 implementation of member configuration expression
    /// </summary>
    public class UmbracoMemberConfigurationExpression<TSource, TMember>(
        TypeMappingMetadata mappingMetadata,
        string memberName,
        Type memberType)
        : IMemberConfigurationExpression<TSource, TMember>
    {
        private readonly TypeMappingMetadata _mappingMetadata = mappingMetadata;
        private readonly string _memberName = memberName;
        private readonly Type _memberType = memberType;

        public void MapFrom<TSourceMember>(Expression<Func<TSource, TSourceMember>> sourceMember)
        {
            // Extract the member name from the expression
            if (sourceMember.Body is MemberExpression memberExpression)
            {
                var sourceMemberName = memberExpression.Member.Name;

                // Find or create the property mapping
                var propertyMapping = FindOrCreatePropertyMapping();
                propertyMapping.PropertyAlias = sourceMemberName;
            }
        }

        public void MapFromProperty(string propertyAlias)
        {
            // Map from an Umbraco property alias
            var propertyMapping = FindOrCreatePropertyMapping();
            propertyMapping.PropertyAlias = propertyAlias;
        }

        public void ResolveUsing<TResolver>() where TResolver : IValueResolver<TSource, TMember>
        {
            // Configure to use a specific resolver
            var propertyMapping = FindOrCreatePropertyMapping();
            propertyMapping.ValueResolverType = typeof(TResolver);
        }

        public void ConvertUsing(Func<TSource, TMember> converter)
        {
            // Store the conversion function
            // This is simplified - in a real implementation, you'd need a way to store and retrieve the function
        }

        public void Ignore()
        {
            // Mark this member as ignored
            var propertyMapping = FindOrCreatePropertyMapping();
            propertyMapping.IsIgnored = true;
        }

        public void UseValue(TMember value)
        {
            // Use a constant value
            // In a real implementation, you'd store this value in the mapping metadata
        }

        public void Condition(Func<TSource, bool> condition)
        {
            // Apply a condition to the mapping
            // In a real implementation, you'd store this condition in the mapping metadata
        }

        public void NullSubstitute(TMember nullValue)
        {
            // Substitute null values with the provided value
            // In a real implementation, you'd store this in the mapping metadata
        }

        private PropertyMappingMetadata FindOrCreatePropertyMapping()
        {
            var existingMapping = _mappingMetadata.PropertyMappings
                .FirstOrDefault(p => p.MemberName == _memberName);

            if (existingMapping != null)
            {
                return existingMapping;
            }

            var newMapping = new PropertyMappingMetadata
            {
                MemberName = _memberName,
                PropertyAlias = _memberName.ToLowerInvariant(), // Default to lowercased member name
                MemberType = _memberType,
                ValueResolverType = typeof(object) // Default, will be replaced when specific resolver is used
            };

            _mappingMetadata.PropertyMappings.Add(newMapping);
            return newMapping;
        }
    }
}