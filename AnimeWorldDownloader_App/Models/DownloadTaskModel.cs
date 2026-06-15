using Microsoft.Maui.Graphics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnimeWorldDownloader_App.Models
{
    public enum DownloadState { Queued, Running, Paused, Completed, Cancelled, Error }

    public class DownloadTaskModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private DownloadState _state = DownloadState.Queued;
        private double _progress;
        private long _downloadedBytes;
        private long _totalBytes;
        private double _bytesPerSec;
        private string _errorMessage = string.Empty;

        // Intento di pausa: distingue OperationCanceledException "pausa" da "annulla"
        private bool _pauseRequested;

        public EpisodeModel Episode { get; set; } = null!;
        public string SavePath { get; set; } = string.Empty;
        public CancellationTokenSource Cts { get; set; } = new();

        public DownloadState State
        {
            get => _state;
            set
            {
                if (_state == value) return;
                _state = value;
                OnPropertyChanged();
                // Notifica tutte le proprietà derivate dallo stato
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(IsFinished));
                OnPropertyChanged(nameof(IsActive));
                OnPropertyChanged(nameof(CanPause));
                OnPropertyChanged(nameof(CanResume));
                OnPropertyChanged(nameof(CanRetry));
                OnPropertyChanged(nameof(StateColor));
            }
        }

        public bool PauseRequested
        {
            get => _pauseRequested;
            set => _pauseRequested = value;
        }

        // --- Stato derivato ---

        public string Status => State switch
        {
            DownloadState.Queued => "In coda",
            DownloadState.Running => "Download...",
            DownloadState.Paused => "In pausa",
            DownloadState.Completed => "Completato",
            DownloadState.Cancelled => "Annullato",
            DownloadState.Error => "Errore",
            _ => string.Empty
        };

        public bool IsFinished => State is DownloadState.Completed or DownloadState.Cancelled or DownloadState.Error;
        public bool IsActive => State is DownloadState.Queued or DownloadState.Running or DownloadState.Paused;
        public bool CanPause => State == DownloadState.Running;
        public bool CanResume => State == DownloadState.Paused;
        public bool CanRetry => State == DownloadState.Error;

        public Color StateColor => State switch
        {
            DownloadState.Queued => Colors.DeepSkyBlue,
            DownloadState.Running => Colors.Orange,
            DownloadState.Paused => Colors.Gold,
            DownloadState.Completed => Colors.LimeGreen,
            DownloadState.Cancelled => Colors.Gray,
            DownloadState.Error => Colors.Red,
            _ => Colors.Gray
        };

        // --- Avanzamento / metriche ---

        public double Progress
        {
            get => _progress;
            set
            {
                if (_progress == value) return;
                _progress = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressPercent));
            }
        }

        public int ProgressPercent => (int)Math.Round(_progress * 100);

        public long DownloadedBytes
        {
            get => _downloadedBytes;
            set { if (_downloadedBytes != value) { _downloadedBytes = value; OnPropertyChanged(); OnPropertyChanged(nameof(SizeText)); } }
        }

        public long TotalBytes
        {
            get => _totalBytes;
            set { if (_totalBytes != value) { _totalBytes = value; OnPropertyChanged(); OnPropertyChanged(nameof(SizeText)); } }
        }

        public double BytesPerSec
        {
            get => _bytesPerSec;
            set { if (_bytesPerSec != value) { _bytesPerSec = value; OnPropertyChanged(); OnPropertyChanged(nameof(SpeedText)); OnPropertyChanged(nameof(EtaText)); } }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set { if (_errorMessage != value) { _errorMessage = value; OnPropertyChanged(); } }
        }

        public string SizeText
        {
            get
            {
                if (_totalBytes > 0)
                    return $"{FormatBytes(_downloadedBytes)} / {FormatBytes(_totalBytes)}";
                return _downloadedBytes > 0 ? FormatBytes(_downloadedBytes) : string.Empty;
            }
        }

        public string SpeedText => _bytesPerSec > 0 ? $"{FormatBytes((long)_bytesPerSec)}/s" : string.Empty;

        public string EtaText
        {
            get
            {
                if (_bytesPerSec <= 0 || _totalBytes <= 0) return string.Empty;
                long remaining = _totalBytes - _downloadedBytes;
                if (remaining <= 0) return string.Empty;
                var eta = TimeSpan.FromSeconds(remaining / _bytesPerSec);
                return eta.TotalHours >= 1 ? eta.ToString(@"hh\:mm\:ss") : eta.ToString(@"mm\:ss");
            }
        }

        // --- Controllo ---

        // Richiede la pausa: interrompe il trasferimento conservando il file parziale.
        public void RequestPause()
        {
            if (State != DownloadState.Running) return;
            _pauseRequested = true;
            Cts.Cancel();
        }

        // Annulla: il VM eliminerà il file parziale e rimuoverà l'item.
        public void Cancel()
        {
            if (IsFinished) return;
            _pauseRequested = false;
            Cts.Cancel();
        }

        private static string FormatBytes(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB" };
            double val = bytes;
            int u = 0;
            while (val >= 1024 && u < units.Length - 1) { val /= 1024; u++; }
            return $"{val:0.0} {units[u]}";
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
