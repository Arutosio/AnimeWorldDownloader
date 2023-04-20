using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AnimeWorldDownloader_App.ViewModels
{
    internal class AnimeDetailViewModel : AnimeViewModel
    {
        private string _state;
        private DateTime _dateRelease;
        private int _numEpisodes;

        public AnimeDetailViewModel(string uriDetail)
        {
            GetAnimeInfo(uriDetail);
        }

        public string State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public DateTime DateRelease
        {
            get { return _dateRelease; }
            set
            {
                if (_dateRelease != value)
                {
                    _dateRelease = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public int NumEpisodes
        {
            get { return _numEpisodes; }
            set
            {
                if (_numEpisodes != value)
                {
                    _numEpisodes = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        private void GetAnimeInfo(string uriDetail)
        {
            AnimeDetail animeDetail = new();
            // Recupera i dati dalla sorgente ""
            // supponiamo che i dati siano recuperati da un database
            // Esempio di codice di esempio 
            if (!string.IsNullOrWhiteSpace(uriDetail))
            {
                HttpTalker httpTalker =  HttpTalker.GetInstance();
                string html = httpTalker.GetResoultFromUri(uriDetail);

                string valueTag = HtmlReader.GetTagValue(html, "");

                animeDetail = new AnimeDetail {Name = "Charlie", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", NumEpisodes = 12, State = "Finito", DateRelease = DateTime.Now};

                this.Name = animeDetail.Name;
                this.ImageUrl = animeDetail.ImageUrl;
                this.State = animeDetail.State;
                this.DateRelease = animeDetail.DateRelease;
                this.NumEpisodes = animeDetail.NumEpisodes;
            }
        }
    }
}
