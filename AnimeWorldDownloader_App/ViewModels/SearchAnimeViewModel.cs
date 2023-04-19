﻿using System.Windows;
using AnimeWorldDownloader_App.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AnimeWorldDownloader_App.ViewModels
{

    internal class SearchAnimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _searchText = string.Empty;
        private ObservableCollection<AnimeViewModel> _animeViewModels = new();

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public ObservableCollection<AnimeViewModel> AnimeViewModels
        {
            get { return _animeViewModels; }
            set
            {
                if (_animeViewModels != value)
                {
                    _animeViewModels = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public SearchAnimeViewModel()
        {
        }

        public void GetSearchAnime()
        {
            List<Anime> animes = new();
            // Recupera i dati dalla sorgente ""
            // supponiamo che i dati siano recuperati da un database
            // Esempio di codice di esempio 
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                animes.Add(new Anime { Name = "Alice", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", UriDetail = "strin_uriDetail" });
                animes.Add(new Anime { Name = "Bob", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", UriDetail = "strin_uriDetail" });
                animes.Add(new Anime { Name = "Charlie", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", UriDetail = "strin_uriDetail" });
            }

            List<AnimeViewModel> tmpAnimeViewModels = animes.Select(a => new AnimeViewModel(a)).ToList();
            AnimeViewModels = new ObservableCollection<AnimeViewModel>(tmpAnimeViewModels);
        }
    }
}