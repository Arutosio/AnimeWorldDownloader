using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using AnimeWorldDownloader_App.ViewModels;

namespace AnimeWorldDownloader_App.Views;

public partial class AnimeDownloadView : ContentPage
{
    private readonly AnimeDownloadViewModel _viewModel = new();
    private readonly string _uriAnimeDetail;

    public AnimeDownloadView(string uriAnimeDetail)
    {
        InitializeComponent();
        _uriAnimeDetail = uriAnimeDetail;
        BindingContext = _viewModel;

        _viewModel.NavigateToPlayer += OnNavigateToPlayer;
        _viewModel.RequestChangeFolderDialog += OnRequestChangeFolderDialog;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(AnimeDownloadViewModel.ShowDownloadsPanel))
            {
                EpisodeList.IsVisible = !_viewModel.ShowDownloadsPanel;
            }
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.InitializeAsync(_uriAnimeDetail);
    }

    private async void OnNavigateToPlayer(EpisodeModel episode)
    {
        await Navigation.PushModalAsync(new EpisodePlayerView(episode), true);
    }

    private async Task OnRequestChangeFolderDialog()
    {
        string currentPath = AppSettings.DownloadBasePath;
        string? newPath = await DisplayPromptAsync(
            "Cartella Download",
            "Inserisci il percorso della cartella di destinazione:",
            initialValue: currentPath,
            accept: "Salva",
            cancel: "Annulla");

        if (!string.IsNullOrWhiteSpace(newPath))
        {
            AppSettings.DownloadBasePath = newPath;
            _viewModel.StatusMessage = $"Cartella base cambiata: {newPath}";
        }
    }

    private void OnButtonClickedGoBackToDetail(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}
