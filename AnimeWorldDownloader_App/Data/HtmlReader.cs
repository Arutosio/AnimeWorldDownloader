using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace AnimeWorldDownloader_App.Data
{
    public static class HtmlReader
    {
        internal static IHtmlCollection<AngleSharp.Dom.IElement> GetItemsWithClass(string htmlContent, string strClass, string strPattern)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(htmlContent)).Result;

            var parent = document.QuerySelector(strClass);
            return parent?.QuerySelectorAll(strPattern) ?? document.QuerySelectorAll("_none_");
        }

        public static string GetTagValue(string html, string tag)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(html)).Result;
            var element = document.QuerySelector(tag);
            return element?.InnerHtml ?? string.Empty;
        }

        public static string GetImageSrcFromHtml(AngleSharp.Dom.IElement htmlElement)
        {
            var imageElement = htmlElement.QuerySelector("img");
            return imageElement?.GetAttribute("src") ?? string.Empty;
        }

        public static List<string> GetAllTagsFromHtml(string htmlString)
        {
            List<string> tagContents = new();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(htmlString);
            var tags = document.All;

            foreach (var tag in tags)
            {
                if (tag.TagName != "SCRIPT")
                {
                    tagContents.Add(tag.InnerHtml);
                }
            }

            return tagContents;
        }

        public static string GetFirstChildTagsFromHtml(string htmlString)
        {
            var parser = new HtmlParser();
            var document = parser.ParseDocument(htmlString);

            string firstChildTags = document.All
                .Select(x => x.FirstElementChild?.OuterHtml)
                .Where(x => x != null)
                .Aggregate((current, next) => $"{current}\n{next}") ?? string.Empty;

            return firstChildTags;
        }

        public static List<string> GetLinkHrefsFromHtml(AngleSharp.Dom.IElement element)
        {
            List<string> hrefs = new();
            var aTags = element.QuerySelectorAll("a");

            foreach (var aTag in aTags)
            {
                var href = aTag.GetAttribute("href");
                if (href != null)
                    hrefs.Add(href);
            }

            return hrefs;
        }

        public static List<string> GetDivContents(string htmlString)
        {
            List<string> contents = new();
            var parser = new HtmlParser();
            var document = parser.ParseDocument(htmlString);
            var divElements = document.QuerySelectorAll(".inner");

            foreach (var divElement in divElements)
            {
                contents.Add(divElement.OuterHtml);
            }

            return contents;
        }
    }
}
