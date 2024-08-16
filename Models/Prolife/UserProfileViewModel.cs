using NovelSite.Models.Novel;

namespace NovelSite.Models.Prolife
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string AvatarUrl { get; set; }
        public List<VisualNovelList> VisualNovelLists { get; set; }
        public List<VisualNovelListEntry> VisualNovelListEntries { get; set; }
    }
}
