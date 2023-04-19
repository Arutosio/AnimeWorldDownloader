using AnimeWorldDownloader_App.ViewModels;

namespace AnimeWorldDownloader_App.Models
{
    internal class AnimeModel
    {
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public AnimeModel(Anime anime)
        {
            Name = anime.Name;
            ImageUrl = anime.ImageUrl;
        }
    }
}
