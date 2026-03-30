using AngleSharp;
using AnimeWorldDownloader_App.Data;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeModel
    {
        public string Name { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string UriDetail { get; set; } = string.Empty;

        private static AnimeModel GetAnime(AngleSharp.Dom.IElement eDivItem)
        {
            AnimeModel animeModel = new();
            if (!string.IsNullOrWhiteSpace(eDivItem.InnerHtml))
            {
                var eDivInner = eDivItem.QuerySelector("div.inner");
                var eAPoster = eDivInner?.QuerySelector("a.poster");
                var eImage = eAPoster?.QuerySelector("img");
                var eAName = eDivInner?.QuerySelector("a.name");

                animeModel.UriDetail = $"https://www.animeworld.ac{eAPoster?.GetAttribute("href")}";
                animeModel.ImageUrl = eImage?.GetAttribute("src") ?? string.Empty;
                animeModel.Name = eAName?.TextContent ?? string.Empty;
            }

            return animeModel;
        }

        public static async Task<List<AnimeModel>> GetAnimesAsync(string searchText)
        {
            List<AnimeModel> animeModels = new();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string searchTextAdapting = searchText.Replace(" ", "+");
                string searchUri = $"https://www.animeworld.ac/search?keyword={searchTextAdapting}";

                HttpTalker httpTalker = HttpTalker.GetInstance();
                string html = await httpTalker.GetResultFromUriAsync(searchUri);

                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));

                var parent = document.QuerySelector("div.film-list");
                if (parent != null)
                {
                    var childrens = parent.QuerySelectorAll("div.item");
                    foreach (var element in childrens)
                    {
                        animeModels.Add(GetAnime(element));
                    }
                }
            }

            return animeModels;
        }
    }
}
