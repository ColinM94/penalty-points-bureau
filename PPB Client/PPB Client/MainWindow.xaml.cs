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

            // Subscribes to the ChangeView event in CurrentView class which allows the main view to be changed from anywhere in the program. 
            CurrentView.ViewChanged += ChangeView;
        }

        // New views can be added here and changed from the current view class. 
        private void ChangeView(object source, string view)
        {
            if(view.Contains("LoginView"))
                Main.Content = new LoginView();

            if (view.Contains("HomeView"))
                Main.Content = new HomeView();
        }
    }
}
