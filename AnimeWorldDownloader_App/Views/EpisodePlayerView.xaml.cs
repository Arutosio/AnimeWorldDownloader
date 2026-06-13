using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using CommunityToolkit.Maui.Views;

namespace AnimeWorldDownloader_App.Views;

public partial class EpisodePlayerView : ContentPage
{
    private readonly EpisodeModel _episode;
    private bool _isNavigating;

    public EpisodePlayerView(EpisodeModel episode)
    {
        InitializeComponent();
        _episode = episode;
        TitleLabel.Text = $"Episodio {episode.NumberLabel}";
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
            TitleLabel.Text = $"Episodio {_episode.NumberLabel} - Caricamento da: {_episode.UriWatch}";

            string directUrl = await _episode.ResolveDirectDownloadUrlAsync();

            if (!string.IsNullOrEmpty(directUrl))
            {
                TitleLabel.Text = $"Episodio {_episode.NumberLabel} - Streaming: {directUrl}";
                Player.Source = new UriMediaSource { Uri = new Uri(directUrl) };
            }
            else
            {
                TitleLabel.Text = $"Episodio {_episode.NumberLabel} - Link non trovato. UriWatch: {_episode.UriWatch}, UriDownloadPage: {_episode.UriDownloadPage}";
            }
        }
        catch (Exception ex)
        {
            TitleLabel.Text = $"Errore Ep. {_episode.NumberLabel}: {ex.Message} | UriWatch: {_episode.UriWatch} | UriDownloadPage: {_episode.UriDownloadPage}";
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        UnregisterKeyboardHandler();
        Player.Stop();
        Player.Handler?.DisconnectHandler();
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        if (_isNavigating) return;
        _isNavigating = true;
        try
        {
            await Navigation.PopModalAsync();
        }
        catch (Exception ex)
        {
            AppLogger.Instance.Error("Chiusura player fallita", ex, "Navigation");
        }
        finally
        {
            _isNavigating = false;
        }
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
