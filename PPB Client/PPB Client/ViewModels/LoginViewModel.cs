using PPB_Client.Helpers;
using System.Windows.Input;
using PPB_Client.Models;
using Newtonsoft.Json;
using System.Windows;
using System.Threading;
using System;
using System.Security;
using System.Windows.Threading;

namespace PPB_Client.ViewModels
{
    // Logic for Login.
    public class LoginViewModel : BaseViewModel
    {
        private delegate void InvokeDelegate();
        #region Events

        private void OnLoginSuccess(object source, EventArgs e)
        {
            // Allows CurrentView.View to be changed from a non UI thread. 
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => CurrentView.View = "HomeView"));        
        }

        private void OnLoginFailure(object source, EventArgs e)
        {
            LoginMsg = "Login Failed";
        }
        #endregion

        // Constructor. Sets up default property values and commands.  
        public LoginViewModel()
        {
            CurrentUsername = "Username";

            // Sets up login command.
            LoginCommand = new RelayCommand(Login);

            // Subscribes to login events in Server class 
            Server.LoginSuccess += OnLoginSuccess;
            Server.LoginFailure += OnLoginFailure;

        }

        // Login command which will be triggered from the UI. 
        public ICommand LoginCommand { get; private set; }

        // Current username submitted by user.
        public string CurrentUsername { get; set; }

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

        private void SwitchView()
        {
            CurrentView.View = "HomeView";
        }
                         
        // Attempts to log in current user. 
        private void Login(object parameter)
        {            
            SecureString securePassword = null;

            // Extracts entered password from password container.
            var passwordContainer = parameter as IPassword;

            if(passwordContainer != null)
            {
                securePassword = passwordContainer.Password;

                if (CurrentUsername.Length == 0 || CurrentUsername.ToLower() == "username")
                {
                    LoginMsg = "Username field empty!";
                }

                else if (SecureStringToString.Convert(securePassword).Contains("****"))
                {
                    LoginMsg = "Password field empty!";
                }

                else
                {
                    Server.Login(CurrentUsername, securePassword);
                }
                
            }
        }
    }
}
