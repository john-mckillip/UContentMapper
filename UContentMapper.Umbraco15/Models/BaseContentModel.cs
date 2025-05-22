namespace UContentMapper.Umbraco15.Models
{
    /// <summary>
    /// Base model that all content models inherit from
    /// </summary>
    public abstract class BaseContentModel
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public required string Name { get; set; }
        public required string ContentTypeAlias { get; set; }
        public required string Url { get; set; }
        public required string AbsoluteUrl { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }
        public int Level { get; set; }
        public int SortOrder { get; set; }
        public bool IsVisible { get; set; }
        public int? TemplateId { get; set; }
    }
}