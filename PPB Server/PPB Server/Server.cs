using System;
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
        volatile bool running = false;

        public void menu()
        {
            //StartServer();

            bool exit = false;

            while (exit == false)
            {
                Console.Write("Server CMD: ");
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

        //Listen to incoming connections.
        private void StartServer()
        {
            running = true;

            TcpListener server = new TcpListener(ip, port);

            server.Start();

            Console.WriteLine("Server Started on Port " + port + " with IP " + ip);
            Console.WriteLine("Awaiting Connections....");

            Thread listenThread = new Thread(delegate ()
            {               
                while (running == true)
                {
                    TcpClient newClient = server.AcceptTcpClient();

                    Thread clientThread = new Thread(delegate ()
                    {               
                        HandleClient(newClient);
                    });
                    clientThread.IsBackground = true;
                    clientThread.Start();
                }
                server.Stop();
            });
            listenThread.IsBackground = true;
            listenThread.Start();        
        }

        private void HandleClient(object newClient)
        {
            TcpClient client = (TcpClient)newClient;
            NetworkStream stream = client.GetStream();

            String clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            Console.WriteLine(clientPort + ": Client Connected");
                    
            try
            {
                while (PingClient(client, stream))
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

                        Console.WriteLine(clientPort + ": " + Msg.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(clientPort + ": Client Disconnected"); 
            }

            stream.Close();
            client.Close();
        }

        private bool PingClient(TcpClient client, NetworkStream stream)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes("Ping");

            stream.Write(data, 0, data.Length);

            if (stream != null)
            {
                int i;
                Byte[] bytes = new Byte[256];
                String response = null;

                // Loop to receive all the data sent by the client.
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    // Translate data bytes to a ASCII string.
                    response = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Console.WriteLine(response);

                    if (response.ToString().Equals("Pong"))
                    {
                        Console.WriteLine("Pong");
                        return true;
                    }
                }
            }

            return false;
        }

        private void StopServer()
        {
            running = false;

            Console.WriteLine("Server Stopped");
        }

        private void Exit()
        {
            Environment.Exit(0);
        }
    }
}