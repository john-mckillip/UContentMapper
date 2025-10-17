using System.Text.Json.Serialization;

namespace UContentMapper.Core.Exceptions
{
    public class PropertyMappingException : MappingException
    {
        private readonly Type? _destinationType;

        [JsonConstructor]
        public PropertyMappingException(
            string message,
            string propertyAlias,
            string destinationTypeName,
            string memberName) : base(message)
        {
            PropertyAlias = propertyAlias;
            DestinationTypeName = destinationTypeName;
            MemberName = memberName;
        }

        public PropertyMappingException(
            string message,
            string propertyAlias,
            Type destinationType,
            string memberName) : base(message)
        {
            PropertyAlias = propertyAlias;
            _destinationType = destinationType;
            DestinationTypeName = destinationType?.FullName ?? string.Empty;
            MemberName = memberName;
        }

        public string PropertyAlias { get; }

        [JsonIgnore]
        public Type? DestinationType => _destinationType;

        public string DestinationTypeName { get; }

        public string MemberName { get; }
    }
}