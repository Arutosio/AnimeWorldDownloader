using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace AnimeWorldDownloader_App.ViewModels
{
    internal class SearchAnimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private string _searchText = string.Empty;
        private ObservableCollection<AnimeViewModel> _animeViewModels = new();
        private bool _isBusy;

        public string SearchText
        {
            get => _searchText;
            set { if (_searchText != value) { _searchText = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<AnimeViewModel> AnimeViewModels
        {
            get => _animeViewModels;
            set { if (_animeViewModels != value) { _animeViewModels = value; OnPropertyChanged(); } }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public async Task GetSearchAnimeAsync()
        {
            IsBusy = true;
            try
            {
                List<AnimeModel> animeModels = new();
                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    animeModels = await AnimeModel.GetAnimesAsync(SearchText);
                }

                List<AnimeViewModel> animeViewModels = animeModels.Select(a => new AnimeViewModel(a)).ToList();
                AnimeViewModels = new ObservableCollection<AnimeViewModel>(animeViewModels);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Error("Ricerca anime fallita", ex, "Search");
                AnimeViewModels = new ObservableCollection<AnimeViewModel>();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
