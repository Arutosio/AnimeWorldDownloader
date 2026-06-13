using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.ViewModels;
using AnimeWorldDownloader_App.Views;

namespace AnimeWorldDownloader_App
{
    public partial class MainPage : ContentPage
    {
        private readonly SearchAnimeViewModel _searchAnimeViewModel;
        private bool _isNavigating;

        public MainPage()
        {
            InitializeComponent();
            _searchAnimeViewModel = new();
            BindingContext = _searchAnimeViewModel;
        }

        private async void OnButtonSearchClicked(object sender, EventArgs e)
        {
            await _searchAnimeViewModel.GetSearchAnimeAsync();
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Da migliorare, per adesso la disattivo
        }

        private async void Entry_Completed(object sender, EventArgs e)
        {
            await _searchAnimeViewModel.GetSearchAnimeAsync();
        }

        private async void OnButtonClickedGoDetail(object sender, EventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;
            try
            {
                var button = (Button)sender;
                var item = (AnimeViewModel)button.BindingContext;
                var uriDetail = item.UriDetail;

                if (string.IsNullOrEmpty(uriDetail)) return;

                await Navigation.PushModalAsync(new AnimeDetailView(uriDetail), true);
            }
            catch (Exception ex)
            {
                AppLogger.Instance.Error("Navigazione al dettaglio fallita", ex, "Navigation");
            }
            finally
            {
                _isNavigating = false;
            }
        }
    }
}
