using PPB_Client.Views;
using System.Windows;

namespace PPB_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Main.Content = new LoginView();
            Status.Content = new StatusView();
            //DataContext = new LoginViewModel();
        }
    }
}
