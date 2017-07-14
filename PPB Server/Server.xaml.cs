using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace PPB_Server
{
    //Allows the TcpListener Active property to be accessed
    public class TcpListenerEx : TcpListener
    {
        public TcpListenerEx(IPEndPoint localEP) : base(localEP)
        {
        }
        public TcpListenerEx(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        public new bool Active
        {
            get { return base.Active; }
        }
    }

    public partial class Server : Window 
    {
        TcpListenerEx server;
        IPAddress ip = IPAddress.Parse("127.0.0.1");  
        int port = 2000;
        bool running = false;

        public Server()
        {
            InitializeComponent();
            //UpdateServerStatus();
        }

        private void UpdateServerStatus()
        {
            this.ServerIPLabel.Content = "IP Address: " + ip;
            this.ServerPortLabel.Content = "Port: " + port;

            //Checks if TcpListener is listening
            if (server.Active == true)
            {
                running = true;
            }

            else
            {
                running = false;
            }

            if (running == true)
            {
                this.StartServerBtn.IsEnabled = false;
                this.StopServerBtn.IsEnabled = true;

                this.ServerStatusLabel.Content = "Server Online";
                this.ServerStatusLabel.Foreground = new SolidColorBrush(Colors.Green);          
            }     

            else if(running == false)
            {
                this.StartServerBtn.IsEnabled = true;
                this.StopServerBtn.IsEnabled = false;

                this.ServerStatusLabel.Content = "Server Offline";
                this.ServerStatusLabel.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                server = new TcpListenerEx(ip, port);
                server.Start();
 
                UpdateServerStatus();                   
            }
            catch(SocketException)
            {
                MessageBox.Show("TCP Server Error");
            }     
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            server.Stop();
            UpdateServerStatus();
        }

        private void TestDBConn_Click(object sender, RoutedEventArgs e)
        { 
            Login("admin", "password");
            UpdateServerStatus();

        }

        void Listen()  // Listen to incoming connections.
        {
            while (running)
            {
                TcpClient tcpClient = server.AcceptTcpClient();
            }
        }

        void Login(string username, string password)
        {
            try
            {
                MySqlConnection conn;
                MySqlCommand cmd;
                MySqlDataReader reader = null;

                string userID = null;
                string hashedPass = null;

                //Creates and opens MySQLConnection
                conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
                conn.Open();

                //Prepared statement to get user id for entered username
                cmd = new MySqlCommand("SELECT user_id FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Prepare();

                //Executes and read result of query
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userID = reader.GetInt16(0).ToString();
                }
                reader.Close();

                //changes userid + password string into bytes
                byte[] bytes = Encoding.UTF8.GetBytes(userID + password);

                //Hashes byte array
                SHA256Managed hashString = new SHA256Managed();
                byte[] hash = hashString.ComputeHash(bytes);

                //Converts hashed byte array into a string
                foreach (byte x in hash)
                {
                    hashedPass += String.Format("{0:x2}", x);
                }

                //Checks if a user exists with matchin user id and hashed password
                cmd = new MySqlCommand("SELECT * FROM users WHERE user_id = @userID AND password = @password", conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@password", hashedPass);
                cmd.Prepare();

                //Executes query and reads result
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    reader.Close();
                }

                else
                {
                    reader.Close();
                    MessageBox.Show("Incorrect Username or Password");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection Failed");

                //Uncomment for mysql error messages
            
                if (ex is MySqlException)
                {
                    MessageBox.Show(ex.Message);
                }
               
            }
        }
    }
}
