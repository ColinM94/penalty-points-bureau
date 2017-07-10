using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace PPB_Client
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
        }

        //Removes username placeholder text if field is clicked
        private void loginUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            //Gets colour of username text
            String colour = txtBox.Foreground.ToString();
            String black = Brushes.Black.ToString();

            if (txtBox.Text == "Username" && colour != black)
            {
                txtBox.Foreground = Brushes.Black;
                txtBox.Text = "";
            }
        }

        //Resets username placeholder text if username field is empty
        private void loginUsername_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (txtBox.Text == "")
            {
                txtBox.Foreground = Brushes.Gray;
                txtBox.Text = "Username";
            }
        }

        //Removes password placeholder text if field is clicked
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;

            //Gets colour of password text
            String colour = passBox.Foreground.ToString();
            String black = Brushes.Black.ToString();

            //Resets Placeholder text if password field is empty
            if (colour != black)
            {
                passBox.Foreground = Brushes.Black;
                passBox.Password = "";
            }
        }

        //Resets password placeholder text if username field is empty
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;

            if (passBox.Password == "")
            {
                passBox.Foreground = Brushes.Gray;
                passBox.Password = "**********";
            }
        }

        private void loginPassword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //TEMP CODE
                loginUsername.Text = "admin";
                loginPassword.Password = "password";
                //END TEMP CODE


                MySqlConnection conn;
                MySqlCommand cmd;
                MySqlDataReader reader = null;

                string userID = null;
                string username = loginUsername.Text;
                string password = loginPassword.Password;
                string hashedPass = null;

                //Creates and opens MySQLConnection
                conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
                conn.Open();

                //Prepared statement to get user id for entered username
                cmd = new MySqlCommand("SELECT user_id FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", loginUsername.Text);
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

                    //Logs in user and opens main window
                    MainWindow main = new MainWindow();
                    main.Show();
                    this.Close();
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
