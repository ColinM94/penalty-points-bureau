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

namespace PPB_Client
{
    public partial class Client : Window
    {
        public string Server = "localhost";
        public int Port = 2000;

        public Client()
        {
            InitializeComponent();
        }
    }
}
