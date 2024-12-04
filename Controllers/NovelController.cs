
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NovelSite.Data.Identity;
using NovelSite.Models.Novel;
using NovelSite.Services.Extensions;
using NovelSite.Services.Queries;


namespace NovelSite.Controllers
{
    public class NovelController : Controller
    {
        // GET: NovelController
        private readonly UserManager<ApplicationIdentityUser> _userManager;

        //private const string Globals.API_URL = "http://localhost:80/api/"; //Testing Docker
        private const string COVER_IMAGE_URL = Globals.API_URL + "VisualNovel/GetCoverImage?id=";
        private const string BACKGROUND_IMAGE_URL = Globals.API_URL + "VisualNovel/GetBackgroundImage?id=";
        private const string SCREENSHOTS_DATA_URL = Globals.API_URL + "VisualNovel/GetScreenshots?id=";
        private const string GET_IMAGE_BY_PATH_URL = Globals.API_URL + "VisualNovel/GetImageByPath?path=";
        private const long MAX_IMAGE_SIZE = 4194304;

        public NovelController(UserManager<ApplicationIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        //[Route("Novel/{id}")]
        //public async Task<IActionResult> Novel(int id)
        //{
        //    var vn = await HttpQueries.GetVisualNovel(id);

        //    if (vn == null)
        //    {
        //        return NotFound();
        //    }

        //    ViewBag.Vn = vn;
        //    var rating = await HttpQueries.GetVisualNovelRating(id);
        //    ViewBag.VnAverageRating = rating.Item1;
        //    ViewBag.VnRatingCount = rating.Item2;

        //    return View();
        //}

        [Route("Novel/Random")]
        public async Task<IActionResult> GetRandom()
        {
            var vn = await HttpQueries.GetRandomVisualNovel();

            if (vn == null)
            {
                return NotFound();
            }

            return RedirectToAction("Novel", "Novel", new { linkName = vn.LinkName });
        }

        [Route("Novel/Recommendation")]
        public async Task<IActionResult> GetRecommendation(int visualNovelId)
        {
            var recommended = await HttpQueries.GetVisualNovelRecommendation(visualNovelId);

            if (recommended == null)
            {
                return NotFound();
            }

            ViewBag.Recommendation = recommended;

            return PartialView("_RecommendedVisualNovelsPartial");
        }

        [Route("Novel/{linkName}")]
        public async Task<IActionResult> Novel(string linkName)
        {
            var vn = await HttpQueries.GetVisualNovel(linkName);

            if (vn == null)
            {
                return NotFound();
            }

            ViewBag.Vn = vn;
            var rating = await HttpQueries.GetVisualNovelRating(vn.Id);
            ViewBag.VnAverageRating = rating.Item1;
            ViewBag.VnRatingCount = rating.Item2;

            var recommended = await HttpQueries.GetVisualNovelRecommendation(vn.Id);

            ViewBag.Recommendation = recommended;

            return View();
        }

        [Route("Novels")]
        public async Task<IActionResult> Novels()
        {
            ViewBag.Tags = await HttpQueries.GetAllTags();
            ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
            ViewBag.Languages = await HttpQueries.GetLanguages();
            ViewBag.Genres = await HttpQueries.GetGenres();

            //ViewBag.Vns = await HttpQueries.GetVisualNovelsWithRatingsFiltred(itemPerPage: 3);

            return View();
        }

        [Route("Novel/Tags")]
        public async Task<IActionResult> GetVisualNovelTagsMetadata(int visualNovelId, SpoilerLevel spoilerLevel)
        {
            var tagsMetadata = await HttpQueries.GetVisualNovelTagsMetadata(visualNovelId, spoilerLevel);

            ViewBag.TagsMetadata = tagsMetadata;

            return PartialView("_VisualNovelTagsMetadataPartial");
        }

        // GET: NovelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [Route("Novel/CreateFromJson")]
        public async Task<IActionResult> CreateFromJson()
        {
            return View();
        }

        
        [Authorize(Roles = "Admin")] // TODO
        [HttpPost]
        [Route("Novel/CreateFromJson")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFromJson(VisualNovelFromJsonRequest vnRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    VisualNovel vndb = await HttpQueries.AddVisualNovelFromJson(vnRequest);

                    return RedirectToAction("Novel", "Novel", new { linkName = vndb.LinkName });
                }

                return View(vnRequest);
            }
            catch
            {
                throw;
                //return View();
            }
        }

