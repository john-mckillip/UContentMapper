namespace UContentMapper.Core.Models.Content
{
    /// <summary>
    /// Image model
    /// </summary>
    public class ImageModel
    {
        public required string Src { get; set; }
        public required string Alt { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}