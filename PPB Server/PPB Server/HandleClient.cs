using Newtonsoft.Json;
using PPB_Server.Database;
using PPB_Server.Helpers;
using PPB_Server.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PPB_Server
{
    public class HandleClient
    {
        TcpClient client;
        NetworkStream stream;
        MsgClient msgClient;
        bool clientConnected = true;
        bool loggedIn = false;
        string username = "";

        // Deals with a single client.
        public void Handle(TcpClient newClient)
        {
            client = newClient;
            stream = client.GetStream();
            msgClient = new MsgClient();

            String clientPort = ((IPEndPoint)client.Client.RemoteEndPoint).Port.ToString();
            Console.WriteLine($"Client Connected on port " + clientPort);

            // Repeatedly checks client connection.
            Thread testConnThread = new Thread(TestConn);
            testConnThread.Start();

            while (Server.Running && clientConnected)
            {
                try
                {
                    // Gets encrypted message from client stream. 
                    string msg = GetMessage();

                    // Decrypts json string.
                    string json = Encrypt.DecryptString(msg.ToString(), "ppb");

                    if(json != "null")
                    {

                        if (Server.Debug)
                        {
                            if (json != "null")
                            {
                                // Shows encrypted message.
                                Console.WriteLine($"{clientPort}: {msg.ToString()}");
                                // Shows decrypted message.
                                Console.WriteLine($"{clientPort}: {json}");
                            }
                        }

                        // Deserialize json into ServerCommand object. 
                        ServerCommand cmd = JsonConvert.DeserializeObject<ServerCommand>(json);

                        // If client wants to log in.
                        if (cmd.Command == "login" && loggedIn == false)
                        {
                            // Creates User object.
                            User user = new User();

                            // Converts parameters back to a User object.
                            user = DictionaryObjectConverter.ToObject<User>(cmd.Parameters);

                            // Sets current username to username sent from client. 
                            username = user.Username;

                            if (Server.Debug)
                            {
                                Console.WriteLine($"{clientPort}: Attempting to login user \"{username}\".");
                            }

                            // Creates instance of LoginUser class. 
                            LoginUser login = new LoginUser();

                            // Creates a command to tell the client if login was successful or not. 
                            ServerCommand clientCmd = new ServerCommand();

                            // Creates dictionary to store parameters. 
                            var dictionary = new Dictionary<string, object>();

                            // Checks if loginAttempts dictionary doesn't contain entered username. 
                            if (!Server.LoginAttempts.ContainsKey(username))
                            {
                                // Adds username to dictionary so login attemps can be tracked. 
                                Server.LoginAttempts.Add(username, 0);
                            }

                            // Checks if login was successful and user has doesn't have too many login attempts. 
                            if (login.Login(user) && Server.LoginAttempts[username] < 3)
                            {
                                if (Server.Debug)
                                {
                                    Console.WriteLine($"{username}: Login Successful");
                                }

                                // If an entry exists it is deleted. 
                                Server.LoginAttempts.Remove(username);

                                clientCmd.Command = "login_success";

                                msgClient.Send(clientCmd, client, stream);

                                loggedIn = true;
                            }

                            else
                            {
                                if (Server.Debug)
                                {
                                    Console.WriteLine($"{username}: Login failed");
                                }

                                // Increases number of failed login attempts. 
                                Server.LoginAttempts[username]++;

                                string errorMsg = "Login failed!";

                                if (Server.LoginAttempts[username] == 1)
                                {
                                    errorMsg = "Login failed. 2 login attempts remaining.";
                                }

                                else if (Server.LoginAttempts[username] == 2)
                                {
                                    errorMsg = "Login failed. 1 login attempts remaining.";
                                }

                                else if (Server.LoginAttempts[username] > 2)
                                {
                                    errorMsg = "Account Locked! Please contact an admin.";

                                    Logger logger = new Logger();
                                    logger.AccountLocked(username);
                                    
                                }

                                // Adds error message to parameters so it can be displayed to the user. 
                                dictionary.Add("login_failed", errorMsg);

                                // Command 
                                clientCmd.Command = "login_failed";
                                clientCmd.Parameters = dictionary;

                                msgClient.Send(clientCmd, client, stream);

                                loggedIn = false;
                            }                  
                        }

                        // Placeholder for other command methods.
                        else if (cmd.Command == "idle_logout")
                        {
                            Logger logger = new Logger();
                            logger.Idle(username);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex);
                    Console.WriteLine($"{clientPort}: Client disconnected");
                    clientConnected = false;
                }
            }
        }

        private string GetMessage()
        {
            // Creates bytes array to store bytes of incoming message.
            byte[] msgBytes = new byte[1024];

            // Creates a string to store message from client. 
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

            new ManualResetEvent(false).WaitOne(1000);

            return msg.ToString();
        }

        private void TestConn()
        {
            while(clientConnected)
            {
                // Tracks number of connection attempts.       
                int tries = 0;

                // Test client connection.
                MsgClient msgClient = new MsgClient();

                if (!msgClient.Send(null, client, stream))
                {
                    tries++;
                }

                else
                {
                    tries = 0;
                }

                if (tries > 2)
                {
                    Console.WriteLine("Client Disconnected");
                    clientConnected = false;
                }
                new ManualResetEvent(false).WaitOne(3000);
            }           
        }
    }
}
