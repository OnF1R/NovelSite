using Microsoft.AspNetCore.Mvc;
using NovelSite.Services.Extensions;
using System.ComponentModel.DataAnnotations;

namespace NovelSite.Models.Novel
{
    public class VisualNovelFromJsonRequest
    {
        [Required(ErrorMessage = "Не указано название ссылки для визуальной новеллы")]
        [RegularExpression(@"^[a-z0-9 -]+$", ErrorMessage = "Некорректный формат")]
        [Remote(action: "CheckLinkName", controller: "NovelConfirmation", ErrorMessage = "Такое название уже используется")]
        public string LinkName { get; set; }
        public string? SoundtrackYoutubePlaylistLink { get; set; }
        public IFormFile VisualNovelJson { get; set; }
        [Required(ErrorMessage = "Нет обложки визуальной новеллы")]
        public IFormFile? CoverImage { get; set; } = null;
        public IFormFile? BackgroundImage { get; set; } = null;
        public List<IFormFile>? Screenshots { get; set; } = null;
    }

    public class VisualNovelRequest
    {
        [Required(ErrorMessage = "Не указано название визуальной новеллы")]
        public string Title { get; set; }
        public string? VndbId { get; set; }
        [Required(ErrorMessage = "Не указано название ссылки для визуальной новеллы")]
        [RegularExpression(@"^[a-z0-9 -]+$", ErrorMessage = "Некорректный формат")]
        [Remote(action: "CheckLinkName", controller: "NovelConfirmation", ErrorMessage = "Такое название уже используется")]
        public string LinkName { get; set; }
        public List<string>? AnotherTitles { get; set; }
        [Required(ErrorMessage = "Нет обложки визуальной новеллы")]
        public IFormFile? CoverImage { get; set; } = null;
        public IFormFile? BackgroundImage { get; set; } = null;
        public List<IFormFile>? Screenshots { get; set; } = null;
        public Status Status { get; set; }
        public List<int>? Platforms { get; set; }
        public ReadingTime ReadingTime { get; set; }
        public List<int>? Translator { get; set; }
        public List<int>? Authors { get; set; }
        [Required(ErrorMessage = "Не указаны жанры визуальной новеллы")]
        public List<int>? Genres { get; set; }
        public List<int>? NoneSpoilerTags { get; set; }
        public List<int>? MinorSpoilerTags { get; set; }
        public List<int>? MajorSpoilerTags { get; set; }
        [Required(ErrorMessage = "Не указаны языки визуальной новеллы")]
        public List<int>? Languages { get; set; }
        [Required(ErrorMessage = "Не указал год выхода визуальной новеллы")]
        public ushort? ReleaseYear { get; set; }
        public ushort? ReleaseMonth { get; set; }
        public ushort? ReleaseDay { get; set; }
        public DateTime? DateAdded { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Guid AdddeUserId { get; set; }
        public string AddedUserName { get; set; }
        [Required(ErrorMessage = "Не указано описание визуальной новеллы")]
        public string Description { get; set; }
        public List<int>? DownloadLinkGamingPlatformId { get; set; }
        public List<string>? DownloadLinkUrl { get; set; }
        public List<string>? OtherLinkName { get; set; }
        public List<string>? OtherLinkUrl { get; set; }
        public List<int>? RelatedNovels { get; set; }
        public List<string>? RelatedAnimeLinkName { get; set; }
        public List<string>? RelatedAnimeLinkUrl { get; set; }
        public string? SteamLink { get; set; }
        public string? TranslateLinkForSteam { get; set; }
        public string? SoundtrackYoutubePlaylistLink { get; set; }
    }
}
