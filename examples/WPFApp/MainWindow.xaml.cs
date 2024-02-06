using FeatBit.ClientSdk;
using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IFbClient _fbClient;
        public MainWindow(MainViewModel viewModel, IFbClient fbClient)
        {
            InitializeComponent();
            this.DataContext = viewModel;
            _fbClient = fbClient;
        }
    }
}