using System.Windows;
using AnimeWorldDownloader_App.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace AnimeWorldDownloader_App.ViewModels
{
    internal class Anime
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    internal class AnimeViewModel : INotifyPropertyChanged
    {
        string _searchText = string.Empty;
        public ObservableCollection<AnimeModel> _animeModels = null;

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

        public ObservableCollection<AnimeModel> AnimeModels
        {
            get { return _animeModels; }
            set
            {
                if (_animeModels != value)
                {
                    _animeModels = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public AnimeViewModel()
        {
            // Recupera i dati dal modello e prepara il viewmodel
            List<Anime> animes = GetAnimeFromDatabase(); // supponiamo che i dati siano recuperati da un database
            AnimeModels = new(animes.Select(a => new AnimeModel(a)).ToList());
        }

        private List<Anime> GetAnimeFromDatabase()
        {
            List<Anime> animes = new();
            // Recupera i dati dal database
            // Esempio di codice di esempio
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                animes.Add(new Anime { Name = "Alice", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg" });
                animes.Add(new Anime { Name = "Bob", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg" });
                animes.Add(new Anime { Name = "Charlie", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg" });
            }

            return animes;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string name = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
