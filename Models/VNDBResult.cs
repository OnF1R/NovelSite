﻿using Newtonsoft.Json;
using NovelSite.Models.Novel;

namespace NovelSite.Models
{
    public enum DevelopmentStatus
    {
        Finished = 0,
        InDevelopment = 1,
        Canceled = 2
    }

    public enum DeveloperType
    {
        Co,
        In,
        Ng,
    }

    public class VNDBDeveloper
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Original { get; set; }
        public List<string>? Aliases { get; set; }
        [JsonProperty("lang")]
        public string? Language { get; set; }
        public DeveloperType Type { get; set; }
        public string? Description { get; set; }
    }

    public class VNDBTitle
    {
        [JsonProperty("lang")]
        public string? Language { get; set; }
        public string? Title { get; set; }
        public string? Latin { get; set; }
        [JsonProperty("official")]
        public bool? IsOfficial { get; set; }
        [JsonProperty("main")]
        public bool? IsMain { get; set; }
    }

    public class VNDBImage
    {
        public string? Id { get; set; }
        public string? Url { get; set; }
        public int[]? Dims { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }
        public double? Sexual { get; set; }
        public double? Violence { get; set; }
        public int? VoteCount { get; set; }
    }

    public class VNDBScreenshot : VNDBImage
    {
        public VNDBScreenshot() : base()
        {
            if (ThumbnailDims != null)
            {
                ThumbnailWidth = ThumbnailDims[0];
                ThumbnailHeight = ThumbnailDims[1];
            }
        }

        public string? Thumbnail { get; set; }
        [JsonProperty("thumbnail_dims")]
        public int[]? ThumbnailDims { get; set; }
        public int? ThumbnailWidth { get; set; }
        public int? ThumbnailHeight { get; set; }
    }

    public class VNDBQueryResult<T>
    {
        public bool More { get; set; }
        public List<T>? Results { get; set; }
    }
    public class VNDBResult
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? AltTitle { get; set; }
        public List<VNDBTitle>? Titles { get; set; }
        public List<string>? Aliases { get; set; }
        public string? Olang { get; set; }
        public DevelopmentStatus? Devstatus { get; set; }
        public string? Released { get; set; }
        public VNDBImage? Image { get; set; }
        public int? Length { get; set; }
        [JsonProperty("length_minutes")]
        public int? LengthInMinutes { get; set; }
        [JsonProperty("length_votes")]
        public int? LengthVotes { get; set; }
        public string? Description { get; set; }
        public double? Rating { get; set; }
        public int? VoteCount { get; set; }
        public List<VNDBScreenshot>? Screenshots { get; set; }
        public List<VNDBDeveloper>? Developers { get; set; }
        public List<VNDBTag> Tags { get; set; }

        //Todo: Maybe add other https://api.vndb.org/kana#query-format end on relations
    }

    public class VNDBTag
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public List<string>? Aliases { get; set; }
        public string? Description { get; set; }
        public TagCategory? Category { get; set; }
        public bool? Searchable { get; set; }
        public bool? Applicable { get; set; }
        [JsonProperty("vn_count")]
        public int? VisualNovelCount { get; set; }
        public float? Rating { get; set; }
        public SpoilerLevel? Spoiler { get; set; }
        public bool? Lie { get; set; }
    }

    public enum TagCategory
    {
        cont,
        ero,
        tech,
    }
}
