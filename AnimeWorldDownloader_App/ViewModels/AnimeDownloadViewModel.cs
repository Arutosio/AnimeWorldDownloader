using AnimeWorldDownloader_App.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimeWorldDownloader_App.ViewModels
{
    public class AnimeDownloadViewModel : AnimeViewModel
    {
        private ObservableCollection<EpisodeModel> _episodeModels = new();

        public AnimeDownloadViewModel(string uriAnimeDetail)
        {
            AnimeDownloadModel animeDownloadModel = AnimeDownloadModel.GetAnimeDownloadModel(uriAnimeDetail);
            this.Name = animeDownloadModel.Name;
            this.UriDetail = animeDownloadModel.UriDetail;
            this.ImageUrl = animeDownloadModel.ImageUrl;
            this.EpisodeModels = new(animeDownloadModel.EpisodeModels);
        }

        public ObservableCollection<EpisodeModel> EpisodeModels
        {
            get { return _episodeModels; }
            set
            {
                if (_episodeModels != null && _episodeModels != value)
                {
                    _episodeModels = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }
    }
}
