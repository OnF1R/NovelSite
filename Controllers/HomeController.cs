using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NovelSite.Models;
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
            ViewBag.Vns = await GetVisualNovels();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        private static async Task<List<VisualNovel>> GetVisualNovels()
        {
            string vnData = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel?Page=1&ItemsPerPage=50");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                vnData = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(vnData).ToList();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}