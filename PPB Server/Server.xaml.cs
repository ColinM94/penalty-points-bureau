using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;

namespace PPB_Server
{
    public partial class Server : Window
    {
        public IPAddress ip = IPAddress.Parse("127.0.0.1");
        public int port = 2000;
        public TcpListener server;

        public Server()
        {
            InitializeComponent();
        }

        private void StartServer_Click(object sender, RoutedEventArgs e)
        {
            //server.Start();
            MessageBox.Show("Server Started");
        }

        private void StopServer_Click(object sender, RoutedEventArgs e)
        {
            //server.Stop();
            MessageBox.Show("Server Stopped");
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
                /* 
                if (ex is MySqlException)
                {
                    MessageBox.Show(ex.Message);
                }
                */
            }
        }
    }
}
