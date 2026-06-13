using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.ViewModels;

namespace AnimeWorldDownloader_App.Views;

public partial class AnimeDetailView : ContentPage
{
    private readonly AnimeDetailViewModel _viewModel = new();
    private readonly string _uriDetail;
    private bool _isNavigating;

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

    private async void OnButtonClickedGoBackToSearch(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        try
        {
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Error("Navigazione indietro fallita", ex, "Navigation");
        }
        finally
        {
            _isNavigating = false;
        }
    }

    private async void OnButtonClickedGoToDownload(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        try
        {
            var uriDetail = _viewModel.UriDetailParam;
            if (!string.IsNullOrEmpty(uriDetail))
            {
                await Navigation.PushModalAsync(new AnimeDownloadView(uriDetail), true);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Error("Navigazione al download fallita", ex, "Navigation");
        }
        finally
        {
            _isNavigating = false;
        }
    }
}
