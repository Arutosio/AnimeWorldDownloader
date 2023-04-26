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

    private void OnButtonClickedGoBackToSearch(object sender, EventArgs e)
    {
        this.Navigation.PopModalAsync();
    }

    private void OnButtonClickedGoToDownload(object sender, EventArgs e)
    {
		var botton = (Button)sender;
		var item = (AnimeViewModel)botton.BindingContext;
		var uriDetail = item.UriDetail;

		this.Navigation.PushModalAsync(new AnimeDownloadView(uriDetail), true);
    }
}