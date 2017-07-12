using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;

namespace PPB_Server
{
    public partial class MainWindow : Window
    {
        public IPAddress up = IPAddress.Parse("127.0.0.1");
        public int port = 2000;
        public TcpListener server;

        public MainWindow()
        {
            InitializeComponent();
        }

        void StartServer()  // Setup connection 
        {
            server.Start();
        }

        void CloseConn() // Close connection.
        {
            server.Stop();
        }
    }
}
