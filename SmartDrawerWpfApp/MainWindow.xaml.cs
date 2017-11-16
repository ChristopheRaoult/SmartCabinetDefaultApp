using SmartDrawerDatabase.DAL;
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

namespace SmartDrawerWpfApp
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (var ctx = new SmartDrawerDatabaseContext())
            {
                GrantedUser adminUser = ctx.GrantedUsers.SingleOrDefault(au => au.UserRankId == 1);
                if (adminUser != null)
                    if (SmartDrawerDatabase.PasswordHashing.Sha256Of("Rfid123456") == adminUser.Password)
                        MessageBox.Show("Login : " + adminUser.Login + "\r\nPassword : Rfid123456");
                    else
                        MessageBox.Show("Login : " + adminUser.Login + "\r\nPassword : Wrong Password");
            }
        }
    }
}
