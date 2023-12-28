namespace NovelSite.Models
{
    public class VisualNovel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? OriginalTitle { get; set; }
        public byte[]? CoverImage { get; set; } = null;

        //public int PageViewesCount { get; set; }
        //public int CommentsCount { get; set; }

        public virtual List<GamingPlatform> Platforms { get; set; }

        public ReadingTime ReadingTime { get; set; }

        //public Translator? Translator { get; set; }
        public string? Translator { get; set; }
        //public Autor Autor { get; set; }
        public string Autor { get; set; }

        public virtual List<Genre> Genres { get; set; }
        public virtual List<TagMetadata> Tags { get; set; }

        public virtual List<Language> Languages { get; set; }

        public int ReleaseYear { get; set; }

        public DateTime DateAdded { get; set; }
        public DateTime DateUpdated { get; set; }

        public Guid AdddeUserId { get; set; }
        public string AddedUserName { get; set; }

        public string Description { get; set; }

        //public DownloadLink[] Links { get; set; }
    }

    public enum ReadingTime
    {
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
        public string Description { get; set; }
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

    public class Autor
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? AutorSource { get; set; }
    }

    public class Translator
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? TranslatorSource { get; set; }
    }

    
}
