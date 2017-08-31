using Newtonsoft.Json;
using PPB_Client.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace PPB_Client.Helpers
{
    public static class Server 
    {
        public static event EventHandler ServerConnected;
        public static event EventHandler ServerDisconnected;
        public static event EventHandler MsgRevieved;

        private static void OnServerConnected()
        {
            Connected = true;
            ServerConnected?.Invoke(null, EventArgs.Empty);
        }

        private static void OnServerDisconnected()
        {
            Connected = false;
            ServerDisconnected?.Invoke(null, EventArgs.Empty);
        }

        public static void OnMsgRecieved()
        {
            MsgRevieved?.Invoke(null, EventArgs.Empty);
        }

        static Server()
        {
            Thread connectionThread = new Thread(() => Connect());
            connectionThread.Start();
        }

        static String server = "127.0.0.1";
        static int port = 2000;
        static TcpClient client;
        public static bool Connected { get; set; } = false;

        /// <summary>
        /// Attempts to open connection to server.
        /// </summary>
        public static void Connect()
        {
            while (!Connected)
            {
                try
                {              
                    client = new TcpClient(server, port);
                    MsgServer("test");
                }
                catch (Exception ex)
                {
                    //MessageBox.Show("Error connecting to PPB Server!\n\n" + ex.ToString());
                }

                Thread.Sleep(1000);
            }            
        }
     
        // Send message to server.
        public static void MsgServer(string msg)
        {            
            Byte[] data = Encoding.ASCII.GetBytes(msg);

            try
            {
                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);
                OnServerConnected();
            }
            catch
            {
                OnServerDisconnected();
                Thread connectionThread = new Thread(() => Connect());
                connectionThread.Start();
            }
        }

        // Sends json object containing properties and the method to be called on the server.
        public static void SendJson(object ModelObject)
        {
            string json = JsonConvert.SerializeObject(ModelObject);
            string encryptedJson = Encrypt.EncryptString(json, "ppb");

            MsgServer(encryptedJson);
        }

        public static bool Login(string username, string password)
        {
            MsgServer($"LoginUsername=${username}$");

            //TODO : Get user id from client and login

            return false;

            //Send
            /*
            Send()
            // Creates user object with entered username and password. 
            UserModel user = new UserModel();
            user.Method = "Login";
            user.Username = CurrentUsername;
            user.Password = CurrentPassword;

            Server.Send(user);
            */
        }

        private static void Disconnect()
        {
            client.Close();

            MsgServer("test");
        }
    }
}
