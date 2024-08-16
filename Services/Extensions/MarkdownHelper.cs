using Markdig;

namespace NovelSite.Services.Extensions
{
    public static class MarkdownHelper
    {
        public static string ToHtml(string markdown)
        {
            var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .Build();

            markdown = System.Text.RegularExpressions.Regex.Replace(markdown, @"\|\|(.+?)\|\|", @"<span class=""spoiler"">$1</span>");

            return Markdown.ToHtml(markdown, pipeline);
        }
    }
}
