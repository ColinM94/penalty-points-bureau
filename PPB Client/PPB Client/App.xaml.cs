using System;
using System.Windows;

namespace PPB_Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Code runs on program shutdown. 
        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
