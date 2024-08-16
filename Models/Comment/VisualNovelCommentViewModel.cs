namespace NovelSite.Models.Comment
{
    public class VisualNovelCommentViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public int VisualNovelId { get; set; }
        public DateTime PostedDate { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsUpdated { get; set; }
        public IEnumerable<VisualNovelCommentRatingModel> Rating { get; set; } = new List<VisualNovelCommentRatingModel>();
        public ICollection<VisualNovelCommentViewModel> Replies { get; set; } = new List<VisualNovelCommentViewModel>();
        public bool HasReplies => Replies.Any();

        public VisualNovelCommentViewModel() { }

        public VisualNovelCommentViewModel(VisualNovelCommentModel comment)
        {
            Id = comment.Id;
            UserId = comment.UserId;
            VisualNovelId = comment.VisualNovelId;
            PostedDate = comment.PostedDate;
            Content = comment.Content;
            ParentCommentId = comment.ParentCommentId;
            IsDeleted = comment.IsDeleted;
            IsUpdated = comment.IsUpdated;
        }

        public VisualNovelCommentViewModel(VisualNovelCommentWithRating model)
        {
            Id = model.Comment.Id;
            UserId = model.Comment.UserId;
            VisualNovelId = model.Comment.VisualNovelId;
            PostedDate = model.Comment.PostedDate;
            Content = model.Comment.Content;
            ParentCommentId = model.Comment.ParentCommentId;
            IsDeleted = model.Comment.IsDeleted;
            IsUpdated = model.Comment.IsUpdated;
            Rating = model.Rating;
    }
    }
}
