namespace AnimeWorldDownloader_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell())
            {
                Height = 700,
                Width = 520
            };

            return window;
        }
    }
}