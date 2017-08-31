using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PPB_Server.Helpers;
using PPB_Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Security.Cryptography;
using System.Dynamic;

namespace PPB_Server
{
    public class Server
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Penalty Points Bureau Server");
            Console.WriteLine("Type \"HELP\" for list of commands\n");

            ServerConsole server = new ServerConsole();
        }
    }

    public class ServerConsole
    {
        public ServerConsole()
        {
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

                //newClient.Close();
            }
        }

        // Deals with a single client.
        private void HandleClient(object newClient)
        {
            TcpClient client = (TcpClient)newClient;
            NetworkStream stream = client.GetStream();
            string username;

            bool userLoggedIn = false;

            String clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            Console.WriteLine("Client Connected on port " + clientPort);

            try
            {
                while (running == true)
                {                    
                    // Test client connection.
                    MsgClient("test", client, stream);

                    byte[] msgBytes = new byte[1024];
                    StringBuilder msg = new StringBuilder();
                    int numBytes = 0;

                    do
                    {
                        numBytes = stream.Read(msgBytes, 0, msgBytes.Length);
                        msg.AppendFormat($"{Encoding.ASCII.GetString(msgBytes, 0, numBytes)}");
                    }
                    while (stream.DataAvailable);

                    if (msg.ToString().Contains("{"))
                    {
                        string json = Encrypt.EncryptString(msg.ToString(), "ppb");

                        // If client is attempting to log in.
                        if (json.Contains("\"Method\":\"Login\""))
                        {
                            UserModel user = JsonConvert.DeserializeObject<UserModel>(json);
                            LoginUser loginUser = new LoginUser();
                            if(loginUser.Login(user))
                            {
                                userLoggedIn = true;

                                dynamic login = new ExpandoObject();
                                login.Method = "login";

                            }
                        }
                    }

                    // If client sends username then the corresponding userid needs to be returned. 
                    else if(msg.ToString().Contains("LoginUsername"))
                    {
                        string input = msg.ToString();
                        string output = input.Split('$', '$')[1];

                        // TODO : Get userid
                        // TODO : Send userid to client
                    }

                    else
                    {
                        Console.WriteLine(msg);
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
            Byte[] data = Encoding.ASCII.GetBytes("testconn");

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