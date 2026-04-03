using System.Collections.Concurrent;
using System.Text;

namespace AnimeWorldDownloader_App.Data
{
    /// <summary>
    /// Logger centralizzato che scrive file di log nella cartella logs/.
    /// Thread-safe, con flush automatico e rotazione giornaliera.
    /// </summary>
    public sealed class AppLogger : IDisposable
    {
        private static AppLogger? _instance;
        private static readonly object _lock = new();

        private readonly string _logDirectory;
        private readonly ConcurrentQueue<string> _messageQueue = new();
        private readonly Timer _flushTimer;
        private readonly SemaphoreSlim _writeSemaphore = new(1, 1);
        private bool _disposed;

        public enum LogLevel
        {
            DEBUG,
            INFO,
            WARN,
            ERROR
        }

        public static AppLogger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AppLogger();
                    }
                }
                return _instance;
            }
        }

        private AppLogger()
        {
            _logDirectory = Path.Combine(AppSettings.DownloadBasePath, "..", "logs");
            Directory.CreateDirectory(_logDirectory);

            // Flush ogni 2 secondi
            _flushTimer = new Timer(_ => _ = FlushAsync(), null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2));
        }

        private string LogFilePath =>
            Path.Combine(_logDirectory, $"AnimeWorldDownloader_{DateTime.Now:yyyy-MM-dd}.log");

        public void Debug(string message, string? source = null) => Log(LogLevel.DEBUG, message, source);
        public void Info(string message, string? source = null) => Log(LogLevel.INFO, message, source);
        public void Warn(string message, string? source = null) => Log(LogLevel.WARN, message, source);
        public void Error(string message, string? source = null) => Log(LogLevel.ERROR, message, source);

        public void Error(string message, Exception ex, string? source = null)
        {
            string fullMessage = $"{message} | Exception: {ex.GetType().Name}: {ex.Message}";
            if (ex.InnerException != null)
                fullMessage += $" | Inner: {ex.InnerException.Message}";
            fullMessage += $"\n  StackTrace: {ex.StackTrace}";
            Log(LogLevel.ERROR, fullMessage, source);
        }

        public void LogHttpRequest(string method, string url, string? source = null)
        {
            Log(LogLevel.DEBUG, $"HTTP {method} → {url}", source);
        }

        public void LogHttpResponse(string url, int statusCode, long? contentLength = null, string? source = null)
        {
            string size = contentLength.HasValue ? $" ({FormatBytes(contentLength.Value)})" : "";
            Log(LogLevel.DEBUG, $"HTTP {statusCode} ← {url}{size}", source);
        }

        public void LogDownloadStart(int episodeNumber, string url, string savePath)
        {
            Log(LogLevel.INFO, $"Download START Ep.{episodeNumber} | URL: {url} | SavePath: {savePath}", "Download");
        }

        public void LogDownloadComplete(int episodeNumber, string savePath, long fileSize)
        {
            Log(LogLevel.INFO, $"Download COMPLETE Ep.{episodeNumber} | File: {savePath} | Size: {FormatBytes(fileSize)}", "Download");
        }

        public void LogDownloadError(int episodeNumber, string url, Exception ex)
        {
            Error($"Download FAILED Ep.{episodeNumber} | URL: {url}", ex, "Download");
        }

        private void Log(LogLevel level, string message, string? source = null)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string threadId = Environment.CurrentManagedThreadId.ToString().PadLeft(3);
            string src = string.IsNullOrEmpty(source) ? "" : $"[{source}] ";
            string line = $"[{timestamp}] [{level,-5}] [T{threadId}] {src}{message}";

            _messageQueue.Enqueue(line);

            // Flush immediato per errori
            if (level == LogLevel.ERROR)
                _ = FlushAsync();
        }

        public async Task FlushAsync()
        {
            if (_messageQueue.IsEmpty) return;

            await _writeSemaphore.WaitAsync();
            try
            {
                var sb = new StringBuilder();
                while (_messageQueue.TryDequeue(out string? msg))
                    sb.AppendLine(msg);

                if (sb.Length > 0)
                    await File.AppendAllTextAsync(LogFilePath, sb.ToString());
            }
            catch
            {
                // Non bloccare l'app se il log fallisce
            }
            finally
            {
                _writeSemaphore.Release();
            }
        }

        /// <summary>
        /// Restituisce il percorso della cartella dei log.
        /// </summary>
        public string GetLogDirectory() => _logDirectory;

        /// <summary>
        /// Restituisce il percorso del file di log corrente.
        /// </summary>
        public string GetCurrentLogFilePath() => LogFilePath;

        private static string FormatBytes(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB"];
            int order = 0;
            double size = bytes;
            while (size >= 1024 && order < sizes.Length - 1)
            {
                order++;
                size /= 1024;
            }
            return $"{size:0.##} {sizes[order]}";
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _flushTimer.Dispose();
            FlushAsync().GetAwaiter().GetResult();
            _writeSemaphore.Dispose();
        }
    }
}
