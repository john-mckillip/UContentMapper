namespace UContentMapper.Core.Models.Metadata
{
    public class PropertyMappingMetadata
    {
        public required string PropertyAlias { get; set; }
        public required string MemberName { get; set; }
        public required Type MemberType { get; set; }
        public bool IsIgnored { get; set; }
        public required Type ValueResolverType { get; set; }
    }
}