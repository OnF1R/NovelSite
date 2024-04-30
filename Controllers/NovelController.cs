
using Microsoft.AspNetCore.Mvc;
using NovelSite.Models;
using NovelSite.Services.Extensions;
using NovelSite.Services.Queries;
using System.Text;


namespace NovelSite.Controllers
{
    public class NovelController : Controller
    {
        // GET: NovelController
        private const string API_URL = "https://localhost:7022/api/";
        private const string COVER_IMAGE_URL = "https://localhost:7022/api/VisualNovel/GetCoverImage?id=";
        private const string BACKGROUND_IMAGE_URL = "https://localhost:7022/api/VisualNovel/GetBackgroundImage?id=";
        private const string SCREENSHOTS_DATA_URL = "https://localhost:7022/api/VisualNovel/GetScreenshots?id=";
        private const string GET_IMAGE_BY_PATH_URL = "https://localhost:7022/api/VisualNovel/GetImageByPath?path=";
        private const long MAX_IMAGE_SIZE = 4194304;

        [Route("Novel/{id}-{spoilerLevel}")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public async Task<IActionResult> Novel(int id, SpoilerLevel spoilerLevel)
        {
            var vn = await HttpQueries.GetVisualNovel(id, spoilerLevel);
            ViewBag.Vn = vn;
            var rating = await HttpQueries.GetVisualNovelRating(id);
            ViewBag.VnAverageRating = rating.Item1;
            ViewBag.VnRatingCount = rating.Item2;
            ViewBag.CoverImage = COVER_IMAGE_URL + $"{id}";
            ViewBag.BackgroundImage = BACKGROUND_IMAGE_URL + $"{id}";
            ViewBag.ScreenshotsData = await HttpQueries.GetScreenshotsPathes(id);
            ViewBag.GetImageByPathURL = GET_IMAGE_BY_PATH_URL;

            return View();
        }

        [Route("Filter/{id}-{filter}")]
        public async Task<IActionResult> FiltredBy(int id, int filter)
        {
            try
            {
                List<VisualNovel> vns = null;
                StringBuilder sb = new StringBuilder();

                switch (filter)
                {
                    case 1:
                        vns = await HttpQueries.GetVisualNovelWithTag(id);
                        Tag tag = await HttpQueries.GetTag(id);
                        sb.Append($"Визуальные новеллы с тегом: {tag.Name}");
                        break;
                    case 2:
                        vns = await HttpQueries.GetVisualNovelWithGenre(id);
                        Genre genre = await HttpQueries.GetGenre(id);
                        sb.Append($"Визуальные новеллы с жанром: {genre.Name}");
                        break;
                    case 3:
                        vns = await HttpQueries.GetVisualNovelWithLanguage(id);
                        Language language = await HttpQueries.GetLanguage(id);
                        sb.Append($"Визуальные новеллы на языке: {language.Name}");
                        break;
                    case 4:
                        vns = await HttpQueries.GetVisualNovelWithGamingPlatform(id);
                        GamingPlatform gamingPlatform = await HttpQueries.GetGamingPlatform(id);
                        sb.Append($"Визуальные новеллы на {gamingPlatform.Name}");
                        break;
                    default:
                        break;
                }

                ViewBag.Vns = vns;
                ViewBag.Message = sb.ToString();

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> Search(string query)
        {
            var vns = await HttpQueries.SearchVisualNovel(query);

            if (vns == null)
            {
                RedirectToAction("Index"); // TODO
            }

            if (vns.Count == 1)
            {
                var vn = vns.First();

                return RedirectToAction("Novel", "Novel", new { id = vn.Id, spoilerLevel = SpoilerLevel.None });
            }

            ViewBag.Vns = vns;

            ViewBag.Message = $"Найдено совпадение по запросу: {query}";

            return View();
        }

        // GET: NovelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NovelController/Create
        [Route("Novel/Create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Tags = await HttpQueries.GetTags(1, 50);
            ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
            ViewBag.Languages = await HttpQueries.GetLanguages();
            ViewBag.Genres = await HttpQueries.GetGenres();

            ViewBag.Authors = await HttpQueries.GetAuthors();
            ViewBag.Translators = await HttpQueries.GetTranslators();


            return View();
        }

        [Route("Novel/CreateAuthor")]
        [HttpPost]
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisualNovelRequest vnRequest)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    if (vnRequest.CoverImage != null)
                    {
                        vnRequest.CoverImage.CopyTo(ms);
                        var byteImage = ms.ToArray();

                        if (byteImage.Length > MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                        {
                            return null;
                        }
                    }
                }

                using (MemoryStream ms = new MemoryStream())
                {
                    if (vnRequest.BackgroundImage != null)
                    {
                        vnRequest.BackgroundImage.CopyTo(ms);
                        var byteImage = ms.ToArray();

                        if (byteImage.Length > MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                        {
                            return null;
                        }
                    }
                }

                if (vnRequest.Screenshots != null)
                {
                    foreach (IFormFile file in vnRequest.Screenshots)
                    {
                        using MemoryStream ms = new MemoryStream();
                        file.CopyTo(ms);
                        var byteImage = ms.ToArray();

                        if (byteImage.Length > MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                        {
                            return null;
                        }
                    }
                }

                VisualNovel vndb = await HttpQueries.AddVisualNovel(vnRequest);

                return RedirectToAction("Novel", "Novel", new { id = vndb.Id, spoilerLevel = SpoilerLevel.None });
            }
            catch
            {
                throw;
                //return View();
            }
        }

        // GET: NovelController/Edit/5
        public async Task <IActionResult> Edit(int id)
        {
            ViewBag.Vn = await HttpQueries.GetVisualNovel(id, SpoilerLevel.Major);
            ViewBag.Tags = await HttpQueries.GetTags(1, 50);
            ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
            ViewBag.Languages = await HttpQueries.GetLanguages();
            ViewBag.Genres = await HttpQueries.GetGenres();
            ViewBag.Authors = await HttpQueries.GetAuthors();
            ViewBag.Translators = await HttpQueries.GetTranslators();
            return View();
        }

        // POST: NovelController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisualNovelRequest vnRequest)
        {
            try
            {
                await HttpQueries.UpdateVisualNovel(id, vnRequest);

                return RedirectToAction("Novel", "Novel", new { id = id, spoilerLevel = SpoilerLevel.None });
            }
            catch
            {
                return View();
            }
        }

        // GET: NovelController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NovelController/Delete/5
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
