using PPB_Client.Helpers;
using PPB_Client.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Security;

namespace PPB_Client.Views
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : UserControl, IPassword
    {
        /// <summary>
        /// Login constructor
        /// </summary>
        public LoginView()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();

            UsernameTxtBox.Text = "Username";
            PasswordBox.Password = "**********";
        }

        /// <summary>
        /// Implemenation of IPassword, which allows the entered password to be passed securely to the ViewModel.
        /// </summary>
        public SecureString Password
        {
            get
            {
                return PasswordBox.SecurePassword;
            }
        }

        /// <summary>
        /// Removes username placeholder text if field is clicked.
        /// </summary>
        /// <param name="sender">UsernameTxtBox control</param>
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

        /// <summary>
        /// Resets username placeholder text if username field is empty.
        /// </summary>
        /// <param name="sender">UsernameTxtBox control</param>
        private void UsernameTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (txtBox.Text == "")
            {
                txtBox.Foreground = Brushes.Gray;
                txtBox.Text = "Username";
            }
        }

        /// <summary>
        /// Removes password placeholder text if field is clicked.
        /// </summary>
        /// <param name="sender">PasswordBox control</param>
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

        /// <summary>
        /// Resets password placeholder text if username field is empty.
        /// </summary>
        /// <param name="sender">PasswordBox control</param>
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
