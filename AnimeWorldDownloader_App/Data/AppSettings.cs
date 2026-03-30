namespace AnimeWorldDownloader_App.Data
{
    /// <summary>
    /// Impostazioni dell'app persistite con Preferences di MAUI.
    /// </summary>
    public static class AppSettings
    {
        private const string DownloadPathKey = "DownloadBasePath";

        public static string DefaultDownloadPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "AnimeDownloads");

        public static string DownloadBasePath
        {
            get => Preferences.Get(DownloadPathKey, DefaultDownloadPath);
            set => Preferences.Set(DownloadPathKey, value);
        }
    }
}
