namespace UContentMapper.Core.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MapperConfigurationAttribute : Attribute
    {
        public required Type SourceType { get; set; }
        public required string ContentTypeAlias { get; set; }
    }
}