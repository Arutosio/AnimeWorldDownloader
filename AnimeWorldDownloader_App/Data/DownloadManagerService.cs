using AnimeWorldDownloader_App.Models;
using System.Collections.ObjectModel;

namespace AnimeWorldDownloader_App.Data
{
    public class DownloadManagerService
    {
        private static readonly Lazy<DownloadManagerService> _instance = new(() => new());
        public static DownloadManagerService Instance => _instance.Value;

        private DownloadManagerService() { }

        public ObservableCollection<DownloadTaskModel> ActiveDownloads { get; } = new();

        public int ActiveDownloadCount => ActiveDownloads.Count(d => !d.IsFinished);
    }
}
