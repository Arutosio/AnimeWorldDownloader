namespace AnimeWorldDownloader_App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            var windows = base.CreateWindow(activationState);

            // Add here your sizing code
            windows.Height = 1020;
            windows.Width = 720;
            // Add here your positioning code
            //windows.X = 600;
            //windows.Y = 30;

            return windows;
        }
    }
}