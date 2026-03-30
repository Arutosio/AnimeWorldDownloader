using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AnimeWorldDownloader_App.Models
{
    public class DownloadTaskModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private double _progress;
        private string _status = "In attesa...";

        public EpisodeModel Episode { get; set; } = null!;
        public string SavePath { get; set; } = string.Empty;
        public CancellationTokenSource Cts { get; set; } = new();

        public double Progress
        {
            get => _progress;
            set { if (_progress != value) { _progress = value; OnPropertyChanged(); } }
        }

        public string Status
        {
            get => _status;
            set { if (_status != value) { _status = value; OnPropertyChanged(); } }
        }

        public bool IsFinished => Status is "Completato" or "Annullato" or "Errore";

        public void Cancel()
        {
            if (!IsFinished)
            {
                Cts.Cancel();
                Status = "Annullato";
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
