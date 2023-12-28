namespace NovelSite.Models
{
    public class ImageRequest
    {
        public Guid Id { get; set; }
        public IFormFile CoveImage { get; set; }
    }
}
