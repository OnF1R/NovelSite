using System.ComponentModel.DataAnnotations;

namespace NovelSite.Models.Novel
{
    public class VisualNovelListType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsMutuallyExclusive { get; set; }
    }

    public class VisualNovelList
    {
        public int Id { get; set; }
        [Display(Name = "Название списка")]
        [Required(ErrorMessage = "Не указано название списка")]
        public string Name { get; set; }
        public bool IsCustom { get; set; }
        [Display(Name = "Приватный список")]
        public bool IsPrivate { get; set; }
        public string UserId { get; set; }
        public VisualNovelListType? ListType { get; set; }
        public virtual ICollection<VisualNovelListEntry>? VisualNovelListEntries { get; set; }
    }

    public class VisualNovelListEntry
    {
        public int Id { get; set; }
        public VisualNovelList VisualNovelList { get; set; }
        public VisualNovel VisualNovel { get; set; }
        public DateTime AddingTime { get; set; }
    }
}
