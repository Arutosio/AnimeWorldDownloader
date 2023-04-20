using System.Windows;
using AnimeWorldDownloader_App.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Input;
using AnimeWorldDownloader_App.Data;

namespace AnimeWorldDownloader_App.ViewModels
{

    internal class SearchAnimeViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        string _searchText = string.Empty;
        private ObservableCollection<AnimeViewModel> _animeViewModels = new();

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public ObservableCollection<AnimeViewModel> AnimeViewModels
        {
            get { return _animeViewModels; }
            set
            {
                if (_animeViewModels != value)
                {
                    _animeViewModels = value;
                    OnPropertyChanged(); // reports this property
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string name = "") =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public SearchAnimeViewModel()
        {

        }

        public void GetSearchAnime()
        {
            List<Anime> animes = new();
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                string searchTextAdatpting = SearchText.Replace(" ", "+");
                string searchUri = $"https://www.animeworld.tv/search?keyword={searchTextAdatpting}";

                HttpTalker httpTalker = HttpTalker.GetInstance();
                // Recupero la sorgente html
                string html = httpTalker.GetResoultFromUri(searchUri);

                // 'film-list' è la classe del div dove si trovano gli anime
                List<string> elements = HtmlReader.GetItemsWithClass(html, "film-list", "div.item");

                // itera tutti i i contenuti di ogni div "item" e stampa il loro contenuto
                foreach (string ele in elements)
                {
                    //List<string> items = HtmlReader.GetAllTagsFromHtml(ele);
                    List<string> childElements = HtmlReader.GetItemsWithClass(ele, "inner", "a");

                    Anime a = new();

                    a.UriDetail = string.Concat("https://www.animeworld.tv", HtmlReader.GetLinkHrefsFromHtml(ele)[0]);
                    a.ImageUrl = HtmlReader.GetImageSrcFromHtml(childElements[0]);
                    a.Name = childElements[1];

                    animes.Add(a);
                }
            }

            List<AnimeViewModel> tmpAnimeViewModels = animes.Select(a => new AnimeViewModel(a)).ToList();
            AnimeViewModels = new ObservableCollection<AnimeViewModel>(tmpAnimeViewModels);
        }
    }
}
