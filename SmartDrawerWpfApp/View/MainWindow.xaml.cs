using MahApps.Metro.Controls;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.StaticHelpers;
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

        private async void myDatagrid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();

                if (ctx.Columns != null && ctx.Columns.Count() > 0)
                {
                    foreach (Column col in ctx.Columns)
                    {
                        myDatagrid.Columns[col.ColumnIndex].IsHidden = false;
                        myDatagrid.Columns[col.ColumnIndex].HeaderText = col.ColumnName;
                    }
                }
                myDatagrid.Columns["Drawer"].IsHidden = false;
                ctx.Database.Connection.Close();
                ctx.Dispose();
            }
            catch (Exception err)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(err, "Error Datagrid loaded");
                exp.ShowDialog();
            }
        }
    }
}
