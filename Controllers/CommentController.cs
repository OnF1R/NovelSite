using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovelSite.Data.Identity;
using NovelSite.Models.Comment;
using NovelSite.Services.Extensions;
using NovelSite.Services.Queries;

namespace NovelSite.Controllers
{
    public class CommentController : Controller
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        public CommentController(UserManager<ApplicationIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int visualNovelId)
        {
            var comments = await CommentQueries.GetVisualNovelComments(visualNovelId);

            ViewBag.VisualNovelId = visualNovelId;

            if (comments == null)
            {
                return PartialView("_CommentMainPartial", null);
            }

            var rootComments = comments
                .Where(c => c.Comment.ParentCommentId == null)
                .Select(c => c.ToViewModel(comments))
                .ToList();


            return PartialView("_CommentMainPartial", rootComments);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(VisualNovelCommentModel model)
        {
            model.ParentCommentId = null;

            var comment = await CommentQueries.Add(model);

            var commentViewModel = new VisualNovelCommentViewModel()
            {
                Id = comment.Id,
                ParentCommentId = comment.ParentCommentId,
                IsDeleted = comment.IsDeleted,
                PostedDate = comment.PostedDate,
                Content = comment.Content,
                UserId = comment.UserId,
                VisualNovelId = comment.VisualNovelId,
            };

            ViewBag.VisualNovelId = comment.VisualNovelId;

            return PartialView("_CommentPartial", new List<VisualNovelCommentViewModel> { commentViewModel });
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reply(VisualNovelCommentModel model)
        {
            try
            {
                var comment = await CommentQueries.Add(model);

                var commentViewModel = new VisualNovelCommentViewModel()
                {
                    Id = comment.Id,
                    ParentCommentId = comment.ParentCommentId,
                    IsDeleted = comment.IsDeleted,
                    PostedDate = comment.PostedDate,
                    Content = comment.Content,
                    UserId = comment.UserId,
                    VisualNovelId = comment.VisualNovelId,
                };

                ViewBag.VisualNovelId = comment.VisualNovelId;

                return PartialView("_CommentPartial", new List<VisualNovelCommentViewModel> { commentViewModel });
            }
            catch (Exception)
            {
                
                throw;
            }
        }

        [Authorize]
        [HttpPut]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VisualNovelCommentModel model)
        {
            var comment = await CommentQueries.Update(model);

            var commentViewModel = new VisualNovelCommentViewModel()
            {
                Id = comment.Id,
                ParentCommentId = comment.ParentCommentId,
                IsDeleted = comment.IsDeleted,
                PostedDate = comment.PostedDate,
                Content = comment.Content,
                UserId = comment.UserId,
                VisualNovelId = comment.VisualNovelId,
                IsUpdated = comment.IsUpdated,
            };

            ViewBag.VisualNovelId = comment.VisualNovelId;

            return PartialView("_CommentPartial", new List<VisualNovelCommentViewModel> { commentViewModel });
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete(Guid id)
        {
            var comment = await CommentQueries.Delete(id);

            var commentViewModel = new VisualNovelCommentViewModel()
            {
                Id = comment.Id,
                ParentCommentId = comment.ParentCommentId,
                IsDeleted = comment.IsDeleted,
                PostedDate = comment.PostedDate,
                Content = comment.Content,
                UserId = comment.UserId,
                VisualNovelId = comment.VisualNovelId,
            };

            ViewBag.VisualNovelId = comment.VisualNovelId;

            return PartialView("_CommentPartial", new List<VisualNovelCommentViewModel> { commentViewModel });
        }

        [HttpGet]
        public async Task<VisualNovelCommentRatingModel> GetRating(Guid userId, Guid commentId)
        {
            var rating = await CommentQueries.GetRating(userId, commentId);

            return rating;
        }

        [Authorize]
        [HttpPost]
        public async Task AddRating(Guid userId, Guid commentId, bool isLike)
        {
            await CommentQueries.AddRating(userId, commentId, isLike);
        }

        [Authorize]
        [HttpDelete]
        public async Task RemoveRating(Guid userId, Guid commentId)
        {
            var rating = await CommentQueries.GetRating(userId, commentId);
            await CommentQueries.DeleteRating(rating.Id);
        }

        [Authorize]
        [HttpPut]
        public async Task UpdateRating(Guid userId, Guid commentId, bool isLike)
        {
            var rating = await CommentQueries.GetRating(userId, commentId);
            await CommentQueries.UpdateRating(rating.Id, userId, commentId, isLike);
        }
    }
}
