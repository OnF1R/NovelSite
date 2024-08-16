using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NovelSite.Models;
using NovelSite.Models.Comment;
using System.ComponentModel.Design;
using System.Net.Http.Headers;
using System.Text;

namespace NovelSite.Services.Queries
{
    public class CommentQueries
    {
        [HttpGet]
        public static async Task<VisualNovelCommentWithRating> Get(Guid commentId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client
                .GetAsync(Globals.API_URL + $"VisualNovelComment?commentId={commentId}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentWithRating>(data);
        }

        [HttpGet]
        public static async Task<List<VisualNovelCommentWithRating>> GetVisualNovelComments(int visualNovelId)
        {
            try
            {
                string data = @"";
                using HttpClient client = new HttpClient();
                HttpResponseMessage response = await client
                    .GetAsync(Globals.API_URL + $"VisualNovelComment/GetVisualNovelComments?visualNovelId={visualNovelId}");

                if (response.IsSuccessStatusCode)
                {
                    data = await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return null;
                }

                if (!string.IsNullOrEmpty(data))
                {
                    return JsonConvert.DeserializeObject<VisualNovelCommentWithRating[]>(data).ToList();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        public static async Task<List<VisualNovelCommentModel>> GetUserComments(Guid userId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client
                .GetAsync(Globals.API_URL + $"VisualNovelComment/GetVisualNovelComments?userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentModel[]>(data).ToList();
        }

        [HttpGet]
        public static async Task<List<VisualNovelCommentModel>> GetCommentReplies(Guid parentCommentId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client
                .GetAsync(Globals.API_URL + $"VisualNovelComment/GetVisualNovelComments?parentCommentId={parentCommentId}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentModel[]>(data).ToList();
        }

        [HttpPost]
        public static async Task<VisualNovelCommentModel> Add(VisualNovelCommentModel comment)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client
                .PostAsync(Globals.API_URL + $"VisualNovelComment" +
                $"?userId={comment.UserId}&visualNovelId={comment.VisualNovelId}&content={comment.Content}&parentCommentId={comment.ParentCommentId}", content);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentModel>(data);
        }

        [HttpPut]
        public static async Task<VisualNovelCommentModel> Update(VisualNovelCommentModel comment)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client
                .PutAsJsonAsync(Globals.API_URL + $"VisualNovelComment?commentId={comment.Id}", comment);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentModel>(data);
        }

        [HttpDelete]
        public static async Task<VisualNovelCommentModel> Delete(Guid commentId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            HttpResponseMessage response = await client
                .DeleteAsync(Globals.API_URL + $"VisualNovelComment?commentId={commentId}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentModel>(data);
        }

        [HttpGet]
        public static async Task<VisualNovelCommentRatingModel> GetRating(Guid userId, Guid commentId)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client
                .GetAsync(Globals.API_URL + $"VisualNovelCommentRating/ByUserAndCommentId?userId={userId}&commentId={commentId}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return null;
            }

            return JsonConvert.DeserializeObject<VisualNovelCommentRatingModel>(data);
        }

        [HttpPost]
        public static async Task AddRating(Guid userId, Guid commentId, bool isLike)
        {
            string data = @"";
            using HttpClient client = new HttpClient();
            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await client
                .PostAsync(Globals.API_URL + $"VisualNovelCommentRating?userId={userId}&commentId={commentId}&isLike={isLike}", content);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return;
            }
        }

        [HttpPut]
        public static async Task UpdateRating(Guid ratingId, Guid userId, Guid commentId, bool isLike)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            var content = new StringContent(string.Empty);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpResponseMessage response = await client
                .PutAsync(Globals.API_URL + $"VisualNovelCommentRating?ratingId={ratingId}&userId={userId}&commentId={commentId}&isLike={isLike}", content);

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return;
            }
        }

        [HttpDelete]
        public static async Task DeleteRating(Guid id)
        {
            string data = @"";
            using HttpClient client = new HttpClient();

            HttpResponseMessage response = await client
                .DeleteAsync(Globals.API_URL + $"VisualNovelCommentRating?id={id}");

            if (response.IsSuccessStatusCode)
            {
                data = await response.Content.ReadAsStringAsync();
            }
            else
            {
                return;
            }
        }
    }
}
