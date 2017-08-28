using PPB_Client.ViewModels;
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
            ActiveItem.Content = new LoginView();
            //DataContext = new LoginViewModel();
        }
    }
}
