using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PPB_Client.Helpers
{
    public class Server
    {
        public delegate void ServerConnectedEventHandler(object source, EventArgs args);
        public event ServerConnectedEventHandler ServerConnected;

        public delegate void ServerDisconnectedEventHandler(object source, EventArgs args);
        public event ServerConnectedEventHandler ServerDisconnected;

        public Server()
        {
            //Connected = false;
            //Connect();
        }

        public bool Connected { get; private set; }

        String server = "127.0.0.1";
        int port = 2000;
        TcpClient client;
        string ServerStatus;

        protected virtual void OnServerConnected()
        {
            ServerConnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnServerDisconnected()
        {
            ServerDisconnected?.Invoke(this, EventArgs.Empty);
        }


        public void Connect()
        {
            try
            {
                client = new TcpClient(server, port);
                PingServer();
                /*
                if (client.Connected != true)
                {
                    

                   // var portnum = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

                    
               
                    //Connected = true;
                }
                */
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Error connecting to PPB Server!\n\n" + ex.ToString());

                PingServer();
            }
        }

        private void Disconnect()
        {
            client.Close();

            PingServer();
        }

        public void PingServer()
        {
            try
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes("Ping");

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                if (client.Connected == true)
                {
                    var portnum = ((IPEndPoint)client.Client.RemoteEndPoint).Port;

                    OnServerConnected();
                }
            }
            catch (Exception ex)
            {
                OnServerDisconnected();
                //MessageBox.Show("Error connecting to PPB Server!\n\n" + ex.ToString());
            }
        }
    }
}
