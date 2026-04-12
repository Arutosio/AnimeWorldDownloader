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
        RegisterKeyboardHandler();
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
        UnregisterKeyboardHandler();
        Player.Stop();
        Player.Handler?.DisconnectHandler();
    }

    private void OnCloseClicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }

    #region Keyboard – Spacebar play/pause (Windows)

    private void RegisterKeyboardHandler()
    {
#if WINDOWS
        var nativeWindow = this.Window?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow?.Content is Microsoft.UI.Xaml.UIElement root)
        {
            root.KeyDown += OnNativeKeyDown;
        }
#endif
    }

    private void UnregisterKeyboardHandler()
    {
#if WINDOWS
        var nativeWindow = this.Window?.Handler?.PlatformView as Microsoft.UI.Xaml.Window;
        if (nativeWindow?.Content is Microsoft.UI.Xaml.UIElement root)
        {
            root.KeyDown -= OnNativeKeyDown;
        }
#endif
    }

#if WINDOWS
    private void OnNativeKeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {
        if (e.Key == Windows.System.VirtualKey.Space)
        {
            TogglePlayPause();
            e.Handled = true;
        }
    }
#endif

    private void TogglePlayPause()
    {
        if (Player.CurrentState == CommunityToolkit.Maui.Core.MediaElementState.Playing)
            Player.Pause();
        else
            Player.Play();
    }

    #endregion
}
