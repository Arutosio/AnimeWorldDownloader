using AnimeWorldDownloader_App.ViewModels;

namespace AnimeWorldDownloader_App.Views;

public partial class AnimeDetailView : ContentPage
{
	AnimeDetailViewModel animeDetailViewModel;

	public AnimeDetailView(string uriDetail)
	{
		InitializeComponent();
		
		animeDetailViewModel = new AnimeDetailViewModel(uriDetail);
		this.BindingContext = animeDetailViewModel;
	}

    private void OnButtonClickedGoToSearch(object sender, EventArgs e)
    {
        Application.Current.MainPage.Navigation.PopAsync();
    }

    private void OnButtonClickedGoToDownload(object sender, EventArgs e)
    {
		var botton = (Button)sender;
		var item = (AnimeViewModel)botton.BindingContext;
		var uriDetail = item.UriDetail;

		//Application.Current.MainPage.Navigation.PushModalAsync(new AnimeDetailView(uriDetail), true);
		this.Navigation.PushModalAsync(new AnimeDownloadView(uriDetail), true);
    }
}