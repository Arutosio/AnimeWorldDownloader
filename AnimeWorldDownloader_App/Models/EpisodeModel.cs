using AngleSharp;
using AnimeWorldDownloader_App.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Web;

namespace AnimeWorldDownloader_App.Models
{
    public class EpisodeModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private bool _isSelected;

        public int NEpisode { get; set; }
        public string UriWatch { get; set; } = string.Empty;
        public string UriDownloadPage { get; set; } = string.Empty;
        public string UriDirectDownload { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        /// <summary>
        /// Risolve il link diretto di download seguendo il flusso:
        /// 1. Fetch pagina episodio -> trova #downloadLink href (download-file.php?id=...)
        /// 2. Dall'URL intermedio, costruisce il link diretto al file .mp4
        ///    Esempio: https://srv18.../download-file.php?id=DDL/ANIME/File.mp4
        ///          -> https://srv18.../DDL/ANIME/File.mp4
        /// Se la costruzione diretta non funziona, fa fetch della pagina intermedia
        /// e cerca il link "Scarica".
        /// </summary>
        public async Task<string> ResolveDirectDownloadUrlAsync()
        {
            if (!string.IsNullOrEmpty(UriDirectDownload))
                return UriDirectDownload;

            HttpTalker httpTalker = HttpTalker.GetInstance();

            // Step 1: fetch pagina episodio per trovare #downloadLink
            if (string.IsNullOrEmpty(UriDownloadPage))
            {
                if (string.IsNullOrEmpty(UriWatch))
                    throw new InvalidOperationException("UriWatch e' vuoto");

                string episodeHtml = await httpTalker.GetResultFromUriAsync(UriWatch);
                var ctx1 = BrowsingContext.New(Configuration.Default);
                var doc1 = await ctx1.OpenAsync(req => req.Content(episodeHtml));

                string? downloadPageHref = doc1.QuerySelector("#downloadLink")?.GetAttribute("href");
                if (string.IsNullOrEmpty(downloadPageHref))
                    throw new InvalidOperationException($"#downloadLink non trovato nella pagina: {UriWatch}");

                UriDownloadPage = downloadPageHref;
            }

            // Step 2: prova a costruire il link diretto dal parametro "id" dell'URL intermedio
            // Esempio: https://srv18.example.org/download-file.php?id=DDL/ANIME/File.mp4
            //       -> https://srv18.example.org/DDL/ANIME/File.mp4
            if (Uri.TryCreate(UriDownloadPage, UriKind.Absolute, out var intermediateUri))
            {
                string? idParam = HttpUtility.ParseQueryString(intermediateUri.Query).Get("id");
                if (!string.IsNullOrEmpty(idParam))
                {
                    string directUrl = $"{intermediateUri.Scheme}://{intermediateUri.Host}/{idParam}";
                    UriDirectDownload = directUrl;
                    UpdateFileLocationFromUrl(directUrl);
                    return UriDirectDownload;
                }
            }

            // Fallback: fetch la pagina intermedia e cerca il bottone "Scarica"
            string intermediateHtml = await httpTalker.GetResultFromUriAsync(UriDownloadPage);
            var ctx2 = BrowsingContext.New(Configuration.Default);
            var doc2 = await ctx2.OpenAsync(req => req.Content(intermediateHtml));

            var downloadButton = doc2.QuerySelector("a.btn.btn-primary")
                               ?? doc2.QuerySelector("a[download]")
                               ?? doc2.QuerySelector("a.btn");

            string? fallbackUrl = downloadButton?.GetAttribute("href");
            if (string.IsNullOrEmpty(fallbackUrl))
                throw new InvalidOperationException(
                    $"Link diretto non trovato. Pagina intermedia: {UriDownloadPage}");

            // Se l'URL e' relativo, risolvi rispetto alla pagina intermedia
            if (!fallbackUrl.StartsWith("http") && intermediateUri != null)
                fallbackUrl = $"{intermediateUri.Scheme}://{intermediateUri.Host}/{fallbackUrl.TrimStart('/')}";

            UriDirectDownload = fallbackUrl;
            UpdateFileLocationFromUrl(fallbackUrl);
            return UriDirectDownload;
        }

        private void UpdateFileLocationFromUrl(string url)
        {
            if (string.IsNullOrEmpty(FileLocation)) return;

            if (Uri.TryCreate(url, UriKind.Absolute, out var parsedUri))
            {
                string fileName = Path.GetFileName(parsedUri.LocalPath);
                if (!string.IsNullOrEmpty(fileName))
                {
                    string dir = Path.GetDirectoryName(FileLocation) ?? string.Empty;
                    FileLocation = Path.Combine(dir, fileName);
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
