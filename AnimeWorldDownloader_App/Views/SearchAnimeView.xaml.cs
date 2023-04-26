using System.Windows;
using AnimeWorldDownloader_App.Models;
using AnimeWorldDownloader_App.ViewModels;
using System.Collections.ObjectModel;
//using Windows.UI.ApplicationSettings;
using AnimeWorldDownloader_App.Views;

namespace AnimeWorldDownloader_App
{

    public partial class MainPage : ContentPage
    {
        SearchAnimeViewModel searchAnimeViewModel;
       // Task task = new(searchAnimeViewModel.GetSearchAnimeAsync());

        public MainPage()
        {
            InitializeComponent();

            // istanzia il view-model
            searchAnimeViewModel = new();
            // assegna il view-model alla proprietà BindingContext del ContentPage
            this.BindingContext = searchAnimeViewModel;
        }

        private void OnButtonSearchClicked(object sender, EventArgs e)
        {
            searchAnimeViewModel.GetSearchAnime();
            //count++;

            //if (count == 1)
            //    CounterBtn.Text = $"Clicked {count} {searchAnimeViewModel.SearchText} time";
            //else
            //    CounterBtn.Text = $"Clicked {count} {searchAnimeViewModel.SearchText} times";

            //SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Da migliorare, per adesso la disattivo
            // searchAnimeViewModel.GetSearchAnime();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            //searchAnimeViewModel.GetSearchAnime();
            searchAnimeViewModel.GetSearchAnimeAsync();
        }

        private void OnButtonClickedGoDetail(object sender, EventArgs e)
        {
            var botton = (Button)sender;
            var item = (AnimeViewModel)botton.BindingContext;
            var uriDetail = item.UriDetail;

            //Application.Current.MainPage.Navigation.PushModalAsync(new AnimeDetailView(uriDetail), true);
            this.Navigation.PushModalAsync(new AnimeDetailView(uriDetail), true);
        }
    }
}