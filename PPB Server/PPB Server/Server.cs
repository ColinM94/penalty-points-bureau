using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PPB_Server.Database;
using PPB_Server.Helpers;

namespace PPB_Server
{
    public static class Server
    {
        static void Main(string[] args)
        {
            Menu();
        }

        /// <summary>
        /// Server IP address.
        /// </summary>
        public static IPAddress IP = IPAddress.Parse("127.0.0.1");

        /// <summary>
        /// Server Port.
        /// </summary>
        public static int Port = 2000;

        /// <summary>
        /// Listens for connections from TCP network clients.
        /// </summary>
        public static TcpListener Listener;

        /// <summary>
        /// Keeps track of whether server is running or not. 
        /// </summary>
        public static bool Running = false;

        /// <summary>
        /// Stores number of login attempts for user.
        /// </summary>
        public static Dictionary<string, int> LoginAttempts = new Dictionary<string, int>();

        /// <summary>
        /// If set to true then debug information will be displayed in the console. 
        /// </summary>
        public static bool Debug = false;

        /// <summary>
        /// Opens server command menu. 
        /// </summary>
        public static void Menu()
        {
            SwitchDebug();
            StartServer();

            Console.WriteLine("Penalty Points Bureau Server");
            Console.WriteLine("Type \"HELP\" for list of commands\n");

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
                    case "DEBUG":
                        SwitchDebug();
                        break;
                    case "HELP":
                        Help();
                        break;
                    case "EXIT":
                        Exit();
                        break;
                    default:
                        Console.WriteLine("Unknown Command");
                        break;
                }
            }
        }

        // Starts PPB server.
        private static void StartServer()
        {            
            // Listens for clients on a separate thread.  
            Thread listenThread = new Thread(Listen);
            listenThread.Start();
        }

        // Deals with new client connections.
        private static void Listen()
        {
            Running = true;

            // Starts Listener server.
            Listener = new TcpListener(IP, Port);
            Listener.Start();

            Console.WriteLine("Server Started on Port " + Port + " with IP " + IP);
            Console.WriteLine("Awaiting Connections....");

            while (Running == true)
            {
                // Waits for a new client. 
                TcpClient newClient = Listener.AcceptTcpClient();

                // Creates instance of HandleClient class. 
                var handleClient = new HandleClient();

                // New client is handed off to a HandleClient thread. 
                Thread clientThread = new Thread(() => handleClient.Handle(newClient));
                clientThread.Start();
            }
        }
      
        // Stops PPB server.
        private static void StopServer()
        {
            Listener.Stop();

            Running = false;

            Console.WriteLine("Server Stopped");
        }

        // Switches debug messages on and off. 
        private static void SwitchDebug()
        {
            if (Debug)
            {
                Debug = false;
                Console.WriteLine("Debugging Disabled");
            }
            else
            {
                Debug = true;
                Console.WriteLine("Debugging Enabled");
            }
        }

        // Lists server commands for user.
        private static void Help()
        {
            Console.WriteLine("Commands:");
            Console.WriteLine("STARTSERVER - Starts PPB server.");
            Console.WriteLine("STOPSERVER - Stops PPB server");
            Console.WriteLine("DEBUG - Enables debug messages.");
            Console.WriteLine("EXIT - Closes application.");
        }

        // Closes application. 
        private static void Exit()
        {
            Environment.Exit(0);
        }
    }
}
