using MahApps.Metro.Controls;
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

namespace SmartDrawerAdmin
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            if (string.IsNullOrEmpty(Properties.Settings.Default.DbPassword))
            {
                string pwd = "rfid";
                var secureString = pwd.ToSecureString();
                Properties.Settings.Default.DbPassword = secureString.EncryptString();
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Upgrade();
            }
        }
    }
}
