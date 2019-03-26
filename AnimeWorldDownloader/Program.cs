using System;
using System.Net;
using System.IO;
using AnimeWorldDownloader.ArutosioLib;
using System.ComponentModel;

namespace AnimeWorldDownloader
{
    class Program
    {
        public static ProgressLine pL;
        
        static void Main(string[] args)
        {
            /*Declaration Fase */
            string pLink, link, nFile, replace, wRepla, path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            path = path.Replace(path.Split('\\')[path.Split('\\').Length - 1], "");
            int riprendiDalla = 0, nEpisodi = 0;
            /*Preparation FASE*/
            Console.WriteLine("--- <-_ Benvenuti su AnimeWorldDownloader by Arutosio - Testo a cura di Jamlegend _-> ---");
            Console.Write("         ~  "); CColor.WriteC("Per informazione consultare la pagina GitHub della repository", "cyan"); Console.WriteLine("  ~        ");

            Console.Write("Path: "); CColor.WriteC(path,"yellow");
            do
            {
                Console.WriteLine();
                LineFase("Inizio fase PREPARATORIA");

                Console.WriteLine("--- Inserisci l'URL diretto dell'episodio da Scaricare: ");
                pLink = CColor.ReadLineC("cyan");

                nFile = pLink.Split('/')[pLink.Split('/').Length - 1];

                Console.Write("--- Inserisci il Numero dell'episodio tratto dall'URL (ES:00, 04, 11): ");
                replace = CColor.ReadLineC("Yellow");

                nEpisodi = GetNumberOfC("--- Inserisci il numero di episodi: ", "yellow");

                if (IsRipresa()) { riprendiDalla = GetNumberOfC("--- Inserisci il numero dell'episodio da cui vuoi riprendere a scaricare: ", "yellow"); } else riprendiDalla = 0;

                /*Procces FASE*/
                LineFase("Inizio fase SCARICAMENTO!");

                CreateFolder(path, nFile.Split('_')[0]);
                for (int i = riprendiDalla; nEpisodi >= i; i++)
                {
                    pL = new ProgressLine(30);
                    wRepla = i < 10 ? "0" + i : "" + i;
                    link = pLink.Replace(replace, wRepla);
                    Console.Write(" GET> "); CColor.WriteLineC(link,"cyan");
                    FileDownloader.DoAGetRequest(link, path + nFile.Split('_')[0] + @"\" + nFile.Replace(replace, wRepla));
                    pL.SincePrintProgress();
                }
                Console.Write("\r\n======> "); CColor.WriteC("PROCESSO CONCLUSO!","green"); Console.WriteLine(" <======");
                Console.Write("Premi "); CColor.WriteC("Y", "green"); Console.Write(" se vuoi scaricare un'altro anime, altrimenti premi un altro tasto per "); CColor.WriteC("USCIRE", "red"); Console.Write(": ");
            } while (Console.ReadKey().KeyChar.ToString().ToLower().Equals("y"));
        }
        ///STATIC Method!
        public static double PersentageCalculation(double current, double maximum)
        {
            return (current / maximum) * 100;
        }
        public static void LineFase(string text)
        {
            Console.BackgroundColor = ConsoleColor.White; Console.Write("::::::::::::::::::::::");
            CColor.WriteC(text, "black");
            Console.BackgroundColor = ConsoleColor.White;  Console.WriteLine("::::::::::::::::::::::\r\n");
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
                catch { Console.WriteLine("--- Non hai inserito un numero valido, Riprova: "); }
            } while (repeatC);
            return res;
        }
        public static bool IsRipresa()
        {
            Console.Write("--- Devi riprendere da una certa puntata? ["); CColor.WriteC("Y", "Green"); Console.Write(" = "); CColor.WriteC("Yes", "green");  Console.Write("]: ");
            if (Console.ReadKey().KeyChar.ToString().ToLower().Equals("y"))
            {
                Console.WriteLine();
                return true;
            }
            Console.WriteLine();
            return false;
        }
        public static void CreateFolder(string path, string name)
        {
            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(path+name))
                {
                    Console.Write("Verrà creata una cartella con il nome: "); CColor.WriteLineC(name, "yellow");
                } else return;

                // Try to create the directory.
                DirectoryInfo di = Directory.CreateDirectory(path+name);
                Console.Write("La cartella e stata creata con successo il ");
                CColor.WriteLineC(Directory.GetCreationTime(path+name).ToString(),"magenta"); 

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
    }
}
