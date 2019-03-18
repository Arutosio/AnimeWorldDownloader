using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Declaration Fase */
            string pLink, link, nFile, replace, wRepla;
            int riprendiDalla = 0, nEpisodi = 0;
            /*Preparation Fase*/
            Console.WriteLine("--- ~~~Welcome to MultiDownloadFile whit a link Reference!~~~ ---");
            pLink = getLinkT();
            nFile = getFileName(pLink);
            replace = getRemplaceChange();
            nEpisodi = getNEpisodi();
            if (isRipresa()) { riprendiDalla = getNERiprendere(); }
            /*Star proces Fase*/
            for (int i = riprendiDalla; nEpisodi >= i; i++)
            {
                wRepla = i < 10 ? "0" + i : "" + i;
                link = pLink.Replace(replace, wRepla);
                Console.WriteLine();
                Console.WriteLine("--- GET ~ " + link);
                DoAGetRequest(link, nFile.Replace(replace, wRepla));
            }
            Console.WriteLine("--- PROCESS COMPETE! ---");
            Console.ReadKey();
        }
        public static string getLinkT()
        {
            Console.Write("--- Inserisci il link di template: ");
            return Console.ReadLine();
        }
        public static string getFileName(string pLink)
        {
            return pLink.Split('/')[pLink.Split('/').Length - 1];
        }
        public static string getRemplaceChange()
        {
            Console.Write("--- Inserisci la parte da incrementare: ");
            return Console.ReadLine();
        }
        public static int getNEpisodi()
        {
            bool repeatC = true;
            int res = 0;
            do
            {
                Console.Write("--- Inserisci il numero di episodi: ");
                try { res = Convert.ToInt32(Console.ReadLine()); repeatC = false; }
                catch { Console.WriteLine("--- Non hai inserito un numero valido"); }
            } while (repeatC);
            return res;
        }
        public static bool isRipresa()
        {
            Console.Write("--- Devi riprendere da una certa puntata? [y = yes]: ");
            if (Console.ReadKey().KeyChar.ToString().Equals("y"))
                return true;
            return false;
        }
        public static int getNERiprendere()
        {
            bool repeatC = true;
            int res = 0;
            do
            {
                Console.Write("--- Inserisci il numero del episodi che vuoi riprendere a scaricare: ");
                try { res = Convert.ToInt32(Console.ReadLine()); repeatC = false; }
                catch { Console.WriteLine("--- Non hai inserito un numero valido"); }
            } while (repeatC);
            return res;
        }
        public static void DoAGetRequest(string link, string nFileFix)
        {
            // Construct HTTP request to get the file
            Console.Write("--- Dowloading... " + nFileFix);
            try
            {
                using (var client = new WebClient())
                { client.DownloadFile(link, nFileFix); }
                Console.WriteLine("---> DONE!");
            }
            catch { Console.WriteLine("--- Error File Not Found: " + nFileFix); }
        }
    }
}
