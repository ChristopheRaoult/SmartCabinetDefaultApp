using MahApps.Metro.Controls;
using SmartDrawerWpfApp.Model;
using SmartDrawerWpfApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace SmartDrawerWpfApp.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow2.xaml
    /// </summary>
    public delegate void NotifyHandlerBadgeReaderDelegate(Object sender, string badgeID);
    public delegate void NotifyHandlerM2MCardDelegate(Object sender, string cardID);
    public partial class MainWindow2 : MetroWindow
    {
        public ObservableCollection<BaseObject> Data;
        public event NotifyHandlerBadgeReaderDelegate NotifyBadgeReaderEvent;
        public event NotifyHandlerBadgeReaderDelegate NotifyM2MCardEvent;
        public MainViewModel theModel = null;
        public MainWindow2()
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

        private string IncomeMessage = string.Empty;
        bool IsAccessCardInAccess = false;
        private bool shiftPressed;
        private void ProcessMessage()
        {

            int frameStart = IncomeMessage.IndexOf("!");
            int frameEnd = IncomeMessage.IndexOf(";", 0);
            string message = IncomeMessage.Substring(1, frameEnd - 1);
            string messageType = message.Substring(0, 4);
            switch (messageType)
            {
                case "CARD":                  
                    if (NotifyM2MCardEvent != null) NotifyM2MCardEvent(this, message.Substring(4));
                    break;
                case "PROX":                 
                    if (NotifyBadgeReaderEvent != null) NotifyBadgeReaderEvent(this, message.Substring(4));
                    break;
            }

            IncomeMessage = string.Empty;
            IsAccessCardInAccess = false;
        }
        private void MetroWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                shiftPressed = true;
                return;
            }

            if (((e.Key == Key.Oem8) && (!shiftPressed)) || ((e.Key == Key.D1) && (shiftPressed)))
                IsAccessCardInAccess = true;
            if (IsAccessCardInAccess)
            {
                if (((e.Key == Key.Oem8) && (!shiftPressed)) || ((e.Key == Key.D1) && (shiftPressed)))
                    IncomeMessage = "!";

                else if ((e.Key >= Key.D0) && (e.Key <= Key.D9))
                {
                    IncomeMessage += ((int)e.Key - (int)Key.D0).ToString();
                }
                else if ((e.Key == Key.OemPeriod) || (e.Key == Key.Oem1))
                    IncomeMessage += ";";
                else
                    IncomeMessage += e.Key;
                e.Handled = true;

                if ((e.Key == Key.OemPeriod) || (e.Key == Key.Oem1))
                {
                    ProcessMessage();                
                }
            }
        }
        private void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                shiftPressed = false;
            }
        }

        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (theModel != null)
                theModel.DeleteCard();
        }
        private void DataGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (theModel != null)
                theModel.LightSelectionFromList();
        }


    }
}
