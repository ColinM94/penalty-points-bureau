using Newtonsoft.Json;
using PPB_Server.Helpers;
using PPB_Server.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Dynamic;
using System.Collections.Generic;
using PPB_Server.Database;
using System.IO;

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
        Dictionary<string, int> loginAttempts = new Dictionary<string, int>();
        
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

            // Instantiates and starts Listener server.
            server = new TcpListener(ip, port);
            server.Start();

            Console.WriteLine("Server Started on Port " + port + " with IP " + ip);
            Console.WriteLine("Awaiting Connections....");

            // Hands off 
            //Thread listenThread = new Thread(()=>Listen());
            //listenThread.Start();
            Listen();
           
        }

        // Waits for a new client to connect and hands any new clients off to separate threads. 
        private void Listen()
        {
            while (running == true)
            {
                // Waits for a new client. 
                TcpClient newClient = server.AcceptTcpClient();

                // New client is handed off to a HandleClient thread. 
                Thread clientThread = new Thread(()=>HandleClient(newClient));
                clientThread.Start();
            }
        }

        // Deals with a single client.
        private void HandleClient(object newClient)
        {
            TcpClient client = (TcpClient)newClient;
            NetworkStream stream = client.GetStream();

            bool loggedIn = false;
            string username = "Client";
          
            String clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            Console.WriteLine($"Client Connected on port " + clientPort);

            while (running == true)
            {
                try
                {
                    // Test client connection.
                    //MsgClient("&test&", client, stream);
                    //new ManualResetEvent(false).WaitOne(1000);

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

                    if(cmd.Command == "login")
                    {
                        // Creates User object.
                        User user = new User();

                        // Converts parameters dictionary back to a User object.
                        user = DictionaryObjectConverter.ToObject<User>(cmd.Parameters);

                        // Creates instance of LoginUser class. 
                        LoginUser login = new LoginUser();

                        // Creates a command to tell the client login was successful. 
                        ServerCommand clientCmd = new ServerCommand();

                        // Creates dictionary to store parameters. 
                        var dictionary = new Dictionary<string, object>();

                        // Checks if loginAttempts dictionary doesn't contain entered username. 
                        if(!loginAttempts.ContainsKey(user.Username))
                        {
                            // Adds username to dictionary so login attemps can be tracked. 
                            loginAttempts.Add(user.Username, 0);
                        }

                        // Checks if login was successful and user has doesn't have too many login attempts. 
                        if(login.Login(user) && loginAttempts[user.Username] < 3 )
                        {
                            // If an entry exists it is deleted. 
                            loginAttempts.Remove(user.Username);

                            // Sets logged in username. 
                            username = user.Username;

                            clientCmd.Command = "login_success";

                            Console.WriteLine($"{username}: Login Successful");

                            SendToClient(clientCmd, client, stream);

                            loggedIn = true;

                        }  
                        
                        else
                        {
                            Console.WriteLine($"{user.Username}: Login failed");

                            // Increases number of failed login attempts. 
                            loginAttempts[user.Username]++;

                            // Adds number of login attempts to parameters so the user can be warned. 
                            dictionary.Add("login_attempts", loginAttempts[user.Username]);

                            // Command 
                            clientCmd.Command = "login_failed";
                            clientCmd.Parameters = dictionary;

                            SendToClient(clientCmd, client, stream);

                            loggedIn = false;

                        }
                    }
                }  
                catch(Exception ex)
                {
                    //Console.WriteLine(ex);
                }
            }                                        
        }

        private bool SendToClient(ServerCommand command, TcpClient client, NetworkStream stream)
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