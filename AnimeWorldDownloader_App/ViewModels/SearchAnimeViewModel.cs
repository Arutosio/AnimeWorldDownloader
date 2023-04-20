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
                // Recupera i dati dalla sorgente ""
                string html = httpTalker.GetResoultFromUri(searchUri);

                // nome div classe dove si trovano gli anime 'film-list'
                List<string> elements = HtmlReader.GetItemsWithClass(html, "film-list", "div.item");

                // itera tutti i i contenuti di ogni div "item" e stampa il loro contenuto
                foreach (string ele in elements)
                {
                    //List<string> items = HtmlReader.GetAllTagsFromHtml(ele);
                    List<string> childElements = HtmlReader.GetItemsWithClass(ele, "inner", "a");


                    animes.Add(new Anime
                    {
                        UriDetail = HtmlReader.GetLinkHrefsFromHtml(ele)[0],
                        ImageUrl = HtmlReader.GetImageSrcFromHtml(childElements[0]), 
                        Name = HtmlReader.GetTagValue(childElements[1], "name")
                    });
                }

                // Recupera i dati dalla sorgente ""
                // supponiamo che i dati siano recuperati da un database
                // Esempio di codice di esempio 

                animes.Add(new Anime { Name = "Alice", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", UriDetail = "https://www.animeworld.tv/play/the-idolmster-cinderella-girls-u149.9H-A8/3Rmqy4" });
                animes.Add(new Anime { Name = "Bob", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", UriDetail = "https://www.animeworld.tv/play/the-idolmster-cinderella-girls-u149.9H-A8/3Rmqy4" });
                animes.Add(new Anime { Name = "Charlie", ImageUrl = "https://img.animeworld.tv/locandine/68073l.jpg", UriDetail = "https://www.animeworld.tv/play/the-idolmster-cinderella-girls-u149.9H-A8/3Rmqy4" });
            }

            List<AnimeViewModel> tmpAnimeViewModels = animes.Select(a => new AnimeViewModel(a)).ToList();
            AnimeViewModels = new ObservableCollection<AnimeViewModel>(tmpAnimeViewModels);
        }
    }
}
