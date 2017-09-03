using PPB_Client.Connection;
using PPB_Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PPB_Client.Views
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        static DispatcherTimer timer;

        public HomeView()
        {
            InitializeComponent();
            StartIdleTimer();
        }

        // Starts timer on user going idle. 
        private void StartIdleTimer()
        {
            // Stops current timer 
            if(timer != null)
            {
                timer.Stop();
            }

            // Starts new timer. 
            timer = new DispatcherTimer
            (
                TimeSpan.FromMinutes(15), DispatcherPriority.ApplicationIdle, (s, e) => { IdleLogout(); }, Application.Current.Dispatcher
            );
        }

        // Closes application and returns to login screen. 
        private static void IdleLogout()
        {
            // Creates a IdleLogout command so server can log.
            ServerCommand idleCmd = new ServerCommand();
            idleCmd.Command = "idle_logout";

            // Tells server of idle logout. 
            Server.SendToServer(idleCmd);

            new ManualResetEvent(false).WaitOne(5000);

            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        // Resets idle timer on key press.
        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            StartIdleTimer();
        }

        // Resets idle timer on mouse move.
        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            StartIdleTimer();
        }
    }
}
