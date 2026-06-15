using AnimeWorldDownloader_App.Data;
using AnimeWorldDownloader_App.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace AnimeWorldDownloader_App.ViewModels
{
    public class AnimeDownloadViewModel : AnimeViewModel
    {
        private ObservableCollection<EpisodeModel> _episodeModels = new();
        private bool _isDownloading;
        private bool _isBusy;
        private string _statusMessage = string.Empty;
        private int _selectedCount;
        private bool _showDownloadsPanel;
        private string _downloadFolderPath = string.Empty;
        private bool _initialized;
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
            CancelDownloadCommand = new Command<DownloadTaskModel>(CancelDownload);
            PauseDownloadCommand = new Command<DownloadTaskModel>(dt => dt.RequestPause());
            ResumeDownloadCommand = new Command<DownloadTaskModel>(async dt => await ResumeDownloadAsync(dt));
            RetryDownloadCommand = new Command<DownloadTaskModel>(async dt => await ResumeDownloadAsync(dt));
            RemoveDownloadCommand = new Command<DownloadTaskModel>(RemoveDownload);
            CancelAllDownloadsCommand = new Command(async () => await CancelAllDownloadsAsync());
            OpenDownloadFolderCommand = new Command(OpenDownloadFolder);
            ChangeDownloadFolderCommand = new Command(async () => { if (RequestChangeFolderDialog != null) await RequestChangeFolderDialog.Invoke(); });
        }

        public async Task InitializeAsync(string uriAnimeDetail)
        {
            if (_initialized) return;
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
                _initialized = true;
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
            if (HasActiveDownloadFor(episode))
            {
                StatusMessage = $"Ep. {episode.NEpisode} è già in download";
                ShowDownloadsPanel = true;
                return;
            }

            CreateDownloadTask(episode); // accodato (Queued)
            await PumpQueueAsync();
        }

        // --- Download selezionati ---
        private async Task DownloadSelectedAsync()
        {
            var selected = EpisodeModels.Where(ep => ep.IsSelected).ToList();
            if (selected.Count == 0) return;

            // Evita task duplicati per episodi già in download
            foreach (var ep in selected.Where(ep => !HasActiveDownloadFor(ep)))
                CreateDownloadTask(ep); // tutti Queued

            foreach (var ep in selected)
                ep.IsSelected = false;
            UpdateSelectedCount();

            await PumpQueueAsync();
        }

        // Coda sequenziale (uno alla volta). Si ferma se l'item attivo viene
        // messo in pausa: gli episodi in coda NON partono finché non lo si
        // riprende. La ripresa riavvia la coda (vedi ResumeDownloadAsync).
        private bool _queueRunning;

        private async Task PumpQueueAsync()
        {
            if (_queueRunning) return;
            _queueRunning = true;
            try
            {
                while (true)
                {
                    var next = ActiveDownloads.FirstOrDefault(d => d.State == DownloadState.Queued);
                    if (next == null) break;

                    long offset = next.ResumeFrom;
                    next.ResumeFrom = 0;
                    await ExecuteDownloadAsync(next, offset);

                    // Pausa dell'item attivo = ferma la coda
                    if (next.State == DownloadState.Paused) break;
                }
            }
            finally
            {
                _queueRunning = false;
            }
        }

        // I task completati/annullati si auto-rimuovono: ogni item ancora in
        // lista (Queued/Running/Paused/Error) blocca un download duplicato.
        private bool HasActiveDownloadFor(EpisodeModel episode)
            => ActiveDownloads.Any(dt => dt.Episode == episode);

        private DownloadTaskModel CreateDownloadTask(EpisodeModel episode)
        {
            var dt = new DownloadTaskModel
            {
                Episode = episode,
                SavePath = episode.FileLocation,
                State = DownloadState.Queued
            };
            ActiveDownloads.Add(dt);
            OnPropertyChanged(nameof(ActiveDownloadCount));
            ShowDownloadsPanel = true;
            return dt;
        }

        private async Task ExecuteDownloadAsync(DownloadTaskModel dt, long resumeFrom = 0)
        {
            if (dt.IsFinished) return;

            int epNum = dt.Episode.NEpisode;
            _log.Info($"--- Inizio download Ep.{epNum} (resumeFrom={resumeFrom}) ---", "ViewModel");

            try
            {
                dt.PauseRequested = false;
                dt.State = DownloadState.Running;
                StatusMessage = resumeFrom > 0
                    ? $"Ripresa Ep. {epNum}..."
                    : $"Risoluzione link Ep. {epNum}...";
                _log.Info($"Resolving URL per Ep.{epNum}: UriWatch={dt.Episode.UriWatch}", "ViewModel");

                dt.Cts.Token.ThrowIfCancellationRequested();

                string directUrl = await dt.Episode.ResolveDirectDownloadUrlAsync();
                dt.SavePath = dt.Episode.FileLocation;
                _log.Info($"URL risolto Ep.{epNum}: {directUrl} → {dt.SavePath}", "ViewModel");

                StatusMessage = $"Download Ep. {epNum}...";
                _log.LogDownloadStart(epNum, directUrl, dt.SavePath);

                HttpTalker httpTalker = HttpTalker.GetInstance();
                await httpTalker.DownloadFileAsync(
                    directUrl,
                    dt.SavePath,
                    (downloaded, total, speed) => MainThread.BeginInvokeOnMainThread(() =>
                    {
                        dt.DownloadedBytes = downloaded;
                        dt.TotalBytes = total;
                        dt.BytesPerSec = speed;
                        dt.Progress = total > 0 ? (double)downloaded / total : 0;
                    }),
                    resumeFrom,
                    dt.Cts.Token);

                long fileSize = new FileInfo(dt.SavePath).Length;
                dt.BytesPerSec = 0;
                dt.State = DownloadState.Completed;
                StatusMessage = $"Ep. {epNum} completato!";
                _log.LogDownloadComplete(epNum, dt.SavePath, fileSize);

                // Completato: sparisce dal pannello
                RemoveTask(dt);
            }
            catch (OperationCanceledException)
            {
                dt.BytesPerSec = 0;
                if (dt.PauseRequested)
                {
                    // Pausa: conserva il parziale e l'offset, l'item resta
                    dt.DownloadedBytes = SafeFileLength(dt.SavePath);
                    dt.State = DownloadState.Paused;
                    StatusMessage = $"Ep. {epNum} in pausa";
                    _log.Info($"Download Ep.{epNum} in pausa a {dt.DownloadedBytes} bytes", "ViewModel");
                }
                else
                {
                    // Annullo: elimina il parziale e rimuovi l'item
                    dt.State = DownloadState.Cancelled;
                    StatusMessage = $"Ep. {epNum} annullato";
                    _log.Warn($"Download Ep.{epNum} annullato dall'utente", "ViewModel");
                    TryDeleteFile(dt.SavePath);
                    RemoveTask(dt);
                }
            }
            catch (Exception ex)
            {
                dt.BytesPerSec = 0;
                dt.State = DownloadState.Error;
                dt.ErrorMessage = ex.Message;
                StatusMessage = $"Errore Ep. {epNum}: {ex.Message}";
                _log.LogDownloadError(epNum, string.IsNullOrEmpty(dt.Episode.UriDirectDownload) ? dt.Episode.UriWatch : dt.Episode.UriDirectDownload, ex);
                // Errore: l'item resta (parziale conservato per Riprova)
            }
            finally
            {
                OnPropertyChanged(nameof(ActiveDownloadCount));
                await _log.FlushAsync();
            }
        }

        // --- Pausa / Ripresa / Riprova ---
        // Rimette il task in coda con l'offset del parziale: lo esegue il pump
        // (un solo download alla volta, niente esecuzioni concorrenti).
        private async Task ResumeDownloadAsync(DownloadTaskModel dt)
        {
            if (dt == null || dt.State == DownloadState.Running || dt.State == DownloadState.Queued) return;

            dt.Cts = new CancellationTokenSource();
            dt.ErrorMessage = string.Empty;
            dt.ResumeFrom = SafeFileLength(dt.SavePath);
            dt.State = DownloadState.Queued;
            _log.Info($"Ripresa/Riprova Ep.{dt.Episode.NEpisode} da offset {dt.ResumeFrom}", "ViewModel");
            await PumpQueueAsync();
        }

        // --- Annulla singolo ---
        private void CancelDownload(DownloadTaskModel dt)
        {
            if (dt == null) return;

            if (dt.State == DownloadState.Running)
            {
                // Il loop attivo lancerà OperationCanceled: il catch gestisce delete + remove
                dt.Cancel();
            }
            else
            {
                // Queued/Paused/Error: nessun loop in esecuzione, cleanup sincrono
                dt.Cancel();
                dt.State = DownloadState.Cancelled;
                TryDeleteFile(dt.SavePath);
                RemoveTask(dt);
                StatusMessage = $"Ep. {dt.Episode.NEpisode} annullato";
            }
        }

        // --- Rimuovi item in errore (senza riscaricare) ---
        private void RemoveDownload(DownloadTaskModel dt)
        {
            if (dt == null) return;
            TryDeleteFile(dt.SavePath);
            RemoveTask(dt);
        }

        private void RemoveTask(DownloadTaskModel dt)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (ActiveDownloads.Remove(dt))
                {
                    dt.Cts.Dispose();
                    OnPropertyChanged(nameof(ActiveDownloadCount));
                }
            });
        }

        private static long SafeFileLength(string path)
            => File.Exists(path) ? new FileInfo(path).Length : 0L;

        private void TryDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    _log.Info($"File parziale rimosso: {path}", "ViewModel");
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"Impossibile rimuovere file {path}: {ex.Message}", "ViewModel");
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

        // --- Cancellazione di massa (con conferma) ---
        public event Func<string, Task<bool>>? RequestConfirm;

        private async Task CancelAllDownloadsAsync()
        {
            if (ActiveDownloads.Count == 0) return;

            if (RequestConfirm != null)
            {
                bool ok = await RequestConfirm.Invoke(
                    $"Annullare tutti i {ActiveDownloads.Count} download? I file parziali verranno eliminati.");
                if (!ok) return;
            }

            // Copia: CancelDownload modifica la collezione
            foreach (var dt in ActiveDownloads.ToList())
                CancelDownload(dt);

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
            set
            {
                if (_episodeModels != value)
                {
                    if (_episodeModels != null)
                        foreach (var ep in _episodeModels)
                            ep.PropertyChanged -= OnEpisodePropertyChanged;

                    _episodeModels = value;

                    if (_episodeModels != null)
                        foreach (var ep in _episodeModels)
                            ep.PropertyChanged += OnEpisodePropertyChanged;

                    OnPropertyChanged();
                    UpdateSelectedCount();
                }
            }
        }

        private void OnEpisodePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EpisodeModel.IsSelected))
                UpdateSelectedCount();
        }

        public ObservableCollection<DownloadTaskModel> ActiveDownloads
            => DownloadManagerService.Instance.ActiveDownloads;

        public int ActiveDownloadCount
            => DownloadManagerService.Instance.ActiveDownloadCount;

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
        public ICommand PauseDownloadCommand { get; }
        public ICommand ResumeDownloadCommand { get; }
        public ICommand RetryDownloadCommand { get; }
        public ICommand RemoveDownloadCommand { get; }
        public ICommand CancelAllDownloadsCommand { get; }
        public ICommand OpenDownloadFolderCommand { get; }
        public ICommand ChangeDownloadFolderCommand { get; }
    }
}
