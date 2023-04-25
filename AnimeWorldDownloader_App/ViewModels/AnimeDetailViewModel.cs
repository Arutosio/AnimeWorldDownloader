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
    public class AnimeDetailViewModel : AnimeViewModel
    {
        private string _state;
        private string _dateRelease;
        private string _numEpisodes;
        private string _genere;
        private string _time;
        private double _views;
        private string _description;

        public AnimeDetailViewModel(string uriDetail)
        {
            AnimeDetailModel animeDetailModel = GetAnimeDetail(uriDetail);
            this.Name = animeDetailModel.Name;
            this.UriDetail = animeDetailModel.UriDetail;
            this.ImageUrl = animeDetailModel.ImageUrl;
            this.State = animeDetailModel.State;
            this.DateRelease = animeDetailModel.DateRelease;
            this.NumEpisodes = animeDetailModel.NumEpisodes;
            this.Genere = string.Join(", ", animeDetailModel.Genere);
            this.Time = animeDetailModel.Time;
            this.Views = animeDetailModel.Views;
            this.Description = animeDetailModel.Description;
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

        public string DateRelease
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

        public string NumEpisodes
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

        public string Genere
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

        public string Time
        {
            get { return _time; }
            set
            {
                if (_time != value)
                {
                    _time = value;
                }
            }
        }

        public double Views
        {
            get { return _views; }
            set
            {
                if (_views != value)
                {
                    _views = value;
                }
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
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
