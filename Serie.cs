using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader
{
    class Serie
    {
        public readonly string nameSerie;
        public readonly int nEpisodes;
        private string nameFile;
        private string baseNameFile;
        private string linkSerie;
        public NumberEpisode numberEpisode;

        public Serie(string nameSerie, int nEpisodes, string nameFile, NumberEpisode numberEpisode)
        {
            this.nameSerie = nameSerie;
            this.nEpisodes = nEpisodes;
            this.nameFile = nameFile;
            this.numberEpisode = numberEpisode;
        }
        public Serie(string nameSerie, int nEpisodes, string linkSerie, string nameFile, string baseNameFile, NumberEpisode numberEpisode)
        {
            this.nameSerie = nameSerie;
            this.nEpisodes = nEpisodes;
            this.linkSerie = linkSerie;
            this.nameFile = nameFile;
            this.baseNameFile = baseNameFile;
            this.numberEpisode = numberEpisode;
        }
        public string GetNameFile()
        {
            return nameFile.Replace(numberEpisode.nEpStr, numberEpisode.ToString());
        }
    }
}
