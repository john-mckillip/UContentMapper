namespace UContentMapper.Core.Models.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MapFromAttribute(string propertyAlias) : Attribute
    {
        public string PropertyAlias { get; } = propertyAlias;
        public bool Recursive { get; set; }
    }
}