using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using NovelSite.Models.Novel;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace NovelSite.Services.Queries
{
    public class HttpQueries
    {
        //public const string Globals.API_URL = "http://localhost:80/api/"; // Testing Docker
        public const string COVER_IMAGE_URL = Globals.API_URL + "VisualNovel/GetCoverImage?id=";
        public const string BACKGROUND_IMAGE_URL = Globals.API_URL + "VisualNovel/GetBackgroundImage?id=";
        public const string SCREENSHOTS_DATA_URL = Globals.API_URL + "VisualNovel/GetScreenshots?id=";
        public const string GET_IMAGE_BY_PATH_URL = Globals.API_URL + "VisualNovel/GetImageByPath?path=";

        [HttpPut]
        public static async Task IncrementPageViewsCount(int visualNovelId)
        {
            using HttpClient client = new HttpClient();

            var requestUrl = $"{Globals.API_URL}VisualNovel/IncrementPageViewsCount?visualNovelId={visualNovelId}";

            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PutAsync(requestUrl, content);
        }
             
        [HttpGet]
        public static async Task<List<VisualNovel>> GetVisualNovels()
        {
            string vnData = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel?Page=1&ItemsPerPage=50");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/withRating?Page=1&ItemsPerPage=50");
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
                Sort sort = Sort.DateUpdatedDescending,
                string search = "",
                int page = 1,
                int itemPerPage = 20
            )
        {
            using HttpClient client = new HttpClient();
            string vnData = @"";

            string query = Globals.API_URL + $"VisualNovel/withRatingFiltred?Page={page}&ItemsPerPage={itemPerPage}";

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

            if (!string.IsNullOrEmpty(search))
                query += $"&search={search}";

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
        public static async Task<VisualNovel> GetVisualNovel(int id)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/id?id=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel>(data);
        }

        [HttpGet]
        public static async Task<VisualNovel> GetRandomVisualNovel()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/GetRandom");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel>(data);
        }

        [HttpGet]
        public static async Task<VisualNovel> GetVisualNovel(string linkName)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/linkName?linkName=" + linkName);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/WithTag?tagId=" + id.ToString());
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/WithTagAndSpoilerLevel?tagId=" + id.ToString()
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/WithGenre?genreId=" + id.ToString());
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/WithLanguage?languageId=" + id.ToString());
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/WithGamingPlatform?gamingPlatformId=" + id.ToString());
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpPut]
        public static async Task UpdateVisualNovel(int vnId, VisualNovel editedNovel, VisualNovelRequest visualNovel)
        {
            try
            {
                List<TagMetadata> vnTags = new List<TagMetadata>();

                if (visualNovel.NoneSpoilerTags != null && visualNovel.NoneSpoilerTags.Count > 0)
                {
                    List<TagMetadata> vnNoneSpoilerTags = new List<TagMetadata>();
                    foreach (var id in visualNovel.NoneSpoilerTags)
                        vnNoneSpoilerTags.Add(new TagMetadata()
                        {
                            Id = Guid.NewGuid(),
                            Tag = new Tag { Id = id, Name = "" }, // await GetTag(id)
                            SpoilerLevel = SpoilerLevel.None,
                        });

                    vnTags.AddRange(vnNoneSpoilerTags);
                }

                if (visualNovel.MinorSpoilerTags != null && visualNovel.MinorSpoilerTags.Count > 0)
                {
                    List<TagMetadata> vnMinorSpoilerTags = new List<TagMetadata>();
                    foreach (var id in visualNovel.MinorSpoilerTags)
                        vnMinorSpoilerTags.Add(new TagMetadata()
                        {
                            Id = Guid.NewGuid(),
                            Tag = new Tag { Id = id, Name = "" }, // await GetTag(id)
                            SpoilerLevel = SpoilerLevel.Minor,
                        });

                    vnTags.AddRange(vnMinorSpoilerTags);
                }

                if (visualNovel.MajorSpoilerTags != null && visualNovel.MajorSpoilerTags.Count > 0)
                {
                    List<TagMetadata> vnMajorSpoilerTags = new List<TagMetadata>();
                    foreach (var id in visualNovel.MajorSpoilerTags)
                        vnMajorSpoilerTags.Add(new TagMetadata()
                        {
                            Id = Guid.NewGuid(),
                            Tag = new Tag { Id = id, Name = "" }, // await GetTag(id)
                            SpoilerLevel = SpoilerLevel.Major,
                        });

                    vnTags.AddRange(vnMajorSpoilerTags);
                }

                List<Genre> vnGenres = new List<Genre>();
                if (visualNovel.Genres != null && visualNovel.Genres.Count > 0)
                {
                    foreach (var id in visualNovel.Genres)
                        vnGenres.Add(new Genre { Id = id, Name = "" }); // await GetGenre(id)
                }

                List<Language> vnLanguages = new List<Language>();
                if (visualNovel.Languages != null && visualNovel.Languages.Count > 0)
                {
                    foreach (var id in visualNovel.Languages)
                        vnLanguages.Add(new Language { Id = id, Name = "" }); // await GetLanguage(id)
                }

                List<GamingPlatform> vnPlatforms = new List<GamingPlatform>();
                if (visualNovel.Platforms != null && visualNovel.Platforms.Count > 0)
                {
                    foreach (var id in visualNovel.Platforms)
                        vnPlatforms.Add(new GamingPlatform { Id = id, Name = "" }); // await GetGamingPlatform(id)
                }
                List<Author> vnAuthors = new List<Author>();

                if (visualNovel.Authors != null && visualNovel.Authors.Count > 0)
                {
                    foreach (var authorId in visualNovel.Authors)
                        vnAuthors.Add(new Author { Id = authorId, Name = "", VisualNovels = new List<VisualNovel>() }); // await GetAuthor(authorId)
                }

                List<Translator> vnTranslators = new List<Translator>();
                if (visualNovel.Translator != null && visualNovel.Translator.Count > 0)
                {
                    foreach (var id in visualNovel.Translator)
                        vnTranslators.Add(new Translator { Id = id, Name = "" }); // await GetTranslator((int)visualNovel.Translator)
                }

                //if (visualNovel.Translator != null) { }
                //vnTranslator = await GetTranslator((int)visualNovel.Translator);

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
                            GamingPlatform = new GamingPlatform { Id = visualNovel.DownloadLinkGamingPlatformId[i], Name = "" }, // await GetGamingPlatform(visualNovel.DownloadLinkGamingPlatformId[i])
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

                List<RelatedAnimeLink> vnRelatedAnime = new List<RelatedAnimeLink>();

                if (visualNovel.RelatedAnimeLinkName != null && visualNovel.RelatedAnimeLinkUrl != null
                    && visualNovel.RelatedAnimeLinkName.Count == visualNovel.RelatedAnimeLinkUrl.Count)
                {
                    for (int i = 0; i < visualNovel.RelatedAnimeLinkName.Count; i++)
                    {
                        RelatedAnimeLink relatedAnime = new RelatedAnimeLink()
                        {
                            Id = Guid.NewGuid(),
                            Name = visualNovel.RelatedAnimeLinkName[i],
                            Url = visualNovel.RelatedAnimeLinkUrl[i],
                        };

                        vnRelatedAnime.Add(relatedAnime);
                    }
                }

                List<RelatedNovel> vnRelatedNovel = new List<RelatedNovel>();

                if (visualNovel.RelatedNovels != null && visualNovel.RelatedNovels.Count > 0)
                {
                    if (editedNovel.RelatedNovels != null)
                        editedNovel.RelatedNovels.Clear();

                    for (int i = 0; i < visualNovel.RelatedNovels.Count; i++)
                    {
                        var tempRelatedNovel = await GetVisualNovel(visualNovel.RelatedNovels[i]);
                        if (tempRelatedNovel.RelatedNovels != null)
                            tempRelatedNovel.RelatedNovels.Clear();

                        RelatedNovel relatedNovel = new RelatedNovel()
                        {
                            RelatedVisualNovelId = visualNovel.RelatedNovels[i],
                            RelatedVisualNovel = tempRelatedNovel,
                            VisualNovelId = editedNovel.Id,
                            VisualNovel = editedNovel,
                        };

                        vnRelatedNovel.Add(relatedNovel);
                    }
                }

                VisualNovel vn = new VisualNovel()
                {
                    Id = editedNovel.Id,
                    Title = visualNovel.Title,
                    VndbId = visualNovel.VndbId,
                    AnotherTitles = visualNovel.AnotherTitles,
                    SoundtrackYoutubePlaylistLink = visualNovel.SoundtrackYoutubePlaylistLink,
                    LinkName = visualNovel.LinkName,
                    Status = visualNovel.Status,
                    ReadingTime = visualNovel.ReadingTime,
                    ReleaseYear = visualNovel.ReleaseYear,
                    ReleaseMonth = visualNovel.ReleaseMonth,
                    ReleaseDay = visualNovel.ReleaseDay,

                    AdddeUserId = editedNovel.AdddeUserId,
                    SteamLink = visualNovel.SteamLink,
                    TranslateLinkForSteam = visualNovel.TranslateLinkForSteam,
                    AddedUserName = editedNovel.AddedUserName,
                    Description = visualNovel.Description,

                    Author = vnAuthors,
                    Tags = vnTags,
                    Genres = vnGenres,
                    Languages = vnLanguages,
                    Platforms = vnPlatforms,
                    DownloadLinks = vnDownloadLinks,
                    OtherLinks = vnOtherLinks,
                    RelatedNovels = vnRelatedNovel,
                    AnimeLinks = vnRelatedAnime,
                    Translator = vnTranslators,
                };

                //string content = JsonConvert.SerializeObject(vn);

                //Console.WriteLine(content);

                HttpClient client = new HttpClient();

                var response = await client.PutAsJsonAsync(Globals.API_URL + $"VisualNovel/id?id={vnId}", vn);

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();

                    VisualNovel vndb = JsonConvert.DeserializeObject<VisualNovel>(data);

                    if (vndb != null)
                    {
                        if (visualNovel.CoverImage != null)
                            await AddCoverImageToVisualNovel(vndb.Id, visualNovel.CoverImage);

                        if (visualNovel.BackgroundImage != null)
                            await AddBackgroundImageToVisualNovel(vndb.Id, visualNovel.BackgroundImage);

                        if (visualNovel.Screenshots != null)
                        {
                            var screenshotFileNames = visualNovel.Screenshots.Select(file => $"{vndb.Id}/Screenshots/{file.FileName}");

                            if (vndb.ScreenshotLinks == null || !screenshotFileNames.SequenceEqual(vndb.ScreenshotLinks))
                            {
                                await DeleteScreenshotsFolderS3(vndb.Id);
                                await AddScreenshotsToVisualNovel(vndb.Id, visualNovel.Screenshots);
                            }
                        }
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Error Content: {errorContent}");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private static async Task DeleteScreenshotsFolderS3(int vnId)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + $"VisualNovel/DeleteScreenshotsFolderS3?vnId=" + vnId);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
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
                        var response = await client.PutAsync(Globals.API_URL + "VisualNovel/AddCoverImage?id=" + id, formData);

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
                // var response = await client.PutAsync(Globals.API_URL + "VisualNovel/AddImage?id=" + id, form);
                // считываем ответ
                //var responseText = await response.Content.ReadAsStringAsync();

                //Console.WriteLine(responseText);

                //var response = await client.PutAsync(Globals.API_URL + "VisualNovel/AddImage?id=" + id, file);
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
                        var response = await client.PutAsync(Globals.API_URL + "VisualNovel/AddBackgroundImage?id=" + id, formData);

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
                using HttpClient client = new HttpClient();

                foreach (IFormFile file in files)
                {
                    using (var stream = new MemoryStream())
                    {
                        await file.CopyToAsync(stream);
                        stream.Seek(0, SeekOrigin.Begin);

                        var formData = new MultipartFormDataContent();
                        var fileContent = new StreamContent(stream);
                        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "screenshots",
                            FileName = file.FileName
                        };
                        formData.Add(fileContent, "screenshots", file.FileName);

                        var response = await client.PutAsync(Globals.API_URL + "VisualNovel/AddScreenshots?id=" + id, formData);

                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Скриншот {file.FileName} успешно отправлен.");
                        }
                        else
                        {
                            Console.WriteLine($"Ошибка при отправке скриншота {file.FileName}. Код ответа: {response.StatusCode}");
                        }
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
                List<TagMetadata> vnTags = new List<TagMetadata>();

                if (visualNovel.NoneSpoilerTags != null && visualNovel.NoneSpoilerTags.Count > 0)
                {
                    List<TagMetadata> vnNoneSpoilerTags = new List<TagMetadata>();
                    foreach (var id in visualNovel.NoneSpoilerTags)
                        vnNoneSpoilerTags.Add(new TagMetadata()
                        {
                            Id = Guid.NewGuid(),
                            Tag = new Tag { Id = id, Name = "" }, // await GetTag(id)
                            SpoilerLevel = SpoilerLevel.None,
                        });

                    vnTags.AddRange(vnNoneSpoilerTags);
                }

                if (visualNovel.MinorSpoilerTags != null && visualNovel.MinorSpoilerTags.Count > 0)
                {
                    List<TagMetadata> vnMinorSpoilerTags = new List<TagMetadata>();
                    foreach (var id in visualNovel.MinorSpoilerTags)
                        vnMinorSpoilerTags.Add(new TagMetadata()
                        {
                            Id = Guid.NewGuid(),
                            Tag = new Tag { Id = id, Name = "" }, // await GetTag(id)
                            SpoilerLevel = SpoilerLevel.Minor,
                        });

                    vnTags.AddRange(vnMinorSpoilerTags);
                }

                if (visualNovel.MajorSpoilerTags != null && visualNovel.MajorSpoilerTags.Count > 0)
                {
                    List<TagMetadata> vnMajorSpoilerTags = new List<TagMetadata>();
                    foreach (var id in visualNovel.MajorSpoilerTags)
                        vnMajorSpoilerTags.Add(new TagMetadata()
                        {
                            Id = Guid.NewGuid(),
                            Tag = new Tag { Id = id, Name = "" }, // await GetTag(id)
                            SpoilerLevel = SpoilerLevel.Major,
                        });

                    vnTags.AddRange(vnMajorSpoilerTags);
                }

                List<Genre> vnGenres = new List<Genre>();
                if (visualNovel.Genres != null && visualNovel.Genres.Count > 0)
                {
                    foreach (var id in visualNovel.Genres)
                        vnGenres.Add(new Genre { Id = id, Name = "" }); // await GetGenre(id)
                }

                List<Language> vnLanguages = new List<Language>();
                if (visualNovel.Languages != null && visualNovel.Languages.Count > 0)
                {
                    foreach (var id in visualNovel.Languages)
                        vnLanguages.Add(new Language { Id = id, Name = "" }); // await GetLanguage(id)
                }

                List<GamingPlatform> vnPlatforms = new List<GamingPlatform>();
                if (visualNovel.Platforms != null && visualNovel.Platforms.Count > 0)
                {
                    foreach (var id in visualNovel.Platforms)
                        vnPlatforms.Add(new GamingPlatform { Id = id, Name = "" }); // await GetGamingPlatform(id)
                }
                List<Author> vnAuthors = new List<Author>();

                if (visualNovel.Authors != null && visualNovel.Authors.Count > 0)
                {
                    foreach (var authorId in visualNovel.Authors)
                        vnAuthors.Add(new Author { Id = authorId, Name = "", VisualNovels = new List<VisualNovel>() }); // await GetAuthor(authorId)
                }

                List<Translator> vnTranslators = new List<Translator>();
                if (visualNovel.Translator != null && visualNovel.Translator.Count > 0)
                {
                    foreach (var id in visualNovel.Translator)
                        vnTranslators.Add(new Translator { Id = id, Name = "" }); // await GetTranslator((int)visualNovel.Translator)
                }

                //if (visualNovel.Translator != null) { }
                //vnTranslator = await GetTranslator((int)visualNovel.Translator);

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
                            GamingPlatform = new GamingPlatform { Id = visualNovel.DownloadLinkGamingPlatformId[i], Name = "" }, // await GetGamingPlatform(visualNovel.DownloadLinkGamingPlatformId[i])
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

                List<RelatedAnimeLink> vnRelatedAnime = new List<RelatedAnimeLink>();

                if (visualNovel.RelatedAnimeLinkName != null && visualNovel.RelatedAnimeLinkUrl != null
                    && visualNovel.RelatedAnimeLinkName.Count == visualNovel.RelatedAnimeLinkUrl.Count)
                {
                    for (int i = 0; i < visualNovel.RelatedAnimeLinkName.Count; i++)
                    {
                        RelatedAnimeLink relatedAnime = new RelatedAnimeLink()
                        {
                            Id = Guid.NewGuid(),
                            Name = visualNovel.RelatedAnimeLinkName[i],
                            Url = visualNovel.RelatedAnimeLinkUrl[i],
                        };

                        vnRelatedAnime.Add(relatedAnime);
                    }
                }

                List<RelatedNovel> vnRelatedNovel = new List<RelatedNovel>();

                if (visualNovel.RelatedNovels != null && visualNovel.RelatedNovels.Count > 0)
                {
                    for (int i = 0; i < visualNovel.RelatedNovels.Count; i++)
                    {
                        RelatedNovel relatedAnime = new RelatedNovel()
                        {
                            RelatedVisualNovelId = visualNovel.RelatedNovels[i]
                        };

                        vnRelatedNovel.Add(relatedAnime);
                    }
                }

                //if (visualNovel.AnotherTitles != null && visualNovel.AnotherTitles.Count > 0)
                //{
                //    foreach (var anotherTitle in visualNovel.AnotherTitles)
                //    {
                //        Another otherLink = new OtherLink()
                //        {
                //            Id = Guid.NewGuid(),
                //            Name = visualNovel.OtherLinkName[i],
                //            Url = visualNovel.OtherLinkUrl[i],
                //        };

                //        vnOtherLinks.Add(otherLink);
                //    }
                //}

                VisualNovel vn = new VisualNovel()
                {
                    Title = visualNovel.Title,
                    VndbId = visualNovel.VndbId,
                    AnotherTitles = visualNovel.AnotherTitles,
                    SoundtrackYoutubePlaylistLink = visualNovel.SoundtrackYoutubePlaylistLink,
                    LinkName = visualNovel.LinkName,
                    Status = visualNovel.Status,

                    //Translator = vnTranslator,
                    ReadingTime = visualNovel.ReadingTime,
                    ReleaseYear = visualNovel.ReleaseYear,
                    ReleaseMonth = visualNovel.ReleaseMonth,
                    ReleaseDay = visualNovel.ReleaseDay,

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
                    DownloadLinks = vnDownloadLinks,
                    OtherLinks = vnOtherLinks,
                    RelatedNovels = vnRelatedNovel,
                    AnimeLinks = vnRelatedAnime,
                    Translator = vnTranslators,
                };

                //string content = JsonConvert.SerializeObject(vn);

                //Console.WriteLine(content);

                HttpClient client = new HttpClient();

                var response = await client.PostAsJsonAsync(Globals.API_URL + "VisualNovel", vn);

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

                    return await GetVisualNovel(vndb.Id);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}");
                    Console.WriteLine($"Error Content: {errorContent}");
                }

                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        public static async Task<VisualNovel> AddVisualNovelFromJson(VisualNovelFromJsonRequest visualNovelFromJsonRequest)
        {
            string data = @"";
            HttpClient client = new HttpClient();

            var requestUrl = Globals.API_URL + $"VisualNovel/AddVisualNovelFromJson?linkName={visualNovelFromJsonRequest.LinkName}";
               
            if (visualNovelFromJsonRequest.SoundtrackYoutubePlaylistLink != null)
                requestUrl += $"&soundtrackYoutubePlaylistLink={visualNovelFromJsonRequest.SoundtrackYoutubePlaylistLink}";

            using (var formData = new MultipartFormDataContent())
            {
                // Создаем MemoryStream из файла
                using (var stream = new MemoryStream())
                {
                    await visualNovelFromJsonRequest.VisualNovelJson.CopyToAsync(stream);
                    stream.Seek(0, SeekOrigin.Begin);

                    // Создаем экземпляр StreamContent и добавляем его в MultipartFormDataContent
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "visualNovelJson", // Имя параметра, на который сервер ожидает файл
                        FileName = visualNovelFromJsonRequest.VisualNovelJson.FileName // Имя файла
                    };
                    formData.Add(fileContent);

                    // Добавляем другие данные, если необходимо
                    // formData.Add(new StringContent("value"), "key");

                    // Отправляем запрос PUT на внешний API с данными в форме multipart/form-data
                    var response = await client.PostAsync(requestUrl, formData);

                    // Обрабатываем ответ, если необходимо
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Файл успешно отправлен.");
                        data = await response.Content.ReadAsStringAsync();

                        VisualNovel vndb = JsonConvert.DeserializeObject<VisualNovel>(data);

                        if (vndb == null)
                            return null;

                        if (visualNovelFromJsonRequest.CoverImage != null)
                            await AddCoverImageToVisualNovel(vndb.Id, visualNovelFromJsonRequest.CoverImage);

                        if (visualNovelFromJsonRequest.BackgroundImage != null)
                            await AddBackgroundImageToVisualNovel(vndb.Id, visualNovelFromJsonRequest.BackgroundImage);

                        if (visualNovelFromJsonRequest.Screenshots != null)
                            await AddScreenshotsToVisualNovel(vndb.Id, visualNovelFromJsonRequest.Screenshots);

                        return await GetVisualNovel(vndb.Id);
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ошибка при отправке файла. Код ответа: {response.StatusCode}");
                        Console.WriteLine($"Error Content: {errorContent}");
                        return null;
                    }
                }
            }
        }

        [HttpGet]
        public static async Task<List<Tag>> GetAllTags()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + $"Tag/All");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Tag[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<List<Tag>> GetTags(int page, int itemsPerPage)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + $"Tag?Page={page}&ItemsPerPage={itemsPerPage}");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Tag/id?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<Tag>(data);
        }

        [HttpGet]
        public static async Task<List<TagMetadata>> GetVisualNovelTagsMetadata(int visualNovelId, SpoilerLevel spoilerLevel = SpoilerLevel.None)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "TagMetadata/visualNovelId?visualNovelId=" + visualNovelId + "&spoilerLevel=" + spoilerLevel);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<List<TagMetadata>>(data);
        }

        [HttpGet]
        public static async Task<List<GamingPlatform>> GetGamingPlatforms()
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "GamingPlatform");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "GamingPlatform/id?id=" + id);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Language");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Language/id?id=" + id);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Genre");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Genre/id?id=" + id);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovelRating/average?id=" + id);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<(double, int)>(data);
        }

        [HttpPost]
        public static async Task<VisualNovelRating> AddVisualNovelRating(string userId, int visualNovelId, int rating)
        {
            string data = @"";
            HttpClient client = new HttpClient();

            var requestUrl = Globals.API_URL + $"VisualNovelRating?userId={userId}&visualNovelId={visualNovelId}&rating={rating}";

            var str = new StringContent(string.Empty);

            str.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl, str);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.Created)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelRating>(data);
        }

        [HttpGet]
        public static async Task<int> GetVisualNovelUserRating(string userId, int visualNovelId)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client
                .GetAsync(Globals.API_URL + $"VisualNovelRating/GetVisualNovelUserRating/?userId={userId}&VisualNovelId={visualNovelId}");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<int>(data);
        }

        [HttpPut]
        public static async Task<VisualNovelRating> UpdateRatingByUser(string userId, int visualNovelId, int rating)
        {
            string data = @"";
            HttpClient client = new HttpClient();

            var requestUrl = $"{Globals.API_URL}VisualNovelRating/UpdateRatingByUser?userId={userId}&visualNovelId={visualNovelId}&rating={rating}";

            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client.PutAsync(requestUrl, content);
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelRating>(data);
        }

        [HttpDelete]
        public static async Task<string> RemoveRatingByUser(string userId, int visualNovelId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            var requestUrl = $"{Globals.API_URL}VisualNovelRating/RemoveRatingByUser?userId={userId}&visualNovelId={visualNovelId}";

            HttpResponseMessage response = await client.DeleteAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return data;
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

                using HttpClient client = new HttpClient();
                message = await client.PostAsync(Globals.API_URL + $"Author", content);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Author");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Author/id?id=" + id);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Translator");
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "Translator/id?id=" + id);
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
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovel/Search?query=" + query);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovel[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<VisualNovelList> GetVisualNovelList(int listId)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovelList/VisualNovelList?listId=" + listId);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelList>(data);
        }

        [HttpGet]
        public static async Task<List<VisualNovelList>> GetVisualNovelLists(string userId, bool showPrivate)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovelList/VisualNovelLists?userId=" + userId + "&showPrivate=" + showPrivate);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelList[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<List<VisualNovelListEntry>> GetVisualNovelInAnyUserList(string userId, int visualNovelId)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + "VisualNovelList/VisualNovelInAnyUserList?userId=" + userId + "&visualNovelId=" + visualNovelId);
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelListEntry[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<List<VisualNovelListEntry>> GetUserVisualNovelsInLists(string userId, bool showPrivate)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + $"VisualNovelList/UserVisualNovelsInLists?userId={userId}&showPrivate={showPrivate}");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelListEntry[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<List<VisualNovelListEntry>> GetVisualNovelsInList(string userId, int listId)
        {
            string data = @"";
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(Globals.API_URL + $"VisualNovelList/VisualNovelsInList?userId={userId}&listId={listId}");
            HttpContent content = response.Content;
            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }

            return JsonConvert.DeserializeObject<VisualNovelListEntry[]>(data).ToList();
        }

        [HttpPost]
        public static async Task<bool> CreateBaseLists(string userId)
        {
            using HttpClient client = new HttpClient();
            var str = new StringContent(string.Empty);

            str.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Globals.API_URL + $"VisualNovelList/CreateBaseLists?userId={userId}", str);

            if (response.StatusCode == HttpStatusCode.Created)
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public static async Task<VisualNovelList> CreateCustomList(string userId, VisualNovelList visualNovelList)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            var str = new StringContent(string.Empty);
            //visualNovelList.VisualNovelListEntries = new List<VisualNovelListEntry>();
            str.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsJsonAsync(Globals.API_URL + $"VisualNovelList/CreateCustomList?userId={userId}", visualNovelList);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine($"Error Content: {errorContent}");
            }

            return JsonConvert.DeserializeObject<VisualNovelList>(data);
        }

        [HttpPut]
        public static async Task<VisualNovelList> UpdateVisualNovelList(string userId, int listId, VisualNovelList visualNovelList)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            var response = await client.PutAsJsonAsync(Globals.API_URL + $"VisualNovelList?userId={userId}&listId={listId}", visualNovelList);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine($"Error Content: {errorContent}");
            }

            return JsonConvert.DeserializeObject<VisualNovelList>(data);
        }

        [HttpPut]
        public static async Task DeleteVisualNovelList(string userId, int listId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            var response = await client.DeleteAsync(Globals.API_URL + $"VisualNovelList/DeleteList?userId={userId}&listId={listId}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode}");
                Console.WriteLine($"Error Content: {errorContent}");
            }

            return;
        }

        [HttpPost]
        public static async Task<bool> AddToList(string userId, int listId, int visualNovelId)
        {
            try
            {
                var requestUrl = Globals.API_URL + $"VisualNovelList/AddToList?userId={userId}&listId={listId}&visualNovelId={visualNovelId}";

                var str = new StringContent(string.Empty);

                str.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                using HttpClient client = new HttpClient();

                var message = await client.PostAsync(requestUrl, str);

                if (message.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpDelete]
        public static async Task<bool> RemoveFromList(string userId, int listId, int visualNovelId)
        {
            try
            {
                HttpResponseMessage message;

                using HttpClient client = new HttpClient();
                message = await client.DeleteAsync(Globals.API_URL + $"VisualNovelList/RemoveFromList?userId={userId}&listId={listId}&visualNovelId={visualNovelId}");

                if (message.IsSuccessStatusCode)
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
