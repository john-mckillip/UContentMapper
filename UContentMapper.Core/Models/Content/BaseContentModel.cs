namespace UContentMapper.Core.Models.Content
{
    /// <summary>
    /// Represents the base model for content items, providing common properties shared across content types.
    /// </summary>
    /// <remarks>This abstract class serves as a foundation for content models, encapsulating metadata such as
    /// identifiers, URLs, creation and update timestamps, and visibility status. Derived classes can extend this model
    /// to include additional properties specific to their content type.</remarks>
    public abstract class BaseContentModel
    {
        /// <summary>
        /// Gets or sets the the Id of the content entity.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Gets or sets the unique identifier for the content entity.
        /// </summary>
        public Guid Key { get; set; }
        /// <summary>
        /// Gets or sets the name of the content entity.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the content type alias of the content entity.
        /// </summary>
        public string ContentTypeAlias { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the Url of the content entity. 
        /// </summary>
        public string Url { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the absolute URL of the content entity.
        /// </summary>
        public string AbsoluteUrl { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the date the content entity was created.
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// Gets or sets the date and time when the content entity was last updated.
        /// </summary>
        public DateTime UpdateDate { get; set; }
        /// <summary>
        /// Gets or sets the level of the entity in the content tree.
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Gets or sets the sort order of the content entity.
        /// </summary>
        public int SortOrder { get; set; }
        /// <summary>
        /// Gets or sets a bool value for if the content entity is visible.
        /// </summary>
        public bool IsVisible { get; set; }
        /// <summary>
        /// Gets or sets the template Id of the content entity.
        /// </summary>
        public int? TemplateId { get; set; }
    }
}