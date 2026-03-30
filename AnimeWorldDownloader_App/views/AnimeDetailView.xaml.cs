using AnimeWorldDownloader_App.ViewModels;

namespace AnimeWorldDownloader_App.Views;

public partial class AnimeDetailView : ContentPage
{
    private readonly AnimeDetailViewModel _viewModel = new();
    private readonly string _uriDetail;

    public AnimeDetailView(string uriDetail)
    {
        InitializeComponent();
        _uriDetail = uriDetail;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync(_uriDetail);
    }

    private void OnButtonClickedGoBackToSearch(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    private void OnButtonClickedGoToDownload(object sender, EventArgs e)
    {
        var uriDetail = _viewModel.UriDetailParam;
        if (!string.IsNullOrEmpty(uriDetail))
        {
            Navigation.PushModalAsync(new AnimeDownloadView(uriDetail), true);
        }
    }
}
