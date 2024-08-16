using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovelSite.Data.Identity;
using NovelSite.Models.Novel;
using NovelSite.Services.Queries;
using NovelSite.Services.S3.Interfaces;
using System.Collections.Generic;

namespace NovelSite.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        private readonly IFileService _fileService;
        public UserController(UserManager<ApplicationIdentityUser> userManager, IFileService fileService)
        {
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task GetUserVisualNovelLists(string userId)
        {
            var currentUserId = _userManager.GetUserId(User);

            bool isOwner = currentUserId != null && currentUserId == userId;

            if (isOwner)
            {
                await HttpQueries.GetVisualNovelLists(userId, isOwner);
            }
            else
            {
                await HttpQueries.GetVisualNovelLists(userId, isOwner);
            }
        }

        [Authorize]
        public async Task<IActionResult> UpdateUserAvatar(IFormFile file)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            int indexLastDot = file.FileName.LastIndexOf('.');
            int fileExtensionsNameLength = file.FileName.Length - indexLastDot;
            string fileExtensionName = file.FileName.Substring(indexLastDot + 1, fileExtensionsNameLength - 1);
            string fileName = $"{currentUser.UserName}.{fileExtensionName}";

            string avatarUrl = await _fileService.UploadFileAsync(file, currentUser.UserName, fileName);

            currentUser.AvatarFileName = avatarUrl;

            var updateResult = await _userManager.UpdateAsync(currentUser);

            return RedirectToAction("Index", "Profile", new { username = currentUser.UserName });
        }

        public async Task<IActionResult> CreateCustomList(string userId, string listName, bool isPrivate)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null && currentUser.Id == userId)
            {
                var list = new VisualNovelList()
                {
                    IsCustom = true,
                    ListType = null,
                    UserId = userId,
                    VisualNovelListEntries = new List<VisualNovelListEntry>(),
                    IsPrivate = isPrivate,

                    Name = listName,
                };

                var addedList = await HttpQueries.CreateCustomList(userId, list);

                return RedirectToAction("Index", "Profile", new { username = currentUser.UserName } ); //TODO AJAX

            }

            return null;
        }
        [Authorize]
        public async Task AddToUserList(string userId, int listId, int visualNovelId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId != null && currentUserId == userId)
            {
                await HttpQueries.AddToList(userId, listId, visualNovelId);
            }
        }

        [Authorize]
        public async Task RemoveFromUserList(string userId, int listId, int visualNovelId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId != null && currentUserId == userId)
            {
                await HttpQueries.RemoveFromList(userId, listId, visualNovelId);
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditVisualNovelList(VisualNovelList visualNovelList)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                var list = await HttpQueries.GetVisualNovelList(visualNovelList.Id);

                if (currentUser.Id != null && list != null && list.UserId == currentUser.Id)
                {
                    if (list.ListType == null)
                    {
                        list.Name = visualNovelList.Name;
                    }

                    list.IsPrivate = visualNovelList.IsPrivate;

                    list.VisualNovelListEntries ??= new List<VisualNovelListEntry>();

                    await HttpQueries.UpdateVisualNovelList(currentUser.Id, list.Id, list);

                    return RedirectToAction("Index", "Profile", new { username = currentUser.UserName }); // TODO AJAX
                }
            }

            return null;
        }

        [Authorize]
        [HttpDelete]
        public async Task DeleteVisualNovelList(int listId)
        {
            var currentUserId = _userManager.GetUserId(User);

            var list = await HttpQueries.GetVisualNovelList(listId);

            if (currentUserId != null && list != null && list.UserId == currentUserId)
            {
                if (list.ListType == null)
                {
                    await HttpQueries.DeleteVisualNovelList(currentUserId, listId);
                }
            }
        }

        public async Task AddUserRating(string userId, int visualNovelId, int rating)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId != null && currentUserId == userId)
            {
                await HttpQueries.AddVisualNovelRating(userId, visualNovelId, rating);
            }
        }

        public async Task UpdateUserRating(string userId, int visualNovelId, int rating)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId != null && currentUserId == userId)
            {
                await HttpQueries.UpdateRatingByUser(userId, visualNovelId, rating);
            }
        }

        public async Task RemoveUserRating(string userId, int visualNovelId)
        {
            var currentUserId = _userManager.GetUserId(User);

            if (currentUserId != null && currentUserId == userId)
            {
                await HttpQueries.RemoveRatingByUser(userId, visualNovelId);
            }
        }

        // GET: UserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
