using System;
using System.Net;
using System.IO;
using static System.Net.Mime.MediaTypeNames;

namespace AnimeWorldDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            /*Declaration Fase */
            string pLink, link, nFile, replace, wRepla, path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Replace(path.Split('\\')[path.Split('\\').Length -1],"");
            int riprendiDalla = 0, nEpisodi = 0;
            /*Preparation Fase*/
            Console.WriteLine("--- <-_ Benvenuti su AnimeWorldDownloader by Arutosio - Testo a cura di Jamlegend _-> ---");
            Console.Write("        ~  "); Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("Per informazione consultare la pagina GitHub della repository");
            Console.ResetColor(); Console.WriteLine("  ~       ");
            Console.ResetColor();
            Console.Write("Path: "); Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine(path); Console.ResetColor();
            LineFase("Inizio fase PREPARATORIA");
            pLink = GetPLink();
            nFile = GetFileName(pLink);
            replace = GetRemplaceChange();
            nEpisodi = GetNEpisodi();
            if (IsRipresa()) { riprendiDalla = GetNERiprendere(); }
            /*Star proces Fase*/
            LineFase("Inizio fase SCARICAMENTO!");
            CreateFolder(path, nFile.Split('_')[0]);
            for (int i = riprendiDalla; nEpisodi >= i; i++)
            {
                wRepla = i < 10 ? "0" + i : "" + i;
                link = pLink.Replace(replace, wRepla);
                Console.WriteLine();
                Console.Write("GET> ");
                Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine(link); ; Console.ResetColor();
                DoAGetRequest(link, path+ nFile.Split('_')[0]+@"\"+nFile.Replace(replace, wRepla));
            }
            Console.Write("\r\n======"); Console.ForegroundColor = ConsoleColor.Green; Console.Write("PROCESSO COMPLETATO!"); Console.ResetColor(); Console.WriteLine("====== premi un tasto per chiudere il programma.");
            Console.ReadKey();
        }
        public static void LineFase(String text)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write("::::::::::::::::::::::"); Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(text); Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("::::::::::::::::::::::\r\n"); Console.ResetColor();
        }
        public static string GetPLink()
        {
            Console.Write("--- Inserisci l'URL diretto dell'episodio da Scaricare: ");
            string res;
            Console.ForegroundColor = ConsoleColor.Cyan; res = Console.ReadLine(); Console.ResetColor();
            return res;
        }
        public static string GetFileName(string pLink)
        {
            return pLink.Split('/')[pLink.Split('/').Length - 1];
        }
        public static string GetRemplaceChange()
        {
            string res;
            Console.Write("--- Inserisci il Numero dell'episodio tratto dall'URL (ES:00, 04, 11): ");
            Console.ForegroundColor = ConsoleColor.Yellow; res = Console.ReadLine(); Console.ResetColor();
            return res;
        }
        public static int GetNEpisodi()
        {
            bool repeatC = true;
            int res = 0;
            do
            {
                Console.Write("--- Inserisci il numero di episodi: "); Console.ForegroundColor = ConsoleColor.Yellow;
                try { res = Convert.ToInt32(Console.ReadLine()); Console.ResetColor(); repeatC = false; }
                catch { Console.ResetColor(); Console.WriteLine("--- Non hai inserito un numero valido"); }
            } while (repeatC);
            return res;
        }
        public static bool IsRipresa()
        {
            Console.Write("--- Devi riprendere da una certa puntata?["); Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("y"); Console.ResetColor(); Console.Write("= yes]: "); Console.ForegroundColor = ConsoleColor.Green;
            if (Console.ReadKey().KeyChar.ToString().ToLower().Equals("y"))
            {
                Console.ResetColor();
                Console.WriteLine();
                return true;
            }
            Console.ResetColor();
            return false;
        }
        public static int GetNERiprendere()
        {
            bool repeatC = true;
            int res = 0;
            do
            {
                Console.Write("--- Inserisci il numero dell'episodio da cui vuoi riprendere a scaricare: "); Console.ForegroundColor = ConsoleColor.Yellow;
                try { res = Convert.ToInt32(Console.ReadLine()); repeatC = false; }
                catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("--- Non hai inserito un numero valido"); }
                Console.ResetColor();
            } while (repeatC);
            return res;
        }
        public static string GetFolderName(string pLink)
        {
            string res = pLink.Split('/')[pLink.Split('/').Length - 1].Split('_')[0];
            Console.Write("Imposto per la creazione della cartella..  "); Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(res); Console.ResetColor();
            return res;
        }
        public static void CreateFolder(string path, string name)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path+name))
                {
                    Console.WriteLine("Verrà creata una cartella con il nome: " + name);
                } else return;

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path+name);
                Console.Write("La cartella e stata creata con successo. "); Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(Directory.GetCreationTime(path+name)); Console.ResetColor();

                // Delete the directory.
                //di.Delete();
                //Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("The process failed: {0}");Console.WriteLine(e.ToString());
                Console.ResetColor();
            }
            finally { }
        }
        public static void DoAGetRequest(string link, string nFileFix)
        {
            // Construct HTTP request to get the file
            Console.Write("--- Dowloading: "); Console.ForegroundColor = ConsoleColor.Yellow;  Console.WriteLine(GetFileName(link)); Console.ResetColor();
            try
            {
                using (var client = new WebClient())
                { client.DownloadFile(link, nFileFix); }
                Console.Write("   ===> "); Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("DONE!"); Console.ResetColor();
            }
            catch { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("   ### Error ###"); Console.ResetColor(); }
        }
    }
}
