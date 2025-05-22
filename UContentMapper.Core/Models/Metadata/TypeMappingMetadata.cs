namespace UContentMapper.Core.Models.Metadata
{
    public class TypeMappingMetadata
    {
        public required Type SourceType { get; set; }
        public required Type DestinationType { get; set; }
        public required string ContentTypeAlias { get; set; }
        public IList<PropertyMappingMetadata> PropertyMappings { get; set; } = new List<PropertyMappingMetadata>();
    }
}