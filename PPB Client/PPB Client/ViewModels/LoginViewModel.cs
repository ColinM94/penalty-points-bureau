using System.Windows;
using PPB_Client.Helpers;
using System.Windows.Input;
using System.Diagnostics;

namespace PPB_Client.ViewModels
{
    /// <summary>
    /// Logic for login view.
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {   
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LoginViewModel()
        {
            // Sets up login command
            LoginCommand = new RelayCommand(Login);
        }

        /// <summary>
        /// Keeps track of number of login attempts
        /// </summary>
        private int loginAttempts = 0;

        private string username;
        /// <summary>
        /// Gets and sets login username
        /// </summary>
        public string Username
        {
            get
            {
                return username;
            }

            set
            {
                username = value;
                OnPropertyChanged();
            }
        }

        private string password;
        /// <summary>
        /// Gets and sets login password
        /// </summary>
        public string Password
        {
            get
            {
                return password;
            }
            set
            {
                password = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Login command property
        /// </summary>
        public ICommand LoginCommand
        {
            get;
            private set;
        }

        /// <summary>
        /// Attempts to log in user. 
        /// </summary>
        /// <param name="parameter"></param>
        private void Login(object parameter)
        {
            var passwordContainer = parameter as IPassword;

            if (passwordContainer != null)
            {
                var secureString = passwordContainer.Password;
                Password = SecureStringToString.Convert(secureString);
            }

            if(username == "username" && password == "password")
                MessageBox.Show("Logged in successfully");

            loginAttempts++;

            if (loginAttempts > 2)
                MessageBox.Show("Account locked");

        }
    }
}
