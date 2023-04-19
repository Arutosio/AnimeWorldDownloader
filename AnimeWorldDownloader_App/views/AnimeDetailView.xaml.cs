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
}