        // GET: NovelController/Create
        [Authorize(Roles = "Admin")] // TODO
        [Route("Novel/Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Tags = await HttpQueries.GetAllTags();
            ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
            ViewBag.Languages = await HttpQueries.GetLanguages();
            ViewBag.Genres = await HttpQueries.GetGenres();

            ViewBag.Authors = await HttpQueries.GetAuthors();
            ViewBag.Translators = await HttpQueries.GetTranslators();


            return View();
        }

        [Route("Novel/CreateAuthor")]
        [HttpPost]
        [Authorize(Roles = "Admin")] // TODO
        [ValidateAntiForgeryToken]
        public async Task CreateAuthor(AuthorRequest authorRequest)
        {
            try
            {
                await HttpQueries.AddAuthor(authorRequest);
            }
            catch
            {
                throw;
                //return View();
            }
        }

        // POST: NovelController/Create
        [Route("Novel/Create")]
        [Authorize(Roles = "Admin")] // TODO
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisualNovelRequest vnRequest)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    if (vnRequest.CoverImage != null)
                    //    {
                    //        vnRequest.CoverImage.CopyTo(ms);
                    //        var byteImage = ms.ToArray();

                    //        if (byteImage.Length > MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                    //        {
                    //            return null;
                    //        }
                    //    }
                    //}

                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    if (vnRequest.BackgroundImage != null)
                    //    {
                    //        vnRequest.BackgroundImage.CopyTo(ms);
                    //        var byteImage = ms.ToArray();

                    //        if (byteImage.Length > MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                    //        {
                    //            return null;
                    //        }
                    //    }
                    //}

                    //if (vnRequest.Screenshots != null)
                    //{
                    //    foreach (IFormFile file in vnRequest.Screenshots)
                    //    {
                    //        using MemoryStream ms = new MemoryStream();
                    //        file.CopyTo(ms);
                    //        var byteImage = ms.ToArray();

                    //        if (byteImage.Length > MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                    //        {
                    //            return null;
                    //        }
                    //    }
                    //}

                    VisualNovel vndb = await HttpQueries.AddVisualNovel(vnRequest);

                    return RedirectToAction("Novel", "Novel", new { linkName = vndb.LinkName });
                }

                ViewBag.Tags = await HttpQueries.GetAllTags();
                ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
                ViewBag.Languages = await HttpQueries.GetLanguages();
                ViewBag.Genres = await HttpQueries.GetGenres();

                ViewBag.Authors = await HttpQueries.GetAuthors();
                ViewBag.Translators = await HttpQueries.GetTranslators();

                return View(vnRequest);
            }
            catch
            {
                throw;
                //return View();
            }
        }

        // GET: NovelController/Edit/5
        [Route("Novel/Edit/{linkName}")]
        [Authorize(Roles = "Admin")] // TODO
        public async Task <IActionResult> Edit(string linkName)
        {
            var vn = await HttpQueries.GetVisualNovel(linkName);
            var currentUser = await _userManager.GetUserAsync(User);

            bool isAlowed = false;

            if (currentUser.Id != vn.AdddeUserId.ToString())
                isAlowed = false; else isAlowed = true;

            if (User.IsInRole("Admin"))
                isAlowed = true;

            if (!isAlowed)
                return NotFound();

            ViewBag.Vn = vn;
            ViewBag.Tags = await HttpQueries.GetAllTags();
            ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
            ViewBag.Languages = await HttpQueries.GetLanguages();
            ViewBag.Genres = await HttpQueries.GetGenres();
            ViewBag.Authors = await HttpQueries.GetAuthors();
            ViewBag.Translators = await HttpQueries.GetTranslators();
            ViewBag.VnTags = await HttpQueries.GetVisualNovelTagsMetadata(vn.Id, SpoilerLevel.Major);
            return View();
        }

        // POST: NovelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Novel/Edit/{linkName}")]
        [Authorize(Roles = "Admin")] // TODO
        public async Task<IActionResult> Edit(string linkName, VisualNovelRequest vnRequest)
        {
            try
            {
                var vn = await HttpQueries.GetVisualNovel(linkName);

                await HttpQueries.UpdateVisualNovel(vn.Id, vn, vnRequest);

                return RedirectToAction("Novel", "Novel", new { linkName });
            }
            catch
            {
                return View();
            }
        }

        // GET: NovelController/Delete/5
        public ActionResult Delete(string linkName)
        {
            return View();
        }

        // POST: NovelController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")] // TODO
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
