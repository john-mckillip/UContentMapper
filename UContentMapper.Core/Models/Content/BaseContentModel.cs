namespace UContentMapper.Core.Models.Content
{
    /// <summary>
    /// Base model that all content models inherit from
    /// </summary>
    public abstract class BaseContentModel
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContentTypeAlias { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string AbsoluteUrl { get; set; } = string.Empty;
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Level { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
        public int? TemplateId { get; set; }
    }
}