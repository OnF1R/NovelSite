using Microsoft.AspNetCore.Identity;

namespace NovelSite.Data.Identity
{
    public class ApplicationIdentityUser : IdentityUser
    {
        public string? AvatarFileName { get; set; }
    }
}
