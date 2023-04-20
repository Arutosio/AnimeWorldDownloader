using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader_App.Data
{
    public static class HtmlReader
    {
        internal static List<string> GetItemsWithClass(string strHtml, string strClass, string strPattern)
        {
            List<string> ret = new ();
            // crea un contesto e apre il documento HTML
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(strHtml)).Result;

            // seleziona il div padre e tutti i div figli con la classe "item"
            var parent = document.QuerySelector($"div.{strClass}");
            var items = parent.QuerySelectorAll($"{strPattern}");

            // itera tutti i div "item" e stampa il loro contenuto
            foreach (var item in items)
            {
                ret.Add(item.InnerHtml);
            }

            return ret;
        }

        public static string GetTagValue(string html, string tag)
        {
            var context = BrowsingContext.New(Configuration.Default);
            var document = context.OpenAsync(req => req.Content(html)).Result;
            var element = document.QuerySelector(tag);
            if (element != null)
            {
                return element.InnerHtml;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetImageSrcFromHtml(string htmlString)
        {
            // Creazione del parser HTML
            var parser = new HtmlParser();

            // Parsing del documento HTML
            var document = parser.ParseDocument(htmlString);

            // Recupero del primo elemento immagine e del valore dell'attributo src
            var imageElement = document.QuerySelector("img");
            var imageSrc = imageElement?.GetAttribute("src");

            // Ritorna il valore dell'attributo src
            return imageSrc;
        }

        public static List<string> GetAllTagsFromHtml(string htmlString)
        {
            List<string> tagContents = new List<string>();

            // Crea il parser HTML
            var parser = new HtmlParser();

            // Parsing del documento HTML
            var document = parser.ParseDocument(htmlString);

            // Recupera tutti i tag HTML nel documento
            var tags = document.All;

            // Itera su tutti i tag per estrarre il contenuto di ogni tag
            foreach (var tag in tags)
            {
                // E' necessario verificare se il tag è uno script, in tal caso il contenuto viene ignorato
                if (tag.TagName != "SCRIPT")
                {
                    tagContents.Add(tag.InnerHtml);
                }
            }

            // Ritorna una lista di stringhe contenente il contenuto di tutti i tag HTML nel documento
            return tagContents;
        }


        public static string GetFirstChildTagsFromHtml(string htmlString)
        {
            // Creazione del parser HTML
            var parser = new HtmlParser();

            // Parsing del documento HTML
            var document = parser.ParseDocument(htmlString);

            // Sceglie il primo tag figlio di ogni elemento nel documento
            string firstChildTags = document.All
                .Select(x => x.FirstElementChild?.OuterHtml)
                .Where(x => x != null)
                .Aggregate((current, next) => $"{current}\n{next}");

            // Ritorna una stringa che rappresenta i primi tag figli di ogni elemento nel documento
            return firstChildTags;
        }

        public static List<string> GetLinkHrefsFromHtml(string htmlString)
        {
            List<string> hrefs = new List<string>();

            // Crea il parser HTML
            var parser = new HtmlParser();

            // Parsing del documento HTML
            var document = parser.ParseDocument(htmlString);

            // Recupera tutti i tag "a" nel documento
            var aTags = document.QuerySelectorAll("a");

            // Itera su tutti gli elementi "a" e recupera il valore dell'attributo "href"
            foreach (var aTag in aTags)
            {
                var href = aTag.GetAttribute("href");
                hrefs.Add(href);
            }

            // Ritorna una lista di stringhe contenente tutti i valori degli attributi "href" degli elementi "a" nel documento
            return hrefs;
        }


        public static List<string> GetDivContents(string htmlString)
        {
            List<string> contents = new List<string>();

            // Crea il parser HTML
            var parser = new HtmlParser();

            // Parsing del documento HTML
            var document = parser.ParseDocument(htmlString);

            // Recupera tutti gli elementi "div" con classe "item"
            var divElements = document.QuerySelectorAll(".inner");

            // Itera su tutti gli elementi "div" e recupera il contenuto di ciascuno
            foreach (var divElement in divElements)
            {
                contents.Add(divElement.OuterHtml);
            }

            // Ritorna una lista di stringhe contenente il contenuto di tutti gli elementi "div" con classe "item" nel documento
            return contents;
        }
    }
}
