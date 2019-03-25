using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader.ArutosioLib
{
    class ProgressBar
    {
        private int _lastOutputLength;
        private readonly int _maximumWidth;
        public double Percent { get; set; }

        public ProgressBar(int maximumWidth)
        {
            _maximumWidth = maximumWidth;
        }
        public void BarUpdate()
        {
           do
           {
                // Remove the last state           
                string clear = string.Empty.PadRight(_lastOutputLength, '\b');
                Console.Write(clear);
                // Generate new state           
                int width = (int)(Percent / 100 * _maximumWidth);
                int fill = _maximumWidth - width;
                string output = string.Format(" {0}% - [ {1}{2} ]", Percent.ToString("0.00"), string.Empty.PadLeft(width, '█'), string.Empty.PadLeft(fill, ' '));
                //Console.Write(" {0}% - [ ", Percent.ToString("0.00")); CColor.WriteC(string.Empty.PadLeft(width, '█'),"green"); Console.Write("{0} ]", string.Empty.PadLeft(fill, ' '));
                Console.Write(output);
                _lastOutputLength = output.Length;
           } while (Percent < 100);
            //Console.WriteLine();
        }
    }
}
