using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Windows.Media.Protection.PlayReady;
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
    }
}
