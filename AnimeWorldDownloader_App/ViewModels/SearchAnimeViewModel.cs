using System.Windows;
using AnimeWorldDownloader_App.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AnimeWorldDownloader_App.Data;

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
            List<AnimeModel> animeModels = new();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                animeModels = AnimeModel.GetAnimes(SearchText);
                // animeModels = AnimeModel.GetAnimeResultsSync(SearchText);
            }

            List<AnimeViewModel> animeViewModels = animeModels.Select(a => new AnimeViewModel(a)).ToList();
            AnimeViewModels = new ObservableCollection<AnimeViewModel>(animeViewModels);
        }
    }
}
