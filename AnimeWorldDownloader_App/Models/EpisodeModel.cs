using AngleSharp;
using AngleSharp.Dom;
using AnimeWorldDownloader_App.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader_App.Models
{
    public class EpisodeModel
    {
        public int NEpisode { get; set; }
        public string UriEpisode { get; set; }
        public string FileLocation { get; set; }

        public static EpisodeModel GetEpisode(Uri urlEpisode, string animePath)
        {
            EpisodeModel episodeModel = new();

            HttpTalker httpTalker = HttpTalker.GetInstance();
            // Recupero la sorgente html
            string html = httpTalker.GetResoultFromUri(urlEpisode.ToString());

            // crea un contesto e apre il documento HTML
            IBrowsingContext context = BrowsingContext.New(Configuration.Default);
            IDocument documentPageEpisode = context.OpenAsync(req => req.Content(html)).Result;

            string relativeUrlDownloadEpisode = documentPageEpisode.QuerySelector("a.btn.btn-primary.p-2").GetAttribute("href").Replace("..", "");
            Uri fullUrlEpisode = new($"http://{urlEpisode.Host}{relativeUrlDownloadEpisode}");


            episodeModel.UriEpisode = fullUrlEpisode.ToString();
            episodeModel.FileLocation = Path.Combine(animePath, Path.GetFileName(urlEpisode.ToString()));

            return episodeModel;
        }
    }
}
