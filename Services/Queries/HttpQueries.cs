using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NovelSite.Models;
using System.Net;
using System.Net.Http.Headers;

namespace NovelSite.Services.Queries
{
    public class HttpQueries
    {
        public const string API_URL = "https://localhost:7022/api/";
        public const string COVER_IMAGE_URL = "https://localhost:7022/api/VisualNovel/GetCoverImage?id=";
        public const string BACKGROUND_IMAGE_URL = "https://localhost:7022/api/VisualNovel/GetBackgroundImage?id=";
        public const string SCREENSHOTS_DATA_URL = "https://localhost:7022/api/VisualNovel/GetScreenshots?id=";
        public const string GET_IMAGE_BY_PATH_URL = "https://localhost:7022/api/VisualNovel/GetImageByPath?path=";

        [HttpGet]
        public static async Task<List<VisualNovel>> GetVisualNovels()
        {
            string vnData = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel?Page=1&ItemsPerPage=50");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                vnData = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(vnData).ToList();
        }

        [HttpGet]
        public static async Task<List<VisualNovelWithRating>> GetVisualNovelsWithRatings()
        {
            string vnData = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/withRating?Page=1&ItemsPerPage=50");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                vnData = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            var vns = JsonConvert.DeserializeObject<VisualNovelWithRating[]>(vnData).ToList();

            //List<string> vnWithVndbId = vns.Where(vn => vn.VisualNovel.VndbId != null).Select(vn => vn.VisualNovel.VndbId).ToList();

            //var vndbResult = await VNDBQueries.GetRating(vnWithVndbId);

            //if (vndbResult != null && vndbResult.Results != null)
            //{
            //    foreach (var vn in vndbResult.Results)
            //    {
            //        vns.Where(vnr => vnr.VisualNovel.VndbId == vn.Id).First().VndbRating = Math.Round((double)vn.Rating / 10, 2);
            //    }
            //}

            return vns;
        }

        [HttpGet]
        public static async Task<VisualNovelWithRatingResult> GetVisualNovelsWithRatingsFiltred
            (
                List<int> genres = null,
                List<int> tags = null,
                List<int> languages = null,
                List<int> platforms = null,
                SpoilerLevel spoilerLevel = SpoilerLevel.None,
                ReadingTime readingTime = ReadingTime.Any,
                Sort sort = Sort.DateUpdatedDescending
            )
        {
            string vnData = @"";
            HttpClient client = new HttpClient();
            string query = API_URL + "VisualNovel/withRatingFiltred?Page=1&ItemsPerPage=50";

            if (genres != null)
                foreach (var genre in genres)
                    query += $"&genres={genre}";

            if (tags != null)
                foreach (var tag in tags)
                    query += $"&tags={tag}";

            if (languages != null)
                foreach (var language in languages)
                    query += $"&languages={language}";

            if (platforms != null)
                foreach (var platform in platforms)
                    query += $"&platforms={platform}";

            HttpResponseMessage response = await client.GetAsync(query + $"&spoilerLevel={spoilerLevel}&readingTime={readingTime}&sort={sort}");
            HttpContent content = response.Content;
            HttpResponseHeaders headers = response.Headers;
            IEnumerable<string> paginationParams;
            headers.TryGetValues("X-pagination", out paginationParams);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                vnData = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            var paginationParamsList = JsonConvert.DeserializeObject<PaginationParams>(paginationParams.First());

            var vns = JsonConvert.DeserializeObject<VisualNovelWithRating[]>(vnData).ToList();

            VisualNovelWithRatingResult result = new VisualNovelWithRatingResult()
            {
                Result = vns,
                CurrentPage = paginationParamsList.CurrentPage,
                TotalPages = paginationParamsList.TotalPages,
                TotalCount = paginationParamsList.TotalCount,
                HasPrevious = paginationParamsList.HasPrevious,
                HasNext = paginationParamsList.HasNext,
            };

            //List<string> vnWithVndbId = vns.Where(vn => vn.VisualNovel.VndbId != null).Select(vn => vn.VisualNovel.VndbId).ToList();

            //var vndbResult = await VNDBQueries.GetRating(vnWithVndbId);

            //if (vndbResult != null && vndbResult.Results != null)
            //{
            //    foreach (var vn in vndbResult.Results)
            //    {
            //        vns.Where(vnr => vnr.VisualNovel.VndbId == vn.Id).First().VndbRating = Math.Round((double)vn.Rating / 10, 2);
            //    }
            //}

            return result;
        }

