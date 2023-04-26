using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
//using Windows.Media.Protection.PlayReady;
using System.Net;

namespace AnimeWorldDownloader_App.Data
{
    public class HttpTalker
    {
        private static HttpTalker httpTalker = null;
        public static HttpTalker GetInstance()
        {
            if(httpTalker == null)
            {
                httpTalker = new HttpTalker();
            }
            return httpTalker;
        }

        //private HttpClient httpClient = null;


        private HttpTalker()
        {
            //httpClient = new HttpClient();
            //httpClient.DefaultRequestHeaders.Accept.Clear();
            //httpClient.DefaultRequestHeaders.Accept.Add(
            //    new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            //httpClient.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
        }

        public string GetResoultFromUri(string url)
        {
            string html = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                html = response.Content.ReadAsStringAsync().Result;
            }

            return html;
        }

        public async Task<string> GetAsyncResoultFromUri(string url)
        {
            string html = string.Empty;

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                html = await response.Content.ReadAsStringAsync();
            }

            return html;
        }

        public void DownloadFile(string strUrl, string destinationPath)
        {
            // crea un HttpClient e una richiesta HTTP GET per scaricare il file dal URL
            var client = new HttpClient();
            var response = client.GetAsync(strUrl).Result;

            // verifica se la risposta ha successo (HTTP 2xx)
            response.EnsureSuccessStatusCode();

            // apre in scrittura il file di destinazione usando Stream
            using (var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                // scarica il contenuto del file nella memoria usando Stream
                response.Content.CopyToAsync(fileStream).Wait();
            }
        }

        private void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            Console.WriteLine("{0}% scaricato", e.ProgressPercentage);
        }

        public async Task DownloadFileAsync(string url, string savePath, Action<double> UpdateDownloadProgress)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.TransferEncodingChunked = true;

                HttpResponseMessage response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;
                long readBytes = 0L;
                byte[] buffer = new byte[4096];

                using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (Stream stream = await response.Content.ReadAsStreamAsync())
                {
                    int bytesRead;
                    do
                    {
                        bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        readBytes += bytesRead;

                        double percentage = (double)Math.Round((decimal)readBytes / totalBytes.Value * 100);
                        UpdateDownloadProgress.Invoke(percentage);
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }

        public static void CreateFolder(string path, string name)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path + name))
                {
                    //Console.Write("Verrà creata una cartella con il nome: "); CColor.WriteLineC(name, "yellow");
                }
                else return;

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path + name);
                //Console.Write("La cartella e stata creata con successo il ");
                //CColor.WriteLineC(Directory.GetCreationTime(path + name).ToString(), "magenta");

                // Delete the directory.
                //di.Delete();
                //Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
                //Console.ForegroundColor = ConsoleColor.Red;
                //Console.Write("The process failed: {0}"); Console.WriteLine(e.ToString());
                //Console.ResetColor();
            }
            finally { }
        }
    }
}
