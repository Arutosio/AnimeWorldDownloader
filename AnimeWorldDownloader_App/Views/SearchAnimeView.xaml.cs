using AnimeWorldDownloader_App.ViewModels;
using AnimeWorldDownloader_App.Views;

namespace AnimeWorldDownloader_App
{
    public partial class MainPage : ContentPage
    {
        private readonly SearchAnimeViewModel _searchAnimeViewModel;

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

        private void OnButtonClickedGoDetail(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var item = (AnimeViewModel)button.BindingContext;
            var uriDetail = item.UriDetail;

            Navigation.PushModalAsync(new AnimeDetailView(uriDetail), true);
        }
    }
}
