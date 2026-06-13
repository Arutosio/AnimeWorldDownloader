using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;

namespace AnimeWorldDownloader_App.ViewModels
{
    public class AnimeDetailViewModel : AnimeViewModel
    {
        private string _state = string.Empty;
        private string _dateRelease = string.Empty;
        private string _numEpisodes = string.Empty;
        private string _genere = string.Empty;
        private string _time = string.Empty;
        private double _views;
        private string _description = string.Empty;
        private bool _isBusy;

        public string UriDetailParam { get; set; } = string.Empty;

        public async Task InitializeAsync(string uriDetail)
        {
            IsBusy = true;
            try
            {
                UriDetailParam = uriDetail;
                AnimeDetailModel animeDetailModel = await AnimeDetailModel.GetAnimeDetailAsync(uriDetail);

                Name = animeDetailModel.Name;
                UriDetail = animeDetailModel.UriDetail;
                ImageUrl = animeDetailModel.ImageUrl;
                State = animeDetailModel.State;
                DateRelease = animeDetailModel.DateRelease;
                NumEpisodes = animeDetailModel.NumEpisodes;
                Genere = animeDetailModel.Genere;
                Time = animeDetailModel.Time;
                Views = animeDetailModel.Views;
                Description = animeDetailModel.Description;
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Error("Caricamento dettaglio anime fallito", ex, "AnimeDetail");
                Description = $"Errore caricamento: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public string State
        {
            get => _state;
            set { if (_state != value) { _state = value; OnPropertyChanged(); } }
        }

        public string DateRelease
        {
            get => _dateRelease;
            set { if (_dateRelease != value) { _dateRelease = value; OnPropertyChanged(); } }
        }

        public string NumEpisodes
        {
            get => _numEpisodes;
            set { if (_numEpisodes != value) { _numEpisodes = value; OnPropertyChanged(); } }
        }

        public string Genere
        {
            get => _genere;
            set { if (_genere != value) { _genere = value; OnPropertyChanged(); } }
        }

        public string Time
        {
            get => _time;
            set { if (_time != value) { _time = value; OnPropertyChanged(); } }
        }

        public double Views
        {
            get => _views;
            set { if (_views != value) { _views = value; OnPropertyChanged(); } }
        }

        public string Description
        {
            get => _description;
            set { if (_description != value) { _description = value; OnPropertyChanged(); } }
        }
    }
}
