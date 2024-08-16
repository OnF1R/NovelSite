using Microsoft.AspNetCore.Mvc;
using NovelSite.Services.Queries;

namespace NovelSite.Controllers
{
    public class VisitorController : Controller
    {
        [HttpPut]
        public async Task IncrementPageViewsCount(int visualNovelId)
        {
            var visitorCookieName = $"VisitedVisualNovel_{visualNovelId}";
            var visitorId = Request.Cookies[visitorCookieName];
            if (visitorId == null)
            {
                var newVisitorId = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddYears(1),
                    HttpOnly = true
                };

                Response.Cookies.Append(visitorCookieName, newVisitorId, cookieOptions);

                await HttpQueries.IncrementPageViewsCount(visualNovelId);
            }
        }
    }
}
