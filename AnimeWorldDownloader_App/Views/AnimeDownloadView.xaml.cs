using AnimeWorldDownloader_App.ViewModels;

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
        Application.Current.MainPage.Navigation.PopAsync();
    }
}