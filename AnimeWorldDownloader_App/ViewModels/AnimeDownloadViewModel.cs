using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace AnimeWorldDownloader_App.ViewModels
{
    public class AnimeDownloadViewModel : AnimeViewModel
    {
        private ObservableCollection<EpisodeModel> _episodeModels = new();
        private ObservableCollection<DownloadTaskModel> _activeDownloads = new();
        private bool _isDownloading;
        private bool _isBusy;
        private string _statusMessage = string.Empty;
        private int _selectedCount;
        private bool _showDownloadsPanel;
        private string _downloadFolderPath = string.Empty;
        private readonly AppLogger _log = AppLogger.Instance;

        public AnimeDownloadViewModel()
        {
            WatchEpisodeCommand = new Command<EpisodeModel>(async (ep) => await WatchEpisodeAsync(ep));
            DownloadEpisodeCommand = new Command<EpisodeModel>(async (ep) => await DownloadSingleEpisodeAsync(ep));
            ToggleSelectCommand = new Command<EpisodeModel>(ToggleSelect);
            SelectAllCommand = new Command(() => SetAllSelected(true));
            DeselectAllCommand = new Command(() => SetAllSelected(false));
            DownloadSelectedCommand = new Command(async () => await DownloadSelectedAsync(), () => SelectedCount > 0);
            ToggleDownloadsPanelCommand = new Command(() => ShowDownloadsPanel = !ShowDownloadsPanel);
            CancelDownloadCommand = new Command<DownloadTaskModel>(dt => dt.Cancel());
            CancelAllDownloadsCommand = new Command(CancelAllDownloads);
            OpenDownloadFolderCommand = new Command(OpenDownloadFolder);
            ChangeDownloadFolderCommand = new Command(async () => { if (RequestChangeFolderDialog != null) await RequestChangeFolderDialog.Invoke(); });
        }

        public async Task InitializeAsync(string uriAnimeDetail)
        {
            IsBusy = true;
            StatusMessage = $"Caricamento da: {uriAnimeDetail}";
            _log.Info($"=== Inizializzazione download per: {uriAnimeDetail} ===", "ViewModel");
            try
            {
                AnimeDownloadModel animeDownloadModel = await AnimeDownloadModel.GetAnimeDownloadModelAsync(uriAnimeDetail);
                Name = animeDownloadModel.Name;
                UriDetail = animeDownloadModel.UriDetail;
                ImageUrl = animeDownloadModel.ImageUrl;
                DownloadFolderPath = animeDownloadModel.DownloadFolderPath;
                EpisodeModels = new ObservableCollection<EpisodeModel>(animeDownloadModel.EpisodeModels);
                StatusMessage = $"{EpisodeModels.Count} episodi trovati — Salvataggio: {DownloadFolderPath}";
                _log.Info($"Inizializzazione completata: '{Name}', {EpisodeModels.Count} episodi", "ViewModel");
            }
            catch (Exception ex)
            {
                StatusMessage = $"Errore caricamento: {ex.Message}";
                _log.Error("Errore durante inizializzazione download", ex, "ViewModel");
            }
            finally
            {
                IsBusy = false;
            }
        }

        // --- Player ---
        public event Action<EpisodeModel>? NavigateToPlayer;

        private async Task WatchEpisodeAsync(EpisodeModel episode)
        {
            NavigateToPlayer?.Invoke(episode);
        }

        // --- Download singolo ---
        private async Task DownloadSingleEpisodeAsync(EpisodeModel episode)
        {
            var task = CreateDownloadTask(episode);
            await ExecuteDownloadAsync(task);
        }

        // --- Download selezionati ---
        private async Task DownloadSelectedAsync()
        {
            var selected = EpisodeModels.Where(ep => ep.IsSelected).ToList();
            if (selected.Count == 0) return;

            var tasks = selected.Select(CreateDownloadTask).ToList();

            foreach (var task in tasks)
            {
                await ExecuteDownloadAsync(task);
            }

            StatusMessage = $"Download completato! ({selected.Count} episodi)";
        }

        private DownloadTaskModel CreateDownloadTask(EpisodeModel episode)
        {
            var dt = new DownloadTaskModel
            {
                Episode = episode,
                SavePath = episode.FileLocation,
                Status = "In attesa..."
            };
            ActiveDownloads.Add(dt);
            OnPropertyChanged(nameof(ActiveDownloadCount));
            ShowDownloadsPanel = true;
            return dt;
        }

        private async Task ExecuteDownloadAsync(DownloadTaskModel dt)
        {
            if (dt.IsFinished) return;

            int epNum = dt.Episode.NEpisode;
            _log.Info($"--- Inizio download Ep.{epNum} ---", "ViewModel");

            try
            {
                dt.Status = $"Resolving Ep. {epNum}...";
                StatusMessage = $"Resolving Ep. {epNum}... (UriWatch: {dt.Episode.UriWatch})";
                _log.Info($"Resolving URL per Ep.{epNum}: UriWatch={dt.Episode.UriWatch}", "ViewModel");

                dt.Cts.Token.ThrowIfCancellationRequested();

                string directUrl = await dt.Episode.ResolveDirectDownloadUrlAsync();
                dt.SavePath = dt.Episode.FileLocation;
                _log.Info($"URL risolto Ep.{epNum}: {directUrl}", "ViewModel");
                _log.Info($"SavePath Ep.{epNum}: {dt.SavePath}", "ViewModel");

                dt.Status = $"Download Ep. {epNum}...";
                StatusMessage = $"Download Ep. {epNum}... → {dt.SavePath}";
                _log.LogDownloadStart(epNum, directUrl, dt.SavePath);

                HttpTalker httpTalker = HttpTalker.GetInstance();
                await httpTalker.DownloadFileAsync(
                    directUrl,
                    dt.SavePath,
                    progress => MainThread.BeginInvokeOnMainThread(() => dt.Progress = progress),
                    dt.Cts.Token);

                long fileSize = new FileInfo(dt.SavePath).Length;
                dt.Status = "Completato";
                StatusMessage = $"Ep. {epNum} completato! → {dt.SavePath}";
                _log.LogDownloadComplete(epNum, dt.SavePath, fileSize);
            }
            catch (OperationCanceledException)
            {
                dt.Status = "Annullato";
                StatusMessage = $"Ep. {epNum} annullato";
                _log.Warn($"Download Ep.{epNum} annullato dall'utente", "ViewModel");
            }
            catch (Exception ex)
            {
                dt.Status = "Errore";
                StatusMessage = $"Errore Ep. {epNum}: {ex.Message}";
                _log.LogDownloadError(epNum, dt.Episode.UriDirectDownload ?? dt.Episode.UriWatch, ex);
            }
            finally
            {
                OnPropertyChanged(nameof(ActiveDownloadCount));
                await _log.FlushAsync();
            }
        }

        // --- Selezione ---
        private void ToggleSelect(EpisodeModel episode)
        {
            episode.IsSelected = !episode.IsSelected;
            UpdateSelectedCount();
        }

        private void SetAllSelected(bool selected)
        {
            foreach (var ep in EpisodeModels)
                ep.IsSelected = selected;
            UpdateSelectedCount();
        }

        private void UpdateSelectedCount()
        {
            SelectedCount = EpisodeModels.Count(ep => ep.IsSelected);
            ((Command)DownloadSelectedCommand).ChangeCanExecute();
        }

        // --- Cancellazione ---
        private void CancelAllDownloads()
        {
            foreach (var dt in ActiveDownloads)
                dt.Cancel();
            StatusMessage = "Tutti i download annullati";
        }

        // --- Apri cartella download ---
        private void OpenDownloadFolder()
        {
            string path = !string.IsNullOrEmpty(DownloadFolderPath)
                ? DownloadFolderPath
                : AppSettings.DownloadBasePath;

            Directory.CreateDirectory(path);

            Process.Start(new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = true
            });
        }

        // --- Cambia cartella download (gestito dal code-behind con prompt) ---
        public event Func<Task>? RequestChangeFolderDialog;

        // --- Properties ---

        public ObservableCollection<EpisodeModel> EpisodeModels
        {
            get => _episodeModels;
            set { if (_episodeModels != value) { _episodeModels = value; OnPropertyChanged(); } }
        }

        public ObservableCollection<DownloadTaskModel> ActiveDownloads
        {
            get => _activeDownloads;
            set { if (_activeDownloads != value) { _activeDownloads = value; OnPropertyChanged(); } }
        }

        public int ActiveDownloadCount => ActiveDownloads.Count(d => !d.IsFinished);

        public bool IsDownloading
        {
            get => _isDownloading;
            set { if (_isDownloading != value) { _isDownloading = value; OnPropertyChanged(); } }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy != value) { _isBusy = value; OnPropertyChanged(); } }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { if (_statusMessage != value) { _statusMessage = value; OnPropertyChanged(); } }
        }

        public int SelectedCount
        {
            get => _selectedCount;
            set { if (_selectedCount != value) { _selectedCount = value; OnPropertyChanged(); } }
        }

        public bool ShowDownloadsPanel
        {
            get => _showDownloadsPanel;
            set { if (_showDownloadsPanel != value) { _showDownloadsPanel = value; OnPropertyChanged(); } }
        }

        public string DownloadFolderPath
        {
            get => _downloadFolderPath;
            set { if (_downloadFolderPath != value) { _downloadFolderPath = value; OnPropertyChanged(); } }
        }

        // --- Commands ---
        public ICommand WatchEpisodeCommand { get; }
        public ICommand DownloadEpisodeCommand { get; }
        public ICommand ToggleSelectCommand { get; }
        public ICommand SelectAllCommand { get; }
        public ICommand DeselectAllCommand { get; }
        public ICommand DownloadSelectedCommand { get; }
        public ICommand ToggleDownloadsPanelCommand { get; }
        public ICommand CancelDownloadCommand { get; }
        public ICommand CancelAllDownloadsCommand { get; }
        public ICommand OpenDownloadFolderCommand { get; }
        public ICommand ChangeDownloadFolderCommand { get; }
    }
}
