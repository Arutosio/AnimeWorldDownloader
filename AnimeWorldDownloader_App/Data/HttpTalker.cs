using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;

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

        /// <summary>
        /// Scarica un file con supporto alla ripresa (HTTP Range).
        /// </summary>
        /// <param name="onProgress">Callback (downloadedBytes, totalBytes, bytesPerSec). totalBytes=0 se sconosciuto.</param>
        /// <param name="resumeFrom">Offset da cui riprendere (0 = da capo). Se &gt;0 invia Range e apre il file in append.</param>
        /// <remarks>
        /// NON elimina il file parziale in caso di errore/annullamento: la
        /// decisione spetta al chiamante (pausa/errore tengono il parziale per
        /// la ripresa, l'annullamento lo elimina). Qui si fa solo rethrow.
        /// </remarks>
        public async Task DownloadFileAsync(string url, string savePath, Action<long, long, double> onProgress, long resumeFrom = 0, CancellationToken ct = default)
        {
            _log.LogHttpRequest($"GET (download, resumeFrom={resumeFrom})", url, "HttpTalker");

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, url);
                if (resumeFrom > 0)
                    request.Headers.Range = new RangeHeaderValue(resumeFrom, null);

                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct);
                int statusCode = (int)response.StatusCode;
                long? contentLength = response.Content.Headers.ContentLength;

                _log.LogHttpResponse(url, statusCode, contentLength, "HttpTalker");

                if (!response.IsSuccessStatusCode)
                {
                    string errorBody = await response.Content.ReadAsStringAsync(ct);
                    _log.Error($"Download HTTP {statusCode} per {url} | Body preview: {errorBody[..Math.Min(500, errorBody.Length)]}", "HttpTalker");
                }

                response.EnsureSuccessStatusCode();

                // Se abbiamo chiesto un Range ma il server ha risposto 200 (no range),
                // ricominciamo da zero per non corrompere il file.
                bool serverHonoredRange = response.StatusCode == HttpStatusCode.PartialContent;
                long startOffset = (resumeFrom > 0 && serverHonoredRange) ? resumeFrom : 0;
                if (resumeFrom > 0 && !serverHonoredRange)
                    _log.Warn($"Server non ha onorato il Range (status {statusCode}): riparto da 0", "HttpTalker");

                // Totale assoluto: per il 206 ContentLength è la parte rimanente.
                long totalBytes = contentLength.HasValue ? startOffset + contentLength.Value : 0L;

                _log.Debug($"Download stream avviato: {url} → {savePath} (offset {startOffset}, totale {(totalBytes > 0 ? totalBytes.ToString() : "sconosciuto")} bytes)", "HttpTalker");

                Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

                FileMode mode = startOffset > 0 ? FileMode.Append : FileMode.Create;
                long downloaded = startOffset;
                byte[] buffer = new byte[81920];

                using FileStream fileStream = new(savePath, mode, FileAccess.Write, FileShare.None);
                using Stream stream = await response.Content.ReadAsStreamAsync(ct);

                var sw = Stopwatch.StartNew();
                long lastReport = 0L;
                long bytesSinceReport = 0L;
                onProgress(downloaded, totalBytes, 0);

                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer, ct)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct);
                    downloaded += bytesRead;
                    bytesSinceReport += bytesRead;

                    // Campiona ~ogni 500 ms per non floodare la UI
                    long elapsedMs = sw.ElapsedMilliseconds;
                    if (elapsedMs - lastReport >= 500)
                    {
                        double intervalSec = (elapsedMs - lastReport) / 1000.0;
                        double speed = intervalSec > 0 ? bytesSinceReport / intervalSec : 0;
                        onProgress(downloaded, totalBytes, speed);
                        lastReport = elapsedMs;
                        bytesSinceReport = 0;
                    }
                }

                onProgress(downloaded, totalBytes, 0);
                _log.Info($"Download file completato: {savePath} ({downloaded} bytes)", "HttpTalker");
            }
            catch (OperationCanceledException)
            {
                // Pausa o annullo: il parziale resta su disco, decide il chiamante.
                _log.Warn($"Download interrotto (pausa/annullo): {url}", "HttpTalker");
                throw;
            }
            catch (Exception ex)
            {
                _log.Error($"Download fallito: {url} → {savePath}", ex, "HttpTalker");
                throw;
            }
        }
    }
}
