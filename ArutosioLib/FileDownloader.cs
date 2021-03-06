﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
            // Construct HTTP request to get the file
            Console.Write("--- Dowloading: "); CColor.WriteLineC(Program.FixStringChar(link.Split('/').Last()), "yellow");
            using (WebClient client = new WebClient())
            {
                try
                {
                    // Specify that the DownloadFileCallback method gets called
                    // Specify a progress notification handler.
                    client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressCallback);
                    // when the download completes.
                    //client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCallback);
                    client.DownloadFileAsync(new Uri(link), nFileFix);
                }
                catch (Exception ex) { Program.pL.Percent = -1; CColor.WriteLineC(" ### Error # \r\n" + ex.Message, "red"); }
            }
        }
        public static bool CheckURLValid(string source)
        {
            bool res = false;
            Uri uriResult;
            if (Uri.TryCreate(source, UriKind.Absolute, out uriResult) && uriResult.Scheme == Uri.UriSchemeHttp)
            {
                WebRequest request = WebRequest.Create(source);
                // If required by the server, set the credentials.  
                request.Credentials = CredentialCache.DefaultCredentials;
                // Get the response.  
                using (WebResponse response = request.GetResponse()) 
                {
                    // Display the status.  
                    Console.WriteLine(((HttpWebResponse)response).StatusDescription);
                    // Get the stream containing content returned by the server.  
                    using (Stream dataStream = response.GetResponseStream()) 
                    {
                        // Open the stream using a StreamReader for easy access.  
                        using (StreamReader reader = new StreamReader(dataStream))
                        {
                            // Read the content.  
                            string responseFromServer = reader.ReadToEnd();
                            // Display the content.  
                            Console.WriteLine(responseFromServer);
                            // with USING Clean up the streams and the response.
                            reader.Close();
                        }
                        dataStream.Close();
                    }
                    response.Close();
                }
            }
            res = true;
            return res;
        }
        public static bool IsURLExist(string url)
        {
            bool valid = false;
            try
            {
                WebRequest req = WebRequest.Create(url);
                using (WebResponse res = req.GetResponse()) { }
                valid = true;
            }
            catch (WebException ex)
            {
                Console.Write(" =====> "); CColor.WriteLineC(" ### " + ex.Message, "red");
            }
            return valid;
        }
        private static void DownloadProgressCallback(object sender, DownloadProgressChangedEventArgs e)
        {
            Program.pL.Percent = e.ProgressPercentage;
        }
        private static void DownloadFileCallback(object sender, AsyncCompletedEventArgs e)
        {
            CColor.WriteC(" DONE", "green"); Console.WriteLine("!");
        }
    }
}
