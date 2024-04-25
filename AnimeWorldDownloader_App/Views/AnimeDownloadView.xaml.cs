using System.Windows;
using AnimeWorldDownloader_App.Models;
using AnimeWorldDownloader_App.ViewModels;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;

namespace AnimeWorldDownloader_App.Views;

public partial class AnimeDownloadView : ContentPage
{
    private AnimeDownloadViewModel animeDownloadViewModel;

	public AnimeDownloadView(string uriAnimeDetail)
	{
		InitializeComponent();

        animeDownloadViewModel = new(uriAnimeDetail);

        this.BindingContext = animeDownloadViewModel;
    }

    private void OnButtonClickedGoBackToDetail(object sender, EventArgs e)
    {
        this.Navigation.PopModalAsync();
    }
}