using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader
{
    class NumberEpisode
    {
        private int number = 0;
        public readonly string nEpStr;
        private int qZero;
        public NumberEpisode(string nEpStr)
        {
            this.number = 0;
            this.nEpStr = nEpStr;
            SetQZero(nEpStr);
        }
        public NumberEpisode(int number, string nEpStr)
        {
            this.number = number;
            this.nEpStr = nEpStr;
            SetQZero(nEpStr);
        }

        public int GetNumber()
        {
            return number;
        }
        public void SetNumber(int value)
        {
            this.number = value;
        }
        public string GetQZero()
        {
            string res = string.Empty;
            for( int i = qZero; i > number.ToString().Length; i--)
            {
                res += '0';
            }
            return res;
        }
        public void SetQZero(string value)
        {
            if(value[0].Equals('0'))
            {
                qZero = value.Length; 
            }
            else
            {
                qZero = 2;
            }
        }
        public void IncrementNumber()
        {
            number++;
        }
        public override string ToString()
        {
            return GetQZero() + number;
        }
    }
}
