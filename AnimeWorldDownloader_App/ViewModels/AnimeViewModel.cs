﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Interop;
using AnimeWorldDownloader_App.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace AnimeWorldDownloader_App.ViewModels
{

    public class AnimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _name = string.Empty;
        private string _imageUrl = string.Empty;
        private string _uriDetail = string.Empty;

        public AnimeViewModel() { }

        public AnimeViewModel(Anime anime)
        {
            this.Name = anime.Name;
            this.ImageUrl = anime.ImageUrl;
            this.UriDetail = anime.UriDetail;
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public string ImageUrl
        {
            get { return _imageUrl; }
            set
            {
                if (_imageUrl != value)
                {
                    _imageUrl = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public string UriDetail
        {
            get { return _uriDetail; }
            set
            {
                if (_uriDetail != value)
                {
                    _uriDetail = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }
    }
}