namespace UContentMapper.Umbraco15.Models
{
    /// <summary>
    /// Base model for elements/compositions
    /// </summary>
    public abstract class BaseElementModel
    {
        public Guid Key { get; set; }
        public required string ContentTypeAlias { get; set; }
    }
}