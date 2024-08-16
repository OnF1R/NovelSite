namespace NovelSite.Models.Comment
{
    public class VisualNovelCommentModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int VisualNovelId { get; set; }
        public DateTime PostedDate { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }
    }

    public class VisualNovelCommentWithRating
    {
        public VisualNovelCommentModel Comment { get; set; }
        public IEnumerable<VisualNovelCommentRatingModel> Rating { get; set; }
    }
}
