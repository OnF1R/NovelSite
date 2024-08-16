using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovelSite.Data.Identity;
using NovelSite.Models.Novel;
using NovelSite.Services.Queries;
using System.ComponentModel.DataAnnotations;

namespace NovelSite.Controllers
{
    public class NovelConfirmationController : Controller
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        public NovelConfirmationController(UserManager<ApplicationIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [Authorize]
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckLinkName(string linkName)
        {
            var vn = await HttpQueries.GetVisualNovel(linkName); //TODO Метод, для проверки сущестования такой новеллы (true, false)

            if (vn == null)
            {
                return Json(true);
            }

            return Json(false);
        }

        [Authorize]
        [AcceptVerbs("Post")]
        public async Task<IActionResult> CheckFile(IFormFile CoverImage)
        {
            List<string> extensions = new List<string>() { ".png", ".jpg", ".jpeg", ".gif" };

            long maxFileSize = 5 * 1024 * 1024;

            var fileName = CoverImage.FileName.ToLower();

            if (CoverImage.Length <= maxFileSize && extensions.Any(ext => fileName.EndsWith(ext)))
            {
                return Json(true);
            }

            await Task.CompletedTask;

            return Json(false);
        }

        [Authorize]
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> CheckFiles(List<IFormFile> screenshots)
        {
            List<string> extensions = new List<string>() { ".png", ".jpg", ".jpeg", ".gif" };

            long maxFileSize = 5 * 1024 * 1024;

            int successCount = 0;

            foreach (var file in screenshots)
            {
                var fileName = file.FileName.ToLower();

                if (file.Length <= maxFileSize && extensions.Any(ext => fileName.EndsWith(ext)))
                {
                    successCount++;
                }
            }

            return successCount == screenshots.Count ? Json(true) : Json(false);
        }
    }
}
