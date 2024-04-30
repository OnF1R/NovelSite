namespace NovelSite.Models
{
    public class VisualNovelWithRating
    {
        public VisualNovel VisualNovel { get; set; }
        public double AverageRating { get; set; }
        public int NumberOfRatings { get; set; }
    }

    public class VisualNovelWithRatingResult : PaginationParams
    {
        public List<VisualNovelWithRating> Result { get; set; }
    }

    public class PaginationParams
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
