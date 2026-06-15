using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using CommunityToolkit.Maui.Views;

namespace AnimeWorldDownloader_App.Views;

public partial class EpisodePlayerView : ContentPage
{
    private readonly EpisodeModel _episode;
    private bool _isNavigating;
    private bool _isClosing;
    private bool _loaded;

    public EpisodePlayerView(EpisodeModel episode)
    {
        InitializeComponent();
        _episode = episode;
        TitleLabel.Text = $"Episodio {episode.NumberLabel}";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _isClosing = false;
        RegisterKeyboardHandler();

        // Carica/avvia una sola volta: evita replay e reload su minimize-restore
        if (_loaded) return;
        _loaded = true;
        await LoadAndPlayAsync();
    }

    private async Task LoadAndPlayAsync()
    {
        try
        {
            TitleLabel.Text = $"Episodio {_episode.NumberLabel} - Caricamento da: {_episode.UriWatch}";

            string directUrl = await _episode.ResolveDirectDownloadUrlAsync();

            // L'utente potrebbe aver chiuso il player durante il fetch:
            // non toccare il MediaElement se l'handler è già disconnesso
            if (_isClosing) return;

            if (!string.IsNullOrEmpty(directUrl))
            {
                TitleLabel.Text = $"Episodio {_episode.NumberLabel} - Streaming: {directUrl}";
                Player.Source = new UriMediaSource { Uri = new Uri(directUrl) };
            }
            else
            {
                TitleLabel.Text = $"Episodio {_episode.NumberLabel} - Link non trovato. UriWatch: {_episode.UriWatch}, UriDownloadPage: {_episode.UriDownloadPage}";
                AppLogger.Instance.Error(
                    $"Link streaming non trovato Ep.{_episode.NumberLabel} | ApiId: {_episode.EpisodeApiId} | UriWatch: {_episode.UriWatch}",
                    "Player");
            }
        }
        catch (Exception ex)
        {
            TitleLabel.Text = $"Errore Ep. {_episode.NumberLabel}: {ex.Message} | UriWatch: {_episode.UriWatch} | UriDownloadPage: {_episode.UriDownloadPage}";
            AppLogger.Instance.Error(
                $"Caricamento player fallito Ep.{_episode.NumberLabel} | ApiId: {_episode.EpisodeApiId} | UriWatch: {_episode.UriWatch}",
                ex, "Player");
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _isClosing = true;
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
            // Idempotente: rimuovi prima per evitare doppia registrazione
            // (OnAppearing può rifirare su minimize-restore) -> doppio toggle
            root.KeyDown -= OnNativeKeyDown;
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
