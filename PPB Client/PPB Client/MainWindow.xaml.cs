using PPB_Client.Helpers;
using PPB_Client.Views;
using System;
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
            Status.Content = new StatusView();
            Main.Content = new LoginView();

            CurrentView.ViewChanged += ChangeView;


        }

        private void ChangeView(object source, string view)
        {
            if(view.Contains("LoginView"))
                Main.Content = new LoginView();

            if (view.Contains("HomeView"))
                Main.Content = new HomeView();
        }
    }
}
