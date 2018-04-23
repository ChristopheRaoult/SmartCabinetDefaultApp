using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using SmartDrawerWpfApp.StaticHelpers;
using System.Linq;
using SmartDrawerDatabase.DAL;
using System;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.Windows.Threading;

using System.Windows;
using System.Threading;
using System.Threading.Tasks;
using SmartDrawerWpfApp.Model.DeviceModel;
using SDK_SC_RfidReader;
using System.Collections.Generic;
using SmartDrawerWpfApp.Model;
using System.Data;
using SmartDrawerWpfApp.Wcf;
using System.ServiceModel;
using System.IO;
using Newtonsoft.Json;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using SmartDrawerWpfApp.StaticHelpers.Security;
using SmartDrawerWpfApp.Fingerprint;
using SecurityModules.FingerprintReader;
using System.Windows.Input;
using System.Diagnostics;
using System.ServiceProcess;
using System.Reflection;
using System.Windows.Data;
using System.Globalization;
using MahApps.Metro;
using SmartDrawerWpfApp.View;
using SmartDrawerWpfApp.WcfServer;
using SmartDrawerAdmin.ViewModel;
using SmartDrawerDatabase;
using System.Data.Entity;
using System.Data.Entity.Validation;

namespace SmartDrawerWpfApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Constant
        Brush _borderInScan = new SolidColorBrush(Color.FromRgb(0xF5, 0xA6, 0x23));
        Brush _borderDrawerOpen = Brushes.Green;
        Brush _borderNotReady = new SolidColorBrush(Color.FromRgb(0xD0, 0x02, 0x1B)); 
        Brush _borderReady = new SolidColorBrush(Color.FromRgb(0xD8, 0xD8, 0xD8));
        Brush _borderReadyToPull = new SolidColorBrush(Color.FromRgb(0x33, 0xBC, 0xBA));
        Brush _borderWhite = Brushes.WhiteSmoke;
        Brush _borderLight = Brushes.Brown;
        #endregion
        #region Variables
        private MainWindow2 mainview0;

        private ProgressDialogController myConTroller = null;
        private readonly TouchKeyboardProvider _touchKeyboardProvider = new TouchKeyboardProvider();

        private DispatcherTimer startTimer;
        private DispatcherTimer AutoConnectTimer;
        private DispatcherTimer AutoLockTimer;
        private DispatcherTimer ScanTimer;

        private bool bLatchUnlocked = false;
        private int _autoLockCpt = 120;
        bool bNeedUpdateCriteria = false;
        bool bNeedUpdateCriteriaAfterScan = false;
        private bool _InLightProcess = false;

        private bool _bStopWall = false;
        private int _lastDrawerOpen = -1;
        private int _recheckLightDrawer = -1;
        private int _lightDrawer = -1;
        private int _autoLightDrawer = -1;
        private bool[] bDrawerToLight = new bool[8];
        private bool[] bDrawerToRefreshLight = new bool[8];
        private int _currentDrawerInLight = -1;
        private List<string> _tagToLightFromTextBox = new List<string>();

        volatile TagFromTxtBox tagOnBadDrawer = new TagFromTxtBox();
        private bool bWasInAutoLight = false;

        private List<BaseObject> _SelectedBaseObjects;

        #endregion
        #region Properties

       

        private bool _validationSuccess = true;
        internal bool ValidationSuccess
        {
            get { return _validationSuccess; }
            set { _validationSuccess = value; }
        }

        string _dayDate;
        public string dayDate
        {
            get { return _dayDate; }
            set
            {
                if (_dayDate != value)
                {
                    _dayDate = value;
                    RaisePropertyChanged(() => dayDate);
                }
            }
        }

        string _dayTime;
        public string dayTime
        {
            get { return _dayTime; }
            set
            {
                if (_dayTime != value)
                {
                    _dayTime = value;
                    RaisePropertyChanged(() => dayTime);
                }
            }
        }

        string _ServerIp;
        public string ServerIp
        {
            get
            {
                _ServerIp = Properties.Settings.Default.ServerIp;
                return _ServerIp;
            }
            set
            {
                if (_ServerIp != value)
                {
                    _ServerIp = value;
                    Properties.Settings.Default.ServerIp = _ServerIp;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                    RaisePropertyChanged(() => ServerIp);
                }
            }
        }

        int _ServerPort;
        public int ServerPort
        {
            get
            {
                _ServerPort = Properties.Settings.Default.ServerPort;
                return _ServerPort;
            }
            set
            {
                if (_ServerPort != value)
                {
                    _ServerPort = value;
                    Properties.Settings.Default.ServerPort = _ServerPort;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                    RaisePropertyChanged(() => ServerPort);
                }
            }
        }

        string _NotificationIp;
        public string NotificationIp
        {
            get
            {
                _NotificationIp = Properties.Settings.Default.NotificationIp;
                return _NotificationIp;
            }
            set
            {
                if (_NotificationIp != value)
                {
                    _NotificationIp = value;
                    Properties.Settings.Default.NotificationIp = _NotificationIp;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                    RaisePropertyChanged(() => NotificationIp);
                }
            }
        }

        int _NotificationPort;
        public int NotificationPort
        {
            get
            {
                _NotificationPort = Properties.Settings.Default.NotificationPort;
                return _NotificationPort;
            }
            set
            {
                if (_NotificationPort != value)
                {
                    _NotificationPort = value;
                    Properties.Settings.Default.NotificationPort = _NotificationPort;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged(() => NotificationPort);
                }
            }
        }


        string _WallSerial;
        public string WallSerial
        {
            get
            {
                _WallSerial = Properties.Settings.Default.WallSerial;
                return _WallSerial;
            }
            set
            {
                if (_WallSerial != value)
                {
                    _WallSerial = value;
                    Properties.Settings.Default.WallSerial = _WallSerial;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged(() => WallSerial);
                }
            }
        }

        string _WallName;
        public string WallName
        {
            get
            {
                _WallName = Properties.Settings.Default.WallName;
                return _WallName;
            }
            set
            {
                if (_WallName != value)
                {
                    _WallName = value;
                    Properties.Settings.Default.WallName = _WallName;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged(() => WallName);
                }
            }
        }

        string _WallLocation;
        public string WallLocation
        {
            get
            {
                _WallLocation = Properties.Settings.Default.WallLocation;
                return _WallLocation;
            }
            set
            {
                if (_WallLocation != value)
                {
                    _WallLocation = value;
                    Properties.Settings.Default.WallLocation = _WallLocation;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged(() => WallLocation);
                }
            }
        }

        string _RfidSerial;
        public string RfidSerial
        {
            get
            {
                _RfidSerial = Properties.Settings.Default.RfidSerial;
                return _RfidSerial;
            }
            set
            {
                if (_RfidSerial != value)
                {
                    _RfidSerial = value;                   
                    Properties.Settings.Default.RfidSerial = _RfidSerial;
                    Properties.Settings.Default.Save();
                    RaisePropertyChanged(() => RfidSerial);
                }
            }
        }

        private string _wallStatus;
        public string wallStatus
        {
            get { return _wallStatus; }
            set
            {
                _wallStatus = value ;
                RaisePropertyChanged("wallStatus");
            }
        }

        private int _TotalCassettesPulled;
        public int TotalCassettesPulled
        {
            get { return _TotalCassettesPulled; }
            set
            {
                _TotalCassettesPulled = value;
                RaisePropertyChanged(() => TotalCassettesPulled);
            }
        }
        private int _TotalCassettesToPull;
        public int TotalCassettesToPull
        {
            get { return _TotalCassettesToPull; }
            set
            {
                _TotalCassettesToPull = value;
                RaisePropertyChanged(() => TotalCassettesToPull);
            }
        }

        private int _WallTotalStones;
        public int WallTotalStones
        {
            get { return _WallTotalStones; }
            set
            {
                _WallTotalStones = value;
                RaisePropertyChanged(() => WallTotalStones);
            }
        }

        string _WallStatusOperational;
        public string WallStatusOperational
        {
            get { return _WallStatusOperational; }
            set
            {
                if (_WallStatusOperational != value)
                {
                    _WallStatusOperational = value;
                    RaisePropertyChanged(() => WallStatusOperational);
                }
            }
        }

        string _LastScanInfo;
        public string LastScanInfo
        {
            get { return _LastScanInfo; }
            set
            {
                if (_LastScanInfo != value)
                {
                    _LastScanInfo = value;
                    RaisePropertyChanged(() => LastScanInfo);
                }
            }
        }

        bool _OverallStatus;
        public bool OverallStatus
        {
            get { return _OverallStatus; }
            set
            {
                if (_OverallStatus != value)
                {
                    _OverallStatus = value;
                    RaisePropertyChanged("OverallStatus");
                }
            }
        }

        bool _NetworkStatus;
        public bool NetworkStatus
        {
            get { return _NetworkStatus; }
            set
            {
                _NetworkStatus = value;
                RaisePropertyChanged("NetworkStatus");
            }
        }

        bool _RfidStatus;
        public bool RfidStatus
        {
            get { return _RfidStatus; }
            set
            {
                _RfidStatus = value;
                RaisePropertyChanged("RfidStatus");
            }
        }

        bool _GpioStatus;
        public bool GpioStatus
        {
            get { return _GpioStatus; }
            set
            {
                _GpioStatus = value;
                RaisePropertyChanged("GpioStatus");

            }
        }

        string _LoggedUser;
        public string LoggedUser
        {
            get { return _LoggedUser; }
            set
            {
                _LoggedUser = value;
                RaisePropertyChanged("LoggedUser");
            }
        }

        string _AutoLockMsg;
        public string AutoLockMsg
        {
            get { return _AutoLockMsg; }
            set
            {
                _AutoLockMsg = value;
                RaisePropertyChanged("AutoLockMsg");
            }
        }

        private bool _IsAutoLightDrawerChecked;
        public bool IsAutoLightDrawerChecked
        {
            get { return _IsAutoLightDrawerChecked; }
            set
            {
                _IsAutoLightDrawerChecked = value;
                if ((IsFlyoutCassettePositionOpen) || (IsFlyoutCassetteInfoOpen))
                {
                    //do nothing
                }
                else
                    bWasInAutoLight = _IsAutoLightDrawerChecked;
                RaisePropertyChanged(() => IsAutoLightDrawerChecked);

                if (_IsAutoLightDrawerChecked)
                {
                    if (_lightDrawer == -1) // No light in progress
                    {
                        _bStopWall = true;
                        _autoLightDrawer = _lastDrawerOpen; //give Number drawer to autolight
                    }
                }
                else
                {
                    if (_autoLightDrawer != -1) //switch of drawer
                    {
                        try
                        {
                            if (DevicesHandler.DevicesConnected)
                                DevicesHandler.StopLighting(_autoLightDrawer);
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
        private ObservableCollection<string> _DrawerStatus;
        public ObservableCollection<string> DrawerStatus
        {
            get { return _DrawerStatus; }
            set
            {
                _DrawerStatus = value;
                RaisePropertyChanged("DrawerStatus");
            }
        }

        private ObservableCollection<string> _DrawerTagQty;
        public ObservableCollection<string> DrawerTagQty
        {
            get { return _DrawerTagQty; }
            set
            {
                _DrawerTagQty = value;
                RaisePropertyChanged("DrawerTagQty");
            }

        }

        private ObservableCollection<Brush> _BrushDrawer;
        public ObservableCollection<Brush> BrushDrawer
        {
            get { return _BrushDrawer; }
            set
            {
                _BrushDrawer = value;
                RaisePropertyChanged("BrushDrawer");
            }
        }

        private bool _IsFlyoutCassettePositionOpen;
        public bool IsFlyoutCassettePositionOpen
        {
            get { return _IsFlyoutCassettePositionOpen; }
            set
            {
                _IsFlyoutCassettePositionOpen = value;
                RaisePropertyChanged("IsFlyoutCassettePositionOpen");
                if (_IsFlyoutCassettePositionOpen == false)
                {
                    if (!IsFlyoutCassetteInfoOpen)
                        cancelLighting();
                    SelectedCassette = null;
                    if (bNeedUpdateCriteria)
                    {
                        bNeedUpdateCriteria = false;
                    }
                }
                else
                {
                    //reset Light all button
                    btLightText = "Light All";
                    // automatic light when checked
                    LightAll();
                }

            }
        }
        private bool _IsFlyoutCassetteInfoOpen;
        public bool IsFlyoutCassetteInfoOpen
        {
            get { return _IsFlyoutCassetteInfoOpen; }
            set
            {
                _IsFlyoutCassetteInfoOpen = value;
                RaisePropertyChanged(() => IsFlyoutCassetteInfoOpen);
                if (_IsFlyoutCassetteInfoOpen == false)
                {
                    if (!IsFlyoutCassettePositionOpen)
                        cancelLighting();
                }
                else
                {
                    IsAutoLightDrawerChecked = false;
                }
            }
        }

        private string _btLightText;
        public string btLightText
        {
            get { return _btLightText; }
            set
            {
                _btLightText = value;
                RaisePropertyChanged("btLightText");
            }
        }

        private string _txtNbSelectedItem;
        public string txtNbSelectedItem
        {
            get { return _txtNbSelectedItem; }
            set
            {
                _txtNbSelectedItem = value;
                RaisePropertyChanged("txtNbSelectedItem");
            }
        }

        private CassettesSelection _previousSelectedCassettes;
        private CassettesSelection _SelectedCassette;
        public CassettesSelection SelectedCassette
        {
            get { return _SelectedCassette; }
            set
            {
                _SelectedCassette = value;
                RaisePropertyChanged("SelectedCassette");
            }
        }

        Visibility _btUserVisibility;
        public Visibility btUserVisibility
        {
            get {  return _btUserVisibility; }
            set
            {
            _btUserVisibility = value;
                RaisePropertyChanged(() => btUserVisibility);
            }
        }

        bool _IsUser;
        public bool IsUser { get { return _IsUser; } set { _IsUser = value; RaisePropertyChanged(() => IsUser); } }

        private string _txtSearchCtrl;
        public string txtSearchCtrl
        {
            get { return _txtSearchCtrl; }
            set
            {
                _txtSearchCtrl = value;
                RaisePropertyChanged(() => txtSearchCtrl);
               
                mainview0.myDatagrid.SearchHelper.AllowFiltering = true;
                mainview0.myDatagrid.SearchHelper.ClearSearch();
                mainview0.myDatagrid.SearchHelper.Search(_txtSearchCtrl);
                mainview0.myDatagrid.SearchHelper.FindNext(_txtSearchCtrl);
            }
        }

        private ObservableCollection<string> _ListCtrlPerDrawer;
        public ObservableCollection<string> ListCtrlPerDrawer
        {
            get { return _ListCtrlPerDrawer; }
            set
            {
                _ListCtrlPerDrawer = value;
                RaisePropertyChanged(() => ListCtrlPerDrawer);
            }
        }

        private string _DrawerSelected;
        public string DrawerSelected
        {
            get { return _DrawerSelected; }
            set
            {
                _DrawerSelected = value;
                RaisePropertyChanged(() => DrawerSelected);
            }
        }

        private string _DrawerCtrlCount;
        public string DrawerCtrlCount
        {
            get { return _DrawerCtrlCount; }
            set
            {
                _DrawerCtrlCount = value;
                RaisePropertyChanged(() => DrawerCtrlCount);
            }
        }
        #endregion
        #region admin
        public bool isAdmin = false;
        Visibility _btAdminVisibility;
        public Visibility btAdminVisibility
        {
            get { return _btAdminVisibility; }
            set
            {
                _btAdminVisibility = value;
                RaisePropertyChanged(() => btAdminVisibility);
            }
        }

        private ObservableCollection<UsersViewModel> _dataUser = new ObservableCollection<UsersViewModel>();
        public ObservableCollection<UsersViewModel> DataUser
        {
            get { return _dataUser; }
            set
            {
                _dataUser = value;
                RaisePropertyChanged(() => DataUser);
            }
        }
        private UsersViewModel _selectedUser;
        public UsersViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                EditedUser = SelectedUser;
                RaisePropertyChanged(() => SelectedUser);
            }
        }
        private UsersViewModel _editedUser = new UsersViewModel();
        public UsersViewModel EditedUser
        {
            get { return _editedUser; }
            set
            {
                _editedUser = value;
                RaisePropertyChanged(() => EditedUser);
            }
        }

        public async void PopulateUser(int? userId)
        {
            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var lstDev = ctx.GrantedUsers
                    .Where(u => u.UserRankId > 1).ToList();

                EditedUser = new UsersViewModel();
                DataUser.Clear();
                foreach (var dv in lstDev)
                {

                    if (userId.HasValue)
                    {
                        if (dv.ServerGrantedUserId != userId.Value) continue;
                    }

                    UsersViewModel uvm = new UsersViewModel()
                    {
                        Id = dv.GrantedUserId,
                        ServerId = dv.ServerGrantedUserId,
                        Login = dv.Login,
                        FirstName = dv.FirstName,
                        LastName = dv.LastName,
                        BadgeId = dv.BadgeNumber,
                        Fingerprints = dv.Fingerprints.Count
                    };
                    DataUser.Add(uvm);
                }
                ctx.Database.Connection.Close();
                ctx.Dispose();
            }
            catch (Exception error)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate Device");
                    exp.ShowDialog();
                }));
            }

        }

        public RelayCommand btEnrollUser { get; set; }
        private async void EnrollUser()
        {

            if (SelectedUser != null)  //Update existing
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var original = ctx.GrantedUsers.Find(SelectedUser.Id);
                if (original != null)
                {
                    var enrollForm = new EnrollFingersForm(original);
                    if (!enrollForm.IsDisposed)
                    {
                        enrollForm.ShowDialog();
                    }
                    await ProcessSelectionFromServer.UpdateUserAsync(original.Login);
                }
            }
            else
            {
                await mainview0.ShowMessageAsync("INFORMATION", "Please create and save an user before enroll any fingerprint");
                return;
            }
            if (isAdmin)
                PopulateUser(null);
            else
                PopulateUser(SelectedUser.ServerId);


        }

        public RelayCommand btSaveUser { get; set; }
        private async void SaveUser()
        {
            if (EditedUser != null)
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                if (SelectedUser != null)  //Update existing
                {
                    if (string.IsNullOrEmpty(EditedUser.Login) || string.IsNullOrEmpty(EditedUser.FirstName) || string.IsNullOrEmpty(EditedUser.LastName))
                    {
                        await mainview0.ShowMessageAsync("INFORMATION", "Please Fill user Login , Firstname and Lastname before saving");
                        return;
                    }
                    var original = ctx.GrantedUsers.Find(SelectedUser.Id);
                    if (original != null)
                    {
                        original.Login = SelectedUser.Login;
                        if (!string.IsNullOrWhiteSpace(SelectedUser.Password))
                            original.Password = PasswordHashing.Sha256Of(SelectedUser.Password);
                        original.LastName = SelectedUser.LastName;
                        original.FirstName = SelectedUser.FirstName;
                        original.BadgeNumber = SelectedUser.BadgeId;
                        ctx.Entry(original).State = EntityState.Modified;
                        ctx.SaveChanges();

                        // Update server
                        await ProcessSelectionFromServer.UpdateUserAsync(original.Login);
                    }
                }
                else //save new
                {
                    if (string.IsNullOrEmpty(EditedUser.Login) || string.IsNullOrEmpty(EditedUser.FirstName) || string.IsNullOrEmpty(EditedUser.LastName))
                    {
                        await mainview0.ShowMessageAsync("INFORMATION", "Please Fill user Login , Firstname and Lastname before saving");
                        return;
                    }
                    else
                    {
                        GrantedUser newUser = null;
                        if (!string.IsNullOrWhiteSpace(EditedUser.Password))
                        {
                            newUser = new GrantedUser()
                            {
                                Login = EditedUser.Login,
                                Password = PasswordHashing.Sha256Of(EditedUser.Password),
                                FirstName = EditedUser.FirstName,
                                LastName = EditedUser.LastName,
                                BadgeNumber = EditedUser.BadgeId,
                                UserRankId = 3,
                            };

                            ctx.GrantedUsers.Add(newUser);
                        }
                        else
                        {
                            newUser = new GrantedUser()
                            {
                                Login = EditedUser.Login,
                                FirstName = EditedUser.FirstName,
                                LastName = EditedUser.LastName,
                                BadgeNumber = EditedUser.BadgeId,
                                UserRankId = 3,
                            };

                            ctx.GrantedUsers.Add(newUser);
                        }
                        ctx.SaveChanges();

                        // Update server
                        await ProcessSelectionFromServer.UpdateUserAsync(newUser.Login);

                    }
                }
                ctx.Database.Connection.Close();
                ctx.Dispose();
                if (isAdmin)
                    PopulateUser(null);
                else
                    PopulateUser(SelectedUser.ServerId);
            }
        }
        public RelayCommand btDeleteUser { get; set; }
        private async void DeleteUser()
        {
            if (SelectedUser != null)  //Update existing
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var original = ctx.GrantedUsers.Find(SelectedUser.Id);
                if (original != null)
                    ctx.GrantedUsers.Remove(original);
                ctx.SaveChanges();
                ctx.Database.Connection.Close();
                ctx.Dispose();
                if (isAdmin)
                    PopulateUser(null);
                else
                    PopulateUser(SelectedUser.ServerId);
            }
        }
        public RelayCommand btResetUser { get; set; }
        private void ResetUser()
        {
            SelectedUser = null;
            EditedUser = new UsersViewModel();
        }

        #endregion


        #region Datagrid

        private ObservableCollection<BaseObject> _data = new ObservableCollection<BaseObject>();
        public ObservableCollection<BaseObject> Data
        {
            get { return _data; }
            set
            {
                _data = value;
                RaisePropertyChanged(() => Data);
            }
        }
        private ObservableCollection<object> _selectedItems = new ObservableCollection<object>();
        public ObservableCollection<object> SelectedItems
        {
            get
            {
                return _selectedItems;
            }
            set
            {
                _selectedItems = value;
                RaisePropertyChanged("SelectedItems");
                if (_selectedItems.Count() > 0)
                {
                    txtNbSelectedItem = string.Format("Stones Selected : {0}", _selectedItems.Count());
                }
                else
                {
                   
                    if (mainview0.myDatagrid.View != null)
                        txtNbSelectedItem = string.Format("Stones Selected : {0}", mainview0.myDatagrid.View.Records.Count());
                }
            }
        }
        DataTable _sourceTable;
        public DataTable SourceTable
        {
            get
            {
                return _sourceTable;
            }
            set
            {
                _sourceTable = value;
                RaisePropertyChanged(() => SourceTable);
            }
        }
        private static Object thisLock = new Object();
        public async void getCriteria()
        {
            var controller = await mainview0.ShowProgressAsync("Please wait", "Retrieving information from Database");
            try
            {               
                controller.SetIndeterminate();
                await Task.Run(() =>
               {
                   lock (thisLock)
                   {
                       Data.Clear();
                       var ctx = RemoteDatabase.GetDbContext();
                       int nbCol = ctx.Columns.Count();

                       if (nbCol > 1)
                       {
                           foreach (KeyValuePair<string, int> entry in DevicesHandler.ListTagPerDrawer)
                           {
                               RfidTag tag = ctx.RfidTags.AddIfNotExisting(entry.Key);
                               Product pct = ctx.Products.GetByTagUid(entry.Key);
                               if (pct != null)
                               {
                                   Data.Add(new BaseObject(pct, entry.Value));
                               }
                               else
                               {
                                   Product tmpProd = new Product() { RfidTag = tag, ProductInfo0 = "Unreferenced" };
                                   Data.Add(new BaseObject(tmpProd, entry.Value));
                               }

                           }
                           ctx.Database.Connection.Close();
                           ctx.Dispose();
                       }
                       else //Only one column , as just tagUID no need to search in DB
                       {
                           foreach (KeyValuePair<string, int> entry in DevicesHandler.ListTagPerDrawer)
                           {
                               RfidTag tag = new RfidTag() { TagUid = entry.Key };
                               Product tmpProd = new Product() { RfidTag = tag };
                               Data.Add(new BaseObject(tmpProd, entry.Value));
                           }
                       }
                       

                       if (Data.Count != 0)
                           SourceTable = PopulateDataGrid(Data);
                       else
                           SourceTable = null;

                       if (_IsFlyoutCassettePositionOpen)
                           bNeedUpdateCriteria = true;
                   }

               });
                mainview0.Data = Data;
                mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                if ((controller != null) && (controller.IsOpen))
                    await controller.CloseAsync();
            }
            catch (Exception error)
            {
                if ((controller != null) && (controller.IsOpen))
                    await controller.CloseAsync();
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error getting criteria");
                    exp.ShowDialog();
                }));
            }
            finally
            {
                if ((controller != null) && (controller.IsOpen))
                    await controller.CloseAsync();
            }
        }
        public DataTable PopulateDataGrid(ObservableCollection<BaseObject> giaData)
        {
            var ctx = RemoteDatabase.GetDbContext();
            int nbCol = ctx.Columns.Count();
            ctx.Database.Connection.Close();
            ctx.Dispose();

            DataTable tmpDt = new DataTable();
            tmpDt.Columns.Add(new System.Data.DataColumn("UID", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column1", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column2", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column3", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column4", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column5", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column6", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column7", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column8", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column9", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column10", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column11", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column12", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column13", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column14", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column15", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column16", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column17", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column18", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column19", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Drawer", typeof(string)));

            foreach (BaseObject bo in giaData)
            {
                var row = tmpDt.NewRow();
                tmpDt.Rows.Add(row);
                row["UID"] = bo.Productinfo.RfidTag.TagUid;
                row["Drawer"] = bo.drawerId;
                for (int loop = 0; loop < nbCol; loop++)
                {
                    string colName = "Column" + loop;
                    switch (loop)
                    {
                        case 0: row["UID"] = bo.Productinfo.RfidTag.TagUid; break;
                        case 1: row[colName] = bo.Productinfo.ProductInfo0; break;
                        case 2: row[colName] = bo.Productinfo.ProductInfo1; break;
                        case 3: row[colName] = bo.Productinfo.ProductInfo2; break;
                        case 4: row[colName] = bo.Productinfo.ProductInfo3; break;
                        case 5: row[colName] = bo.Productinfo.ProductInfo4; break;
                        case 6: row[colName] = bo.Productinfo.ProductInfo5; break;
                        case 7: row[colName] = bo.Productinfo.ProductInfo6; break;
                        case 8: row[colName] = bo.Productinfo.ProductInfo7; break;
                        case 9: row[colName] = bo.Productinfo.ProductInfo8; break;
                        case 10: row[colName] = bo.Productinfo.ProductInfo9; break;
                        case 11: row[colName] = bo.Productinfo.ProductInfo10; break;
                        case 12: row[colName] = bo.Productinfo.ProductInfo11; break;
                        case 13: row[colName] = bo.Productinfo.ProductInfo12; break;
                        case 14: row[colName] = bo.Productinfo.ProductInfo13; break;
                        case 15: row[colName] = bo.Productinfo.ProductInfo14; break;
                        case 16: row[colName] = bo.Productinfo.ProductInfo15; break;
                        case 17: row[colName] = bo.Productinfo.ProductInfo16; break;
                        case 18: row[colName] = bo.Productinfo.ProductInfo17; break;
                        case 19: row[colName] = bo.Productinfo.ProductInfo18; break;
                        case 20: row[colName] = bo.Productinfo.ProductInfo19; break;
                    }
                }
            }
            return tmpDt;
        }

        private int _SelectedTabIndex;
        public int SelectedTabIndex
        {
            get { return _SelectedTabIndex; }
            set
            {
                _SelectedTabIndex = value;
                RaisePropertyChanged(() => SelectedTabIndex);
            }
        }
        

        #endregion
        #region Selection
        private ObservableCollection<SelectionViewModel> _selection = new ObservableCollection<SelectionViewModel>();
        public ObservableCollection<SelectionViewModel> Selection
        {
            get { return _selection; }
            set
            {
                _selection = value;
                RaisePropertyChanged(() => Selection);
            }
        }

        private SelectionViewModel _SelectionSelected;
        public SelectionViewModel SelectionSelected
        {
            get { return _SelectionSelected; }
            set
            {
                _SelectionSelected = value;

                if (_selection != null)
                {
                    foreach (var item in Selection)
                    {
                        if (item == _SelectionSelected)
                            item.IsSelected = true;
                        else
                            item.IsSelected = false;
                    }
                }

                RaisePropertyChanged(() => SelectionSelected);
            }
        }

        #endregion
        #region Command

        public RelayCommand btSaveDevice { get; set; }
        public async void ReloadDevice()
        {
            Properties.Settings.Default.Upgrade();

            // Add device in local DB
            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                ctx.Devices.Clear();
                Device newDev = new Device()
                {
                    DeviceTypeId = 15,
                    DeviceName = Properties.Settings.Default.WallName,
                    DeviceSerial = Properties.Settings.Default.WallSerial,
                    DeviceLocation = Properties.Settings.Default.WallLocation,                    
                    RfidSerial = Properties.Settings.Default.RfidSerial,
                    UpdateAt = DateTime.Now,
                };


                ctx.Devices.Add(newDev);
                await ctx.SaveChangesAsync();
                ctx.Database.Connection.Close();
                ctx.Dispose();

                DevicesHandler.ResetDEviceEntity();

            }
            catch (DbEntityValidationException ex)
            {
                foreach (var eve in ex.EntityValidationErrors)
                {
                    Console.WriteLine(@"Entity of type ""{0}"" in state ""{1}"" 
                           has the following validation errors:",
                        eve.Entry.Entity.GetType().Name,
                        eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine(@"- Property: ""{0}"", Error: ""{1}""",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch(Exception exp)
            {
                Console.WriteLine(exp.Message);
            }

            Device myDev = await ProcessSelectionFromServer.GetCabinet(Properties.Settings.Default.WallSerial);
            if (myDev == null)
            {
                Device CreatedDev = new Device()
                {
                    DeviceSerial = Properties.Settings.Default.WallSerial,
                    DeviceName = Properties.Settings.Default.WallName,
                    DeviceLocation = Properties.Settings.Default.WallLocation,
                    IpAddress = Utils.GetLocalIp(),
                };
                await ProcessSelectionFromServer.CreateCabinet(CreatedDev);
            }
            else
            {

                myDev.DeviceName = Properties.Settings.Default.WallName;
                myDev.DeviceLocation = Properties.Settings.Default.WallLocation;
                myDev.IpAddress = Utils.GetLocalIp();
                await ProcessSelectionFromServer.UpdateCabinet(myDev);

            }


            startTimer.Start();
            startTimer.IsEnabled = true;

        }
        public RelayCommand BtRemoveCardSelection { get; set; }
        public async void DeleteCard()
        {
            removeSelection();
        }

        public RelayCommand BtRefreshSelection { get; set; }
        public async void getSelection()
        {
            //Clear pull info
            if (IsFlyoutCassettePositionOpen) //Stop lighting id lighting ON
                IsFlyoutCassettePositionOpen = false; 

            CassettesSelection tmpCassette = new CassettesSelection();
            tmpCassette.CassetteDrawer1Number = String.Empty;
            tmpCassette.CassetteDrawer2Number = String.Empty;
            tmpCassette.CassetteDrawer3Number = String.Empty;
            tmpCassette.CassetteDrawer4Number = String.Empty;
            tmpCassette.CassetteDrawer5Number = String.Empty;
            tmpCassette.CassetteDrawer6Number = String.Empty;
            tmpCassette.CassetteDrawer7Number = String.Empty;
            tmpCassette.ListControlNumber = new List<string>(); // Mandatory to be not null but need to be emtpy
            SelectedCassette = tmpCassette;

            BrushDrawer[1] = _borderReady;
            BrushDrawer[2] = _borderReady;
            BrushDrawer[3] = _borderReady;
            BrushDrawer[4] = _borderReady;
            BrushDrawer[5] = _borderReady;
            BrushDrawer[6] = _borderReady;
            BrushDrawer[7] = _borderReady;


            myConTroller = await mainview0.ShowProgressAsync("Please wait","Get Selection From server");
            myConTroller.SetIndeterminate();
            try
            {
                getCriteria();

                /* Get Selection from API */
                bool gotSel = await ProcessSelectionFromServer.GetAndStoreSelectionAsync();

                if (gotSel)
                {

                    List<PullItem> lstToRemove = new List<PullItem>();
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    Selection.Clear();
                    foreach (var sel in ctx.PullItems)
                    {
                        SelectionViewModel svm = new SelectionViewModel();
                        svm.IsSelected = false;
                        svm.PullItemId = sel.PullItemId;
                        svm.ServerPullItemId = sel.ServerPullItemId;

                        TimeSpan ts = DateTime.Now - sel.PullItemDate;
                        if (ts.TotalDays < 1)
                            svm.PullItemDate = sel.PullItemDate.ToString("hh:mm tt");
                        else if (ts.TotalDays == 2)
                            svm.PullItemDate = "Yesterday\r\n" + sel.PullItemDate.ToString("hh:mm tt");
                        else
                            svm.PullItemDate = sel.PullItemDate.ToString("MMM dd") + "\r\n" + sel.PullItemDate.ToString("hh:mm tt");

                        svm.Description = sel.Description;
                        if (sel.GrantedUser != null)
                            svm.User = sel.GrantedUser.FirstName + " " + sel.GrantedUser.LastName;
                        svm.TotalToPull = sel.TotalToPull;

                        svm.lstTopull = new List<string>();
                        int nbInDevice = 0;

                        foreach (var pit in sel.PullItems)
                        {
                            if (DevicesHandler.ListTagPerDrawer.ContainsKey(pit.RfidTag.TagUid))
                            {
                                svm.lstTopull.Add(pit.RfidTag.TagUid);
                                nbInDevice++;
                            }
                        }
                        // if (1 == 1)
                        if (nbInDevice > 0)
                        {
                            svm.TotalToPullInDevice = nbInDevice;
                            Selection.Add(svm);
                        }
                        else   //remove selection bigger thant 1 week having no selection
                        {
                            if (sel.PullItemDate.AddDays(7) < DateTime.Now)
                                lstToRemove.Add(sel);
                        }
                    }

                    if (lstToRemove.Count > 0)
                    {
                        foreach (var sel in lstToRemove)
                            ctx.PullItems.Remove(sel);
                        await ctx.SaveChangesAsync();
                    }
                    ctx.Database.Connection.Close();
                    ctx.Dispose();

                    mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                    if ((myConTroller != null) && (myConTroller.IsOpen))
                        await myConTroller.CloseAsync();
                }
                else
                {
                    mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                    if ((myConTroller != null) && (myConTroller.IsOpen))
                        await myConTroller.CloseAsync();

                    MessageDialogResult messageResult = await mainview0.ShowMessageAsync(" Information", "Error while updating selection ...");


                }
            }
            catch { }
            finally
            {
                try
                {
                    if ((myConTroller != null) && (myConTroller.IsOpen))
                        await myConTroller.CloseAsync();
                }
                catch
                { }
            }


        }


        public RelayCommand btLightFilteredTagSelection { get; set; }
        public void LightSelectionFromList()
        {
            try
            {
                if (SelectionSelected == null)
                    return;

                if (RfidStatus == false)
                {
                    IsFlyoutCassettePositionOpen = false;
                    return;
                }
                CassettesSelection tmpCassette = new CassettesSelection();
                _SelectedBaseObjects = new List<BaseObject>();
                tmpCassette.ListControlNumber = new List<string>();

                for (int loop = 0; loop < 8; loop++)
                    tmpCassette.TagToLight[loop] = new List<string>();


                List<string> TmpListCtrlPerDrawer1 = new List<string>(DevicesHandler.GetTagFromDictionnary(1, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer2 = new List<string>(DevicesHandler.GetTagFromDictionnary(2, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer3 = new List<string>(DevicesHandler.GetTagFromDictionnary(3, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer4 = new List<string>(DevicesHandler.GetTagFromDictionnary(4, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer5 = new List<string>(DevicesHandler.GetTagFromDictionnary(5, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer6 = new List<string>(DevicesHandler.GetTagFromDictionnary(6, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer7 = new List<string>(DevicesHandler.GetTagFromDictionnary(7, DevicesHandler.ListTagPerDrawer));
                foreach (string uid in _SelectionSelected.lstTopull)
                {
                    BaseObject theBo = (from c in Data
                                        where c.Productinfo.RfidTag.TagUid.Equals(uid)
                                        select c).SingleOrDefault<BaseObject>();

                    if (theBo != null)
                    {
                        _SelectedBaseObjects.Add(theBo);
                        if (!tmpCassette.ListControlNumber.Contains(theBo.Productinfo.RfidTag.TagUid))
                        {
                            switch (theBo.drawerId)
                            {
                                case 1:
                                    if (TmpListCtrlPerDrawer1.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[1].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;
                                case 2:
                                    if (TmpListCtrlPerDrawer2.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[2].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;
                                case 3:
                                    if (TmpListCtrlPerDrawer3.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[3].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;
                                case 4:
                                    if (TmpListCtrlPerDrawer4.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[4].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;
                                case 5:
                                    if (TmpListCtrlPerDrawer5.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[5].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;
                                case 6:
                                    if (TmpListCtrlPerDrawer6.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[6].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;
                                case 7:
                                    if (TmpListCtrlPerDrawer7.Contains(theBo.Productinfo.RfidTag.TagUid))
                                    {
                                        tmpCassette.TagToLight[7].Add(theBo.Productinfo.RfidTag.TagUid);
                                        tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                    }
                                    break;

                            }
                        }
                    }
                }

                tmpCassette.CassetteDrawer1Number = tmpCassette.TagToLight[1].Count == 0 ? "-" : tmpCassette.TagToLight[1].Count.ToString();
                tmpCassette.CassetteDrawer2Number = tmpCassette.TagToLight[2].Count == 0 ? "-" : tmpCassette.TagToLight[2].Count.ToString(); 
                tmpCassette.CassetteDrawer3Number = tmpCassette.TagToLight[3].Count == 0 ? "-" : tmpCassette.TagToLight[3].Count.ToString(); 
                tmpCassette.CassetteDrawer4Number = tmpCassette.TagToLight[4].Count == 0 ? "-" : tmpCassette.TagToLight[4].Count.ToString(); 
                tmpCassette.CassetteDrawer5Number = tmpCassette.TagToLight[5].Count == 0 ? "-" : tmpCassette.TagToLight[5].Count.ToString(); 
                tmpCassette.CassetteDrawer6Number = tmpCassette.TagToLight[6].Count == 0 ? "-" : tmpCassette.TagToLight[6].Count.ToString(); 
                tmpCassette.CassetteDrawer7Number = tmpCassette.TagToLight[7].Count == 0 ? "-" : tmpCassette.TagToLight[7].Count.ToString();

                BrushDrawer[1] = tmpCassette.TagToLight[1].Count == 0 ? BrushDrawer[1] : _borderReadyToPull;
                BrushDrawer[2] = tmpCassette.TagToLight[2].Count == 0 ? BrushDrawer[2] : _borderReadyToPull;
                BrushDrawer[3] = tmpCassette.TagToLight[3].Count == 0 ? BrushDrawer[3] : _borderReadyToPull;
                BrushDrawer[4] = tmpCassette.TagToLight[4].Count == 0 ? BrushDrawer[4] : _borderReadyToPull;
                BrushDrawer[5] = tmpCassette.TagToLight[5].Count == 0 ? BrushDrawer[5] : _borderReadyToPull;
                BrushDrawer[6] = tmpCassette.TagToLight[6].Count == 0 ? BrushDrawer[6] : _borderReadyToPull;
                BrushDrawer[7] = tmpCassette.TagToLight[7].Count == 0 ? BrushDrawer[7] : _borderReadyToPull;
              

                tmpCassette.CassetteSelectionTotalNumber = tmpCassette.ListControlNumber.Count;

                if (_SelectedBaseObjects.Count > 0)
                {
                    SelectedCassette = tmpCassette;
                    _previousSelectedCassettes = SelectedCassette;
                    TotalCassettesToPull = SelectedCassette.CassetteSelectionTotalNumber;
                    TotalCassettesPulled = 0;
                    IsFlyoutCassettePositionOpen = true;
                }
                else
                {
                    IsFlyoutCassettePositionOpen = false;
                    _InLightProcess = false;
                }
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error selection");
                exp.ShowDialog();
            }
        }
        public RelayCommand btRemoveSelection { get; set; }
        private async void removeSelection()
        {  
            if (SelectionSelected != null)
            {
                /*var ctx = await RemoteDatabase.GetDbContextAsync();

                var sel = ctx.PullItems.GetByServerId(SelectionSelected.ServerPullItemId);
                if (sel != null)
                    ctx.PullItems.Remove(sel);
                await ctx.SaveChangesAsync();

                ctx.Database.Connection.Close();
                ctx.Dispose();
                getSelection();*/

                await ProcessSelectionFromServer.DeleteSelectionAsync(SelectionSelected.ServerPullItemId);
            }
        }

        public RelayCommand btSettingCommand { get; set; }
        public async void Settings()
        {

            if (GrantedUsersCache.LastAuthenticatedUser != null)
            {
                var enrollForm = new EnrollFingersForm(GrantedUsersCache.LastAuthenticatedUser);
                if (!enrollForm.IsDisposed)
                {
                    enrollForm.ShowDialog();
                    GrantedUsersCache.Reload();
                    logout();
                }
            }
            else
            {
                LoginDialogSettings lds = new LoginDialogSettings();
                lds.ColorScheme = MetroDialogColorScheme.Theme;
                lds.InitialUsername = "Admin";
                LoginDialogData result = await mainview0.ShowLoginAsync("Login", "Enter Password", lds);
                if (result != null)
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    GrantedUser adminUser = ctx.GrantedUsers.GetByLogin(result.Username);
                    if ((adminUser.Password == result.Password) || (adminUser.Password == SmartDrawerDatabase.PasswordHashing.Sha256Of(result.Password)))
                    {
                        if (adminUser.UserRank == ctx.UserRanks.Administrator())
                        {
                            isAdmin = true;
                            IsUser = false;
                        }

                        else
                        {
                            IsUser = true;
                            isAdmin = false;
                        }
                            btAdminVisibility = Visibility.Visible;
                       
                        if (isAdmin)
                            PopulateUser(null);
                        else
                            PopulateUser(adminUser.ServerGrantedUserId);

                    }
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                }
            }
        }
        public RelayCommand ResetDeviceCommand { get; set; }
        void Reset(bool renewFP = false)
        {
            mainview0.Dispatcher.Invoke(new System.Action(() =>
            {
                if (AutoConnectTimer != null)
                    AutoConnectTimer.IsEnabled = false;
            }));
            WallStatusOperational = "INITIALISATION";

            try
            {
                GpioStatus = false;
                RfidStatus = false;
                NetworkStatus = false;
                Thread.Sleep(200);
                mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                Thread.Sleep(200);
                mainview0.Dispatcher.BeginInvoke(new ThreadStart(async delegate ()
                {
                    wallStatus = "Devices Released";

                    if (renewFP)
                    {
                        if ((DevicesHandler.FPReader != null) && (DevicesHandler.FPReader.Available))
                        {

                            string fileName = @"C:\devcon\x64\renewFP.lnk";
                            if (File.Exists(fileName))
                            {
                                var t = Task.Run(() => StartAndWaitProcess(fileName));
                                await t;
                            }
                        }
                    }

                    DevicesHandler.ReleaseDevices();
                    GpioStatus = DevicesHandler.GpioCardObject.IsConnected;
                    RfidStatus = DevicesHandler.DevicesConnected;
                    Thread.Sleep(1000);
                    wallStatus = "In Connection";
                    Thread.Sleep(1000);
                    DevicesHandler.TryInitializeLocalDeviceAsync();
                    InitWcfService();

                }));
            }
            catch(Exception error)
            {
                 mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Reset");
                    exp.ShowDialog();
                }));
            }
            finally
            {
                mainview0.Dispatcher.Invoke(new System.Action(() =>
                {
                    if (AutoConnectTimer != null)
                        AutoConnectTimer.IsEnabled = true;
                }));
                
            }
        }

        void StartAndWaitProcess(string path)
        {
            using (var p = Process.Start(path))
                p.WaitForExit();
        }

        public RelayCommand btLightFilteredTag { get; set; }
        void LightFilteredTag()
        {
            try
            {
                if (RfidStatus == false)
                {
                    IsFlyoutCassettePositionOpen = false;
                    return;
                }
                CassettesSelection tmpCassette = new CassettesSelection();
                _SelectedBaseObjects = new List<BaseObject>();
                tmpCassette.ListControlNumber = new List<string>();

                for (int loop = 0; loop < 8; loop++)
                    tmpCassette.TagToLight[loop] = new List<string>();


                List<string> TmpListCtrlPerDrawer1 = new List<string>(DevicesHandler.GetTagFromDictionnary(1, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer2 = new List<string>(DevicesHandler.GetTagFromDictionnary(2, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer3 = new List<string>(DevicesHandler.GetTagFromDictionnary(3, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer4 = new List<string>(DevicesHandler.GetTagFromDictionnary(4, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer5 = new List<string>(DevicesHandler.GetTagFromDictionnary(5, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer6 = new List<string>(DevicesHandler.GetTagFromDictionnary(6, DevicesHandler.ListTagPerDrawer));
                List<string> TmpListCtrlPerDrawer7 = new List<string>(DevicesHandler.GetTagFromDictionnary(7, DevicesHandler.ListTagPerDrawer));

                if (SelectedItems != null && SelectedItems.Count > 0)
                {
                    foreach (DataRowView item in SelectedItems)
                    {
                        string uid = item.Row.ItemArray[0].ToString();
                        BaseObject theBo = (from c in Data
                                            where c.Productinfo.RfidTag.TagUid.Equals(uid)
                                            select c).SingleOrDefault<BaseObject>();

                        if (theBo != null)
                        {
                            _SelectedBaseObjects.Add(theBo);
                            if (!tmpCassette.ListControlNumber.Contains(theBo.Productinfo.RfidTag.TagUid))
                            {
                                switch (theBo.drawerId)
                                {
                                    case 1:
                                        if (TmpListCtrlPerDrawer1.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[1].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 2:
                                        if (TmpListCtrlPerDrawer2.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[2].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 3:
                                        if (TmpListCtrlPerDrawer3.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[3].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 4:
                                        if (TmpListCtrlPerDrawer4.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[4].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 5:
                                        if (TmpListCtrlPerDrawer5.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[5].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 6:
                                        if (TmpListCtrlPerDrawer6.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[6].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 7:
                                        if (TmpListCtrlPerDrawer7.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[7].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;

                                }
                            }
                        }
                    }

                }
                else
                {  // filtered records
                   
                    foreach (RecordEntry re in mainview0.myDatagrid.View.Records)
                    {
                        DataRowView drv = re.Data as DataRowView;
                        string uid = drv.Row[0].ToString();
                        BaseObject theBo = (from c in Data
                                            where c.Productinfo.RfidTag.TagUid.Equals(uid)
                                            select c).SingleOrDefault<BaseObject>();

                        if (theBo != null)
                        {
                            _SelectedBaseObjects.Add(theBo);
                            if (!tmpCassette.ListControlNumber.Contains(theBo.Productinfo.RfidTag.TagUid))
                            {
                                switch (theBo.drawerId)
                                {
                                    case 1:
                                        if (TmpListCtrlPerDrawer1.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[1].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 2:
                                        if (TmpListCtrlPerDrawer2.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[2].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 3:
                                        if (TmpListCtrlPerDrawer3.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[3].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 4:
                                        if (TmpListCtrlPerDrawer4.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[4].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 5:
                                        if (TmpListCtrlPerDrawer5.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[5].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 6:
                                        if (TmpListCtrlPerDrawer6.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[6].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;
                                    case 7:
                                        if (TmpListCtrlPerDrawer7.Contains(theBo.Productinfo.RfidTag.TagUid))
                                        {
                                            tmpCassette.TagToLight[7].Add(theBo.Productinfo.RfidTag.TagUid);
                                            tmpCassette.ListControlNumber.Add(theBo.Productinfo.RfidTag.TagUid);
                                        }
                                        break;

                                }
                            }
                        }
                    }
                }
                tmpCassette.CassetteDrawer1Number = tmpCassette.TagToLight[1].Count.ToString(); ;
                tmpCassette.CassetteDrawer2Number = tmpCassette.TagToLight[2].Count.ToString(); ;
                tmpCassette.CassetteDrawer3Number = tmpCassette.TagToLight[3].Count.ToString(); ;
                tmpCassette.CassetteDrawer4Number = tmpCassette.TagToLight[4].Count.ToString(); ;
                tmpCassette.CassetteDrawer5Number = tmpCassette.TagToLight[5].Count.ToString(); ;
                tmpCassette.CassetteDrawer6Number = tmpCassette.TagToLight[6].Count.ToString(); ;
                tmpCassette.CassetteDrawer7Number = tmpCassette.TagToLight[7].Count.ToString(); ;
                tmpCassette.CassetteSelectionTotalNumber = tmpCassette.ListControlNumber.Count;

                if (_SelectedBaseObjects.Count > 0)
                {
                    SelectedCassette = tmpCassette;
                    _previousSelectedCassettes = SelectedCassette;
                    TotalCassettesToPull = SelectedCassette.CassetteSelectionTotalNumber;
                    TotalCassettesPulled = 0;
                    IsFlyoutCassettePositionOpen = true;
                }
                else
                {
                    IsFlyoutCassettePositionOpen = false;
                    _InLightProcess = false;
                }
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error selection");
                exp.ShowDialog();
            }
        }
        public RelayCommand LightAllCommand { get; set; }
        void LightAll()
        {
            _autoLockCpt = 120;
            if (btLightText == "Light All")
            {
                _InLightProcess = true;
                btLightText = "Cancel Light";
                if (IsWallInScan())
                {
                    _bStopWall = true;

                }
                wallStatus = "Wait user action";
                LightSelection();
            }

            else
            {
                _InLightProcess = false;
                btLightText = "Light All";
                IsFlyoutCassettePositionOpen = false;
                SelectedItems.Clear();
            }
        }
        public RelayCommand openKeyboard { get; set; }
        void openKeyboardFn()
        {
            //wallStatus = "txt Got focus";
            _touchKeyboardProvider.ShowTouchKeyboard();
             mainview0.TxtCtrlNumber.Focus();
        }
        public RelayCommand searchTxtGotCR { get; set; }
        void searchTxtGotCRfn()
        {
            try
            {
                if ((Data == null) || (Data.Count == 0)) return;               

                mainview0.myDatagrid.SearchHelper.AllowFiltering = true;
                mainview0.myDatagrid.SearchHelper.ClearSearch();
                mainview0.myDatagrid.SearchHelper.Search(_txtSearchCtrl);
                mainview0.myDatagrid.SearchHelper.FindNext(_txtSearchCtrl);
                do
                {
                    var rowIndex = mainview0.myDatagrid.SearchHelper.CurrentRowColumnIndex.RowIndex;
                    var record = mainview0.myDatagrid.View.Records[rowIndex - 1];
                    DataRowView drv = record.Data as DataRowView;
                    mainview0.myDatagrid.SelectedItems.Add(drv);
                }
                while (mainview0.myDatagrid.SearchHelper.FindNext(_txtSearchCtrl));
                
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error getting Stone info");
                exp.ShowDialog();
            }
        }
        public RelayCommand btClearSelection { get; set; }
        void ClearSelection()
        {

            CassettesSelection tmpCassette = new CassettesSelection();
            tmpCassette.CassetteDrawer1Number = String.Empty;
            tmpCassette.CassetteDrawer2Number = String.Empty;
            tmpCassette.CassetteDrawer3Number = String.Empty;
            tmpCassette.CassetteDrawer4Number = String.Empty;
            tmpCassette.CassetteDrawer5Number = String.Empty;
            tmpCassette.CassetteDrawer6Number = String.Empty;
            tmpCassette.CassetteDrawer7Number = String.Empty;
            tmpCassette.ListControlNumber = new List<string>(); // Mandatory to be not null but need to be emtpy
            SelectedCassette = tmpCassette;

            BrushDrawer[1] = _borderReady;
            BrushDrawer[2] = _borderReady;
            BrushDrawer[3] = _borderReady;
            BrushDrawer[4] = _borderReady;
            BrushDrawer[5] = _borderReady;
            BrushDrawer[6] = _borderReady;
            BrushDrawer[7] = _borderReady;

            mainview0.myDatagrid.SelectedItems.Clear();
            mainview0.myDatagrid.SearchHelper.ClearSearch();
            mainview0.myDatagrid.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex() { RowIndex = 1, ColumnIndex = 1 });

            IsFlyoutCassettePositionOpen = false;
           
        }
        public RelayCommand LogoutCommand { get; set; }
        public async void logout()
        {
            LoggedUser = null;
            btUserVisibility = Visibility.Collapsed;
            while (IsOneDrawerOpen())
                await mainview0.ShowMessageAsync("Error", "Please Close all drawer before Logout");

            DevicesHandler.LockWall();
            bLatchUnlocked = false;
            GrantedUsersCache.LastAuthenticatedUser = null;
            DevicesHandler.LastScanAccessTypeName = AccessType.Manual;
            AutoLockMsg = "Wait User";
        }


        public RelayCommand BtLighAllPerDrawer { get; set; }
        public void BtLighAllPerDrawerFn()
        {

            if (_lastDrawerOpen <= 0)
            {
                wallStatus = "Open a drawer before press an action !!";
                return;
            }

            if (IsWallInScan())
                StopWallScan();
            DrawerStatus[_lastDrawerOpen] = DrawerStatusList.InLight;
            _currentDrawerInLight = _lastDrawerOpen;
            DrawerStatus[_lastDrawerOpen] = DrawerStatusList.InLight;
            DevicesHandler.StopLighting(_lastDrawerOpen);
            DevicesHandler.SetDrawerActive(_lastDrawerOpen);
            DevicesHandler.LightAll(_lastDrawerOpen);
            DevicesHandler.Device.LEdOnAll(1, 0, false);
        }

        public RelayCommand BtLighListPerDrawer { get; set; }
        public void BtLighListPerDrawerFn()
        {
            if (_lastDrawerOpen <= 0)
            {
                wallStatus = "Open a drawer before press an action !!";
                return;
            }

            if (IsWallInScan())
                StopWallScan();
            _currentDrawerInLight = _lastDrawerOpen;
            DrawerStatus[_lastDrawerOpen] = DrawerStatusList.InLight;
            DevicesHandler.StopLighting(_lastDrawerOpen);
            DevicesHandler.SetDrawerActive(_lastDrawerOpen);
            List<string> listCtrl = DevicesHandler.GetTagFromDictionnary(_lastDrawerOpen, DevicesHandler.ListTagPerDrawer);
            DevicesHandler.LightTags(_lastDrawerOpen, listCtrl, true);
            DevicesHandler.Device.LEdOnAll(1, 0, false);
            mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
            {
                mainview0.ShowMessageAsync("Info Drawer " + _lastDrawerOpen, "Light list process finish");
            }));
        }

        #endregion
        #region timer
        private async void StartTimer_Tick(object sender, EventArgs e)
        {
            startTimer.Stop();
            startTimer.IsEnabled = false;

            // No serial in Configuration - Conenct to get rfid serial
            
            DevicesHandler.FindAndConnectDevice();
            if ((DevicesHandler.Device != null) &&(DevicesHandler.Device.IsConnected))
            {
                if (Properties.Settings.Default.RfidSerial != DevicesHandler.Device.SerialNumber)
                {
                    Properties.Settings.Default.RfidSerial = DevicesHandler.Device.SerialNumber;
                    Properties.Settings.Default.Save();
                    Properties.Settings.Default.Upgrade();
                }
                DevicesHandler.Device.DisconnectReader();
            }
        
          
            // Is Wall In database - refer to Rfid serial so need connection to get number
           // await mainview0.Dispatcher.BeginInvoke(new System.Action(async () =>
           // {

                // Test If serial exist in app
                bool bNeedQuit = false;

                if ((string.IsNullOrEmpty (Properties.Settings.Default.WallSerial)) || (string.IsNullOrEmpty(Properties.Settings.Default.WallName)))
                {
                    bNeedQuit = true;
                    await mainview0.ShowMessageAsync("Wall Information", "Wall Not define in app! \r\n Please Add device ");
                    SelectedTabIndex = 3;           
                }

                if (bNeedQuit) return;

                Device myDev = await ProcessSelectionFromServer.GetCabinet(Properties.Settings.Default.WallSerial);
                if ( (myDev == null) || (myDev.DeviceSerial != Properties.Settings.Default.WallSerial))
                {
                    bNeedQuit = true;
                    await mainview0.ShowMessageAsync("Wall Information", "Wall not found in server or server not found - You have to go in admin mode to setup device");
                    SelectedTabIndex = 3;
                }
                else
                {
                    string tmpIp = Utils.GetLocalIp();
                if (!string.IsNullOrEmpty(Properties.Settings.Default.NotificationIp))
                    tmpIp = Properties.Settings.Default.NotificationIp;

                if ((myDev.DeviceSerial != Properties.Settings.Default.WallSerial) || (myDev.DeviceName != Properties.Settings.Default.WallName) || (myDev.IpAddress != tmpIp))
                    {
                        bNeedQuit = true;

                        Device updDev = new Device()
                        {
                            DeviceSerial = Properties.Settings.Default.WallSerial,
                            DeviceName = Properties.Settings.Default.WallName,
                            DeviceLocation = Properties.Settings.Default.WallLocation,
                            IpAddress = Utils.GetLocalIp(),

                        };
                        bool x = await ProcessSelectionFromServer.UpdateCabinet(updDev);

                        if (!x)
                        {
                            await mainview0.ShowMessageAsync("Wall Information", "Unable to update wall device inforamtion - You have to go in admin mode to setup device");
                            bNeedQuit = true;
                        }
                    }
                    
                }

                if (bNeedQuit) return;


                 var ctx = await RemoteDatabase.GetDbContextAsync();
                 Device mydev = ctx.Devices.GetByRfidSerialNumber(Properties.Settings.Default.RfidSerial);
                 if (mydev == null)
                 {
                        Device newDev = new Device() { DeviceTypeId = 15, DeviceName = Properties.Settings.Default.WallName, DeviceSerial = Properties.Settings.Default.WallSerial, RfidSerial = Properties.Settings.Default.RfidSerial,DeviceLocation = Properties.Settings.Default.WallLocation, UpdateAt = DateTime.Now };
                        ctx.Devices.Add(newDev);

                        try
                        {
                            await ctx.SaveChangesAsync();
                        }
                        catch (DbEntityValidationException ex)
                        {
                            foreach (var eve in ex.EntityValidationErrors)
                            {
                                Console.WriteLine(@"Entity of type ""{0}"" in state ""{1}"" 
                           has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name,
                                    eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine(@"- Property: ""{0}"", Error: ""{1}""",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }

                       
                 }
                
                WallSerial = Properties.Settings.Default.WallSerial;
                WallName = Properties.Settings.Default.WallName;                    
                InitValue();
                 

                 ctx.Database.Connection.Close();
                 ctx.Dispose();

           // }));

            await mainview0.Dispatcher.BeginInvoke(new System.Action( () =>
            {
                refreshUserFromServer();
            }));
        }
        private void AutoConnectTimer_Tick(object sender, EventArgs e)
        {
            AutoConnectTimer.IsEnabled = false;

            dayDate = DateTime.Now.ToString("dddd, dd");
            dayTime = DateTime.Now.ToString("hh:mm tt");

            RfidStatus = DevicesHandler.DevicesConnected;

            if (RfidStatus)
            {
                TimeSpan ts = DateTime.Now - DevicesHandler.LastScanTime;
                LastScanInfo = string.Format("Last scan : {0} min(s) ago", (int)ts.TotalMinutes);
            }

            // 2 drawers open not possible meaning power cut 
            if ((DevicesHandler.DrawerStatus[1] == DrawerStatusList.Open) && (DevicesHandler.DrawerStatus[2] == DrawerStatusList.Open))
                DevicesHandler.ReleaseDevices();

            if (!DevicesHandler.DevicesConnected)
                //DevicesHandler.TryInitializeLocalDeviceAsync();
                DoConnect();

            GpioStatus = DevicesHandler.GpioCardObject.IsConnected;
            if (GpioStatus == false)
            {
                //DevicesHandler.TryInitializeLocalDeviceAsync();
                //DoConnect();
                for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                {

                    DevicesHandler.DrawerStatus[loop] = DrawerStatusList.InConnection;
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                    BrushDrawer[loop] = _borderNotReady;

                }
                Reset(false);
            }
            else
                DevicesHandler.GpioCardObject.GetInValues();

            if (!NetworkStatus)
                InitWcfService();

            AutoConnectTimer.IsEnabled = true;
        }
        private async void AutoLockTimer_Tick(object sender, EventArgs e)
        {
            if (!bLatchUnlocked) return;
            if (_autoLockCpt <= 0)
            {
                LoggedUser = null;
                btUserVisibility = Visibility.Collapsed;
                while (IsOneDrawerOpen())
                    await mainview0.ShowMessageAsync("Error", "Please Close all drawer before Logout");
                DevicesHandler.LockWall();
                bLatchUnlocked = false;
                AutoLockMsg = "Wait User";
                GrantedUsersCache.LastAuthenticatedUser = null;
                DevicesHandler.LastScanAccessTypeName = AccessType.Manual;
            }

            if (_autoLockCpt > 0)
            {
                if (IsWallReady())
                    _autoLockCpt--;
                AutoLockMsg = "Log out in " + _autoLockCpt + "sec";
            }
        }
        private async void ScanTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                ScanTimer.IsEnabled = false;
                ScanTimer.Stop();

                #region status
              
                if (!RfidStatus)
                {
                    OverallStatus = false;
                    WallStatusOperational = "RFID FAILURE";                   
                }
                else if (!GpioStatus)
                {
                    OverallStatus = false;
                    WallStatusOperational = "GPIO FAILURE";                  
                }
                else if (!NetworkStatus)
                {
                    OverallStatus = false;
                    WallStatusOperational = "SERVICE FAILURE";                
                }
                else
                {
                    OverallStatus = true;
                    WallStatusOperational = "OPERATIONAL";
                }                

                #endregion

                #region stop scan
                if (_bStopWall)
                {
                    _bStopWall = false;
                    if (IsWallInScan())
                        StopWallScan();
                }
                #endregion
                #region Recheck light
                else if (_recheckLightDrawer != -1)
                {
                    int _bckrecheckLightDrawer = _recheckLightDrawer;
                    myConTroller = await mainview0.ShowProgressAsync("Please wait", string.Format("Rechecking {0} tags in drawer {1}", SelectedCassette.TagToLight[_bckrecheckLightDrawer].Count, _bckrecheckLightDrawer));
                    myConTroller.SetIndeterminate();

                    List<string> TagToLight = new List<string>(SelectedCassette.TagToLight[_bckrecheckLightDrawer]);

                    await Task.Run(() =>
                    {
                        DevicesHandler.SetDrawerActive(_bckrecheckLightDrawer);
                        if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                        {
                           
                            if (TagToLight.Count > 0)
                            {
                                int nbToFind = TagToLight.Count;
                                wallStatus = "Check " + nbToFind + " stones(s) in drawer " + _bckrecheckLightDrawer;
                                DevicesHandler.StopLighting(_bckrecheckLightDrawer);
                                DevicesHandler.DrawerStatus[_bckrecheckLightDrawer] = DrawerStatusList.InLight;
                                DrawerStatus[_bckrecheckLightDrawer] = DevicesHandler.DrawerStatus[_bckrecheckLightDrawer];
                                BrushDrawer[_bckrecheckLightDrawer] = _borderLight;
                                LightSelectionDrawer(TagToLight, _bckrecheckLightDrawer);
                                if (nbToFind == TagToLight.Count)
                                {
                                    DevicesHandler.StopLighting(_bckrecheckLightDrawer);
                                    DevicesHandler.DrawerStatus[_bckrecheckLightDrawer] = DrawerStatusList.Ready;
                                    DrawerStatus[_bckrecheckLightDrawer] = DevicesHandler.DrawerStatus[_bckrecheckLightDrawer];
                                    BrushDrawer[_bckrecheckLightDrawer] = _borderReady;
                                }
                                else
                                {
                                    DevicesHandler.DrawerStatus[_bckrecheckLightDrawer] = DrawerStatusList.InLight;
                                    DrawerStatus[_bckrecheckLightDrawer] = DevicesHandler.DrawerStatus[_bckrecheckLightDrawer];
                                    BrushDrawer[_bckrecheckLightDrawer] = _borderDrawerOpen;
                                }

                                DevicesHandler.IsDrawerWaitScan[_bckrecheckLightDrawer] = true;
                                int nbTag = TagToLight.Count;
                                TotalCassettesPulled += nbTag;
                                //update remain tag to light

                                foreach (string uid in TagToLight) // tag to light should contain all removed tags
                                {
                                    if (SelectedCassette.TagToLight[_bckrecheckLightDrawer].Contains(uid))
                                    {
                                        SelectedCassette.TagToLight[_bckrecheckLightDrawer].Remove(uid);
                                        SelectedCassette.ListControlNumber.Remove(uid);
                                    }
                                }

                                //Store removed tag at recheck with user    
                                ReaderData rd = DevicesHandler.DrawerInventoryData[_bckrecheckLightDrawer];
                                for (int loop = 0; loop < TagToLight.Count; loop++) // tag to light should contain all removed tags
                                {
                                    string uid = TagToLight[loop];
                                    if (rd.strListTag.Contains(uid))
                                        rd.strListTag.Remove(uid);
                                }
                                DevicesHandler.DrawerInventoryData[_bckrecheckLightDrawer] = rd;
                                DevicesHandler.DrawerTagQty[_bckrecheckLightDrawer] = rd.strListTag.Count;
                                DevicesHandler.RemoveTagFromListForDrawer(_bckrecheckLightDrawer);

                                DevicesHandler.AddTagListForDrawer(_bckrecheckLightDrawer, rd.strListTag);
                                DevicesHandler.UpdateAddedTagToDrawer(_bckrecheckLightDrawer, rd.strListTag);
                                DevicesHandler.UpdateremovedTagToDrawer(_bckrecheckLightDrawer, rd.strListTag);
                                Task.Run(() =>
                               {
                                   InventoryHandler.HandleNewScanCompleted(_bckrecheckLightDrawer);
                               });

                                //Update GUI INFO 
                                SelectedCassette.CassetteDrawer1Number = SelectedCassette.TagToLight[1].Count.ToString(); ;
                                SelectedCassette.CassetteDrawer2Number = SelectedCassette.TagToLight[2].Count.ToString(); ;
                                SelectedCassette.CassetteDrawer3Number = SelectedCassette.TagToLight[3].Count.ToString(); ;
                                SelectedCassette.CassetteDrawer4Number = SelectedCassette.TagToLight[4].Count.ToString(); ;
                                SelectedCassette.CassetteDrawer5Number = SelectedCassette.TagToLight[5].Count.ToString(); ;
                                SelectedCassette.CassetteDrawer6Number = SelectedCassette.TagToLight[6].Count.ToString(); ;
                                SelectedCassette.CassetteDrawer7Number = SelectedCassette.TagToLight[7].Count.ToString(); ;
                                SelectedCassette.CassetteSelectionTotalNumber = SelectedCassette.ListControlNumber.Count;

                                if (TotalCassettesPulled == TotalCassettesToPull)
                                {
                                    IsFlyoutCassetteInfoOpen = false;
                                    IsFlyoutCassettePositionOpen = false;
                                }

                              

                            }
                        }
                        else
                            wallStatus = "Wait user Action";
                        _recheckLightDrawer = -1;
                    });

                    /*****   Update API ****/
                    await Task.Run(() =>
                    {
                        if (SelectionSelected != null)
                        {
                            ProcessSelectionFromServer.UpdateSelectionAsync(SelectionSelected.ServerPullItemId, TagToLight);
                        }

                    });

                    mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                    if ((myConTroller != null) && (myConTroller.IsOpen))
                        await myConTroller.CloseAsync();
                }
                #endregion
                #region Light
                else if (_lightDrawer != -1)
                {
                    int bckDrawer = _lightDrawer;
                    DevicesHandler.SetDrawerActive(bckDrawer);
                    if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                    {
                        if (bDrawerToRefreshLight[bckDrawer] == true)
                        {
                            DevicesHandler.StopLighting(bckDrawer);
                            Thread.Sleep(500);
                            DevicesHandler.Device.LEdOnAll(bckDrawer, 0, false);
                        }

                        else if (bDrawerToLight[bckDrawer] == true)
                        {
                            bDrawerToLight[bckDrawer] = false;
                            myConTroller = await mainview0.ShowProgressAsync("Please wait", string.Format("Lighting {0} tags in drawer {1}", SelectedCassette.TagToLight[_lightDrawer].Count, _lightDrawer));
                            myConTroller.SetIndeterminate();

                            await Task.Run(() =>
                            {
                                DevicesHandler.StopLighting(bckDrawer);
                                List<string> TagToLight = new List<string>(SelectedCassette.TagToLight[bckDrawer]);
                                wallStatus = "Light " + TagToLight.Count + " stones(s) in drawer " + bckDrawer;
                                LightSelectionDrawer(TagToLight, bckDrawer);
                                if (TagToLight.Count > 0)
                                    wallStatus = "Unable to light " + TagToLight.Count + " stones(s) in drawer " + bckDrawer;

                                bDrawerToRefreshLight[bckDrawer] = true;
                            });

                            mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                            if ((myConTroller != null) && (myConTroller.IsOpen))
                                await myConTroller.CloseAsync();
                        }
                        else
                        {
                            DevicesHandler.StopLighting(bckDrawer);
                        }
                    }
                    else //pas de selection
                    {
                        DevicesHandler.Device.LEdOnAll(1, 0, false);
                    }

                }
                #endregion
                #region AutoLight Open drawer
                else if ((IsAutoLightDrawerChecked) && (_autoLightDrawer != -1))
                {
                    int bckDrawer = _autoLightDrawer;
                    wallStatus = "Drawer " + bckDrawer + " in auto light";
                    DevicesHandler.StopLighting(bckDrawer);
                    DevicesHandler.LightAll(bckDrawer);
                    DevicesHandler.Device.LEdOnAll(1, 0, false);
                }
                #endregion
                #region scan
                else if (IsWallReady())
                //else if ((!IsFlyoutCassettePositionOpen) && (IsWallReady()))
                {
                    for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                    {
                        if (DevicesHandler.IsDrawerWaitScan[loop])
                        {
                            if ((DevicesHandler.Device != null) && (DevicesHandler.Device.IsConnected) && (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.Ready))
                            {
                                DevicesHandler.StartManualScan(loop);

                                break;
                            }
                        }
                    }
                }
                #endregion

                if (bNeedUpdateCriteriaAfterScan)
                {
                    if (!IsWaitingForScan())
                    {
                        getCriteria();
                        getSelection();
                        bNeedUpdateCriteriaAfterScan = false;
                       
                       
                    }
                }
            }
            catch (IndexOutOfRangeException err)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(err, "Out of range Exception - value = " + _autoLightDrawer);
                    exp.ShowDialog();
                }));
            }
            catch (Exception error)
            {
                if ((myConTroller != null) && (myConTroller.IsOpen))
                    await myConTroller.CloseAsync();
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Scan Timer");
                    exp.ShowDialog();
                }));
            }
            finally
            {
                if ((myConTroller != null) && (myConTroller.IsOpen))
                    await myConTroller.CloseAsync();

                ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                ScanTimer.IsEnabled = true;
                ScanTimer.Start();
            }
        }
        #endregion
        #region Users
        private async  void  refreshUserFromServer()
        {
            bool gotUsers = false;
            try
            {
                gotUsers = await ProcessSelectionFromServer.GetAndStoreUserAsync();               
            }
            catch (Exception error)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Get User from server");
                    exp.ShowDialog();
                }));               
            }            
        }
        #endregion
        #region WCF Service
        private ServiceHost host = null;
        SslWallNotificationService WallService = new SslWallNotificationService();
        // WebHttpBinding webBinding;
        private void InitWcfService()
        {
            try
            {
                if (host != null)
                {
                    host.Close();
                    host = null;
                }

                WallService.mainview0 = mainview0;
                host = new ServiceHost(WallService);
                
                host.Opened += Host_Opened;
                host.Closed += Host_Closed;
                host.Faulted += Host_Faulted;
                host.Open();
                WallService = host.SingletonInstance as SslWallNotificationService;
                if (WallService != null)
                    WallService.MyHostEvent += Wns_MyHostEvent;
                wallStatus = "WCF service Initialized at " + host.Description.Endpoints[0].ListenUri;
                NetworkStatus = true;

            }
            catch (Exception ex)
            {
                ExceptionMessageBox msg = new ExceptionMessageBox(ex, "Error Wcf Initialisation");
                msg.ShowDialog();
                if (host != null)
                    host.Abort();
            }
        }
        private void Host_Faulted(object sender, EventArgs e)
        {
            NetworkStatus = false;
        }
        private void Host_Closed(object sender, EventArgs e)
        {
            NetworkStatus = false;
        }
        private void Host_Opened(object sender, EventArgs e)
        {
            NetworkStatus = true;
        }
        private void Wns_MyHostEvent(object sender, MyHostEventArgs e)
        {
            try
            {
                wallStatus = DateTime.Now.ToLongTimeString() + " - " + e.NotificationName + " : " + e.Message;

                if (!Directory.Exists(@"c:/temp/WallPanelLog/"))
                    Directory.CreateDirectory(@"c:/temp/WallPanelLog/");

                File.AppendAllText(@"c:/temp/WallPanelLog/logService.txt", wallStatus + "\r\n");

                switch (e.NotificationName)
                {
                    case "GetWallInfo":
                        break;
                    case "UpdateCriteriaNotification":
                        getCriteria();
                        break;
                    case "UpdateUserInfoListNotification":
                        break;
                    case "UpdateUserInfoList":
                        refreshUserFromServer();
                        GrantedUsersCache.Reload();
                        break;
                    case "UpdateCriteria":
                        if (e.Message != null)
                        {
                            getCriteria();
                        }
                        break;
                    case "AddOrUpdateProduct":
                        getCriteria();
                        Thread.Sleep(500);
                        break;
                    case "StockOutProduct":
                        getCriteria();
                        break;

                    case "SelectProduct":
                        if (e.Message != null)
                        {
                           
                           if (mainview0.myDatagrid.SelectedItems.Count > 0)
                                LightFilteredTag();
                            wallStatus = DateTime.Now.ToLongTimeString() + e.Message;
                        }
                        break;

                    case "PullItemsRequest":
                        //getSelection();
                        bNeedUpdateCriteriaAfterScan = true;
                        break;
                    case  "PullItemsRequestbyPut":
                        //getSelection();
                        bNeedUpdateCriteriaAfterScan = true;
                        break;
                    //Notification stop scan 
                    case "StopWallScan":
                        StopWallScan();
                        break;
                }
            }
            catch (Exception exp)
            {
                File.AppendAllText(@"c:/temp/WallPanelLog/logService.txt", exp.Message + "\r\n");
            }
        }
        #endregion
        #region Device
        private void DevicesHandler_GpioConnected(object sender, DrawerEventArgs e)
        {
            wallStatus = "Gpio connected";
            GpioStatus = DevicesHandler.GpioCardObject.IsConnected;
        }
        private async void DeviceHandler_DeviceConnected(object sender, DrawerEventArgs e)
        {
            try
            {
                wallStatus = "Rfid Connected";
                WallService.myWall.DeviceSerial = e.Serial;

                for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                {
                    DevicesHandler.DrawerStatus[loop] = DrawerStatusList.Ready;
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                    BrushDrawer[loop] = _borderReady;
                    string serialDrawer = e.Serial + "_" + loop;

                    SmartDrawerDatabase.DAL.Device _deviceEntity = null;
                    var ctx = RemoteDatabase.GetDbContext();
                    if (_deviceEntity == null)
                    {
                        _deviceEntity = DevicesHandler.GetDeviceEntity();
                        if (_deviceEntity != null)
                        {
                            if (ctx.Inventories != null)
                            {
                                var lastInventory = ctx.Inventories.Where(inv => inv.DeviceId == _deviceEntity.DeviceId && inv.DrawerNumber == loop).OrderByDescending(inv => inv.InventoryId).FirstOrDefault();
                                if (lastInventory != null)
                                {
                                    try
                                    {
                                        DevicesHandler.DrawerInventoryData[loop] = SerializeHelper.DeserializeXML<ReaderData>(lastInventory.InventoryStream);
                                    }
                                    catch
                                    {
                                        DevicesHandler.DrawerInventoryData[loop] = new ReaderData();
                                    }
                                }
                                else
                                    DevicesHandler.DrawerInventoryData[loop] = new ReaderData();

                                DevicesHandler.RemoveTagFromListForDrawer(loop);

                                DevicesHandler.AddTagListForDrawer(loop, DevicesHandler.DrawerInventoryData[loop].strListTag);
                                DevicesHandler.UpdateAddedTagToDrawer(loop, DevicesHandler.DrawerInventoryData[loop].strListTag);
                                DevicesHandler.UpdateremovedTagToDrawer(loop, DevicesHandler.DrawerInventoryData[loop].strListTag);

                                DrawerEventArgs newArg = new DrawerEventArgs(serialDrawer, loop);
                                DeviceHandler_TagRead(this, newArg);
                                // DeviceHandler_ScanCompleted(this, newArg);
                                DevicesHandler.IsDrawerWaitScan[loop] = true;
                            }
                        }
                    }
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                }
                // Load Granted User
                GrantedUsersCache.Reload();
                RfidStatus = DevicesHandler.DevicesConnected;

            }
            catch (Exception error)
            {
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in DeviceConnected");
                    exp.ShowDialog();
                }));
            }

        }
        private void DeviceHandlerOnDeviceDisconnected(object sender, DrawerEventArgs e)
        {
            try
            {
                wallStatus = "Drawer " + e.DrawerId + " disconnected";
                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.InConnection;
                DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                BrushDrawer[e.DrawerId] = Brushes.OrangeRed;
                RfidStatus = DevicesHandler.DevicesConnected;
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in OnDeviceDisconnected");
                    exp.ShowDialog();
                }));
            }
        }
        private void DevicesHandler_DrawerClosed(object sender, DrawerEventArgs e)
        {
            try
            {
               
                List<string> lstCno = DevicesHandler.GetTagFromDictionnary(_lastDrawerOpen, DevicesHandler.ListTagPerDrawer);
                ObservableCollection<string> TmpListCtrlPerDrawer = new ObservableCollection<string>(lstCno);
                ListCtrlPerDrawer = null;
                DrawerSelected = "";
                DrawerCtrlCount = "";
              


                if (string.IsNullOrEmpty(tagOnBadDrawer.TagId))
                    IsFlyoutCassetteInfoOpen = false;

                if (_lastDrawerOpen == e.DrawerId)
                    _lastDrawerOpen = -1;
                if (_currentDrawerInLight == e.DrawerId)
                    _currentDrawerInLight = -1;
                _tagToLightFromTextBox.Clear();
                if (_recheckLightDrawer == e.DrawerId)
                    _recheckLightDrawer = -1;
                if (_lightDrawer == e.DrawerId)
                    _lightDrawer = -1;
                if (_autoLightDrawer == e.DrawerId)
                    _autoLightDrawer = -1;
                wallStatus = "Drawer " + e.DrawerId + " closed";

                bool bWasStopHere = false;

                bDrawerToRefreshLight[e.DrawerId] = false;
                if ((myConTroller != null) && (myConTroller.IsOpen))
                {
                    myConTroller.CloseAsync();
                    bWasStopHere = true;
                }

                if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                {
                    if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InLight)
                    {
                        bDrawerToRefreshLight[e.DrawerId] = false;
                        DevicesHandler.StopLighting(e.DrawerId);

                        if (!bWasStopHere) //no recheck if drawer not  finish  led normally
                            _recheckLightDrawer = e.DrawerId;


                        wallStatus = "Recheck stones in drawer " + e.DrawerId;
                        DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                        DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                        BrushDrawer[e.DrawerId] = _borderReady;
                    }
                    else
                    {
                        DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                        DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                        BrushDrawer[e.DrawerId] = _borderReady;
                    }
                }
                else
                {
                    if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InLight)
                        DevicesHandler.StopLighting(e.DrawerId);
                    DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                    DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                    BrushDrawer[e.DrawerId] = _borderReady;

                    if (!string.IsNullOrEmpty(tagOnBadDrawer.TagId))
                    {

                        // Put drawer in light to avoid scan
                        DevicesHandler.DrawerStatus[tagOnBadDrawer.DrawerToLight] = DrawerStatusList.InLight;
                        DrawerStatus[tagOnBadDrawer.DrawerToLight] = DevicesHandler.DrawerStatus[tagOnBadDrawer.DrawerToLight];
                        BrushDrawer[tagOnBadDrawer.DrawerToLight] = _borderLight;
                    }
                    else
                    {
                        IsFlyoutCassetteInfoOpen = false;
                    }
                }

                if (_recheckLightDrawer == -1)
                {
                    DevicesHandler.IsDrawerWaitScan[e.DrawerId] = true;
                }
            }
            catch (Exception error)
            {               
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in DrawerOClosed");
                    exp.ShowDialog();
                }));
            }   
            
            finally
            {
                if (!ScanTimer.IsEnabled)
                {
                    ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                    ScanTimer.IsEnabled = true;
                    ScanTimer.Start();
                }
            }
        }
        private void DevicesHandler_DrawerOpened(object sender, DrawerEventArgs e)
        {
            try
            {

                // wait open drawer to let time automatic trimming (max filed for comfirmation)
                Thread.Sleep(1000);
                if (_lastDrawerOpen == -1)
                {

                    if (_recheckLightDrawer != -1)
                    {
                        DevicesHandler.StopLighting(_recheckLightDrawer);
                        DevicesHandler.StopScan(_recheckLightDrawer);
                        if ((myConTroller != null) && (myConTroller.IsOpen))
                            myConTroller.CloseAsync();
                        _recheckLightDrawer = -1;
                    }                   

                    _lastDrawerOpen = e.DrawerId;
                    wallStatus = "Drawer " + e.DrawerId + " opened";

                    

                    if (_autoLightDrawer != -1) //switch of drawer
                    {
                        if (DevicesHandler.DevicesConnected)
                            DevicesHandler.StopLighting(_autoLightDrawer);
                    }
                    IsAutoLightDrawerChecked = false;

                    List<string> lstCno = DevicesHandler.GetTagFromDictionnary(_lastDrawerOpen, DevicesHandler.ListTagPerDrawer);
                    ObservableCollection<string> TmpListCtrlPerDrawer = new ObservableCollection<string>(lstCno);
                    ListCtrlPerDrawer = new ObservableCollection<string>(TmpListCtrlPerDrawer.OrderBy(i => i));
                    DrawerSelected = "Drawer " + _lastDrawerOpen;
                    DrawerCtrlCount = " (" + ListCtrlPerDrawer.Count + ")";
                 


                    //Store event drawer 
                    Task.Factory.StartNew(() =>
                    {
                        var ctx = RemoteDatabase.GetDbContext();
                        if (GrantedUsersCache.LastAuthenticatedUser != null)
                            ctx.EventDrawerDetails.Add(new EventDrawerDetail() { DeviceId = DevicesHandler.GetDeviceEntity().DeviceId, DrawerNumber = e.DrawerId, GrantedUserId = GrantedUsersCache.LastAuthenticatedUser.GrantedUserId, InventoryId = null, EventDrawerDate = DateTime.Now });
                        else
                            ctx.EventDrawerDetails.Add(new EventDrawerDetail() { DeviceId = DevicesHandler.GetDeviceEntity().DeviceId, DrawerNumber = e.DrawerId, GrantedUserId = null, InventoryId = null, EventDrawerDate = DateTime.Now });

                        ctx.SaveChanges();
                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                    });

                    //When a search find stone on other drawer than one currently open
                    if (!string.IsNullOrEmpty(tagOnBadDrawer.TagId))
                    {
                        if (tagOnBadDrawer.DrawerToLight == e.DrawerId)
                        {
                            List<string> tags = new List<string>();
                            tags.Add(tagOnBadDrawer.TagId);

                            _lightDrawer = -1;
                            DevicesHandler.StopLighting(tagOnBadDrawer.DrawerToLight);
                            Thread.Sleep(50);
                            DevicesHandler.LightTags(tagOnBadDrawer.DrawerToLight, tags, true);
                            wallStatus = "Light in  wall (drawer " + tagOnBadDrawer.DrawerToLight + ")";
                            DevicesHandler.DrawerStatus[tagOnBadDrawer.DrawerToLight] = DrawerStatusList.InLight;
                            DrawerStatus[tagOnBadDrawer.DrawerToLight] = DevicesHandler.DrawerStatus[tagOnBadDrawer.DrawerToLight];
                            BrushDrawer[tagOnBadDrawer.DrawerToLight] = _borderLight;

                            _lightDrawer = tagOnBadDrawer.DrawerToLight;
                            bDrawerToLight[tagOnBadDrawer.DrawerToLight] = false;
                            bDrawerToRefreshLight[tagOnBadDrawer.DrawerToLight] = false;
                            tagOnBadDrawer.TagId = null;
                        }
                    }
                    else
                    {
                        if ((DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InLight)) 
                        {
                            wallStatus = "Light stones in drawer " + e.DrawerId;
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.InLight;
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                            BrushDrawer[e.DrawerId] = _borderLight;
                            bDrawerToLight[e.DrawerId] = true;
                            _lightDrawer = e.DrawerId;
                        }
                        else
                        {
                            if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InScan)
                                StopWallScan();
                            wallStatus = "Drawer " + e.DrawerId + " opened";
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Open;
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                            BrushDrawer[e.DrawerId] = _borderDrawerOpen;
                        }

                        if (IsAutoLightDrawerChecked)
                        {
                            if ((_lightDrawer == -1) && (_autoLightDrawer == -1))// No light in progress
                            {
                                _bStopWall = true;
                                _autoLightDrawer = e.DrawerId; //give Number drawer to autolight
                            }
                        }
                        else
                            _autoLightDrawer = -1;
                    }
                }
                else
                {
                    if (_lastDrawerOpen != e.DrawerId)
                    {
                        DevicesHandler.IsDrawerWaitScan[e.DrawerId] = true;
                        if ((DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InLight))
                        {
                            wallStatus = "Light stones in drawer " + e.DrawerId;
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.InLight;
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                            BrushDrawer[e.DrawerId] = _borderLight;
                            bDrawerToLight[e.DrawerId] = true;
                            _lightDrawer = e.DrawerId;
                        }
                        else if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.Ready)
                        {
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Open;
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                            BrushDrawer[e.DrawerId] = _borderDrawerOpen;
                        }
                        else
                        {
                            if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InScan)
                                StopWallScan();
                            wallStatus = "Drawer " + e.DrawerId + " opened";
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Open;
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                            BrushDrawer[e.DrawerId] = _borderDrawerOpen;
                        }

                        
                        mainview0.Dispatcher.BeginInvoke(new System.Action(async () =>
                        {
                            string info = string.Format("Drawer {0} already open - please Close drawer {1} to continue", _lastDrawerOpen, e.DrawerId);
                            await mainview0.ShowMessageAsync("Wall Information", info);
                        }));
                    }
                }

            }
            catch (Exception error)
            {            
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in DrawerOpened");
                    exp.ShowDialog();
                }));
            }
            finally
            {
                if (!ScanTimer.IsEnabled)
                {
                    ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
                    ScanTimer.IsEnabled = true;
                    ScanTimer.Start();
                }
            }

        }
        private void DeviceHandler_ScanStarted(object sender, DrawerEventArgs e)
        {
            try
            {
                _autoLockCpt = 120;
                wallStatus = "Drawer " + e.DrawerId + " scan started";
                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.InScan;
                DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                BrushDrawer[e.DrawerId] = _borderInScan;
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in ScanStarted");
                    exp.ShowDialog();
                }));
            }
        }
        private void DevicesHandler_FailStartSscan(object sender, DrawerEventArgs e)
        {
            try
            {
                OverallStatus = false;
                wallStatus = "Drawer " + e.DrawerId + " failed scan started";
                if (DevicesHandler.DrawerStatus[e.DrawerId] != DrawerStatusList.Open)
                {
                    DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.NotReady;
                    DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                }
                BrushDrawer[e.DrawerId] = _borderDrawerOpen;
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in FailStartSscan");
                    exp.ShowDialog();
                }));
            }
        }
        private void DeviceHandler_ScanCompleted(object sender, DrawerEventArgs e)
        {
            wallStatus = "Drawer " + e.DrawerId + " scan completed";           
            TimeSpan ts = DateTime.Now - DevicesHandler.LastScanTime;
            LastScanInfo = string.Format("Last scan : {0} min ago", (int)ts.TotalMinutes);
          
            //Store Inventory
            // - Get Tag from Db to synchronize
            try
            {

                Task.Run(() =>
               {
                   InventoryHandler.HandleNewScanCompleted(e.DrawerId);                 

               });
                DevicesHandler.IsDrawerWaitScan[e.DrawerId] = false;
                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                BrushDrawer[e.DrawerId] = _borderReady;
                CountTotalStones();
                bNeedUpdateCriteriaAfterScan = true;

            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Scan completed");
                    exp.ShowDialog();
                }));
            }
        }
        private void DeviceHandler_ScanCancelledByHost(object sender, DrawerEventArgs e)
        {
            try
            {
                wallStatus = "Drawer " + e.DrawerId + " scan cancelled";
                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                BrushDrawer[e.DrawerId] = _borderReady;
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in ScanCancelledByHost");
                    exp.ShowDialog();
                }));
            }
        }
        private void DeviceHandler_TagRead(object sender, DrawerEventArgs e)
        {
            try
            {
                if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InScan)
                    DevicesHandler.DrawerTagQty[e.DrawerId] = DevicesHandler.Device.ReaderData.nbTagScan;
                else
                    DevicesHandler.DrawerTagQty[e.DrawerId] = DevicesHandler.DrawerInventoryData[e.DrawerId].nbTagScan;

                DrawerTagQty[e.DrawerId] = DevicesHandler.DrawerTagQty[e.DrawerId].ToString();
                CountTotalStones();
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in TagRead");
                    exp.ShowDialog();
                }));
            }
        }
        public async Task DoConnect()
        {
            await Task.Run(() => DevicesHandler.TryInitializeLocalDeviceAsync());
        }
        private bool IsOneDrawerOpen()
        {
            bool bRet = false;
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.Open)
                    bRet = true;
            }
            return bRet;
        }
        private bool IsWallReady()
        {
            if (_InLightProcess) return false;
            bool bRet = true;
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.DrawerStatus[loop] != DrawerStatusList.Ready)
                    bRet = false;
            }
            return bRet;
        }
        private bool IsWaitingForScan()
        {
            bool bRet = false;
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.IsDrawerWaitScan[loop])
                    bRet = true;
            }
            return bRet;
        }
        private bool IsWallInScan()
        {

            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.InScan)
                    return true;
            }
            return false;
        }
        private void StopWallScan()
        {
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.InScan)
                {
                    DevicesHandler.StopScan(loop);
                    DevicesHandler.IsDrawerWaitScan[loop] = true;
                    DevicesHandler.DrawerStatus[loop] = DrawerStatusList.Ready;
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                    BrushDrawer[loop] = _borderReady;
                    DrawerTagQty[loop] = DevicesHandler.GetTagFromDictionnary(1, DevicesHandler.ListTagPerPreviousDrawer).Count.ToString();
                }
            }
        }
        private void cancelLighting(bool bUpdateTreeview = true)
        {
            _InLightProcess = false;
            _autoLockCpt = 120;

            if (!GpioStatus)
                return;

            _lightDrawer = -1;
            _currentDrawerInLight = -1;
            _tagToLightFromTextBox.Clear();
            Thread.Sleep(100);
            for (int loop = 1; loop <= 6; loop++)
            {
                bDrawerToLight[loop] = false;
                bDrawerToRefreshLight[loop] = false;
            }
            if ((DevicesHandler.Device != null) && (DevicesHandler.Device.IsConnected))
            {
                DevicesHandler.Device.StopField();
            }

            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                // if ((DevicesHandler.DrawerStatus[loop] == DrawerStatusList.Open) || (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.InLight))
                if (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.InLight)
                {
                    DevicesHandler.StopLighting(loop);
                    DevicesHandler.DrawerStatus[loop] = DrawerStatusList.Ready;
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                    BrushDrawer[loop] = _borderReady;
                }
                if (_lastDrawerOpen == loop)
                {
                    DevicesHandler.DrawerStatus[loop] = DrawerStatusList.Open;
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                    BrushDrawer[loop] = _borderDrawerOpen;
                }
            }
            IsAutoLightDrawerChecked = bWasInAutoLight;
        }
        private void LightSelection()
        {
            try
            {
                _autoLockCpt = 120;
                for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                {
                    bDrawerToLight[loop] = false;
                    bDrawerToRefreshLight[loop] = false;
                }
                _lightDrawer = -1;
                if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                {
                    for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                    {
                        if ((SelectedCassette.TagToLight[loop] != null) && (SelectedCassette.TagToLight[loop].Count > 0))
                        {
                            DevicesHandler.DrawerStatus[loop] = DrawerStatusList.InLight;
                            DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                            BrushDrawer[loop] = _borderDrawerOpen;
                            bDrawerToLight[loop] = true;
                            if (_lastDrawerOpen == loop) // drawer already open run lighting
                            {
                                _lightDrawer = loop;
                                wallStatus = "Light stone(s) in drawer " + loop;
                            }
                            else
                                wallStatus = "Wait user Action";
                        }
                    }

                    IsFlyoutCassetteInfoOpen = false;
                }
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in LightSelection");
                    exp.ShowDialog();
                }));
            }
        }
        private void LightSelectionDrawer(List<string> TagToLight, int drawerId)
        {
            try
            {
                DevicesHandler.DrawerStatus[drawerId] = DrawerStatusList.InLight;
                DrawerStatus[drawerId] = DevicesHandler.DrawerStatus[drawerId];
                BrushDrawer[drawerId] = _borderLight;
                DevicesHandler.LightTags(drawerId, TagToLight, true);

            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in LightSelectionDrawer");
                    exp.ShowDialog();
                }));
            }
        }
        #endregion
        #region Function
        private void InitValue()
        {          

            DevicesHandler.DeviceConnected += DeviceHandler_DeviceConnected;
            DevicesHandler.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
            DevicesHandler.TagRead += DeviceHandler_TagRead;
            DevicesHandler.ScanStarted += DeviceHandler_ScanStarted;
            DevicesHandler.ScanCompleted += DeviceHandler_ScanCompleted;
            DevicesHandler.ScanCancelledByHost += DeviceHandler_ScanCancelledByHost;
            DevicesHandler.FailStartSscan += DevicesHandler_FailStartSscan;
            DevicesHandler.DrawerOpened += DevicesHandler_DrawerOpened;
            DevicesHandler.DrawerClosed += DevicesHandler_DrawerClosed;
            DevicesHandler.GpioConnected += DevicesHandler_GpioConnected;
            DevicesHandler.FpAuthenticationReceive += DevicesHandler_FpAuthenticationReceive;
         

            AutoConnectTimer = new DispatcherTimer();
            AutoConnectTimer.Tick += new EventHandler(AutoConnectTimer_Tick);
            AutoConnectTimer.Interval = new TimeSpan(0, 0, 30);
            AutoConnectTimer.Start();

            AutoLockTimer = new DispatcherTimer();
            AutoLockTimer.Tick += new EventHandler(AutoLockTimer_Tick);
            AutoLockTimer.Interval = new TimeSpan(0, 0, 1);
            AutoLockTimer.Start();

            DevicesHandler.TryInitializeLocalDeviceAsync();
            InitWcfService();

            ScanTimer = new DispatcherTimer();
            ScanTimer.Tick += new EventHandler(ScanTimer_Tick);
            ScanTimer.Interval = new TimeSpan(0, 0, 5);
            ScanTimer.Start();
        }
        private void CountTotalStones()
        {
            WallTotalStones = 0;
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                WallTotalStones += DevicesHandler.DrawerTagQty[loop];
            }
        }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {

            //reset properties
           // Properties.Settings.Default.Reset();

            // add custom accent and theme resource dictionaries to the ThemeManager
            // you should replace MahAppsMetroThemesSample with your application name
            // and correct place where your custom accent lives
            ThemeManager.AddAccent("CustomAccent1", new Uri("pack://application:,,,/SmartDrawerWpfApp;component/Stylesheet/CustomAccent.xaml"));

            // get the current app style (theme and accent) from the application
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);

            // now change app style to the custom accent and current theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent("CustomAccent1"),
                                        theme.Item1);

            //Get handle of main window
            mainview0 = System.Windows.Application.Current.Windows.OfType<MainWindow2>().FirstOrDefault();
            mainview0.Loaded += Mainview0_Loaded;
            mainview0.NotifyBadgeReaderEvent += Mainview0_NotifyBadgeReaderEvent;
            mainview0.NotifyM2MCardEvent += Mainview0_NotifyM2MCardEvent;
            mainview0.theModel = this;

            #region Initialize command
            ResetDeviceCommand = new RelayCommand(() => Reset(true));
            btSettingCommand = new RelayCommand(() => Settings());
            btLightFilteredTag = new RelayCommand(() => LightFilteredTag());
            LightAllCommand = new RelayCommand(() => LightAll());
            openKeyboard = new RelayCommand(() => openKeyboardFn());
            searchTxtGotCR = new RelayCommand(() => searchTxtGotCRfn());
            btClearSelection = new RelayCommand(() => ClearSelection());
            LogoutCommand = new RelayCommand(() => logout());
            btLightFilteredTagSelection = new RelayCommand(() => LightSelectionFromList());
            btRemoveSelection = new RelayCommand(() => removeSelection());
            BtRefreshSelection = new RelayCommand(() => getSelection());
            BtLighAllPerDrawer = new RelayCommand(() => BtLighAllPerDrawerFn());
            BtLighListPerDrawer = new RelayCommand(() => BtLighListPerDrawerFn());
            BtRemoveCardSelection = new RelayCommand(() => DeleteCard());

            btSaveDevice = new RelayCommand(() => ReloadDevice());

            /**** admin ***/
            btSaveUser = new RelayCommand(() => SaveUser());
            btResetUser = new RelayCommand(() => ResetUser());
            btDeleteUser = new RelayCommand(() => DeleteUser());
            btEnrollUser = new RelayCommand(() => EnrollUser());

            #endregion
        }
        private void Mainview0_NotifyM2MCardEvent(object sender, string CardID)
        {
            txtSearchCtrl = CardID;
            searchTxtGotCRfn();
        }
        private void Mainview0_NotifyBadgeReaderEvent(object sender, string badgeID)
        {            
            LastScanInfo = "Badge: " + badgeID; 
            AutoConnectTimer.Stop();
            AutoConnectTimer.Start();
            foreach (var user in GrantedUsersCache.Cache)
            {
                if (badgeID == user.BadgeNumber)
                {
                    
                    wallStatus = "Drawer Unlock : Hello " + user.FirstName + " " + user.LastName;
                    LoggedUser = user.Login;
                    btUserVisibility = Visibility.Visible;
                    var ctx = RemoteDatabase.GetDbContext();
                    DevicesHandler.LastScanAccessTypeName = AccessType.Badge;
                    ctx.Authentications.Add(new Authentication { GrantedUserId = user.GrantedUserId, DeviceId = DevicesHandler.GetDeviceEntity().DeviceId, AuthentificationDate = DateTime.Now });
                    ctx.SaveChanges();
                    GrantedUsersCache.LastAuthenticatedUser = user;
                    DevicesHandler.UnlockWall();
                    AutoLockMsg = "Logout";
                    bLatchUnlocked = true;
                    _autoLockCpt = 120;
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                    return;
                }
            }
            // If here user not found
            GrantedUsersCache.Reload();
            //Try redo with updated info
            foreach (var user in GrantedUsersCache.Cache)
            {
                if (badgeID == user.BadgeNumber)
                {
                    wallStatus = "Drawer Unlock : Hello " + user.FirstName + " " + user.LastName;
                    LoggedUser = user.Login;
                    btUserVisibility = Visibility.Visible;
                    var ctx = RemoteDatabase.GetDbContext();
                    DevicesHandler.LastScanAccessTypeName = AccessType.Badge;
                    ctx.Authentications.Add(new Authentication { GrantedUserId = user.GrantedUserId, DeviceId = DevicesHandler.GetDeviceEntity().DeviceId, AuthentificationDate = DateTime.Now });
                    ctx.SaveChanges();
                    GrantedUsersCache.LastAuthenticatedUser = user;
                    DevicesHandler.UnlockWall();
                    AutoLockMsg = "Logout";
                    bLatchUnlocked = true;
                    _autoLockCpt = 120;
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                    return;
                }
            }
            wallStatus = "No Granted User with badge " + badgeID;
        }
        private void DevicesHandler_FpAuthenticationReceive(object sender, SecurityModules.FingerprintReader.FingerprintReaderEventArgs args)
        {
            var fpReader = sender as FingerprintReader;
            if (fpReader == null)
            {
                // cannot happen
                return;
            }
            /* if (args.EventType != FingerprintReaderEventArgs.EventTypeValue.FPReaderReadingComplete)
             {
                 return;
             }*/
            switch (args.EventType)
            {
                case FingerprintReaderEventArgs.EventTypeValue.FPReaderReadingComplete:
                    foreach (var user in GrantedUsersCache.Cache)
                    {
                        foreach (var fp in user.Fingerprints)
                        {
                            if (fpReader.DoesTemplateMatch(fp.Template))
                            {
                                wallStatus = "Drawer Unlock : Hello " + user.FirstName + " " + user.LastName;
                                LoggedUser = user.Login;
                                btUserVisibility = Visibility.Visible;
                                var ctx = RemoteDatabase.GetDbContext();
                                DevicesHandler.LastScanAccessTypeName = AccessType.Fingerprint;
                                ctx.Authentications.Add(new Authentication { GrantedUserId = user.GrantedUserId, DeviceId = DevicesHandler.GetDeviceEntity().DeviceId, AuthentificationDate = DateTime.Now });
                                ctx.SaveChanges();
                                GrantedUsersCache.LastAuthenticatedUser = user;
                                DevicesHandler.UnlockWall();
                                AutoLockMsg = "Logout";
                                bLatchUnlocked = true;
                                _autoLockCpt = 120;
                                ctx.Database.Connection.Close();
                                ctx.Dispose();
                                return;
                            }
                        }
                    }
                    // If here user not found
                    GrantedUsersCache.Reload();
                    //Try redo with updated info
                    foreach (var user in GrantedUsersCache.Cache)
                    {
                        foreach (var fp in user.Fingerprints)
                        {
                            if (fpReader.DoesTemplateMatch(fp.Template))
                            {
                                wallStatus = "Drawer Unlock : Hello " + user.FirstName + " " + user.LastName;
                                LoggedUser = user.Login;
                                btUserVisibility = Visibility.Visible;
                                var ctx = RemoteDatabase.GetDbContext();
                                DevicesHandler.LastScanAccessTypeName = AccessType.Fingerprint;
                                ctx.Authentications.Add(new Authentication { GrantedUserId = user.GrantedUserId, DeviceId = DevicesHandler.GetDeviceEntity().DeviceId, AuthentificationDate = DateTime.Now });
                                ctx.SaveChanges();
                                GrantedUsersCache.LastAuthenticatedUser = user;
                                DevicesHandler.UnlockWall();
                                AutoLockMsg = "Logout";
                                bLatchUnlocked = true;
                                _autoLockCpt = 120;
                                ctx.Database.Connection.Close();
                                ctx.Dispose();
                                return;
                            }
                        }
                    }
                    wallStatus = "No Granted User with this fingerprint ";
                    break;
                default:
                    wallStatus = args.EventType.ToString();
                    break;
            }
        }
        ~MainViewModel()
        {

        }
        private void Mainview0_Loaded(object sender, RoutedEventArgs e)
        {
            btUserVisibility = Visibility.Collapsed;
            btAdminVisibility = Visibility.Hidden;
            isAdmin = false;

            NetworkStatus = false;
            GpioStatus = false;
            RfidStatus = false;

            dayDate = DateTime.Now.ToString("dddd, dd");
            dayTime = DateTime.Now.ToString("hh:mm tt");
            WallStatusOperational = "INITIALISATION";

            wallStatus = "In Connection";
            btLightText = "Light All";
            txtNbSelectedItem = "Stones Selected : 0";
            AutoLockMsg = "Wait User";

            DrawerStatus = new ObservableCollection<string>();
            DrawerStatus.Add("Index0");
            DrawerTagQty = new ObservableCollection<string>();
            DrawerTagQty.Add("index0");
            BrushDrawer = new ObservableCollection<Brush>();
            BrushDrawer.Add(Brushes.White);

            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                DevicesHandler.DrawerStatus[loop] = "In Connection";
                DrawerStatus.Add(DevicesHandler.DrawerStatus[loop]);

                DevicesHandler.DrawerTagQty[loop] = 0;
                DrawerTagQty.Add(DevicesHandler.DrawerTagQty[loop].ToString());
                BrushDrawer.Add(_borderNotReady);
            }

            startTimer = new DispatcherTimer();
            startTimer.Interval = new TimeSpan(0, 0, 1);
            startTimer.Tick += StartTimer_Tick;
            startTimer.IsEnabled = true;
            startTimer.Start();
        }

        #endregion
    }
    public class DateTimeToShortDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime)
            {
                return ((DateTime)value).ToShortDateString();
            }
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                return DateTime.Parse(value.ToString());
            }
            return value;
        }
    }
}