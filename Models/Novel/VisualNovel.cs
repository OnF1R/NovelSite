namespace NovelSite.Models.Novel
{
    public class VisualNovel
    {
        public int Id { get; set; }
        public string? VndbId { get; set; }
        public string LinkName { get; set; }
        public string Title { get; set; }
        public virtual List<string>? AnotherTitles { get; set; }
        public string? CoverImageLink { get; set; }
        public string? BackgroundImageLink { get; set; }
        public virtual List<string>? ScreenshotLinks { get; set; }
        public Status Status { get; set; }
        public int PageViewesCount { get; set; }
        public int CommentsCount { get; set; }
        public virtual List<GamingPlatform>? Platforms { get; set; }
        public ReadingTime ReadingTime { get; set; }
        public virtual List<Translator>? Translator { get; set; }
        public virtual List<Author> Author { get; set; }
        public virtual List<Genre>? Genres { get; set; }
        public virtual List<TagMetadata>? Tags { get; set; }
        public virtual List<Language>? Languages { get; set; }
        public ushort? ReleaseYear { get; set; }
        public ushort? ReleaseMonth { get; set; }
        public ushort? ReleaseDay { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid AdddeUserId { get; set; }
        public string AddedUserName { get; set; }
        public string Description { get; set; }
        public virtual List<RelatedNovel>? RelatedNovels { get; set; }
        public virtual List<DownloadLink>? DownloadLinks { get; set; }
        public virtual List<OtherLink>? OtherLinks { get; set; }
        public virtual List<RelatedAnimeLink>? AnimeLinks { get; set; }
        public double? VndbRating { get; set; }
        public int? VndbVoteCount { get; set; }
        public int? VndbLengthInMinutes { get; set; }
        public string? SteamLink { get; set; }
        public string? TranslateLinkForSteam { get; set; }
        public string? SoundtrackYoutubePlaylistLink { get; set; }
    }

    public enum Status
    {
        Release,
        InDevelopment,
        Abandoned,
        Announced
    }

    public enum ReadingTime
    {
        Any,
        LessTwoHours,
        TwoToTenHours,
        TenToThirtyHours,
        ThirtyToFiftyHours,
        OverFiftyHours,
    }

    public enum SpoilerLevel
    {
        None = 0,
        Minor = 1,
        Major = 2,
    }

    public enum Sort
    {
        DateUpdatedDescending,
        DateUpdatedAscending,

        ReleaseDateDescending,
        ReleaseDateAscending,

        RatingDescending,
        RatingAscending,

        VoteCountDescending,
        VoteCountAscending,

        VNDBRatingDescending,
        VNDBRatingAscending,

        VNDBVoteCountDescending,
        VNDBVoteCountAscending,

        DateAddedDescending,
        DateAddedAscending,

        Title,

    }

    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<VisualNovel> VisualNovel { get; set; }
    }

    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? EnglishName { get; set; }
        public string? Description { get; set; }
        public TagCategory? Category { get; set; }
        public string? VndbId { get; set; }
        public bool? Applicable { get; set; }
    }

    public class TagMetadata
    {
        public Guid Id { get; set; }
        public Tag Tag { get; set; }
        public SpoilerLevel SpoilerLevel { get; set; }
        public VisualNovel VisualNovel { get; set; }
    }

    public class Language
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<VisualNovel> VisualNovel { get; set; }
    }

    public class GamingPlatform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<VisualNovel> VisualNovel { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string? VndbId { get; set; }
        public string Name { get; set; }
        public string? Source { get; set; }
        public virtual List<VisualNovel> VisualNovels { get; set; }
    }

    public class AuthorRequest
    {
        //public int Id { get; set; }
        public string? VndbId { get; set; }
        public string Name { get; set; }
        public string? Source { get; set; }
        //public virtual List<VisualNovel> VisualNovels { get; set; } = new();
    }

    public class Translator
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Source { get; set; }
        public virtual List<VisualNovel> VisualNovels { get; set; } = new();
    }

    public class DownloadLink
    {
        public Guid Id { get; set; }
        public VisualNovel VisualNovel { get; set; }
        public GamingPlatform GamingPlatform { get; set; }
        public string Url { get; set; }
    }

    public class OtherLink
    {
        public Guid Id { get; set; }
        public VisualNovel VisualNovel { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }
}
