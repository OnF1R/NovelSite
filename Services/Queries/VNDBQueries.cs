using Newtonsoft.Json;
using NovelSite.Models;
using System.Text;

namespace NovelSite.Services.Queries
{
    public class VNDBQueries
    {
        private const string API_URL = "https://api.vndb.org/kana/vn";

        public static async Task<VNDBQueryResult<VNDBResult>> SearchOnVNDB(string search, string sort = "searchrank")
        {
            string query = "{\"filters\": [\"search\", \"=\", \"" + search + "\"], \"fields\": \"title, olang, released, rating, length, developers.name\", \"sort\": \"" + sort + "\", \"results\": \"16\"}";
            string data = "";
            using (var client = new HttpClient())
            {
                try
                {
                    HttpContent content = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(API_URL, content);

                    if (response.IsSuccessStatusCode)
                    {
                        data = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            return JsonConvert.DeserializeObject<VNDBQueryResult<VNDBResult>>(data);
        }

        public static async Task<VNDBQueryResult<VNDBResult>> GetRating(string id)
        {
            string query = "{\"filters\": [\"id\", \"=\", \"" + id + "\"], \"fields\": \"rating\"}";
            string data = "";
            using (var client = new HttpClient())
            {
                try
                {
                    HttpContent content = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(API_URL, content);

                    if (response.IsSuccessStatusCode)
                    {
                        data = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            return JsonConvert.DeserializeObject<VNDBQueryResult<VNDBResult>>(data);
        }

        public static async Task<VNDBQueryResult<VNDBResult>> GetRating(List<string> ids)
        {

            string firstHalfQuery = "{\"filters\": [\"or\", ";

            for (int i = 0; i < ids.Count; i++)
            {
                if (i != ids.Count - 1)
                {
                    firstHalfQuery += "[\"id\", \"=\", \"" + ids[i] + "\"], ";
                }
                else
                {
                    firstHalfQuery += "[\"id\", \"=\", \"" + ids[i] + "\"]";
                }
            }

            firstHalfQuery += "], ";

            string secondHaflQuery = "\"fields\": \"rating\"}";
            string query = firstHalfQuery + secondHaflQuery;
            string data = "";
            using (var client = new HttpClient())
            {
                try
                {
                    HttpContent content = new StringContent(query, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync(API_URL, content);

                    if (response.IsSuccessStatusCode)
                    {
                        data = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }
            }

            return JsonConvert.DeserializeObject<VNDBQueryResult<VNDBResult>>(data);
        }
    }
}
