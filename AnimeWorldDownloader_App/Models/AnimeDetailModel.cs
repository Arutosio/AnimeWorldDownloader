using AngleSharp.Dom;
using AngleSharp;
using AnimeWorldDownloader_App.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Globalization;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeDetailModel : AnimeModel
    {
        public string State { get; set; }
        public int NumEpisodes { get; set; }
        public string DateRelease { get; set; }
        public List<string> Genere { get; set; }
        public string Time { get; set; }
        public double Views { get; set; }

        public static AnimeDetailModel GetAnimeDetail(string uriDetail)
        {
            AnimeDetailModel animeDetailModel = new();

            if (!string.IsNullOrWhiteSpace(uriDetail))
            {
                HttpTalker httpTalker = HttpTalker.GetInstance();
                // Recupero la sorgente html
                string html = httpTalker.GetResoultFromUri(uriDetail);

                // crea un contesto e apre il documento HTML
                IBrowsingContext context = BrowsingContext.New(Configuration.Default);
                IDocument document = context.OpenAsync(req => req.Content(html)).Result;

                // valorizazione il nome
                animeDetailModel.Name = document.QuerySelector("h2.title").TextContent;

                // valorizazione immagine
                AngleSharp.Dom.IElement eDivDelImag = document.QuerySelector("#mobile-thumbnail-watch");
                animeDetailModel.ImageUrl = eDivDelImag.QuerySelector("img").GetAttribute("src");
 
                // valorizazione Uri
                animeDetailModel.UriDetail = uriDetail;

                // seleziona il div padre e tutti i div figli con la classe "item"
                IHtmlCollection<AngleSharp.Dom.IElement> eDLs = document.QuerySelectorAll("dl.meta.col-sm-6");

                // itera tutti i i contenuti di ogni div "item"
                foreach (AngleSharp.Dom.IElement eDL in eDLs)
                {
                    IHtmlCollection<AngleSharp.Dom.IElement> eDTs = document.QuerySelectorAll("dt");
                    IHtmlCollection<AngleSharp.Dom.IElement> eDDs = document.QuerySelectorAll("dd");
                    for (int i = 0; eDTs.Length > i; i++)
                    {
                        string valueCase = eDTs[i].InnerHtml.ToUpper();

                        // Casi del primo elemento DL
                        if (valueCase.Contains("CATEGORIA")) { }
                        if (valueCase.Contains("AUDIO")) { }
                        if (valueCase.Contains("DATA DI USCITA"))
                        {
                            //animeDetailModel.DateRelease = DateTime.ParseExact(eDDs[i].InnerHtml, "dd MMMM yyyy", CultureInfo.InvariantCulture);
                            animeDetailModel.DateRelease = eDDs[i].InnerHtml;
                        }
                        if (valueCase.Contains("STAGIONE")) { }
                        if (valueCase.Contains("STUDIO")) { }
                        if (valueCase.Contains("GENERE"))
                        {
                            IHtmlCollection<AngleSharp.Dom.IElement> x = eDDs[i].QuerySelectorAll("a");
                            animeDetailModel.Genere = x.Select(g => g.InnerHtml).ToList();
                        }

                        // Casi del secondo elemento DL
                        if (valueCase.Contains("VOTO")) { }
                        if (valueCase.Contains("DURATA"))
                        {
                            animeDetailModel.Time = eDDs[i].InnerHtml;
                        }
                        if (valueCase.Contains("EPISODI"))
                        {
                            animeDetailModel.NumEpisodes = Convert.ToInt32(eDDs[i].InnerHtml);
                        }
                        if (valueCase.Contains("STATO") && string.IsNullOrEmpty(animeDetailModel.State))
                        {
                            animeDetailModel.State = eDDs[i].InnerHtml.Trim().Replace("\\n", "");
                        }
                        if (valueCase.Contains("VISUALIZZAZIONI"))
                        {
                            animeDetailModel.Views = Convert.ToDouble(eDDs[i].InnerHtml);
                        }
                    }
                }
            }

            return animeDetailModel;
        }
    }
}
