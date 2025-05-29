namespace UContentMapper.Core.Models.Content
{
    /// <summary>
    /// Base media model
    /// </summary>
    public class BaseMediaModel
    {
        public int Id { get; set; }
        public Guid Key { get; set; }
        public required string Name { get; set; }
        public required string Url { get; set; }
        public required string Extension { get; set; }
        public int Bytes { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
    }
}