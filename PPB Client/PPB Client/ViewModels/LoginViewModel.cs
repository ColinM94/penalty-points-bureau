using System.Windows;
using PPB_Client.Helpers;
using System.Windows.Input;
using PPB_Client.Models;

namespace PPB_Client.ViewModels
{
    // Logic for Login
    public class LoginViewModel : BaseViewModel
    {
        // Constructor. Sets up default property values and commands.  
        public LoginViewModel()
        {
            loginAttempts = 0;
            CurrentUsername = "Username";

            LoginErrorMsg = "Error Laaaad";

            // Sets up login command.
            LoginCommand = new RelayCommand(Login);
        }

        // Current username submitted by user
        public string CurrentUsername { get; set; }

        // Current password submitted by user
        public string CurrentPassword { get; set; }
        
        // Error message to be displayed to user on entry of wrong username/password combo
        public string LoginErrorMsg { get; set; }

        // Keeps track of number of login attempts by user.
        private int loginAttempts;

        // Login command which will be triggered from the UI. 
        public ICommand LoginCommand { get; private set; }
                      
        // Attempts to log in current user. 
        private void Login(object parameter)
        {
            var passwordContainer = parameter as IPassword;

            if (passwordContainer != null)
            {
                var secureString = passwordContainer.Password;
                CurrentPassword = SecureStringToString.Convert(secureString);
            }

            // Creates user object with entered username and password. 
            User user = new User();
            user.Username = CurrentUsername;
            user.Password = CurrentPassword;

            //server.Connect();
            server.PingServer();


            // Starts connection to server
            //Server.Connect();


            /*

            if(CurrentUsername == "username" && CurrentPassword == "password")
                MessageBox.Show("Logged in successfully");

            loginAttempts++;

            if (loginAttempts > 2)
                MessageBox.Show("Account locked");

            MessageBox.Show(CurrentUsername + CurrentPassword);

            LoginErrorMsg = "hahahaha";
            */
        }
    }
}
