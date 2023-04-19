using System.Windows;
using AnimeWorldDownloader_App.Models;
using AnimeWorldDownloader_App.ViewModels;
using System.Collections.ObjectModel;

namespace AnimeWorldDownloader_App
{

    public partial class MainPage : ContentPage
    {
        SearchAnimeViewModel searchAnimeViewModel;
        //ObservableCollection<AnimeModel> animeCollection;    

        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            // istanzia il view-model
            searchAnimeViewModel = new();
            //animeCollection = animeViewModel.AnimeModels;
            //AnimeModelsCollection.ItemsSource = animeCollection;

            // assegna il view-model alla proprietà BindingContext del ContentPage
            this.BindingContext = searchAnimeViewModel;
            //AnimeModelsCollection.SetBinding(animeViewModel.AnimeModels, )

        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} {searchAnimeViewModel.SearchText} time";
            else
                CounterBtn.Text = $"Clicked {count} {searchAnimeViewModel.SearchText} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

        private void Entry_TextChanged(object sender, TextChangedEventArgs e)
        {
            searchAnimeViewModel.GetSearchAnime();
        }

        private void Entry_Completed(object sender, EventArgs e)
        {
            searchAnimeViewModel.GetSearchAnime();
        }
    }
}