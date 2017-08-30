using PPB_Client.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace PPB_Client.ViewModels
{
    class StatusViewModel : BaseViewModel
    {
        public StatusViewModel()
        {
            ServerStatus = "Disconnected";
            ServerStatusForeground = "Red" ;
            ServerStatusToolTip = "No Connection to PPB Server";
            //CheckStatus();

            server.ServerConnected += OnServerConnected;
            server.ServerDisconnected += OnServerDisconnected;

            server.Connect();
        }

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

        public string ServerStatusToolTip { get; set; }

        public void OnServerConnected(object source, EventArgs e)
        {
            ServerStatus = "Connected";
            //ServerStatus = ((char)0x221A).ToString();
            ServerStatusForeground = "Green";
            ServerStatusToolTip = "Connected to PPB Server";
        }

        public void OnServerDisconnected(object source, EventArgs e)
        {
            ServerStatus = "Disconnected";
            //ServerStatus = ((char)0x221A).ToString();
            ServerStatusForeground = "Red";
            ServerStatusToolTip = "No Connection to PPB Server";
        }
    }
}
