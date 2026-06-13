using AngleSharp;
using AnimeWorldDownloader_App.Data;
using System.Text.RegularExpressions;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeDownloadModel : AnimeModel
    {
        public List<EpisodeModel> EpisodeModels { get; set; } = new();
        public double DownloadProgress { get; set; }
        public string Year { get; set; } = string.Empty;
        public string DownloadFolderPath { get; set; } = string.Empty;

        public static async Task<AnimeDownloadModel> GetAnimeDownloadModelAsync(string uriDetail)
        {
            var log = AppLogger.Instance;
            AnimeDownloadModel model = new();

            if (!string.IsNullOrWhiteSpace(uriDetail))
            {
                log.Info($"Caricamento anime da: {uriDetail}", "AnimeDownloadModel");

                HttpTalker httpTalker = HttpTalker.GetInstance();
                string html = await httpTalker.GetResultFromUriAsync(uriDetail);
                log.Debug($"HTML pagina dettaglio: {html.Length} chars", "AnimeDownloadModel");

                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));

                model.Name = document.QuerySelector("h2.title")?.TextContent ?? string.Empty;
                log.Info($"Titolo anime: '{model.Name}'", "AnimeDownloadModel");

                var eDivDelImag = document.QuerySelector("#mobile-thumbnail-watch");
                model.ImageUrl = eDivDelImag?.QuerySelector("img")?.GetAttribute("src") ?? string.Empty;

                model.UriDetail = uriDetail;
                model.Year = ExtractYear(document);
                log.Debug($"Anno: {model.Year}", "AnimeDownloadModel");

                // Cartella: [BasePath configurabile]\NomeAnime (Anno)\
                string folderName = SanitizeName($"{model.Name} ({model.Year})");
                model.DownloadFolderPath = Path.Combine(AppSettings.DownloadBasePath, folderName);
                log.Info($"Cartella download: {model.DownloadFolderPath}", "AnimeDownloadModel");

                // Nome file base: Nome_Anime (underscore al posto degli spazi)
                string fileNameBase = SanitizeName(model.Name).Replace(' ', '_');

                string baseUrl = GetBaseUrl(uriDetail);
                model.EpisodeModels = GetEpisodes(document, baseUrl, model.DownloadFolderPath, fileNameBase);
                log.Info($"Episodi trovati: {model.EpisodeModels.Count}", "AnimeDownloadModel");

                foreach (var ep in model.EpisodeModels)
                    log.Debug($"  Ep.{ep.NEpisode}: UriWatch={ep.UriWatch}", "AnimeDownloadModel");
            }
            else
            {
                log.Warn("uriDetail vuoto o nullo", "AnimeDownloadModel");
            }

            return model;
        }

        private static string ExtractYear(AngleSharp.Dom.IDocument document)
        {
            var eDTs = document.QuerySelectorAll("dt");
            var eDDs = document.QuerySelectorAll("dd");

            for (int i = 0; i < eDTs.Length && i < eDDs.Length; i++)
            {
                string label = eDTs[i].InnerHtml.ToUpper();
                if (label.Contains("DATA DI USCITA"))
                {
                    var match = Regex.Match(eDDs[i].InnerHtml, @"\d{4}");
                    if (match.Success)
                        return match.Value;
                }
            }

            return "Sconosciuto";
        }

        private static string ParseEpisodeLabel(AngleSharp.Dom.IElement? eA, int index)
        {
            string? raw = eA?.GetAttribute("data-episode-num")
                       ?? eA?.GetAttribute("data-num")
                       ?? eA?.TextContent;

            if (!string.IsNullOrWhiteSpace(raw))
            {
                // Estrae numero anche decimale (es. "6.5")
                var match = Regex.Match(raw, @"\d+(?:\.\d+)?");
                if (match.Success)
                    return match.Value;
            }

            return (index + 1).ToString();
        }

        // Parte intera dell'etichetta, per ordinamento/log (NEpisode)
        private static int IntPart(string label)
        {
            var match = Regex.Match(label, @"\d+");
            return match.Success && int.TryParse(match.Value, out int n) ? n : 0;
        }

        // Suffisso file con parte intera a 2 cifre, mantenendo l'eventuale decimale: "06" / "06.5"
        private static string FormatFileSuffix(string label)
        {
            string[] parts = label.Split('.');
            string intPadded = int.TryParse(parts[0], out int n) ? n.ToString("D2") : parts[0];
            return parts.Length > 1 ? $"{intPadded}.{parts[1]}" : intPadded;
        }

        private static string SanitizeName(string name)
        {
            char[] invalid = Path.GetInvalidFileNameChars();
            foreach (char c in invalid)
                name = name.Replace(c, '_');
            return name.Trim();
        }

        private static string GetBaseUrl(string url)
        {
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
                return $"{uri.Scheme}://{uri.Host}";
            return "https://www.animeworld.ac";
        }

        private static List<EpisodeModel> GetEpisodes(AngleSharp.Dom.IDocument document, string baseUrl, string animePath, string fileNameBase)
        {
            var eServerEpisodeActive = document.QuerySelector("div.server.active");
            if (eServerEpisodeActive == null)
                return new List<EpisodeModel>();

            var eLiEpisode = eServerEpisodeActive.QuerySelectorAll("li.episode");
            List<EpisodeModel> episodeModels = new(eLiEpisode.Length);

            for (int i = 0; i < eLiEpisode.Length; i++)
            {
                var eA = eLiEpisode[i].QuerySelector("a");
                string? href = eA?.GetAttribute("href");
                if (string.IsNullOrEmpty(href)) continue;

                string episodePageUrl = href.StartsWith("http")
                    ? href
                    : $"{baseUrl}{href}";

                // Numero reale dal sito (data-episode-num): gli episodi sono
                // divisi in range, l'indice DOM non corrisponde al numero
                // quando la numerazione non parte da 1 o ha buchi/speciali.
                string numberLabel = ParseEpisodeLabel(eA, i);
                int epNum = IntPart(numberLabel);
                // Nome file: NomeAnime_Ep_01.mp4 (o _Ep_06.5.mp4 per i decimali)
                string fileName = $"{fileNameBase}_Ep_{FormatFileSuffix(numberLabel)}.mp4";

                var episodeModel = new EpisodeModel
                {
                    NEpisode = epNum,
                    NumberLabel = numberLabel,
                    UriWatch = episodePageUrl,
                    FileLocation = Path.Combine(animePath, fileName)
                };

                episodeModels.Add(episodeModel);
            }

            return episodeModels;
        }
    }
}
