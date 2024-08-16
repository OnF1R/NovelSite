using NovelSite.Models.Comment;

namespace NovelSite.Services.Extensions
{
    public static class VisualNovelCommentExtensions
    {
        public static VisualNovelCommentViewModel ToViewModel(this VisualNovelCommentModel comment, IEnumerable<VisualNovelCommentModel> allComments)
        {
            var viewModel = new VisualNovelCommentViewModel(comment);
            viewModel.Replies = allComments
                .Where(c => c.ParentCommentId == comment.Id)
                .Select(c => c.ToViewModel(allComments))
                .ToList();
            return viewModel;
        }

        public static VisualNovelCommentViewModel ToViewModel(this VisualNovelCommentWithRating comment, IEnumerable<VisualNovelCommentWithRating> allComments)
        {
            var viewModel = new VisualNovelCommentViewModel(comment);
            viewModel.Replies = allComments
                .Where(c => c.Comment.ParentCommentId == comment.Comment.Id)
                .Select(c => c.ToViewModel(allComments))
                .ToList();
            return viewModel;
        }
    }
}
