using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader.ArutosioLib
{
    public static class CColor
    {
        public static void WriteC(string write, string color)
        {
            switch (color.ToLower())
            {
                case "black":
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkblue":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkgreen":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkcyan":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkred":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkmagenta":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkyellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "darkgray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "gray":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write(write); Console.ResetColor();
                    break;
                case "white":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write(write); Console.ResetColor();
                    break;
                default:
                    Console.Write(write); Console.Write("(Error:ForegroundColor " + '"' + (color != null ? color + '"' + "not found." : "you did not choose a color"));
                    break;
            }
        }
        public static void WriteLineC(string write, string color)
        {
            switch (color.ToLower())
            {
                case "black":
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkblue":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkgreen":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkcyan":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkred":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkmagenta":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkyellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkgray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "gray":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "white":
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                default:
                    Console.Write(write); Console.Write("(Error:ForegroundColor " + '"' + (color != null ? color + '"' + "not found." : "you did not choose a color"));
                    break;
            }
        }
        public static void BackgroundC(string write, string color)
        {
            switch (color.ToLower())
            {
                case "black":
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkblue":
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkgreen":
                    Console.BackgroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkcyan":
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkred":
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkmagenta":
                    Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkyellow":
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "darkgray":
                    Console.BackgroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "gray":
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "blue":
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "green":
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "cyan":
                    Console.BackgroundColor = ConsoleColor.Cyan;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "red":
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "magenta":
                    Console.BackgroundColor = ConsoleColor.Magenta;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "yellow":
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                case "white":
                    Console.BackgroundColor = ConsoleColor.White;
                    Console.WriteLine(write); Console.ResetColor();
                    break;
                default:
                    Console.Write(write); Console.Write("(Error:BackgroundColor " + '"' + (color != null ? color + '"' + "not found." : "you did not choose a color"));
                    break;
            }
        }
        public static string ReadLineC(string color)
        {
            switch (color.ToLower())
            {
                case "black":
                    Console.ForegroundColor = ConsoleColor.Black;
                    break;
                case "darkblue":
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    break;
                case "darkgreen":
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    break;
                case "darkcyan":
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case "darkred":
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case "darkmagenta":
                    Console.ForegroundColor = ConsoleColor.DarkMagenta;
                    break;
                case "darkyellow":
                    Console.ForegroundColor = ConsoleColor.DarkYellow;
                    break;
                case "darkgray":
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    break;
                case "gray":
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case "blue":
                    Console.ForegroundColor = ConsoleColor.Blue;
                    break;
                case "green":
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case "cyan":
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case "red":
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case "magenta":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;
                case "yellow":
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case "white":
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                default:
                    Console.WriteLine("(Error:ForegroundColor " + '"' + (color != null ? color + '"' + "not found." : "you did not choose a color"));
                    break;
            }
            string res = Console.ReadLine();
            Console.ResetColor();
            return res;
        }
    }
}

