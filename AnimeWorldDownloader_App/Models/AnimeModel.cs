using AngleSharp;
using AngleSharp.Dom;
using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.ViewModels;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeModel
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string UriDetail { get; set; }

        private static AnimeModel GetAnime(AngleSharp.Dom.IElement eDivItem)
        {
            AnimeModel animeModel = new();
            if (!string.IsNullOrWhiteSpace(eDivItem.InnerHtml))
            {
                //Ottengo la parte del URL mancante
                AngleSharp.Dom.IElement eDivInner = eDivItem.QuerySelector("div.inner");
                AngleSharp.Dom.IElement eAPoster = eDivInner.QuerySelector("a.poster");
                AngleSharp.Dom.IElement eImage = eAPoster.QuerySelector("img");
                AngleSharp.Dom.IElement eAName = eDivInner.QuerySelector("a.name");

                // recupero i valori degli elementi
                animeModel.UriDetail = string.Concat($"https://www.animeworld.tv{eAPoster.GetAttribute("href")}");
                animeModel.ImageUrl = eImage.GetAttribute("src");
                animeModel.Name = eAName.TextContent;
            }

            return animeModel;
        }

        public static List<AnimeModel> GetAnimes(string searchText)
        {
            List<AnimeModel> animeModels = new();
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                string searchTextAdatpting = searchText.Replace(" ", "+");
                string searchUri = $"https://www.animeworld.tv/search?keyword={searchTextAdatpting}";

                HttpTalker httpTalker = HttpTalker.GetInstance();
                // Recupero la sorgente html
                string html = httpTalker.GetResoultFromUri(searchUri);

                // crea un contesto e apre il documento HTML
                IBrowsingContext context = BrowsingContext.New(Configuration.Default);
                IDocument document = context.OpenAsync(req => req.Content(html)).Result;

                // seleziona il div padre e tutti i div figli con la classe "item"
                AngleSharp.Dom.IElement parent = document.QuerySelector($"div.film-list");
                IHtmlCollection<AngleSharp.Dom.IElement> childrens = parent.QuerySelectorAll($"div.item");

                // itera tutti i i contenuti di ogni div "item"
                foreach (AngleSharp.Dom.IElement elements in childrens)
                {
                    animeModels.Add(GetAnime(elements));
                }
            }

            return animeModels;
        }

    }
}
