namespace UContentMapper.Core.Models.Metadata
{
    /// <summary>
    /// Represents metadata for mapping a property, including its alias, member name, type, and additional configuration
    /// details.
    /// </summary>
    /// <remarks>This class is typically used to define mappings between source and target properties in
    /// scenarios such as object mapping or data transformation. It includes information about the property's alias, its
    /// corresponding member name, type, and whether it should be ignored during processing.</remarks>
    public class PropertyMappingMetadata
    {
        public required string PropertyAlias { get; set; }
        public required string MemberName { get; set; }
        public required Type MemberType { get; set; }
        public bool IsIgnored { get; set; }
        public required Type ValueResolverType { get; set; }
    }
}