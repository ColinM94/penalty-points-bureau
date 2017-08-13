using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PPB_Client.Views
{
    public partial class LoginView : UserControl
    {
        public LoginView()
        {
            InitializeComponent();
        }

        private void Login()
        {
            string username = usernameTxtBox.Text;
            string password = passwordTxtBox.Password.ToString();

            MessageBox.Show(username + " " + password);
        }

        //Removes username placeholder text if field is clicked
        private void UsernameTxtBox_GotFocus(object sender, RoutedEventArgs e)
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
        private void UsernameTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (txtBox.Text == "")
            {
                txtBox.Foreground = Brushes.Gray;
                txtBox.Text = "Username";
            }
        }

        //Removes password placeholder text if field is clicked
        private void PasswordTxtBox_GotFocus(object sender, RoutedEventArgs e)
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
        private void PasswordTxtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            PasswordBox passBox = sender as PasswordBox;

            if (passBox.Password == "")
            {
                passBox.Foreground = Brushes.Gray;
                passBox.Password = "**********";
            }
        }

        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }
    }
}
