using System;
using System.Security;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows;
using PPB_Client.Helpers;
using PPB_Client.Connection;

namespace PPB_Client.ViewModels
{
    // Logic for Login.
    public class LoginViewModel : BaseViewModel
    {
        // Events
        private void OnLoginSuccess(object source, EventArgs e)
        {
            // Allows CurrentView.View to be changed from a non UI thread. 
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => CurrentView.View = "HomeView"));        
        }

        private void OnLoginFailure(object source, EventArgs e)
        {
            LoginMsg = source.ToString();
        }

        private void OnServerConnected(object source, EventArgs e)
        {
            
        }

        // Constructor.
        public LoginViewModel()
        {
            // Sets field to default text. Password does no
            CurrentUsername = "Username";

            // Sets up login command.
            LoginCommand = new RelayCommand(Login);

            // Subscribes to login events in Server class 
            Server.LoginSuccess += OnLoginSuccess;
            Server.LoginFailure += OnLoginFailure;
            Server.ServerConnected += OnServerConnected;

        }

        // Login command which will be triggered from the UI. 
        public ICommand LoginCommand { get; private set; }

        // Current username submitted by user.
        public string CurrentUsername { get; set; }

        // Current username submitted by user.
        public string CurrentPassword { get; set; }

        // Login error message displayed on login view. 
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
                         
        // Attempts to log in current user. 
        private void Login(object parameter)
        {
            // Displays message if there is no server connection. 
            if (!Server.Connected)
            {
                LoginMsg = "Server connection failed!";
                return;
            }

            // Extracts entered password from password container.
            var passwordContainer = parameter as IPassword;

            if(passwordContainer != null)
            {
                string password = SecureStringToString.Convert(passwordContainer.Password);

                if (CurrentUsername.ToLower() == "username" || CurrentUsername.Length == 0 )
                {
                    LoginMsg = "Username field empty!";
                }

                else if (password.Contains("****") || password.Length == 0 || password == null)
                {
                    LoginMsg = "Password field empty!";
                }

                else
                {
                    Server.Login(CurrentUsername, password);
                }
            }
        }
    }
}
