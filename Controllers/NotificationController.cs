using Microsoft.AspNetCore.Mvc;
using NovelSite.Models.Notification;

namespace NovelSite.Controllers
{
    public class NotificationController : Controller
    {
        public IActionResult SuccessToast(string message) 
        {
            return PartialView("_SuccessToast", new ToastViewModel() { Message = message });
        }
        public IActionResult ErrorToast(string message)
        {
            return PartialView("_ErrorToast", new ToastViewModel() { Message = message });
        }
    }
}
