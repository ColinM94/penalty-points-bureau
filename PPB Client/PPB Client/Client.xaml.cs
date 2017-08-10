using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace PPB_Client
{
    public partial class Client : Window
    {

        IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        int port = 2000;
        TcpClient connection;
        bool connected = false;
        NetworkStream stream;

        public Client()
        {
            InitializeComponent();
        }

        private void Connect()
        {
            try
            {
                connection = new TcpClient(serverIP.ToString(), port);             
                connected = true;
                ConnectionStatus();

                Thread receiveMsgThread = new Thread(delegate ()
                {
                    ReceiveMsg();            
                });
                receiveMsgThread.IsBackground = true;
                receiveMsgThread.Start();

                ConnectionStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to PPB Server!");
                connected = false;
                ConnectionStatus();
            }
        }

        private void SendMsg(String msg)
        {
            try
            {
                stream = connection.GetStream();

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(msg);

                stream.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                connected = false;
                ConnectionStatus();              
            }
        }

        private void ReceiveMsg()
        {
            stream = connection.GetStream();
            try
            {
                while (connected == true)
                {                  
                    if (stream.CanRead)
                    {
                        byte[] readBuffer = new byte[1024];
                        StringBuilder Msg = new StringBuilder();
                        int numBytes = 0;

                        do
                        {
                            numBytes = stream.Read(readBuffer, 0, readBuffer.Length);
                            Msg.AppendFormat("{0}", Encoding.ASCII.GetString(readBuffer, 0, numBytes));
                        }
                        while (stream.DataAvailable);

                        if(Msg.ToString().Equals("Ping"))
                        {
                            SendMsg("Pong");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                connected = false;
            }
        }

        private void Disconnect()
        {
            connection.Close();
            connected = false;
            ConnectionStatus();
        }

        private void ConnectionStatus()
        {
            if (connected == true)
            {
                //Sets server status and colour
                ServerStatusLabel.Content = "Connected";
                ServerStatusLabel.Foreground = new SolidColorBrush(Colors.Green);

                ConnectBtn.IsEnabled = false;
                DisconnectBtn.IsEnabled = true;

                MsgBox.IsEnabled = true;
                SendMsgBtn.IsEnabled = true;
            }

            else
            {
                //Sets server status and colour
                ServerStatusLabel.Content = "Not Connected";
                ServerStatusLabel.Foreground = new SolidColorBrush(Colors.Red);

                ConnectBtn.IsEnabled = true;
                DisconnectBtn.IsEnabled = false;

                MsgBox.IsEnabled = false;
                SendMsgBtn.IsEnabled = false;
            }      
        }
     
        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            Connect();
        }

        private void DisconnectBtn_Click(object sender, RoutedEventArgs e)
        {
            Disconnect();
        }

        private void SendMsgBtn_Click(object sender, RoutedEventArgs e)
        {
            SendMsg(MsgBox.Text);
        }
    }
}

