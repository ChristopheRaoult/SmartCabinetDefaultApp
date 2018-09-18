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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SmartDrawerWpfApp.View
{
    /// <summary>
    /// Logique d'interaction pour PleaseWaitWindow.xaml
    /// </summary>
    public partial class PleaseWaitWindow : MetroWindow
    {
        public PleaseWaitWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                Hide();
            }
            catch
            {

            }
        }
        public void setText(string msg)
        {
            txtInfo.Invoke(() => txtInfo.Text = msg);
        }
    }
}
