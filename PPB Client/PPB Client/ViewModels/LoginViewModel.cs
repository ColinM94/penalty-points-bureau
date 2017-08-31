using PPB_Client.Helpers;
using System.Windows.Input;
using PPB_Client.Models;
using Newtonsoft.Json;
using System.Windows;

namespace PPB_Client.ViewModels
{
    // Logic for Login.
    public class LoginViewModel : BaseViewModel
    {
        // Constructor. Sets up default property values and commands.  
        public LoginViewModel()
        {
            CurrentUsername = "Username";

            // Sets up login command.
            LoginCommand = new RelayCommand(Login);
        }

        // Current username submitted by user.
        public string CurrentUsername { get; set; }

        // Current password submitted by user.
        public string CurrentPassword { get; set; }

        private string loginMsg;
        public string LoginMsg
        {
            get
            {
                return loginMsg;
            }
            set
            {
                loginMsg = value;
                OnPropertyChanged();
            }
        }

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

            if(!Server.Login(CurrentUsername, CurrentPassword))
            {
                LoginMsg = "Login Failed!";
            }

            else
            {
                CurrentView.View = "HomeView";
            }
        }
    }
}
