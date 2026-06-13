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
        private readonly AppLogger _log = AppLogger.Instance;

        public int NEpisode { get; set; }
        // Etichetta numero come sul sito (es. "6" o "6.5"); usata per display e nome file
        public string NumberLabel { get; set; } = string.Empty;
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
            string src = $"Ep.{NEpisode}";

            if (!string.IsNullOrEmpty(UriDirectDownload))
            {
                _log.Debug($"URL diretto gia' risolto: {UriDirectDownload}", src);
                return UriDirectDownload;
            }

            HttpTalker httpTalker = HttpTalker.GetInstance();

            // Step 1: fetch pagina episodio per trovare #downloadLink
            if (string.IsNullOrEmpty(UriDownloadPage))
            {
                if (string.IsNullOrEmpty(UriWatch))
                {
                    _log.Error("UriWatch e' vuoto, impossibile risolvere download URL", src);
                    throw new InvalidOperationException("UriWatch e' vuoto");
                }

                _log.Info($"Step 1: Fetch pagina episodio: {UriWatch}", src);
                string episodeHtml = await httpTalker.GetResultFromUriAsync(UriWatch);
                _log.Debug($"HTML episodio ricevuto: {episodeHtml.Length} chars", src);

                var ctx1 = BrowsingContext.New(Configuration.Default);
                var doc1 = await ctx1.OpenAsync(req => req.Content(episodeHtml));

                string? downloadPageHref = doc1.QuerySelector("#downloadLink")?.GetAttribute("href");

                if (string.IsNullOrEmpty(downloadPageHref))
                {
                    // Log HTML parziale per debug
                    string htmlPreview = episodeHtml.Length > 2000
                        ? episodeHtml[..2000] + "... [TRONCATO]"
                        : episodeHtml;
                    _log.Error($"#downloadLink non trovato nella pagina: {UriWatch}", src);
                    _log.Debug($"HTML preview della pagina (per debug):\n{htmlPreview}", src);

                    // Cerca selettori alternativi
                    var allLinks = doc1.QuerySelectorAll("a[href]");
                    _log.Debug($"Numero totale link nella pagina: {allLinks.Length}", src);
                    foreach (var link in allLinks)
                    {
                        string? href = link.GetAttribute("href");
                        string? id = link.GetAttribute("id");
                        string? cls = link.GetAttribute("class");
                        if (href != null && (href.Contains("download") || href.Contains("DDL") || href.Contains(".mp4")))
                            _log.Debug($"  Link potenziale: id={id ?? "null"}, class={cls ?? "null"}, href={href}", src);
                    }

                    throw new InvalidOperationException($"#downloadLink non trovato nella pagina: {UriWatch}");
                }

                _log.Info($"#downloadLink trovato: {downloadPageHref}", src);
                UriDownloadPage = downloadPageHref;
            }

            // Step 2: prova a costruire il link diretto dal parametro "id" dell'URL intermedio
            _log.Info($"Step 2: Parsing URL intermedio: {UriDownloadPage}", src);
            if (Uri.TryCreate(UriDownloadPage, UriKind.Absolute, out var intermediateUri))
            {
                string? idParam = HttpUtility.ParseQueryString(intermediateUri.Query).Get("id");
                if (!string.IsNullOrEmpty(idParam))
                {
                    string directUrl = $"{intermediateUri.Scheme}://{intermediateUri.Host}/{idParam}";
                    _log.Info($"URL diretto costruito da parametro 'id': {directUrl}", src);
                    UriDirectDownload = directUrl;
                    UpdateFileLocationFromUrl(directUrl);
                    return UriDirectDownload;
                }
                _log.Warn($"Parametro 'id' non trovato in query string: {intermediateUri.Query}", src);
            }
            else
            {
                _log.Warn($"URL intermedio non e' un URI assoluto valido: {UriDownloadPage}", src);
            }

            // Fallback: fetch la pagina intermedia e cerca il bottone "Scarica"
            _log.Info($"Step 3 (Fallback): Fetch pagina intermedia: {UriDownloadPage}", src);
            string intermediateHtml = await httpTalker.GetResultFromUriAsync(UriDownloadPage);
            var ctx2 = BrowsingContext.New(Configuration.Default);
            var doc2 = await ctx2.OpenAsync(req => req.Content(intermediateHtml));

            var downloadButton = doc2.QuerySelector("a.btn.btn-primary")
                               ?? doc2.QuerySelector("a[download]")
                               ?? doc2.QuerySelector("a.btn");

            string? fallbackUrl = downloadButton?.GetAttribute("href");
            if (string.IsNullOrEmpty(fallbackUrl))
            {
                _log.Error($"Nessun bottone download trovato nella pagina intermedia: {UriDownloadPage}", src);
                _log.Debug($"HTML pagina intermedia:\n{intermediateHtml[..Math.Min(2000, intermediateHtml.Length)]}", src);
                throw new InvalidOperationException(
                    $"Link diretto non trovato. Pagina intermedia: {UriDownloadPage}");
            }

            _log.Info($"Fallback URL trovato: {fallbackUrl}", src);

            // Se l'URL e' relativo, risolvi rispetto alla pagina intermedia
            if (!fallbackUrl.StartsWith("http") && intermediateUri != null)
            {
                fallbackUrl = $"{intermediateUri.Scheme}://{intermediateUri.Host}/{fallbackUrl.TrimStart('/')}";
                _log.Debug($"URL relativo risolto a: {fallbackUrl}", src);
            }

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
