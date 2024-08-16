using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NovelSite.Data.Identity;
using NovelSite.Models;
using NovelSite.Models.Novel;
using NovelSite.Models.Prolife;
using NovelSite.Services.Queries;

namespace NovelSite.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationIdentityUser> _userManager;
        public ProfileController(UserManager<ApplicationIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GET: ProfileController
        [Route("Profile/{Username}")]
        public async Task<IActionResult> Index(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return NotFound();
            }

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.NormalizedUserName == username.ToUpper());
            if (user == null)
            {
                return NotFound();
            }
            var profileOwner = await _userManager.FindByNameAsync(username);
            var currentUserId = _userManager.GetUserId(User);

            bool isOwner = currentUserId == profileOwner.Id;

            var model = new UserProfileViewModel
            {
                Id = profileOwner.Id,
                Username = profileOwner.UserName,
                AvatarUrl = profileOwner.AvatarFileName ?? "",
                VisualNovelLists = await HttpQueries.GetVisualNovelLists(user.Id, isOwner),
                VisualNovelListEntries = await HttpQueries.GetUserVisualNovelsInLists(user.Id, isOwner),
            };

            return View(model);
        }

        [Route("Profile/List")]
        public async Task<IActionResult> GetVisualNovelsInList(string userId, int listId)
        {
            //page = page > 0 ? page : 1;
            //itemsPerPage = itemsPerPage > 0 ? itemsPerPage : 20;

            var vnInList = await HttpQueries.GetVisualNovelsInList(userId, listId);

            return PartialView("_UserVisualNovelsInListPartial", vnInList);
        }

        // GET: ProfileController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProfileController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProfileController/Create
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

        // GET: ProfileController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProfileController/Edit/5
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

        // GET: ProfileController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProfileController/Delete/5
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
