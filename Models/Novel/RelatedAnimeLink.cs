namespace NovelSite.Models.Novel
{
    public class RelatedAnimeLink
    {
        public Guid Id { get; set; }
        public VisualNovel? VisualNovel { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
