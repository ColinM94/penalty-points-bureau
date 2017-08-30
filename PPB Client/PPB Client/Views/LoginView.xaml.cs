using PPB_Client.Helpers;
using PPB_Client.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security;

namespace PPB_Client.Views
{
    // Code behind for login UI.
    public partial class LoginView : UserControl, IPassword
    {
        // Constructor. 
        public LoginView()
        {
            InitializeComponent();

            // Sets data context for binding to the LoginViewModel.
            //DataContext = new LoginViewModel();

            PasswordBox.Password = "**********";
        }

        // Implemenation of IPassword, which allows the entered password to be passed securely to the ViewModel.
        public SecureString Password
        {
            get
            {
                return PasswordBox.SecurePassword;
            }
        }

        // Removes username placeholder text if field is clicked.
        private void UsernameTxtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            // Gets colour of username text.
            String colour = txtBox.Foreground.ToString();
            String black = Brushes.Black.ToString();

            if (txtBox.Text == "Username" && colour != black)
            {
                txtBox.Foreground = Brushes.Black;
                txtBox.Text = "";
            }
        }

        // Resets username placeholder text if username field is empty.
        private void UsernameTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (txtBox.Text == "")
            {
                txtBox.Foreground = Brushes.Gray;
                txtBox.Text = "Username";
            }
        }

        // Removes password placeholder text if field is clicked.
        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;

            // Gets colour of password text.
            String colour = passBox.Foreground.ToString();
            String black = Brushes.Black.ToString();

            // Resets Placeholder text if password field is empty.
            if (colour != black)
            {
                passBox.Foreground = Brushes.Black;
                passBox.Password = "";
            }
        }

        // Resets password placeholder text if username field is empty.
        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;

            if (passBox.Password == "")
            {
                passBox.Foreground = Brushes.Gray;
                passBox.Password = "**********";
            }
        }
    }
}

