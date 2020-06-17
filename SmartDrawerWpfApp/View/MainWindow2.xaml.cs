using MahApps.Metro.Controls;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.Model;
using SmartDrawerWpfApp.Model.DeviceModel;
using SmartDrawerWpfApp.StaticHelpers;
using SmartDrawerWpfApp.ViewModel;
using Syncfusion.UI.Xaml.Grid;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
using static SmartDrawerWpfApp.ViewModel.MainViewModel;

namespace SmartDrawerWpfApp.View
{
    /// <summary>
    /// Logique d'interaction pour MainWindow2.xaml
    /// </summary>
    public delegate void NotifyHandlerBadgeReaderDelegate(Object sender, string badgeID);
    public delegate void NotifyHandlerM2MCardDelegate(Object sender, string cardID);
    public partial class MainWindow2 : MetroWindow
    {
       // public ObservableCollection<BaseObject> Data;
        public event NotifyHandlerBadgeReaderDelegate NotifyBadgeReaderEvent;
        public event NotifyHandlerBadgeReaderDelegate NotifyM2MCardEvent;
        public MainViewModel theModel = null;
        private bool IsDatagridLoaded = false;
        public MainWindow2()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(Properties.Settings.Default.DbPassword))
            {
                string pwd = "rfid";
                var secureString = pwd.ToSecureString();
                Properties.Settings.Default.DbPassword = secureString.EncryptString();
                Properties.Settings.Default.Save();
                // Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.Reload();
            }

            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            Title += " - " + fvi.FileVersion;

            if (string.IsNullOrEmpty(Properties.Settings.Default.WallSerial))
            {
                tb_WallSerial.IsReadOnly = false;
            }
            else
            {
                tb_WallSerial.Background = Brushes.LightGray;
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


        string LastHeader = string.Empty;

        private void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            string tabItem = ((sender as TabControl).SelectedItem as TabItem).Header as string;

            if (LastHeader == tabItem) return;

            switch (tabItem)
            {
                case "Admin Mode":
                    if (theModel != null)
                        theModel.Settings();
                    break;
                default:
                    {
                        theModel.isAdmin = false;
                        theModel.btAdminVisibility = Visibility.Hidden;
                        break;
                    }

            }
            LastHeader = tabItem;


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GeneralTabControl.SelectedIndex = 0;
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            LogToFile.LogMessageToFile("------- Close Application  --------");
            Application.Current.Shutdown();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[1] = true;
            theModel.BrushDrawer[1] = Brushes.Cyan;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[2] = true;
            theModel.BrushDrawer[2] = Brushes.Cyan;
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[3] = true;
            theModel.BrushDrawer[3] = Brushes.Cyan;
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[4] = true;
            theModel.BrushDrawer[4] = Brushes.Cyan;
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[5] = true;
            theModel.BrushDrawer[5] = Brushes.Cyan;
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[6] = true;
            theModel.BrushDrawer[6] = Brushes.Cyan;
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            DevicesHandler.IsDrawerWaitScan[7] = true;
            theModel.BrushDrawer[7] = Brushes.Cyan;
        }

        private async void dataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            /*try
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
                //myDatagrid.Columns["Drawer"].IsHidden = false;
                ctx.Database.Connection.Close();
                ctx.Dispose();
            }
            catch (Exception err)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(err, "Error Datagrid loaded main window");
                exp.ShowDialog();
            }*/
            InitDatagrid();
        }
        private async Task<bool> InitDatagrid()
        {
            try
            {
                if (!IsDatagridLoaded)
                {
                    LogToFile.LogMessageToFile("------- Mainview 2 Load Datagrid --------");

                    if (myDatagrid == null) return false;
                    if (myDatagrid.Columns == null) return false;

                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    if (ctx == null) return false;
                    if (ctx.Columns != null && ctx.Columns.Count() > 0)
                    {
                        foreach (Column col in ctx.Columns)
                        {
                            /*myDatagrid.Columns[col.ColumnIndex + 1].Visibility = Visibility.Visible;
                            myDatagrid.Columns[col.ColumnIndex + 1].Header = col.ColumnName;*/
                            if (myDatagrid.Columns.Count() < col.ColumnIndex) continue;
                            if (myDatagrid.Columns[col.ColumnIndex] == null) continue;

                            /*if (col.ColumnName == "Drawer")
                                theModel.IndexColDrawer = col.ColumnIndex - 1;*/

                            if (col.ColumnName == "Tag ID")
                                myDatagrid.Columns[col.ColumnIndex].IsHidden = true;
                            else
                                myDatagrid.Columns[col.ColumnIndex].IsHidden = false;

                            myDatagrid.Columns[col.ColumnIndex].HeaderText = col.ColumnName;

                            try
                            {

                                if (col.ColumnSize == -1)
                                {
                                    myDatagrid.Columns[col.ColumnIndex].ColumnSizer = Syncfusion.UI.Xaml.Grid.GridLengthUnitType.AutoLastColumnFill;
                                }
                                else
                                    myDatagrid.Columns[col.ColumnIndex].Width = col.ColumnSize;
                            }
                            catch
                            {
                                myDatagrid.Columns[col.ColumnIndex].Width = 150;
                            }

                            myDatagrid.Columns[col.ColumnIndex].UseBindingValue = true;
                        }
                        foreach (Column col in ctx.Columns)
                        {
                            try
                            {
                                myDatagrid.Columns[col.ColumnIndex].AllowSorting = true;
                                myDatagrid.Columns[col.ColumnIndex].AllowFiltering = true;
                            }
                            catch
                            {

                            }
                        }


                        if (myDatagrid.SortColumnDescriptions != null)
                            myDatagrid.SortColumnDescriptions.Clear();
                        myDatagrid.SearchHelper = new SearchHelperExt(this.myDatagrid);
                    }
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                    IsDatagridLoaded = true;
                    LogToFile.LogMessageToFile("------- Mainview 2 Datagrid Loaded --------");
                }
            }
            catch (Exception err)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(err, "Error Datagrid loaded mainwindow 2");
                    exp.ShowDialog();
                });
                await InitDatagrid();

            }
            return IsDatagridLoaded;
        }

        public class SearchHelperExt : SearchHelper
        {
            public SearchHelperExt(SfDataGrid datagrid)
                : base(datagrid)
            {
            }

            protected override bool SearchCell(DataColumnBase column, object record, bool ApplySearchHighlightBrush)
            {
                if (column == null)
                    return false;
                if (column.GridColumn.MappingName == "TagId")
                {
                    return base.SearchCell(column, record, ApplySearchHighlightBrush);
                }
                else
                    return false;
            }
        }

    }       
}
