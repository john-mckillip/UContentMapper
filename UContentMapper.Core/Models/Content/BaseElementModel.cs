namespace UContentMapper.Core.Models.Content
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