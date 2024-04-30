namespace NovelSite.Models
{
    public class VisualNovelRequest
    {
        //public int Id { get; set; }
        public string Title { get; set; }
        public string? OriginalTitle { get; set; }
        public string? VndbId { get; set; }
        //[Required(ErrorMessage = "Не указан рейтинг визуальной новеллы")]
        //[Range(1, 5)]
        //public float Rating { get; set; }
        public IFormFile? CoverImage { get; set; } = null;
        public IFormFile? BackgroundImage { get; set; } = null;
        public List<IFormFile>? Screenshots { get; set; } = null;
        //public int PageViewesCount { get; set; }
        //public int CommentsCount { get; set; }
        public Status Status { get; set; }
        public List<int>? Platforms { get; set; }
        public ReadingTime ReadingTime { get; set; }
        public int? Translator { get; set; }
        public List<int>? Authors { get; set; }
        //public int? AuthorId { get; set; }
        public List<int>? Genres { get; set; }
        public List<int>? NoneSpoilerTags { get; set; }
        public List<int>? MinorSpoilerTags { get; set; }
        public List<int>? MajorSpoilerTags { get; set; }
        public List<int>? Languages { get; set; }
        public int ReleaseYear { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid AdddeUserId { get; set; }
        public string AddedUserName { get; set; }
        public string Description { get; set; }
        public List <int>? DownloadLinkGamingPlatformId { get; set; }
        public List<string>? DownloadLinkUrl { get; set; }
        public List<string>? OtherLinkName { get; set; }
        public List<string>? OtherLinkUrl { get; set; }
        public string? SteamLink { get; set; }
        public string? TranslateLinkForSteam { get; set; }
    }
}
