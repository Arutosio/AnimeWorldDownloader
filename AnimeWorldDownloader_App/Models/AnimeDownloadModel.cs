using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using AnimeWorldDownloader_App.Data;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeDownloadModel : AnimeModel
    {
        public List<EpisodeModel> EpisodeModels { get; set; } = new();
        public double DownloadProgress { get; set; }

        private void UpdateDownloadProgress(double progress) { DownloadProgress = progress; }
        public AnimeDownloadModel() { }

        public async void DownloadEpisode(EpisodeModel episodeModel)
        {
            HttpTalker httpTalker = HttpTalker.GetInstance();
            await httpTalker.DownloadFileAsync(episodeModel.UriEpisode, episodeModel.FileLocation, UpdateDownloadProgress);
        }

        public static AnimeDownloadModel GetAnimeDownloadModel(string uriDetail)
        {
            AnimeDownloadModel animeDownloadModel = new();

            if (!string.IsNullOrWhiteSpace(uriDetail))
            {
                HttpTalker httpTalker = HttpTalker.GetInstance();
                // Recupero la sorgente html
                string html = httpTalker.GetResoultFromUri(uriDetail);

                // crea un contesto e apre il documento HTML
                IBrowsingContext context = BrowsingContext.New(Configuration.Default);
                IDocument document = context.OpenAsync(req => req.Content(html)).Result;

                // valorizazione il nome
                animeDownloadModel.Name = document.QuerySelector("h2.title").TextContent;

                // valorizazione immagine
                AngleSharp.Dom.IElement eDivDelImag = document.QuerySelector("#mobile-thumbnail-watch");
                animeDownloadModel.ImageUrl = eDivDelImag.QuerySelector("img").GetAttribute("src");

                // valorizazione Uri
                animeDownloadModel.UriDetail = uriDetail;

                // seleziona il div padre contenente tutti i div range
                animeDownloadModel.EpisodeModels = AnimeDownloadModel.GetEpisodes(document, animeDownloadModel.AnimeDirectoryPath());
            }

            return animeDownloadModel;
        }

        //Directory.GetCurrentDirectory()
        private string AnimeDirectoryPath() { return Path.Combine(@"D:\GitHub\AnimeWorldDownloader\AnimeWorldDownloader_App\bin", Name.Replace(" ", "")); }

        private static List<EpisodeModel> GetEpisodes(IDocument document, string animePath)
        {
            AngleSharp.Dom.IElement eServerEpisodeActive = document.QuerySelector("div.server.active");
            IHtmlCollection<AngleSharp.Dom.IElement> eLiEpisode = eServerEpisodeActive.QuerySelectorAll("li.episode");
            
            List<EpisodeModel> episodeModels = new(eLiEpisode.Length);

            // itera tutti i i contenuti di ogni div "item"
            for (int i = 0; eLiEpisode.Length > i; i++)
            {

                //AngleSharp.Dom.IElement  = eLiEpisode[i].QuerySelector($"[data-comment='{i}']");
                AngleSharp.Dom.IElement eA = eLiEpisode[i].QuerySelector("a");
                string href = eA.GetAttribute("href");
                string urlPageEpisode = $"https://www.animeworld.tv{href}";

                HttpTalker httpTalker = HttpTalker.GetInstance();
                // Recupero la sorgente html
                string html = httpTalker.GetResoultFromUri(urlPageEpisode);

                // crea un contesto e apre il documento HTML
                IBrowsingContext context = BrowsingContext.New(Configuration.Default);
                IDocument documentPageEpisode = context.OpenAsync(req => req.Content(html)).Result;

                AngleSharp.Dom.IElement eidownloadLink = documentPageEpisode.QuerySelector("#downloadLink");
                Uri urlDowloadEpisode = new(eidownloadLink.GetAttribute("href"));
                
                EpisodeModel episodeModel = EpisodeModel.GetEpisode(urlDowloadEpisode, animePath);

                episodeModels.Insert(i, episodeModel);
            }

            return episodeModels;
        }
    }
}
