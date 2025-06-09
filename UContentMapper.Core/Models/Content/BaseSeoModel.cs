namespace UContentMapper.Core.Models.Content
{
    /// <summary>
    /// Base SEO model for content that has SEO properties
    /// </summary>
    public abstract class BaseSeoModel : BaseContentModel
    {
        public string? MetaTitle { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeywords { get; set; }
        public string? OgTitle { get; set; }
        public string? OgDescription { get; set; }
        public ImageModel? OgImage { get; set; }
        public bool NoIndex { get; set; }
    }
}