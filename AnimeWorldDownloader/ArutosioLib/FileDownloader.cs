using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader.ArutosioLib
{
    public class FileDownloader
    {
        public static void DoAGetRequest(string link, string nFileFix)
        {
            if (IsURLExist(link))
            {
                // Construct HTTP request to get the file
                Console.Write("--- Dowloading: "); CColor.WriteLineC(link.Split('/')[link.Split('/').Length - 1], "yellow");
                using (var client = new WebClient())
                {
                    // Specify that the DownloadFileCallback method gets called
                    // Specify a progress notification handler.
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                    // when the download completes.
                    //client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                    client.DownloadFileAsync(new Uri(link), nFileFix);
                }
            }
        }
        public static bool IsURLExist(string url)
        {
            bool valid = false;
            try
            {
                WebRequest req = WebRequest.Create(url);
                WebResponse res = req.GetResponse();
                valid = true;
            }
            catch (WebException ex)
            {
                Program.pL.Percent = -1;
                Console.Write(" =====> "); CColor.WriteLineC(" ### " + ex.Message, "red");
            }
            return valid;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static  void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            Program.pL.Percent = e.ProgressPercentage;
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        private static void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            //CColor.WriteC(" DONE", "green"); Console.WriteLine("!");
        }
    }
}
