using System;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PPB_Client.Helpers;
using PPB_Client.Models;
using System.Collections.Generic;
using System.Windows;

namespace PPB_Client.Connection
{
    /// <summary>
    /// Contains functionality allowing the client to communicate with the PPB Server. 
    /// </summary>
    public static class Server 
    {
        #region Events
        public static event EventHandler ServerConnected;
        public static event EventHandler ServerDisconnected;
        public static event EventHandler LoginSuccess;
        public static event EventHandler LoginFailure;

        private static void OnServerConnected()
        {
            ServerConnected?.Invoke(null, EventArgs.Empty);
        }

        private static void OnServerDisconnected()
        {
            ServerDisconnected?.Invoke(null, EventArgs.Empty);
        }

        public static void OnLoginSuccess()
        {
            LoginSuccess?.Invoke(null, EventArgs.Empty);
        }

        public static void OnLoginFailed(string loginMsg)
        {
            LoginFailure?.Invoke(loginMsg, EventArgs.Empty);
        }

        #endregion

        // Properties
        static String server = "127.0.0.1";
        static int port = 2000;
        static TcpClient client;     
        static NetworkStream stream;
        static SecureString securePassword;
        static string userID;
        static bool LoggedIn;

        public static bool Connected { get; set; } = false;

        // Constructor
        static Server()
        {
            Thread connectionThread = new Thread(() => Connect());
            connectionThread.Start();
        }

        /// <summary>
        /// Attempts to set up connection with server. Monitors connection if connection successful.
        /// </summary>
        public static void Connect()
        {
            do
            {
                try
                {
                    // Setup connection to server on selected ip and port.
                    client = new TcpClient(server, port);

                    // Setup network stream with server.
                    stream = client.GetStream();

                    Connected = true;
                    
                    // Listen for messages from server on a separate thread.
                    Thread listenThread = new Thread(Listen);
                    listenThread.Start();

                    // Repeatedly tests connection with server. 
                    TestConn();
                }
                catch (Exception)
                {
                    // Waits for entered milliseconds. 
                    new ManualResetEvent(false).WaitOne(2000);
                }
            }
            while (!Connected);
        }
     
        // Send message to server.
        public static bool SendToServer(ServerCommand command)
        {
            // Serializes the command object into a json object. 
            string json = JsonConvert.SerializeObject(command);

            // Encrypts the json string. 
            string encryptedJson = Encrypt.EncryptString(json, "ppb");

            // Encodes json string into a sequence of bytes.
            Byte[] data = Encoding.ASCII.GetBytes(encryptedJson);

            try
            {
                // Sends message to server .
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to login user to PPB server. 
        /// </summary>
        public static void Login(string username, string password)
        {
            // Creates user object and adds username password. 
            User user = new User();
            user.Username = username;
            user.Password = password;

            // Converts user object to a Dictionary. 
            var UserInfo = DictionaryObjectConverter.ToDictionary(user);

            // Creates a login command which carries the info and tells the server what to do with it. 
            ServerCommand loginCmd = new ServerCommand();
            loginCmd.Command = "login";
            loginCmd.Parameters = UserInfo;

            // Sends command to server.
            SendToServer(loginCmd);           
        }

        // Listens for messages from the server. 
        private static void Listen()
        {
            do
            {
                try
                {
                    // Creates bytes array to store incoming message.
                    byte[] msgBytes = new byte[1024];

                    // Creates Stringbuilder to store converted byte array.
                    StringBuilder msg = new StringBuilder();

                    // Keeps track of number of bytes in byte array.
                    int numBytes = 0;

                    do
                    {
                        // Sets number of bytes.
                        numBytes = stream.Read(msgBytes, 0, msgBytes.Length);

                        // Converts byte array into String. 
                        msg.AppendFormat($"{Encoding.ASCII.GetString(msgBytes, 0, numBytes)}");
                    }
                    while (stream.DataAvailable);

                    // Decrypts json string.
                    string json = Encrypt.DecryptString(msg.ToString(), "ppb");

                    // Deserialize json into ServerCommand object. 
                    ServerCommand cmd = JsonConvert.DeserializeObject<ServerCommand>(json);

                    // If message is not empty.
                    if (cmd.Command == "login_success")
                    {
                        LoggedIn = true;
                        OnLoginSuccess();
                    }

                    else if (cmd.Command == "login_failed")
                    {
                        string errorMsg = cmd.Parameters["login_failed"].ToString();

                        LoggedIn = false;

                        OnLoginFailed(errorMsg);
                    }
                }
                catch (Exception)
                {
                    // Waits for entered milliseconds. 
                    //new ManualResetEvent(false).WaitOne(2000);
                }
            }
            while (Connected);
        }

        // Tests server connection periodically.
        private static void TestConn()
        {
            while (true)
            {
                if (SendToServer(null))
                {
                    // Triggers server Connected event.
                    OnServerConnected();

                    Connected = true;
                }

                else
                {
                    // Triggers server disConnected event.
                    OnServerDisconnected();

                    Connected = false;

                    // Attempts to reconnect to server again. 
                    Connect();

                    // Breaks out of loop as TestConn will be called again after reconnection.
                    break;
                }

                // Waits for entered milliseconds. 
                new ManualResetEvent(false).WaitOne(4000);
            }
        }

        private static void Disconnect()
        {
            client.Close();
            Connected = false;
            stream.Close();
            securePassword = null;

            SendToServer(null);
        }
    }
}
