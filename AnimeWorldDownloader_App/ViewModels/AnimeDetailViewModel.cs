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
        private List<string> _genere;

        public AnimeDetailViewModel(string uriDetail)
        {
            AnimeDetailModel animeDetailModel = GetAnimeDetail(uriDetail);
            this.Name = animeDetailModel.Name;
            this.UriDetail = animeDetailModel.UriDetail;
            this.ImageUrl = animeDetailModel.ImageUrl;
            this.State = animeDetailModel.State;
            this.NumEpisodes = animeDetailModel.NumEpisodes;
            this.Genere = animeDetailModel.Genere;
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

        public List<string> Genere
        {
            get { return _genere; }
            set
            {
                if (_genere != value)
                {
                    _genere = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        private AnimeDetailModel GetAnimeDetail(string uriDetail)
        {
            AnimeDetailModel animeDetailModel = new();
            if (!string.IsNullOrWhiteSpace(uriDetail))
            {
                animeDetailModel = AnimeDetailModel.GetAnimeDetail(uriDetail);
            }
            return animeDetailModel;
        }
    }
}
