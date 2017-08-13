using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace PPB_Server
{
    class Server
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Penalty Points Bureau Server");
            Console.WriteLine("Type \"HELP\" for list of commands\n");

            ServerConsole server = new ServerConsole();
            server.menu();
        }
    }

    class ServerConsole
    {
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

        //Listens for incoming connections
        private void StartServer()
        {
            running = true;

            //Instiates and starts Listener server
            server = new TcpListener(ip, port);
            server.Start();

            Console.WriteLine("Server Started on Port " + port + " with IP " + ip);
            Console.WriteLine("Awaiting Connections....");

            //Thread waits for a client to connect
            Thread listenThread = new Thread(delegate ()
            {       
                while (running == true)
                {
                    TcpClient newClient = server.AcceptTcpClient();

                    //Connected client is handed off to an instance of clientThread to free up listenThread
                    Thread clientThread = new Thread(delegate ()
                    {               
                        HandleClient(newClient);
                        newClient.Close();
                    });
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();  
        }

        //Deals with a single client
        private void HandleClient(object newClient)
        {
            TcpClient client = (TcpClient)newClient;
            NetworkStream stream = client.GetStream();

            String clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            Console.WriteLine("Client Connected on port " + clientPort);

            try
            {
                while(running == true)
                {
                    MsgClient("testconn", client, stream);

                    byte[] msgBytes = new byte[1024];
                    StringBuilder msg = new StringBuilder();
                    int numBytes = 0;

                    do
                    {
                        numBytes = stream.Read(msgBytes, 0, msgBytes.Length);
                        msg.AppendFormat("{0}", Encoding.ASCII.GetString(msgBytes, 0, numBytes));
                    }
                    while (stream.DataAvailable);

                    Console.WriteLine(msg + " " + clientPort);                 
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
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("Ping");

            stream.Write(data, 0, data.Length);    
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