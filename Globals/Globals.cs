namespace NovelSite
{
    public class Globals
    {
        public const string API_URL = "https://localhost:7022/api/"; // public const string API_URL = "http://localhost:7022/api/"; On Release

        public static string GetFullUrl(HttpContext context)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}{context.Request.QueryString}";
        }
    }
}
