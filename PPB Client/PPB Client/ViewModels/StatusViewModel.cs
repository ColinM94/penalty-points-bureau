using System;
using System.Windows;
using System.Windows.Input;
using PPB_Client.Connection;
using PPB_Client.Helpers;

namespace PPB_Client.ViewModels
{
    // Logic for StatusView.
    public class StatusViewModel : BaseViewModel
    {
        // Events
        private void OnServerConnected(object source, EventArgs e)
        {
            ServerStatus = "✓";
            ServerStatusForeground = "Green";
            ServerStatusToolTip = "Connected to PPB Server";
        }

        private void OnServerDisconnected(object source, EventArgs e)
        {
            ServerStatus = "X";
            ServerStatusForeground = "Red";
            ServerStatusToolTip = "No Connection to PPB Server";
        }       

        // Constructor.
        public StatusViewModel()
        {
            // Defaults server status to disconnected. 
            OnServerDisconnected(null, null);
            
            // Sets up logout command.
            LogoutCommand = new RelayCommand(Logout);

            // Subscribes to events.
            Server.ServerConnected += OnServerConnected;
            Server.ServerDisconnected += OnServerDisconnected;
        }

        // Logout command which will be triggered from the UI. 
        public ICommand LogoutCommand { get; private set; }

        // Server connection status. 
        private string serverStatus;
        public string ServerStatus
        {
            get
            {
                return serverStatus;
            }

            set
            {
                serverStatus = value;
                OnPropertyChanged();
            }
        }

        // Server connection status colour.
        private string serverStatusForeground;
        public string ServerStatusForeground
        {
            get
            {
                return serverStatusForeground;
            }

            set
            {
                serverStatusForeground = value;
                OnPropertyChanged();
            }
        }

        // Server tooltip text. 
        private string serverStatusToolTip;
        public string ServerStatusToolTip
        {
            get
            {
                return serverStatusToolTip;
            }
            set
            {
                serverStatusToolTip = value;
                OnPropertyChanged();
            }
        }

        // Ends current session and restarts program, returning the user to the login screen. 
        private void Logout(object parameter)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}
