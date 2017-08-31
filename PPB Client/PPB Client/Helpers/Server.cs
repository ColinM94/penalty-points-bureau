using Newtonsoft.Json;
using PPB_Client.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PPB_Client.Helpers
{
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

        public static void OnLoginFailed()
        {
            LoginFailure?.Invoke(null, EventArgs.Empty);
        }

        #endregion       

        static String server = "127.0.0.1";
        static int port = 2000;
        static TcpClient client;     
        static NetworkStream stream;
        static SecureString securePassword;
        static string userID;

        public static bool Connected { get; set; } = false;
        public static bool LoggedIn { get; set; } = false;

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

                    // Listen for messages from server on a separate thread.
                    Thread listenThread = new Thread(() => Listen());
                    listenThread.Start();

                    // Test connection with server. 
                    TestConn();

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connect Error!");

                    // Waits for entered milliseconds. 
                    new ManualResetEvent(false).WaitOne(2000);
                }
            }
            while (!Connected);
        }
     
        // Send message to server.
        public static bool MsgServer(string msg)
        {
            // Encodes string into a sequence of bytes.
            Byte[] data = Encoding.ASCII.GetBytes(msg);

            try
            {               
                // Sends message to server .
                stream.Write(data, 0, data.Length);

                // Return true if message sent successfully.
                return true;
            }
            catch
            {
                // Return false if message failed to send.
                return false;
            }
        }

        public static void Login(string username, SecureString securePass)
        {
            securePassword = securePass;

            // Sends username to server so corresponding userID can be returned.
            MsgServer($"&LoginUsername&=${username}$");           
        }

        /// <summary>
        /// Attempts to log user into PPB account. 
        /// </summary>
        private static void LoginUser()
        {         
            UserModel user = new UserModel();
            user.Method = "Login";
            user.UserID = userID;
            user.Password = SecureStringToString.Convert(securePassword);

            SendJson(user);
        }

        // Sends json object containing properties and the method to be called on the server.
        private static void SendJson(object ModelObject)
        {
            string json = JsonConvert.SerializeObject(ModelObject);

            string encryptedJson = Encrypt.EncryptString(json, "ppb");

            MsgServer(encryptedJson);
        }

        // Listens for messages from the server. 
        private static void Listen()
        {
            Console.WriteLine("Listening");

            while (Connected)
            {
                try
                {            
                    // Creates bytes array to store incoming message.
                    byte[] msgBytes = new byte[1024];

                    // Creates Stringbuilder object to store converted byte array.
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

                    // Converts msg to standard string.
                    string message = msg.ToString();

                    // If message is not empty.
                    if (message != null)
                    {
                        // If server is returning userID for a username.
                        if (message.Contains("&LoginUserID&"))
                        {
                            string input = msg.ToString();
                            userID = input.Split('$', '$')[1];

                            LoginUser();

                        }

                        // If server says login was successful.
                        else if (message.Contains("&LoginSuccess&"))
                        {
                            LoggedIn = true;
                            OnLoginSuccess();
                        }

                        // If server says login failed. 
                        else if (message.Contains("&LoginFailed&"))
                        {
                            LoggedIn = false;
                            OnLoginFailed();
                        }

                        // If server is testing connection.
                        else if (message.Contains("&Test&"))
                        {

                        }

                        else if (message.Length > 50)
                        {
                            // Decrypts json string.
                            string json = Encrypt.DecryptString(msg.ToString(), "ppb");
                        }

                        Console.WriteLine($"From Server: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Listen Error!");
                    Console.WriteLine(ex);
                    // Waits for entered milliseconds. 
                    new ManualResetEvent(false).WaitOne(2000);
                }
            }                 
        }

        // Tests server connection periodically.
        private static void TestConn()
        {
            while(true)
            {
                if (MsgServer(""))
                {
                    // Triggers server connected event.
                    OnServerConnected();

                    Connected = true;
                }

                else
                {
                    // Triggers server disconnected event.
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

            MsgServer("test");
        }
    }
}
