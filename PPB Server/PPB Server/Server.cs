using Newtonsoft.Json;
using PPB_Server.Helpers;
using PPB_Server.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Dynamic;

namespace PPB_Server
{
    public class Server
    {
        static void Main(string[] args)
        {            
            ServerConsole server = new ServerConsole();
        }
    }

    public class ServerConsole
    {       
        public ServerConsole()
        {
            Console.WriteLine("Penalty Points Bureau Server");
            Console.WriteLine("Type \"HELP\" for list of commands\n");
            menu();
        }

        IPAddress ip = IPAddress.Parse("127.0.0.1");
        int port = 2000;
        TcpListener server;
        static bool running = false;

        public void menu()
        {
            StartServer();

            bool exit = false;

            while (exit == false)
            {
                String option = Console.ReadLine().ToUpper();

                switch (option)
                {
                    case "STARTSERVER":
                        StartServer();
                        break;
                    case "STOPSERVER":
                        StopServer();
                        break;
                    case "CLIENTLIST":
                        ListClients();
                        break;
                    case "EXIT":
                        Exit();
                        break;
                    case "HELP":
                        Console.WriteLine("Commands:");
                        Console.WriteLine("STARTSERVER");
                        Console.WriteLine("STOPSERVER");
                        Console.WriteLine("EXIT");

                        break;
                    default:
                        Console.WriteLine("Uknown Command");
                        break;
                }
            }
        }

        // Listens for incoming connections.
        private void StartServer()
        {
            running = true;

            // Instiates and starts Listener server.
            server = new TcpListener(ip, port);
            server.Start();

            Console.WriteLine("Server Started on Port " + port + " with IP " + ip);
            Console.WriteLine("Awaiting Connections....");

            // Thread waits for a client to connect.
            Thread listenThread = new Thread(()=>Listen());
            listenThread.Start();
           
        }

        private void Listen()
        {
            while (running == true)
            {
                TcpClient newClient = server.AcceptTcpClient();

                // Connected client is handed off to an instance of clientThread to free up listenThread.
                Thread clientThread = new Thread(()=>HandleClient(newClient));
                clientThread.Start();

            }
        }

        // Deals with a single client.
        private void HandleClient(object newClient)
        {
            TcpClient client = (TcpClient)newClient;
            NetworkStream stream = client.GetStream();

            bool userLoggedIn = false;
            string userID = null;
          
            String clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            Console.WriteLine("Client Connected on port " + clientPort);

            try
            {
                while (running == true)
                {                    
                    // Test client connection.
                    MsgClient("&test&", client, stream);
                    new ManualResetEvent(false).WaitOne(1000);

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

                    string message = msg.ToString();

                    if (message == null)
                    {

                    }

                    // If client sends username then the corresponding userid needs to be returned. 
                    else if (message.Contains("&LoginUsername&"))
                    {
                        Console.WriteLine($"From Client: {msg.ToString()}");
                        string input = msg.ToString();
                        string username = input.Split('$', '$')[1];

                        LoginUser login = new LoginUser();
                        userID = login.GetUserID(username);

                        MsgClient($"&LoginUserID&=${userID}$", client, stream);
                    }

                    else if(message.Contains("&test&"))
                    {
                        Console.WriteLine($"From Client: {msg.ToString()}");
                    }
        
                    // If message is a json string.
                    else
                    {
                        // Decrypts json string.
                        string json = Encrypt.DecryptString(msg.ToString(), "ppb");
                        Console.WriteLine($"From Client: {json}");
                        // If client is attempting to log in.
                        if (json.Contains("\"Method\":\"Login\""))
                        {
                            Console.WriteLine("Attempting login");
                            // Converts json into UserModel object.
                            User user = JsonConvert.DeserializeObject<User>(json);
                            Console.WriteLine(user.UserID + user.Password);
                            Console.WriteLine(json);

                            // Creates instance of LoginUser class.
                            LoginUser loginUser = new LoginUser();

                            // If userid and password exists in db.
                            if(loginUser.Login(user))
                            {
                                userLoggedIn = true;

                                //string encryptedJson = Encrypt.EncryptString(loginJson, "ppb");

                                MsgClient("&LoginSuccess&", client, stream);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client Disconnected");
            }

            client.Close();
        }

        private void MsgClient(string msg, TcpClient client, NetworkStream stream)
        {
            Byte[] data = Encoding.ASCII.GetBytes(msg);

            stream.Write(data, 0, data.Length);
        }

        private void ListClients()
        {

        }

        private void StopServer()
        {
            server.Stop();
            running = false;

            Console.WriteLine("Server Stopped");
        }

        private void Exit()
        {
            Environment.Exit(0);
        }
    }
}