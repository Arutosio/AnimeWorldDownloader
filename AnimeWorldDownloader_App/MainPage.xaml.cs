using System.Windows;
using AnimeWorldDownloader_App.Models;
using AnimeWorldDownloader_App.ViewModels;
using System.Collections.ObjectModel;

namespace AnimeWorldDownloader_App
{

    public partial class MainPage : ContentPage
    {
        AnimeViewModel animeViewModel;
        //ObservableCollection<AnimeModel> animeCollection;    

        int count = 0;

        public MainPage()
        {
            InitializeComponent();

            // istanzia il view-model
            animeViewModel = new();
            //animeCollection = animeViewModel.AnimeModels;
            //AnimeModelsCollection.ItemsSource = animeCollection;

            // assegna il view-model alla proprietà BindingContext del ContentPage
            this.BindingContext = animeViewModel;

        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} {animeViewModel.SearchText} time";
            else
                CounterBtn.Text = $"Clicked {count} {animeViewModel.SearchText} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }

    }
}