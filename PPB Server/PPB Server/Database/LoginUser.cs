using MySql.Data.MySqlClient;
using PPB_Server.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using PPB_Server;

namespace PPB_Server.Database
{
    public class LoginUser
    {       
        public bool Login(User user)
        {
            try
            {
                MySqlConnection conn;
                MySqlCommand cmd;
                MySqlDataReader reader = null;

                string userID = null;
                string username = user.Username;
                string password = user.Password;
                string hashedPass = null;

                // Creates and opens MySQLConnection.
                conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString);
                conn.Open();

                // Prepared statement to get user id for entered username.
                cmd = new MySqlCommand("SELECT userid FROM users WHERE username = @username", conn);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Prepare();

                // Executes and read result of query.
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    userID = reader.GetInt16(0).ToString();
                }
                reader.Close();

                // Changes userid + password string into bytes.
                byte[] bytes = Encoding.UTF8.GetBytes(userID + password);

                // Hashes byte array
                SHA256Managed hashString = new SHA256Managed();
                byte[] hash = hashString.ComputeHash(bytes);

                // Converts hashed byte array into a string.
                foreach (byte x in hash)
                {
                    hashedPass += String.Format("{0:x2}", x);
                }

                Console.WriteLine(hashedPass);

                if (Server.Debug)
                {
                    Console.WriteLine($"Searching DB for Username: \"{username}\" and Password: \"{hashedPass}\"");
                }

                // Checks if a user exists with matchin user id and hashed password.
                cmd = new MySqlCommand("SELECT * FROM users WHERE userid = @userID AND password = @password", conn);
                cmd.Parameters.AddWithValue("@userID", userID);
                cmd.Parameters.AddWithValue("@password", hashedPass);
                cmd.Prepare();

                // Executes query and reads result.
                reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    reader.Close();
                    conn.Close();

                    return true;
                }

                else
                {
                    reader.Close();
                    conn.Close();

                    return false;
                }
            } 
            catch(Exception ex)
            {
                if(Server.Debug)
                {
                    Console.WriteLine($"DB Error: {ex}");
                }
                
                return false;
            }
        }
    }
}
