using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

namespace PPB_Client
{
    public partial class Client : Window
    {
        TcpClient tcpclnt = new TcpClient();
        string server = "localhost";
        int port = 2000;

        public Client()
        {
            InitializeComponent();

            tcpclnt.Connect(server, port);

            

        }
    }
}
