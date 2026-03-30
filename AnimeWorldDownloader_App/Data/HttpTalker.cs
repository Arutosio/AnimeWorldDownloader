namespace AnimeWorldDownloader_App.Data
{
    public class HttpTalker
    {
        private static HttpTalker? _instance;
        private readonly HttpClient _httpClient;

        public static HttpTalker GetInstance()
        {
            _instance ??= new HttpTalker();
            return _instance;
        }

        private HttpTalker()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetResultFromUriAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task DownloadFileAsync(string url, string savePath, Action<double> updateDownloadProgress, CancellationToken ct = default)
        {
            using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;
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
        }
    }
}
