namespace UContentMapper.Core.Models.Metadata
{
    /// <summary>
    /// Represents metadata for mapping between a source type and a destination type,  including associated property
    /// mappings and a content type alias.
    /// </summary>
    /// <remarks>This class is used to define the relationship between two types for mapping purposes, 
    /// including the specific properties to map and an alias that identifies the content type.</remarks>
    public class TypeMappingMetadata
    {
        public required Type SourceType { get; set; }
        public required Type DestinationType { get; set; }
        public required string ContentTypeAlias { get; set; }
        public IList<PropertyMappingMetadata> PropertyMappings { get; set; } = [];
    }
}