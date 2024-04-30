using Microsoft.AspNetCore.Mvc;
using NovelSite.Models;
using NovelSite.Services.Queries;
using System.Diagnostics;

namespace NovelSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private const string API_URL = "https://localhost:7022/api/";

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Tags = await HttpQueries.GetTags(1, 50);
            ViewBag.Platforms = await HttpQueries.GetGamingPlatforms();
            ViewBag.Languages = await HttpQueries.GetLanguages();
            ViewBag.Genres = await HttpQueries.GetGenres();
            ViewBag.Vns = await HttpQueries.GetVisualNovelsWithRatingsFiltred();

            return View();
        }

        public async Task<IActionResult> VisualNovelContent
            (
            List<int> tags = null, List<int> genres = null, List<int> languages = null,
            List<int> platforms = null, SpoilerLevel spoilerLevel = SpoilerLevel.None, 
            ReadingTime readingTime = ReadingTime.Any, Sort sort = Sort.DateUpdatedDescending
            )
        {
            try
            {
                var vns = await HttpQueries.GetVisualNovelsWithRatingsFiltred(genres, tags, languages, platforms, spoilerLevel, readingTime, sort);

                if (vns != null)
                {
                    ViewBag.Vns = vns;
                    return PartialView("_VisualNovelContentPartial");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IActionResult> VNDBSearch(string search)
        {
            try
            {
                var vns = await VNDBQueries.SearchOnVNDB(search);

                if (vns != null)
                {
                    ViewBag.VndbSearchContent = vns.Results;
                    return PartialView("_VndbSearchResult");
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}