namespace UContentMapper.Core.Exceptions
{
    public class PropertyMappingException(
        string message,
        string propertyAlias,
        Type destinationType,
        string memberName) : MappingException(message)
    {
        public string PropertyAlias { get; } = propertyAlias;
        public Type DestinationType { get; } = destinationType;
        public string MemberName { get; } = memberName;
    }
}