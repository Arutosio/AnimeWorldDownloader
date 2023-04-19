using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader_App.Models
{
    public class AnimeDetail : Anime
    {
        public string State { get; set; }
        public DateTime DateRelease { get; set; }
        public int NumEpisodes { get; set; }
    }

    internal class AnimeDetailModel : AnimeModel
    {
        public int NumEpisodes { get; set; }
        public string State { get; set; }
        public DateTime DateRelease { get; set; }
    }
}
