using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using CommunityToolkit.Maui.Views;

namespace AnimeWorldDownloader_App.Views;

public partial class EpisodePlayerView : ContentPage
{
    private readonly EpisodeModel _episode;

    public EpisodePlayerView(EpisodeModel episode)
    {
        InitializeComponent();
        _episode = episode;
        TitleLabel.Text = $"Episodio {episode.NEpisode}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAndPlayAsync();
    }

    private async Task LoadAndPlayAsync()
    {
        try
        {
            TitleLabel.Text = $"Episodio {_episode.NEpisode} - Caricamento da: {_episode.UriWatch}";

            string directUrl = await _episode.ResolveDirectDownloadUrlAsync();

            if (!string.IsNullOrEmpty(directUrl))
            {
                TitleLabel.Text = $"Episodio {_episode.NEpisode} - Streaming: {directUrl}";
                Player.Source = new UriMediaSource { Uri = new Uri(directUrl) };
            }
            else
            {
                TitleLabel.Text = $"Episodio {_episode.NEpisode} - Link non trovato. UriWatch: {_episode.UriWatch}, UriDownloadPage: {_episode.UriDownloadPage}";
            }
        }
        catch (Exception ex)
        {
            TitleLabel.Text = $"Errore Ep. {_episode.NEpisode}: {ex.Message} | UriWatch: {_episode.UriWatch} | UriDownloadPage: {_episode.UriDownloadPage}";
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        Player.Stop();
        Player.Handler?.DisconnectHandler();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}
