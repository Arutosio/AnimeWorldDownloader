using System.Net;

namespace AnimeWorldDownloader_App.Data
{
    public class HttpTalker
    {
        private static HttpTalker? _instance;
        private readonly HttpClient _httpClient;
        private readonly AppLogger _log = AppLogger.Instance;

        public static HttpTalker GetInstance()
        {
            _instance ??= new HttpTalker();
            return _instance;
        }

        private HttpTalker()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10,
                UseCookies = true,
                CookieContainer = new CookieContainer()
            };

            _httpClient = new HttpClient(handler);
            _httpClient.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "it-IT,it;q=0.9,en-US;q=0.8,en;q=0.7");
            _httpClient.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            _httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            _httpClient.Timeout = TimeSpan.FromSeconds(60);

            _log.Info("HttpTalker inizializzato con headers browser e cookie support", "HttpTalker");
        }

        public async Task<string> GetResultFromUriAsync(string url)
        {
            _log.LogHttpRequest("GET", url, "HttpTalker");

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                int statusCode = (int)response.StatusCode;
                long? contentLength = response.Content.Headers.ContentLength;

                _log.LogHttpResponse(url, statusCode, contentLength, "HttpTalker");

                if (!response.IsSuccessStatusCode)
                {
                    string body = await response.Content.ReadAsStringAsync();
                    _log.Error($"HTTP {statusCode} per {url} | Body preview: {body[..Math.Min(500, body.Length)]}", "HttpTalker");
                }

                response.EnsureSuccessStatusCode();
                string html = await response.Content.ReadAsStringAsync();
                _log.Debug($"HTML ricevuto: {html.Length} chars da {url}", "HttpTalker");
                return html;
            }
            catch (Exception ex)
            {
                _log.Error($"GET fallito per {url}", ex, "HttpTalker");
                throw;
            }
        }

        public async Task DownloadFileAsync(string url, string savePath, Action<double> updateDownloadProgress, CancellationToken ct = default)
        {
            _log.LogHttpRequest("GET (download)", url, "HttpTalker");

            try
            {
                using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
                int statusCode = (int)response.StatusCode;
                long? totalBytes = response.Content.Headers.ContentLength;

                _log.LogHttpResponse(url, statusCode, totalBytes, "HttpTalker");

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync(ct);
                    _log.Error($"Download HTTP {statusCode} per {url} | Body preview: {errorBody[..Math.Min(500, errorBody.Length)]}", "HttpTalker");
                }

                response.EnsureSuccessStatusCode();

                _log.Debug($"Download stream avviato: {url} → {savePath} (Size: {totalBytes?.ToString() ?? "sconosciuta"} bytes)", "HttpTalker");

                long readBytes = 0L;
                byte[] buffer = new byte[8192];

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                using FileStream fileStream = new(savePath, FileMode.Create, FileAccess.Write, FileShare.None);
                using Stream stream = await response.Content.ReadAsStreamAsync(ct);

                int bytesRead;
                do
                {
                    ct.ThrowIfCancellationRequested();
                    bytesRead = await stream.ReadAsync(buffer, ct);
                    readBytes += bytesRead;

                    if (totalBytes.HasValue && totalBytes.Value > 0)
                    {
                        double percentage = (double)readBytes / totalBytes.Value;
                        updateDownloadProgress.Invoke(percentage);
                    }

                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                } while (bytesRead > 0);

                _log.Info($"Download file completato: {savePath} ({readBytes} bytes)", "HttpTalker");
            }
            catch (OperationCanceledException)
            {
                _log.Warn($"Download annullato: {url}", "HttpTalker");
                TryDeletePartialFile(savePath);
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"Download fallito: {url} → {savePath}", ex, "HttpTalker");
                TryDeletePartialFile(savePath);
                throw;
            }
        }

        private void TryDeletePartialFile(string path)
        {
            // Le 'using' declaration del FileStream sono già state disposte
            // quando arriviamo qui, quindi il file non è più in lock.
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    _log.Info($"File parziale rimosso: {path}", "HttpTalker");
                }
            }
            catch (Exception ex)
            {
                _log.Warn($"Impossibile rimuovere file parziale {path}: {ex.Message}", "HttpTalker");
            }
        }
    }
}
