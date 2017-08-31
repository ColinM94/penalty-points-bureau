using PPB_Client.Helpers;
using System;
using System.Windows;

namespace PPB_Client.ViewModels
{
    public class StatusViewModel : BaseViewModel
    {
        public StatusViewModel()
        {
            ServerStatus = "Disconnected";
            ServerStatus = "X";
            ServerStatusForeground = "Red";
            ServerStatusToolTip = "No Connection to PPB Server";

            Server.ServerConnected += OnServerConnected;
            Server.ServerDisconnected += OnServerDisconnected;
        }      

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

        public void OnServerConnected(object source, EventArgs e)
        {
            //ServerStatus = "Connected";
            ServerStatus = "✓";
            ServerStatusForeground = "Green";
            ServerStatusToolTip = "Connected to PPB Server";
        }

        public void OnServerDisconnected(object source, EventArgs e)
        {
            //ServerStatus = "Disconnected";
            ServerStatus = "X";
            ServerStatusForeground = "Red";
            ServerStatusToolTip = "No Connection to PPB Server";
        }
    }
}