        [HttpGet]
        public static async Task<VisualNovel> GetVisualNovel(int id, SpoilerLevel spoilerLevel)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/id?id=" + id.ToString()
                + "&spoilerLevel=" + spoilerLevel.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel>(data);
        }

        [HttpGet]
        public static async Task<string[]> GetScreenshotsPathes(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(SCREENSHOTS_DATA_URL + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<string[]>(data);
        }

        [HttpGet]
        public static async Task<List<VisualNovel>> GetVisualNovelWithTag(int id)
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
        public static async Task<List<VisualNovel>> GetVisualNovelWithTagAndSpoilerLevel(int id, SpoilerLevel spoilerLevel)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/WithTagAndSpoilerLevel?tagId=" + id.ToString()
                 + "&spoilerLevel=" + spoilerLevel.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<List<VisualNovel>> GetVisualNovelWithGenre(int id)
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
        public static async Task<List<VisualNovel>> GetVisualNovelWithLanguage(int id)
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
        public static async Task<List<VisualNovel>> GetVisualNovelWithGamingPlatform(int id)
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
        public static async Task UpdateVisualNovel(int id, VisualNovelRequest visualNovel)
        {
            try
            {
                List<TagMetadata> vnNoneSpoilerTags = new List<TagMetadata>();
                foreach (var tagId in visualNovel.NoneSpoilerTags)
                    vnNoneSpoilerTags.Add(new TagMetadata()
                    {
                        Id = Guid.NewGuid(),
                        Tag = await GetTag(tagId),
                        SpoilerLevel = SpoilerLevel.None,
                    });

                List<TagMetadata> vnMinorSpoilerTags = new List<TagMetadata>();
                foreach (var tagId in visualNovel.MinorSpoilerTags)
                    vnMinorSpoilerTags.Add(new TagMetadata()
                    {
                        Id = Guid.NewGuid(),
                        Tag = await GetTag(tagId),
                        SpoilerLevel = SpoilerLevel.Minor,
                    });

                List<TagMetadata> vnMajorSpoilerTags = new List<TagMetadata>();
                foreach (var tagId in visualNovel.MajorSpoilerTags)
                    vnMajorSpoilerTags.Add(new TagMetadata()
                    {
                        Id = Guid.NewGuid(),
                        Tag = await GetTag(tagId),
                        SpoilerLevel = SpoilerLevel.Major,
                    });

                List<TagMetadata> vnTags = new List<TagMetadata>();

                vnTags.AddRange(vnNoneSpoilerTags);
                vnTags.AddRange(vnMinorSpoilerTags);
                vnTags.AddRange(vnMajorSpoilerTags);

                List<Genre> vnGenres = new List<Genre>();
                foreach (var genreId in visualNovel.Genres)
                    vnGenres.Add(await GetGenre(genreId));

                List<Language> vnLanguages = new List<Language>();
                foreach (var languageId in visualNovel.Languages)
                    vnLanguages.Add(await GetLanguage(languageId));

                List<GamingPlatform> vnPlatforms = new List<GamingPlatform>();
                foreach (var platformId in visualNovel.Platforms)
                    vnPlatforms.Add(await GetGamingPlatform(platformId));

                List<Author> vnAuthors = new List<Author>();
                foreach (var authorId in visualNovel.Authors)
                    vnAuthors.Add(await GetAuthor(authorId));

                Translator vnTranslator = null;

                if (visualNovel.Translator != null)
                    vnTranslator = await GetTranslator((int)visualNovel.Translator);

                List<DownloadLink> vnDownloadLinks = new List<DownloadLink>();

                if (visualNovel.DownloadLinkGamingPlatformId != null && visualNovel.DownloadLinkUrl != null
                    && visualNovel.DownloadLinkUrl.Count == visualNovel.DownloadLinkGamingPlatformId.Count)
                {
                    for (int i = 0; i < visualNovel.DownloadLinkGamingPlatformId.Count; i++)
                    {
                        DownloadLink downloadLink = new DownloadLink()
                        {
                            Id = Guid.NewGuid(),
                            Url = visualNovel.DownloadLinkUrl[i],
                            GamingPlatform = await GetGamingPlatform(visualNovel.DownloadLinkGamingPlatformId[i]),
                        };

                        vnDownloadLinks.Add(downloadLink);
                    }
                }

                List<OtherLink> vnOtherLinks = new List<OtherLink>();

                if (visualNovel.OtherLinkName != null && visualNovel.OtherLinkUrl != null
                    && visualNovel.OtherLinkUrl.Count == visualNovel.OtherLinkName.Count)
                {
                    for (int i = 0; i < visualNovel.OtherLinkName.Count; i++)
                    {
                        OtherLink otherLink = new OtherLink()
                        {
                            Id = Guid.NewGuid(),
                            Name = visualNovel.OtherLinkName[i],
                            Url = visualNovel.OtherLinkUrl[i],
                        };

                        vnOtherLinks.Add(otherLink);
                    }
                }

                VisualNovel vn = new VisualNovel()
                {
                    Id = id,
                    Title = visualNovel.Title,
                    VndbId = visualNovel.VndbId,
                    OriginalTitle = visualNovel.OriginalTitle,
                    Status = visualNovel.Status,

                    Translator = vnTranslator,
                    ReadingTime = visualNovel.ReadingTime,
                    ReleaseYear = visualNovel.ReleaseYear,

                    AdddeUserId = Guid.NewGuid(), // TODO: Change
                    SteamLink = visualNovel.SteamLink,
                    TranslateLinkForSteam = visualNovel.TranslateLinkForSteam,
                    AddedUserName = visualNovel.AddedUserName,
                    Description = visualNovel.Description,

                    Author = vnAuthors,
                    Tags = vnTags,
                    Genres = vnGenres,
                    Languages = vnLanguages,
                    Platforms = vnPlatforms,
                    Links = vnDownloadLinks,
                    OtherLinks = vnOtherLinks,
                };

                //string content = JsonConvert.SerializeObject(vn);

                //Console.WriteLine(content);

                HttpClient client = new HttpClient();

                var response = await client.PutAsJsonAsync(API_URL + $"VisualNovel/id?id={id}", vn);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public static async Task AddCoverImageToVisualNovel(int id, IFormFile file)
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
                        var response = await client.PutAsync(API_URL + "VisualNovel/AddCoverImage?id=" + id, formData);

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

        [HttpPut]
        public static async Task AddBackgroundImageToVisualNovel(int id, IFormFile file)
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
                            Name = "backgroundImage", // Имя параметра, на который сервер ожидает файл
                            FileName = file.FileName // Имя файла
                        };
                        formData.Add(fileContent);

                        // Добавляем другие данные, если необходимо
                        // formData.Add(new StringContent("value"), "key");

                        // Отправляем запрос PUT на внешний API с данными в форме multipart/form-data
                        var response = await client.PutAsync(API_URL + "VisualNovel/AddBackgroundImage?id=" + id, formData);

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
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPut]
        public static async Task AddScreenshotsToVisualNovel(int id, List<IFormFile> files)
        {
            try
            {
                HttpClient client = new HttpClient();

                // Создаем экземпляр MultipartFormDataContent, чтобы добавить файл и другие данные, если необходимо
                using (var formData = new MultipartFormDataContent())
                {
                    foreach (IFormFile file in files)
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
                                Name = "backgroundImage", // Имя параметра, на который сервер ожидает файл
                                FileName = file.FileName // Имя файла
                            };
                            formData.Add(fileContent);
                        }
                    }

                    var response = await client.PutAsync(API_URL + "VisualNovel/AddScreenshots?id=" + id, formData);

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
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public static async Task<VisualNovel> AddVisualNovel(VisualNovelRequest visualNovel)
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
                    vnMajorSpoilerTags.Add(new TagMetadata()
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

                List<Author> vnAuthors = new List<Author>();
                foreach (var authorId in visualNovel.Authors)
                    vnAuthors.Add(await GetAuthor(authorId));

                Translator vnTranslator = null;

                if (visualNovel.Translator != null)
                    vnTranslator = await GetTranslator((int)visualNovel.Translator);

                List<DownloadLink> vnDownloadLinks = new List<DownloadLink>();

                if (visualNovel.DownloadLinkGamingPlatformId != null && visualNovel.DownloadLinkUrl != null 
                    && visualNovel.DownloadLinkUrl.Count == visualNovel.DownloadLinkGamingPlatformId.Count)
                {
                    for (int i = 0; i < visualNovel.DownloadLinkGamingPlatformId.Count; i++)
                    {
                        DownloadLink downloadLink = new DownloadLink()
                        {
                            Id = Guid.NewGuid(),
                            Url = visualNovel.DownloadLinkUrl[i],
                            GamingPlatform = await GetGamingPlatform(visualNovel.DownloadLinkGamingPlatformId[i]),
                        };

                        vnDownloadLinks.Add(downloadLink);
                    }
                }

                List<OtherLink> vnOtherLinks = new List<OtherLink>();

                if (visualNovel.OtherLinkName != null && visualNovel.OtherLinkUrl != null 
                    && visualNovel.OtherLinkUrl.Count == visualNovel.OtherLinkName.Count)
                {
                    for (int i = 0; i < visualNovel.OtherLinkName.Count; i++)
                    {
                        OtherLink otherLink = new OtherLink()
                        {
                            Id = Guid.NewGuid(),
                            Name = visualNovel.OtherLinkName[i],
                            Url = visualNovel.OtherLinkUrl[i],
                        };

                        vnOtherLinks.Add(otherLink);
                    }
                }

                VisualNovel vn = new VisualNovel()
                {
                    Title = visualNovel.Title,
                    VndbId = visualNovel.VndbId,
                    OriginalTitle = visualNovel.OriginalTitle,
                    Status = visualNovel.Status,

                    Translator = vnTranslator,
                    ReadingTime = visualNovel.ReadingTime,
                    ReleaseYear = visualNovel.ReleaseYear,

                    AdddeUserId = Guid.NewGuid(), // TODO: Change
                    SteamLink = visualNovel.SteamLink,
                    TranslateLinkForSteam = visualNovel.TranslateLinkForSteam,
                    AddedUserName = visualNovel.AddedUserName,
                    Description = visualNovel.Description,

                    Author = vnAuthors,
                    Tags = vnTags,
                    Genres = vnGenres,
                    Languages = vnLanguages,
                    Platforms = vnPlatforms,
                    Links = vnDownloadLinks,
                    OtherLinks = vnOtherLinks,
                };

                //string content = JsonConvert.SerializeObject(vn);

                //Console.WriteLine(content);

                HttpClient client = new HttpClient();

                var response = await client.PostAsJsonAsync(API_URL + "VisualNovel", vn);

                if (response.StatusCode == HttpStatusCode.Created)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    VisualNovel vndb = JsonConvert.DeserializeObject<VisualNovel>(data);

                    if (vndb == null)
                        return null;

                    if (visualNovel.CoverImage != null)
                        await AddCoverImageToVisualNovel(vndb.Id, visualNovel.CoverImage);

                    if (visualNovel.BackgroundImage != null)
                        await AddBackgroundImageToVisualNovel(vndb.Id, visualNovel.BackgroundImage);

                    if (visualNovel.Screenshots != null)
                        await AddScreenshotsToVisualNovel(vndb.Id, visualNovel.Screenshots);

                    return await GetVisualNovel(vndb.Id, SpoilerLevel.None);
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public static async Task<List<Tag>> GetTags(int page, int itemsPerPage)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + $"Tag?Page={page}&ItemsPerPage={itemsPerPage}");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Tag[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<Tag> GetTag(int id)
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
        public static async Task<List<GamingPlatform>> GetGamingPlatforms()
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
        public static async Task<GamingPlatform> GetGamingPlatform(int id)
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
        public static async Task<List<Language>> GetLanguages()
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
        public static async Task<Language> GetLanguage(int id)
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
        public static async Task<List<Genre>> GetGenres()
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
        public static async Task<Genre> GetGenre(int id)
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

        [HttpGet]
        public static async Task<(double, int)> GetVisualNovelRating(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovelRating/average?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<(double, int)>(data);
        }

        [HttpPost]
        public static async Task AddAuthor(AuthorRequest authorRequest)
        {
            try
            {
                Author author = new Author()
                {
                    Name = authorRequest.Name,
                    Source = authorRequest.Source, 
                    VndbId = authorRequest.VndbId,
                };

                HttpResponseMessage message;
                StringContent content = new StringContent($"?Name={authorRequest.Name}{(authorRequest.VndbId != null ? "&VndbId=" + authorRequest.VndbId : "")}{(authorRequest.Source != null ? "&Source=" + authorRequest.Source : "")}");

                using (HttpClient client = new HttpClient())
                {
                    message = await client.PostAsync(API_URL + $"Author", content);
                }
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        [HttpGet]
        public static async Task<List<Author>> GetAuthors()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Author");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Author[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<Author> GetAuthor(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Author/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Author>(data);
        }

        [HttpGet]
        public static async Task<List<Translator>> GetTranslators()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Translator");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Translator[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<Translator> GetTranslator(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "Translator/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Translator>(data);
        }

        [HttpGet]
        public static async Task<List<VisualNovel>> SearchVisualNovel(string query)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(API_URL + "VisualNovel/Search?query=" + query);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }
    }
}
