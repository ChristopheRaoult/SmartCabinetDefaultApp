using MahApps.Metro.Controls;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.Model;
using SmartDrawerWpfApp.StaticHelpers;
using SmartDrawerWpfApp.ViewModel;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartDrawerWpfApp
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    /// 
    public delegate void NotifyHandlerBadgeReaderDelegate(Object sender, string badgeID);
    public delegate void NotifyHandlerM2MCardDelegate(Object sender, string cardID);
    public partial class MainWindow : MetroWindow
    {
        public ObservableCollection<BaseObject> Data;
        public event NotifyHandlerBadgeReaderDelegate NotifyBadgeReaderEvent;
        public event NotifyHandlerBadgeReaderDelegate NotifyM2MCardEvent;
        public MainWindow()
        {
            InitializeComponent();
            if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
            {
                Title += " - " + System.Deployment.Application.ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
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

        private string IncomeMessage = string.Empty;
        bool IsAccessCardInAccess = false;
        private bool shiftPressed;          
        private void ProcessMessage()
        {

            int frameStart = IncomeMessage.IndexOf("!");
            int frameEnd = IncomeMessage.IndexOf(";", 0);
            string message = IncomeMessage.Substring(1, frameEnd - 1);
            string messageType = message.Substring(0, 4);
            switch(messageType)
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
                    ProcessMessage();
                wallInfo.Text = IncomeMessage;
                
            }  
        }
        private void MetroWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
               shiftPressed = false;
            }            
        }

        private void MetroWindow_KeyDown(object sender, KeyEventArgs e)
        {
            wallInfo.Text = "KeyDown : " + e.Key.ToString();
            
            if ((e.Key == Key.Oem8) && (!shiftPressed))
                IsAccessCardInAccess = true;

            if (IsAccessCardInAccess)
            {
                if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
                {
                    shiftPressed = true;
                }
                else if ((e.Key == Key.Oem8) && (!shiftPressed))
                    IncomeMessage = "!";
                else if ((e.Key >= Key.D0) && (e.Key <= Key.D9))
                {
                    IncomeMessage += ((int)e.Key - (int)Key.D0).ToString();
                }
                else if (e.Key == Key.OemPeriod)
                    IncomeMessage += ";";
                else
                    IncomeMessage += e.Key;
                e.Handled = true;

                if (e.Key == Key.OemPeriod)
                    ProcessMessage();
            }
        }

        private void MetroWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                shiftPressed = false;
            }
        }

        private void UIElement_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is CardView)
            {
                var cardview = sender as CardView;
                if (cardview.SelectedItem != null)
                {
                    var cardviewItem = cardview.ItemsSource != null
                        ? (CardViewItem)cardview.ItemContainerGenerator.ContainerFromItem(cardview.SelectedItem)
                        : (CardViewItem)cardview.SelectedItem;
                    if (cardviewItem != null && cardviewItem.IsInEditMode)
                    {
                        if (e.Key == Key.Escape)
                        {
                            if (cardviewItem.DataContext is MainViewModel)
                                e.Handled = !(cardviewItem.DataContext as MainViewModel).ValidationSuccess;
                        }
                    }
                }

            }
        }

    }
}
