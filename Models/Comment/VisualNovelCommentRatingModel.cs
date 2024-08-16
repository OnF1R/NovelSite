namespace NovelSite.Models.Comment
{
    public class VisualNovelCommentRatingModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CommentId { get; set; }
        public bool IsLike { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
