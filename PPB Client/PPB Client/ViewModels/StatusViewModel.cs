using PPB_Client.Helpers;
using System;
using System.Windows;

namespace PPB_Client.ViewModels
{
    public class StatusViewModel : BaseViewModel
    {
        #region Events
        private void OnServerConnected(object source, EventArgs e)
        {
            SetConnected();
        }

        private void OnServerDisconnected(object source, EventArgs e)
        {
            SetDisconnected();
        }

        private void SetConnected()
        {
            ServerStatus = "✓";
            ServerStatusForeground = "Green";
            ServerStatusToolTip = "Connected to PPB Server";
        }

        private void SetDisconnected()
        {
            ServerStatus = "X";
            ServerStatusForeground = "Red";
            ServerStatusToolTip = "No Connection to PPB Server";
        }
       
        #endregion

        public StatusViewModel()
        {
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
    }
}
