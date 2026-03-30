using AngleSharp;
using AnimeWorldDownloader_App.Data;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeDetailModel : AnimeModel
    {
        public string State { get; set; } = string.Empty;
        public string NumEpisodes { get; set; } = string.Empty;
        public string DateRelease { get; set; } = string.Empty;
        public string Genere { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public double Views { get; set; }
        public string Description { get; set; } = string.Empty;

        public static async Task<AnimeDetailModel> GetAnimeDetailAsync(string uriDetail)
        {
            AnimeDetailModel animeDetailModel = new();

            if (!string.IsNullOrWhiteSpace(uriDetail))
            {
                HttpTalker httpTalker = HttpTalker.GetInstance();
                string html = await httpTalker.GetResultFromUriAsync(uriDetail);

                var context = BrowsingContext.New(Configuration.Default);
                var document = await context.OpenAsync(req => req.Content(html));

                animeDetailModel.Name = document.QuerySelector("h2.title")?.TextContent ?? string.Empty;

                var eDivDelImag = document.QuerySelector("#mobile-thumbnail-watch");
                animeDetailModel.ImageUrl = eDivDelImag?.QuerySelector("img")?.GetAttribute("src") ?? string.Empty;

                animeDetailModel.Description = document.QuerySelector("div.desc")?.InnerHtml ?? string.Empty;

                animeDetailModel.UriDetail = uriDetail;

                var eDLs = document.QuerySelectorAll("dl.meta.col-sm-6");

                foreach (var eDL in eDLs)
                {
                    var eDTs = document.QuerySelectorAll("dt");
                    var eDDs = document.QuerySelectorAll("dd");
                    for (int i = 0; eDTs.Length > i; i++)
                    {
                        string valueCase = eDTs[i].InnerHtml.ToUpper();

                        if (valueCase.Contains("DATA DI USCITA"))
                        {
                            animeDetailModel.DateRelease = eDDs[i].InnerHtml;
                        }
                        if (valueCase.Contains("GENERE"))
                        {
                            var genreLinks = eDDs[i].QuerySelectorAll("a");
                            List<string> generi = genreLinks.Select(g => g.InnerHtml).ToList();
                            animeDetailModel.Genere = string.Join(", ", generi);
                        }
                        if (valueCase.Contains("DURATA"))
                        {
                            animeDetailModel.Time = eDDs[i].InnerHtml;
                        }
                        if (valueCase.Contains("EPISODI"))
                        {
                            animeDetailModel.NumEpisodes = eDDs[i].InnerHtml;
                        }
                        if (valueCase.Contains("STATO") && string.IsNullOrEmpty(animeDetailModel.State))
                        {
                            animeDetailModel.State = eDDs[i].InnerHtml.Trim().Replace("\\n", "");
                        }
                        if (valueCase.Contains("VISUALIZZAZIONI"))
                        {
                            if (double.TryParse(eDDs[i].InnerHtml, out double views))
                                animeDetailModel.Views = views;
                        }
                    }
                }
            }

            return animeDetailModel;
        }
    }
}
