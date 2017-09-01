using System;
using System.Net.Sockets;
using System.Security;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using PPB_Client.Helpers;
using PPB_Client.Models;

namespace PPB_Client.Connection
{
    /// <summary>
    /// Contains functionality allowing the client to communicate with the PPB Server. 
    /// </summary>
    public static class Server 
    {
        // Events
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

        // Properties
        static String server = "127.0.0.1";
        static int port = 2000;
        static TcpClient client;     
        static NetworkStream stream;
        static SecureString securePassword;
        static string userID;
        static bool connected;
        public static bool LoggedIn;

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

                    // Listen for messages from server on a separate thread.
                    Thread listenThread = new Thread(() => Listen());
                    listenThread.Start();

                    // Test connection with server. 
                    TestConn();

                }
                catch (Exception)
                {
                    // Waits for entered milliseconds. 
                    new ManualResetEvent(false).WaitOne(2000);
                }
            }
            while (!connected);
        }
     
        // Send message to server.
        public static bool SendMsg(string msg)
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
            SendMsg($"&LoginUsername&=${username}$");           
        }

        /// <summary>
        /// Attempts to log user into PPB account. 
        /// </summary>
        private static void LoginUser()
        {         
            User user = new User();
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

            SendMsg(encryptedJson);
        }

        // Listens for messages from the server. 
        private static void Listen()
        {
            while (connected)
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

                        else if(message.Contains("&Error&"))
                        {

                        }

                        else
                        {
                            // Decrypts json string.
                            string json = Encrypt.DecryptString(msg.ToString(), "ppb");
                        }

                        Console.WriteLine($"From Server: {message}");
                    }
                }
                catch (Exception)
                {
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
                if (SendMsg(""))
                {
                    // Triggers server connected event.
                    OnServerConnected();

                    connected = true;
                }

                else
                {
                    // Triggers server disconnected event.
                    OnServerDisconnected();

                    connected = false;

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
            connected = false;
            stream.Close();
            securePassword = null;

            SendMsg("test");
        }
    }
}
