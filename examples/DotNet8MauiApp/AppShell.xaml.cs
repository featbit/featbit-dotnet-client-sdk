namespace DotNet8MauiApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("FirstPage", typeof(MainPage));
        }
    }
}
