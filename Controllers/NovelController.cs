
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NovelSite.Models;
using NovelSite.Services.Extensions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NovelSite.Controllers
{
    public class NovelController : Controller
    {
        // GET: NovelController
        private const string API_URL = "https://localhost:7022/api/";
        private const long MAX_IMAGE_SIZE = 4194304;

        //[Route("Novel/{id}")]
        public async Task<IActionResult> Novel(int id)
        {
            ViewBag.Vn = await GetVisualNovel(id);

            return View();
        }

        [Route("FiltredBy")]
        public async Task<IActionResult> FiltredBy(int id, string filterName, string message)
        {
            try
            {
                List<VisualNovel> vns = null;

                switch (filterName)
                {
                    case "tag":
                        vns = await GetVisualNovelWithTag(id);
                        break;
                    case "genre":
                        vns = await GetVisualNovelWithGenre(id);
                        break;
                    case "language":
                        vns = await GetVisualNovelWithLanguage(id);
                        break;
                    case "platform":
                        vns = await GetVisualNovelWithGamingPlatform(id);
                        break;
                    default:
                        break;
                }

                ViewBag.Vns = vns;
                ViewBag.FiltredBy = filterName;
                ViewBag.Message = message;

                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: NovelController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NovelController/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Tags = await GetTags();
            ViewBag.Platforms = await GetGamingPlatforms();
            ViewBag.Languages = await GetLanguages();
            ViewBag.Genres = await GetGenres();
            return View();
        }

        // POST: NovelController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisualNovelRequest vnRequest)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    vnRequest.CoverImage.CopyTo(ms);
                    var byteImage = ms.ToArray();

                    if (byteImage.Length >= MAX_IMAGE_SIZE || ImageFormatExtension.GetImageFormat(byteImage) == ImageFormat.unknown)
                    {
                        return null;
                    }
                }

                VisualNovel vndb = await AddVisualNovel(vnRequest);

                return RedirectToAction("Novel", "Novel", new { id = vndb.Id });
            }
            catch
            {
                throw;
                //return View();
            }
        }

        // GET: NovelController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NovelController/Edit/5
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

        [HttpGet]
        private static async Task<VisualNovel> GetVisualNovel(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/id?id=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel>(data);
        }

        [HttpGet]
        private static async Task<List<VisualNovel>> GetVisualNovelWithTag(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/WithTag?tagId=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<List<VisualNovel>> GetVisualNovelWithGenre(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/WithGenre?genreId=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<List<VisualNovel>> GetVisualNovelWithLanguage(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/WithLanguage?languageId=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<List<VisualNovel>> GetVisualNovelWithGamingPlatform(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/WithGamingPlatform?gamingPlatformId=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpPut]
        private static async Task AddImageToVisualNovel(int id, IFormFile file)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Создаем экземпляр MultipartFormDataContent, чтобы добавить файл и другие данные, если необходимо
                using (var formData = new MultipartFormDataContent())
                {
                    // Создаем MemoryStream из файла
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        // Создаем экземпляр StreamContent и добавляем его в MultipartFormDataContent
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "coverImage", // Имя параметра, на который сервер ожидает файл
                            FileName = file.FileName // Имя файла
                        };
                        formData.Add(fileContent);

                        // Добавляем другие данные, если необходимо
                        // formData.Add(new StringContent("value"), "key");

                        // Отправляем запрос PUT на внешний API с данными в форме multipart/form-data
                        var response = await client.PutAsync(API_URL + "VisualNovel/AddImage?id=" + id, formData);

                        // Обрабатываем ответ, если необходимо
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine("Файл успешно отправлен.");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка при отправке файла. Код ответа: {response.StatusCode}");
                        }
                    }
                }

                // Отправляем файл
                // var response = await client.PutAsync(API_URL + "VisualNovel/AddImage?id=" + id, form);
                // считываем ответ
                //var responseText = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(responseText);

                //var response = await client.PutAsync(API_URL + "VisualNovel/AddImage?id=" + id, file);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        private static async Task<VisualNovel> AddVisualNovel(VisualNovelRequest visualNovel)
        {
            try
            {
                List<TagMetadata> vnNoneSpoilerTags = new List<TagMetadata>();
                foreach (var id in visualNovel.NoneSpoilerTags)
                    vnNoneSpoilerTags.Add(new TagMetadata()
                    {
                        Id = Guid.NewGuid(),
                        Tag = await GetTag(id),
                        SpoilerLevel = SpoilerLevel.None,
                    });

                List<TagMetadata> vnMinorSpoilerTags = new List<TagMetadata>();
                foreach (var id in visualNovel.MinorSpoilerTags)
                    vnMinorSpoilerTags.Add(new TagMetadata()
                    {
                        Id = Guid.NewGuid(),
                        Tag = await GetTag(id),
                        SpoilerLevel = SpoilerLevel.Minor,
                    });

                List<TagMetadata> vnMajorSpoilerTags = new List<TagMetadata>();
                foreach (var id in visualNovel.MajorSpoilerTags)
                    vnMinorSpoilerTags.Add(new TagMetadata()
                    {
                        Id = Guid.NewGuid(),
                        Tag = await GetTag(id),
                        SpoilerLevel = SpoilerLevel.Major,
                    });

                List<TagMetadata> vnTags = new List<TagMetadata>();

                vnTags.AddRange(vnNoneSpoilerTags);
                vnTags.AddRange(vnMinorSpoilerTags);
                vnTags.AddRange(vnMajorSpoilerTags);

                List<Genre> vnGenres = new List<Genre>();
                foreach (var id in visualNovel.Genres)
                    vnGenres.Add(await GetGenre(id));

                List<Language> vnLanguages = new List<Language>();
                foreach (var id in visualNovel.Languages)
                    vnLanguages.Add(await GetLanguage(id));

                List<GamingPlatform> vnPlatforms = new List<GamingPlatform>();
                foreach (var id in visualNovel.Platforms)
                    vnPlatforms.Add(await GetGamingPlatform(id));


                VisualNovel vn = new VisualNovel()
                {
                    Title = visualNovel.Title,
                    OriginalTitle = visualNovel.OriginalTitle,
                    Autor = visualNovel.Autor,
                    Translator = visualNovel.Translator,
                    ReadingTime = visualNovel.ReadingTime,
                    ReleaseYear = visualNovel.ReleaseYear,

                    AdddeUserId = Guid.NewGuid(), // TODO: Change

                    AddedUserName = visualNovel.AddedUserName,
                    Description = visualNovel.Description,

                    Tags = vnTags,
                    Genres = vnGenres,
                    Languages = vnLanguages,
                    Platforms = vnPlatforms,
                };

                //string content = JsonConvert.SerializeObject(vn);

                //Console.WriteLine(content);

                HttpClient client = new HttpClient();

                var response = await client.PostAsJsonAsync(API_URL + "VisualNovel", vn);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    await AddImageToVisualNovel(vn.Id, visualNovel.CoverImage);

                    return await GetVisualNovel(vn.Id);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        private static async Task<List<Tag>> GetTags()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Tag");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Tag[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<Tag> GetTag(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Tag/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Tag>(data);
        }

        [HttpGet]
        private static async Task<List<GamingPlatform>> GetGamingPlatforms()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "GamingPlatform");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<GamingPlatform[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<GamingPlatform> GetGamingPlatform(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "GamingPlatform/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<GamingPlatform>(data);
        }

        [HttpGet]
        private static async Task<List<Language>> GetLanguages()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Language");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Language[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<Language> GetLanguage(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Language/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Language>(data);
        }

        [HttpGet]
        private static async Task<List<Genre>> GetGenres()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Genre");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Genre[]>(data).ToList();
        }

        [HttpGet]
        private static async Task<Genre> GetGenre(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Genre/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Genre>(data);
        }
    }
}
