using System.Windows;
using FeatBit.Sdk.Client;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(
            IFbClient fbClient)
        {
            InitializeComponent();
            DataContext = new MainViewModel(fbClient);
        }
    }
}