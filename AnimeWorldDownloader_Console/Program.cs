using System;
using System.Net;
using System.IO;
using AnimeWorldDownloader.ArutosioLib;
using System.ComponentModel;
using System.Deployment;


namespace AnimeWorldDownloader
{
    class Program
    {
        public static ProgressLine pL;
        public static readonly string version = "3.5";
        //public static System.Deployment.Application.ApplicationDeployment CurrentDeployment { get; }
        static void Main(string[] args)
        {
            /*Declaration FASE */
            string pLink;
            string link;
            string fullNameFile;
            string baseNameFile;
            string replace = "";
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Replace(path.Split('\\')[path.Split('\\').Length - 1], "");
            int riprendiDalla = 0, nEpisodi = 0;
            NumberEpisode objNEs;
            Serie serie;
            /*Preparation FASE*/
            Console.WriteLine($"--- <-_ Benvenuti su AnimeWorldDownloader v{version} by Arutosio - Testo a cura di Jamlegend _-> ---");
            Console.Write("         ~  "); CColor.WriteC("Per informazione consultare la pagina GitHub della repository", "cyan"); Console.WriteLine("  ~        ");
            CColor.WriteLineC("Nota: si consiglia fortemente di scaricare l'eseguibile dal sito dello sviluppatore al seguente link", "darkyellow");
            CColor.WriteLineC("       https://github.com/Arutosio/AnimeWorldDownloader/releases", "cyan");
            Console.Write("Path: "); CColor.WriteC(path, "yellow");
            do
            {
                Console.WriteLine();
                LineFase("Inizio fase PREPARATORIA");
                do
                {
                    Console.WriteLine("--- Inserisci l'URL diretto dell'episodio da Scaricare: ");
                    pLink = CColor.ReadLineC("cyan");

                } while (!IsValidUri(pLink));

                fullNameFile = FixStringChar(pLink.Split('/')[pLink.Split('/').Length - 1]);
                baseNameFile = fullNameFile.Split('_')[0];
                try
                {
                    if (Convert.ToInt32(fullNameFile.Split('_')[2]) != -1)
                    {
                        replace = fullNameFile.Split('_')[2];
                        //numOfZeroOnNumberEp = (fullNameFile.Split('_')[2].Length);
                        Console.Write("Numero dell'episodio riconosciuto con sucesso: ");
                        CColor.WriteLineC(replace, "yellow");
                    }
                }
                catch
                {
                    CColor.WriteLineC("Numero dell'episodio non riconosciuto, inserire manualmente.", "red");
                    Console.Write("--- Inserisci il Numero dell'episodio tratto dall'URL (ES:00, 04, 11): ");
                    replace = CColor.ReadLineC("Yellow");
                }
                nEpisodi = GetNumberOfC("--- Inserisci il numero di episodi: ", "yellow");
                if (IsRipresa()) { 
                    riprendiDalla = GetNumberOfC("--- Inserisci il numero dell'episodio da cui vuoi riprendere a scaricare: ", "yellow");
                    objNEs = new NumberEpisode(riprendiDalla, replace);
                } else objNEs = new NumberEpisode(replace);

                serie = new Serie(baseNameFile, nEpisodi, fullNameFile, objNEs);
                /*Procces FASE*/
                LineFase("Inizio fase SCARICAMENTO!");
                CreateFolder(path, baseNameFile);
                while(serie.nEpisodes >= serie.numberEpisode.GetNumber())
                {
                    pL = new ProgressLine(30);
                    link = pLink.Replace(replace, serie.numberEpisode.ToString());
                    Console.Write(" GET> "); CColor.WriteLineC(link, "cyan");
                    if (FileDownloader.IsURLExist(link))
                    {
                        string filePath = path + serie.nameSerie + @"\" + serie.GetNameFile();
                        FileDownloader.DoAGetRequest(link, filePath);
                        pL.SincePrintProgress();
                    }
                    serie.numberEpisode.IncrementNumber();
                }
                Console.Write("\r\n======> "); CColor.WriteC("PROCESSO CONCLUSO!", "green"); Console.WriteLine(" <======");
                Console.Write("Premi "); CColor.WriteC("Y", "green"); Console.Write(" se vuoi scaricare un'altro anime, altrimenti premi un altro tasto per "); CColor.WriteC("USCIRE", "red"); Console.Write(": ");
            } while (Console.ReadKey().KeyChar.ToString().ToLower().Equals("y"));
        }
        ///STATIC Method!
        public static bool IsValidUri(string link)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(link, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if (result)
            {
                //result = FileDownloader.CheckURLValid(link); TO DO!
            }
            else { Console.WriteLine("non hai inserito un url valido. Riprova."); }
            return result;
        }
        public static double PersentageCalculation(double current, double maximum)
        {
            return (current / maximum) * 100;
        }
        public static void LineFase(string text)
        {
            Console.BackgroundColor = ConsoleColor.White; Console.Write("::::::::::::::::::::::");
            CColor.WriteC(text, "black");
            Console.BackgroundColor = ConsoleColor.White; Console.WriteLine("::::::::::::::::::::::\r\n");
            Console.ResetColor();
        }
        public static int GetNumberOfC(string print, string color)
        {
            Console.Write(print);
            bool repeatC = true;
            int res = 0;
            do
            {
                try { res = Convert.ToInt32(CColor.ReadLineC(color)); Console.ResetColor(); repeatC = false; }
                catch { Console.Write("--- Non hai inserito un numero valido, Riprova: "); }
            } while (repeatC);
            return res;
        }
        public static bool IsRipresa()
        {
            Console.Write("--- Devi riprendere da una certa puntata? ["); CColor.WriteC("Y", "Green"); Console.Write(" = "); CColor.WriteC("Yes", "green"); Console.Write("]: ");
            if (Console.ReadKey().KeyChar.ToString().ToLower().Equals("y"))
            {
                Console.WriteLine();
                return true;
            }
            Console.WriteLine();
            return false;
        }
        public static string FixStringChar(string str)
        {
            str = str.Replace("%5B", "[");
            str = str.Replace("%5D", "]");
            str = str.Replace("%20", " ");
            return str;
        }
        public static void CreateFolder(string path, string name)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path + name))
                {
                    Console.Write("Verrà creata una cartella con il nome: "); CColor.WriteLineC(name, "yellow");
                }
                else return;

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path + name);
                Console.Write("La cartella e stata creata con successo il ");
                CColor.WriteLineC(Directory.GetCreationTime(path + name).ToString(), "magenta");

                // Delete the directory.
                //di.Delete();
                //Console.WriteLine("The directory was deleted successfully.");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("The process failed: {0}"); Console.WriteLine(e.ToString());
                Console.ResetColor();
            }
            finally { }
        }
    }
}
