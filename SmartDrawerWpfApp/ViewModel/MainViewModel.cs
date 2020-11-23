//#define IsTiffany
#define UseInfinity
//#define SendUnrefTag  for mix tag in device comment it

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
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Configuration;
using SmartDrawerWpfApp.InfinityService;
using Quobject.EngineIoClientDotNet.Client.Transports;
using Quobject.SocketIoClientDotNet.Client;
using SmartDrawerWpfApp.InfinityServiceForReact;


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
        Brush _borderScanPending = Brushes.Cyan;

        Brush _selectionBrush = new SolidColorBrush(Color.FromRgb(0x33, 0xBC, 0xBA));
        #endregion
        #region Variables
        private MainWindow2 mainview0;
        public PleaseWaitWindow cob = null;

        //private ProgressDialogController myConTroller = null;
        private readonly TouchKeyboardProvider _touchKeyboardProvider = new TouchKeyboardProvider();

        private DispatcherTimer startTimer;
        private DispatcherTimer AutoConnectTimer;
        private DispatcherTimer AutoLockTimer;
        private DispatcherTimer ScanTimer;
        private DispatcherTimer SelectionLifeTimeTimer;
        private DispatcherTimer InfinityTimer;


        private string InfinityServerUrl = null;
        private string InfinityServerToken;

        public string ReactInfinityUrl = null;
        public string ReactDeviceUser;
        public string ReactDevicePassword;
        public string ReactToken;


        private int AutoScanTimerValue = 0;
        private int DoorOpenTooLongTimerValue = 180;
        public bool IsClosingBusiness = false;
        public bool IsOpeningBusiness = false;
        public bool IsClosingCompleted = false;
        private bool bFirstScan = false;

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

        private List<ItemDetail> _SelectedBaseObjects;

        private DateTime LastDeviceActionTime;

        #endregion
        #region Properties

        private bool _ReadDft = false;
        public bool ReadDft
        {
            get
            {
                _ReadDft = Properties.Settings.Default.bReadDft;
                return _ReadDft;
            }
            set
            {
                _ReadDft = value;
                Properties.Settings.Default.bReadDft = _ReadDft;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
                RaisePropertyChanged(() => ReadDft);
            }
        }
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
                    Properties.Settings.Default.Reload();
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
                    // Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
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
                    //Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
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

        bool _ConfFailure = false;
        public bool ConfFailure
        {
            get { return _ConfFailure; }
            set
            {
                if (_ConfFailure != value)
                {
                    _ConfFailure = value;
                    RaisePropertyChanged("ConfFailure");
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

        bool _RfidError;
        public bool RfidError
        {
            get { return _RfidError; }
            set
            {
                _RfidError = value;
                RaisePropertyChanged("RfidError");
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

        string _txtBarcode;
        public string txtBarcode
        {
            get { return _txtBarcode; }
            set
            {
                _txtBarcode = value;
                if (_txtBarcode.Length == 0)
                {
                    cancelLighting();
                    SelectionSelected = null;
                    SelectedCassette = null;

                    txtStatus = string.Empty;
                    txtRefNumber = string.Empty;
                    txtTagId = string.Empty;
                    txtDrawer = string.Empty;
                    txtDevice = string.Empty;
                    txtLastDate = string.Empty;

                    SelectionLifeTimeTimer.IsEnabled = false;
                    SelectionLifeTimeTimer.Stop();
                }
                else
                {
                    SelectionLifeTimeTimer.IsEnabled = true;
                    SelectionLifeTimeTimer.Stop();
                    SelectionLifeTimeTimer.Start();
                }

                RaisePropertyChanged("txtBarcode");
            }
        }

        string _txtStatus;
        public string txtStatus
        {
            get { return _txtStatus; }
            set
            {
                _txtStatus = value; 
                RaisePropertyChanged("txtStatus");
            }
        }

        string _txtRefNumber;
        public string txtRefNumber
        {
            get { return _txtRefNumber; }
            set
            {
                _txtRefNumber = value;
                RaisePropertyChanged("txtRefNumber");
            }
        }   

        string _txtTagId;
        public string txtTagId
        {
            get { return _txtTagId; }
            set
            {
                _txtTagId = value;
                RaisePropertyChanged("txtTagId");
            }
        }

        string _txtDevice;
        public string txtDevice
        {
            get { return _txtDevice; }
            set
            {
                _txtDevice = value;
                RaisePropertyChanged("txtDevice");
            }
        }

        string _txtDrawer;
        public string txtDrawer
        {
            get { return _txtDrawer; }
            set
            {
                _txtDrawer = value;
                RaisePropertyChanged("txtDrawer");
            }
        }

        string _txtLastDate;
        public string txtLastDate
        {
            get { return _txtLastDate; }
            set
            {
                _txtLastDate = value;
                RaisePropertyChanged("txtLastDate");
            }
        }

        
        string _descDrawer1;
        public string descDrawer1
        {
            get { return _descDrawer1; }
            set
            {
                _descDrawer1 = value;
                RaisePropertyChanged("descDrawer1");
            }
        }
        string _descDrawer2;
        public string descDrawer2
        {
            get { return _descDrawer2; }
            set
            {
                _descDrawer2 = value;
                RaisePropertyChanged("descDrawer2");
            }
        }
        string _descDrawer3;
        public string descDrawer3
        {
            get { return _descDrawer3; }
            set
            {
                _descDrawer3 = value;
                RaisePropertyChanged("descDrawer3");
            }
        }
        string _descDrawer4;
        public string descDrawer4
        {
            get { return _descDrawer4; }
            set
            {
                _descDrawer4 = value;
                RaisePropertyChanged("descDrawer4");
            }
        }
        string _descDrawer5;
        public string descDrawer5
        {
            get { return _descDrawer5; }
            set
            {
                _descDrawer5 = value;
                RaisePropertyChanged("descDrawer5");
            }
        }
        string _descDrawer6;
        public string descDrawer6
        {
            get { return _descDrawer6; }
            set
            {
                _descDrawer6 = value;
                RaisePropertyChanged("descDrawer6");
            }
        }
        string _descDrawer7;
        public string descDrawer7
        {
            get { return _descDrawer7; }
            set
            {
                _descDrawer7 = value;
                RaisePropertyChanged("descDrawer7");
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
                    SelectionSelected = null;
                    SelectedCassette = null;
                    bNeedUpdateCriteriaAfterScan = true;                   
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

                try
                {
                    mainview0.myDatagrid.SearchHelper.AllowFiltering = true;
                    mainview0.myDatagrid.AllowSorting = false;
                    mainview0.myDatagrid.SearchHelper.ClearSearch();             
                    mainview0.myDatagrid.SearchHelper.Search(_txtSearchCtrl);
                    //mainview0.myDatagrid.SearchHelper.FindNext(_txtSearchCtrl);
                    mainview0.myDatagrid.SearchHelper.SearchHighlightBrush = _selectionBrush;
                    mainview0.myDatagrid.SearchHelper.SearchBrush = _selectionBrush;
                }
                catch (Exception e)
               {
                    string txt = e.Message;
                }
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

        private bool _IsInPutItemFastMode = false;
        public bool IsInPutItemFastMode
        {
            get {  return _IsInPutItemFastMode; }
            set {
                _IsInPutItemFastMode = value;
                LastDeviceActionTime = DateTime.Now;
                if (_IsInPutItemFastMode)
                {                  
                    if (IsWallInScan())                   
                        StopWallScan();
                    if (!IsWallReady())
                        cancelLighting();
                    DevicesHandler.IsInAccumulateMode = true;
                }
                else
                {
                    DevicesHandler.IsInAccumulateMode = false;
                }

                RaisePropertyChanged(() => IsInPutItemFastMode);
               }
        }

        private bool PendingFastModeOperation = false;

        private bool FastModeContinuousReading = false;

        #endregion
        #region Config
        private bool getInfinityUrl()
        {
            try
            {
                using (var ctx = RemoteDatabase.GetDbContext())
                {
                    InfinityServerUrl = ctx.Configurations.Where(c => c.Parameter == "RemoteServerUrl").SingleOrDefault().Value;
                    InfinityServerToken = ctx.Configurations.Where(c => c.Parameter == "Token").SingleOrDefault().Value;
                    ReactInfinityUrl = ctx.Configurations.Where(c => c.Parameter == "ReactServerURL").SingleOrDefault().Value;
                    ReactDeviceUser = ctx.Configurations.Where(c => c.Parameter == "ReactDeviceUser").SingleOrDefault().Value;
                    ReactDevicePassword = ctx.Configurations.Where(c => c.Parameter == "ReactDevicePassword").SingleOrDefault().Value;
                    return true;
                }

            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExceptionMessageBox msg = new ExceptionMessageBox(ex, "Error getting Configuration ");
                    msg.Show();
                    return false;
                });
            }
            return false;
        }
        private void getScanTimerValue()
        {
            using (var ctx = RemoteDatabase.GetDbContext())
            {
                string val = ctx.Configurations.Where(c => c.Parameter == "AutoScanTime").SingleOrDefault().Value;
                int.TryParse(val, out AutoScanTimerValue);

                string val2 = ctx.Configurations.Where(c => c.Parameter == "DoorOpenTooLong").SingleOrDefault().Value;
                int.TryParse(val2, out DoorOpenTooLongTimerValue);
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
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate Users");
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
                        ctx.GrantedAccesses.AddOrUpdateAccess(original, DevicesHandler.GetDeviceEntity(), ctx.GrantTypes.All());
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

                            var original = ctx.GrantedUsers.Add(newUser);
                            ctx.SaveChanges();
                            ctx.GrantedAccesses.AddOrUpdateAccess(original, DevicesHandler.GetDeviceEntity(), ctx.GrantTypes.All());
                            ctx.SaveChanges();
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
                            var original = ctx.GrantedUsers.Add(newUser);
                            ctx.SaveChanges();
                            ctx.GrantedAccesses.AddOrUpdateAccess(original, DevicesHandler.GetDeviceEntity(), ctx.GrantTypes.All());
                            ctx.SaveChanges();
                        }
                        

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
            GrantedUsersCache.Reload();
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
        #region Info window
        private void CreateProcessWindow()
        {

            if (cob != null)
                cob.Close();
            cob = new PleaseWaitWindow();   
        }
        private void ShowProcessWindow(string title)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (cob != null)
                {
                    cob.setText(title);
                    if (!cob.IsVisible)
                        cob.Show();
                }

                cob.Activate();
                cob.Topmost = true;
                cob.Topmost = false;
                cob.Focus();
            });
        }
        private void HideProcessWindow()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (cob != null)
                    cob.Hide();
            });
        }
        #endregion
        #region Datagrid


        private ObservableCollection<ItemDetail> _TagDetails;
        public ObservableCollection<ItemDetail> TagDetails
        {
            get { return _TagDetails; }
            set
            {
                _TagDetails = value;
                RaisePropertyChanged(() => TagDetails);
            }
        }
        private ItemDetail _ItemSelected;
        public ItemDetail ItemSelected
        {
            get { return _ItemSelected; }
            set
            {
                _ItemSelected = value;
                if (_ItemSelected != null)
                {
                    foreach (var item in TagDetails)
                    {
                        if (item.TagId == _ItemSelected.TagId)
                        {
                            item.IsSelected = !item.IsSelected;
                            break;
                        }
                    }

                    //Refresh selection counter
                    int nbtag = TagDetails.Where(item => item.IsSelected == true).ToList().Count();
                    txtNbSelectedItem = string.Format("Stone(s) Selected : {0}", nbtag);

                }

                RaisePropertyChanged(() => ItemSelected);
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
                    txtNbSelectedItem = string.Format("Stone(s) Selected : {0}", _selectedItems.Count());
                }
                else
                {

                    /*if (mainview0.myDatagrid.View != null)
                        txtNbSelectedItem = string.Format("Stones Selected : {0}", mainview0.myDatagrid.View.Records.Count());*/
                    txtNbSelectedItem = string.Format("Stone(s) Selected : {0}", 0);
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


        public async void getStoneData()
        {
            //var myConTroller = await mainview0.ShowProgressAsync("Please wait", "Retrieving information from Database",true);
            WaitHandler wh = null;
            try
            {
                wh = new WaitHandler();
                wh.Msg = "Retrieving information from Database";
                wh.Start();

                //myConTroller.SetIndeterminate();
                await Task.Run(() =>
                {
                    lock (thisLock)
                    {
                        using (var ctx = RemoteDatabase.GetDbContext())
                        {
                            int nbCol = ctx.Columns.Count();
                            List<ItemDetail> lstDetails = new List<ItemDetail>();
                            if (nbCol > 1)
                            {
                               
                                UnknownTags = new Dictionary<string, int>();
                                List<string> lstUnreferencedTag = new List<string>();

                                Dictionary<string, int> CopyDic = new Dictionary<string, int>(DevicesHandler.ListTagPerDrawer);
                                if (CopyDic != null)
                                    foreach (KeyValuePair<string, int> entry in CopyDic)
                                    {
                                        Product p = ctx.Products.Where(po => po.RfidTag.TagUid == entry.Key).Include(po => po.RfidTag).FirstOrDefault();
                                        if (p != null)
                                        {
                                            if (p.ProductInfo0 != "Unreferenced")
                                            {
                                                ItemDetail id = new ItemDetail()
                                                {
                                                    IsEnabled = Visibility.Visible,
                                                    TagId = p.RfidTag.TagUid,
                                                    DrawerId = entry.Value,
                                                    valColumn1 = p.ProductInfo0,
                                                    valColumn2 = p.ProductInfo1,
                                                    valColumn3 = p.ProductInfo2,
                                                    valColumn4 = p.ProductInfo3,
                                                    valColumn5 = p.ProductInfo4,
                                                    valColumn6 = entry.Value.ToString(),
                                                    valColumn7 = p.ProductInfo6,
                                                    valColumn8 = p.ProductInfo7,
                                                    valColumn9 = p.ProductInfo8,
                                                };
                                                lstDetails.Add(id);
                                            }
                                            else
                                            {
                                                UnknownTags.Add(entry.Key, entry.Value);
                                            }
                                        }
                                        else
                                        {
                                            UnknownTags.Add(entry.Key, entry.Value);
                                        }
                                    }
                                // try to get Uknown stones
                                if (UnknownTags.Count > 0)
                                {

                                    if (NetworkStatus)
                                    {
                                        List<string> TagListToSearch = new List<string>(this.UnknownTags.Keys);
                                        if (!string.IsNullOrEmpty(InfinityServerUrl))
                                        {
                                            var result = InfinityServiceHandler.GetStonesByList(InfinityServerUrl, InfinityServerToken, WallSerial, TagListToSearch);
                                            if (result.Result)
                                            {
                                                foreach (KeyValuePair<string, int> Tag in UnknownTags)
                                                {
                                                    StoneInfo si = GetStoneInfo(Tag.Key);
                                                    if (si != null)
                                                    {
                                                        ItemDetail id = new ItemDetail()
                                                        {
                                                            IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                                            TagId = Tag.Key,
                                                            DrawerId = Tag.Value,
                                                            ItemEventType = Tag.Value,
                                                            valColumn1 = si.report_number,
                                                            valColumn2 = si.carat_weight,
                                                            valColumn3 = si.shape,
                                                            valColumn4 = si.color,
                                                            valColumn5 = si.clarity,
                                                            valColumn6 = Tag.Value.ToString(),
                                                            valColumn7 = null,
                                                            valColumn8 = null,
                                                            valColumn9 = null,
                                                        };
                                                        lstDetails.Add(id);
                                                    }
                                                    else //pierre non trouvée - cree une unreferenced
                                                    {
                                                        ItemDetail id = new ItemDetail()
                                                        {
                                                            IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                                            TagId = Tag.Key,
                                                            DrawerId = Tag.Value,
                                                            ItemEventType = Tag.Value,
                                                            valColumn1 = "Unreferenced",
                                                            valColumn2 = null,
                                                            valColumn3 = null,
                                                            valColumn4 = null,
                                                            valColumn5 = null,
                                                            valColumn6 = Tag.Value.ToString(),
                                                            valColumn7 = null,
                                                            valColumn8 = null,
                                                            valColumn9 = null,
                                                        };
                                                        lstDetails.Add(id);
                                                        lstUnreferencedTag.Add(Tag.Key);
                                                    }
                                                }
                                            }
                                            else //error from serveur
                                            {
                                                foreach (KeyValuePair<string, int> Tag in UnknownTags)
                                                {
                                                    ItemDetail id = new ItemDetail()
                                                    {
                                                        IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                                        TagId = Tag.Key,
                                                        DrawerId = Tag.Value,
                                                        ItemEventType = Tag.Value,
                                                        valColumn1 = "Unreferenced",
                                                        valColumn2 = null,
                                                        valColumn3 = null,
                                                        valColumn4 = null,
                                                        valColumn5 = null,
                                                        valColumn6 = Tag.Value.ToString(),
                                                        valColumn7 = null,
                                                        valColumn8 = null,
                                                        valColumn9 = null,
                                                    };
                                                    lstDetails.Add(id);
                                                    if (Tag.Value == 1) //notifie que les ajouts
                                                        lstUnreferencedTag.Add(Tag.Key);
                                                }
                                            }
                                        }
                                        if (!string.IsNullOrEmpty(ReactInfinityUrl)) //Do for react
                                        {
                                            var result = InfinityServiceForReactHandler.GetSku(ReactInfinityUrl, ReactToken, TagListToSearch);
                                            if (result.Result)
                                            {
                                                foreach (KeyValuePair<string, int> Tag in UnknownTags)
                                                {
                                                    SkuData sd = GetStoneInfoForReact(Tag.Key);
                                                    if (sd != null)
                                                    {
                                                        ItemDetail id = new ItemDetail()
                                                        {
                                                            IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                                            TagId = Tag.Key,
                                                            DrawerId = Tag.Value,
                                                            ItemEventType = Tag.Value,
                                                            valColumn1 = sd.clientRefId,
                                                            valColumn2 = sd.weight.ToString("0.00"),
                                                            valColumn3 = sd.shape,
                                                            valColumn4 = sd.colorCategory,
                                                            valColumn5 = sd.clarity,
                                                            valColumn6 = Tag.Value.ToString(),
                                                            valColumn7 = sd._id,
                                                            valColumn8 = sd.colorType,
                                                            valColumn9 = sd.infinityRefId,
                                                        };
                                                        lstDetails.Add(id);
                                                    }
                                                    else //pierre non trouvé - cree une unreferenced
                                                    {
                                                        ItemDetail id = new ItemDetail()
                                                        {
                                                            IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                                            TagId = Tag.Key,
                                                            DrawerId = Tag.Value,
                                                            ItemEventType = Tag.Value,
                                                            valColumn1 = "Unreferenced",
                                                            valColumn2 = null,
                                                            valColumn3 = null,
                                                            valColumn4 = null,
                                                            valColumn5 = null,
                                                            valColumn6 = Tag.Value.ToString(),
                                                            valColumn7 = null,
                                                            valColumn8 = null,
                                                            valColumn9 = null,
                                                        };
                                                        lstDetails.Add(id);                                                       
                                                        if (Tag.Value == 1) //notifie que les ajouts
                                                            lstUnreferencedTag.Add(Tag.Key);
                                                    }
                                                }
                                            }
                                            else //error from serveur
                                            {
                                                foreach (KeyValuePair<string, int> Tag in UnknownTags)
                                                {
                                                    ItemDetail id = new ItemDetail()
                                                    {
                                                        IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                                        TagId = Tag.Key,
                                                        DrawerId = Tag.Value,
                                                        ItemEventType = Tag.Value,
                                                        valColumn1 = "Unreferenced",
                                                        valColumn2 = null,
                                                        valColumn3 = null,
                                                        valColumn4 = null,
                                                        valColumn5 = null,
                                                        valColumn6 = Tag.Value.ToString(),
                                                        valColumn7 = null,
                                                        valColumn8 = null,
                                                        valColumn9 = null,
                                                    };
                                                    lstDetails.Add(id);                                                
                                                    if (Tag.Value == 1) //notifie que les ajouts
                                                        lstUnreferencedTag.Add(Tag.Key);
                                                }
                                            }
                                        }

                                        // Run thread to add it in db
                                        ProcessNewStone(TagListToSearch);
                                        #if SendUnrefTag
                                        if (lstUnreferencedTag.Count > 0)
                                            if (GrantedUsersCache.LastAuthenticatedUser != null)
                                                ProcessUnreferencedStones(lstUnreferencedTag, GrantedUsersCache.LastAuthenticatedUser.FirstName, GrantedUsersCache.LastAuthenticatedUser.LastName);
                                            else
                                                ProcessUnreferencedStones(lstUnreferencedTag, null, null);
                                         #endif
                                    }
                                    else
                                    {

                                        LogToFile.LogMessageToFile("------- Quit Get Stone data - no Network  --------");

                                    }
                                }
                               
                            }
                            else
                            {
                                foreach (KeyValuePair<string, int> Tag in DevicesHandler.ListTagPerDrawer)
                                {                                   

                                    ItemDetail id = new ItemDetail()
                                    {
                                        IsEnabled = Tag.Value >= 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden,
                                        TagId = Tag.Key,
                                        DrawerId = Tag.Value,
                                        ItemEventType = Tag.Value,
                                        valColumn1 = "Unreferenced",
                                        valColumn2 = null,
                                        valColumn3 = null,
                                        valColumn4 = null,
                                        valColumn5 = null,
                                        valColumn6 = Tag.Value.ToString(),
                                        valColumn7 = null,
                                        valColumn8 = null,
                                        valColumn9 = null,
                                    };
                                    lstDetails.Add(id);                                  
                                }
                            }
                            TagDetails = new ObservableCollection<ItemDetail>(lstDetails);
                        }                        
                    }
                });

                mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                wh.Stop();
            }
            catch (Exception error)
            {
                try
                {
                    if (wh != null)
                    {
                        wh.Stop();

                    }
                }
                catch
                { }
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error getting criteria");
                    exp.ShowDialog();
                }));
            }
            finally
            {
                try
                {
                    if (wh != null)
                    {
                        wh.Stop();
                    }
                }
                catch
                { }
            }
        }
        public DataTable PopulateDataGrid(ObservableCollection<ItemDetail> Data)
        {

            DataTable tmpDt = new DataTable();
            tmpDt.Columns.Add(new System.Data.DataColumn("TagId", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column1", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column2", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column3", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column4", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column5", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column6", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column7", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column8", typeof(string)));
            tmpDt.Columns.Add(new System.Data.DataColumn("Column9", typeof(string)));


            foreach (ItemDetail bo in Data)
            {
                var row = tmpDt.NewRow();
                tmpDt.Rows.Add(row);
                row["TagId"] = bo.TagId;
                for (int loop = 1; loop < 10; loop++)
                {
                    string colName = "Column" + loop;
                    switch (loop)
                    {
                        case 1: row[colName] = bo.valColumn1; break;
                        case 2: row[colName] = bo.valColumn2; break;
                        case 3: row[colName] = bo.valColumn3; break;
                        case 4: row[colName] = bo.valColumn4; break;
                        case 5: row[colName] = bo.valColumn5; break;
                        case 6: row[colName] = bo.valColumn6; break;
                        case 7: row[colName] = bo.valColumn7; break;
                        case 8: row[colName] = bo.valColumn8; break;
                        case 9: row[colName] = bo.valColumn9; break;
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
        public async Task<bool> ReloadDevice()
        {
            bool ret = false;
            // Add device in local DB
            try
            {
               
                if (String.IsNullOrEmpty(ServerIp))
                {
                    return false;
                }

                if ((string.IsNullOrEmpty(Properties.Settings.Default.WallSerial)) || (string.IsNullOrEmpty(Properties.Settings.Default.WallName)))
                {
                    return false;
                }

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
                ret = await ProcessSelectionFromServer.CreateCabinet(CreatedDev);
            }
            else
            {

                myDev.DeviceName = Properties.Settings.Default.WallName;
                myDev.DeviceLocation = Properties.Settings.Default.WallLocation;
                myDev.IpAddress = Utils.GetLocalIp();
                ret = await ProcessSelectionFromServer.UpdateCabinet(myDev);

            }


            startTimer.Start();
            startTimer.IsEnabled = true;
            return ret;

        }
        public RelayCommand BtRemoveCardSelection { get; set; }
        public void DeleteCard()
        {
            removeSelection();
            Thread.Sleep(1000);
            getSelection();
            bNeedUpdateCriteriaAfterScan = false;
        }

        public RelayCommand BtRefreshSelection { get; set; }
        public void refreshSelection()
        {
            //Clear pull info
            LastDeviceActionTime = DateTime.Now;

            SelectionSelected = null;
            mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
            Thread.Sleep(100);

            if (IsFlyoutCassettePositionOpen) //Stop lighting if lighting ON
                IsFlyoutCassettePositionOpen = false;

            if (IsInPutItemFastMode)
            {
                for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                {
                    if (DevicesHandler.IsDrawerWaitScan[loop])
                    {
                        DevicesHandler.DrawerStatus[loop] = DrawerStatusList.ScanPending;
                        BrushDrawer[loop] = _borderScanPending;
                    }
                    else
                    {
                        DevicesHandler.DrawerStatus[loop] = DrawerStatusList.Ready;
                        BrushDrawer[loop] = _borderReady;
                    }
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                }
            }
            IsInPutItemFastMode = false;
            bNeedUpdateCriteriaAfterScan = true;
        }
        public async void getSelection()
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


            //var myConTroller = await mainview0.ShowProgressAsync("Please wait","Get Selection From server",true);          
            //myConTroller.SetIndeterminate();
           // WaitHandler wh = null;
            try
            {

                /* wh = new WaitHandler();
                 wh.Msg = "Get Selection From server";
                 wh.Start();*/
                ShowProcessWindow("Get Selection From server");

                Selection.Clear();                

                #if (IsTiffany)
                #else

                /* Get Selection from API */
                LogToFile.LogMessageToFile("------- Start Getting Selection --------");
                bool gotSel = await ProcessSelectionFromServer.GetAndStoreSelectionAsync();
                LogToFile.LogMessageToFile("------- Selection Retrieve from server --------");
                if (gotSel)
                {                    

                    if (ProcessSelectionFromServer.lastSelection != null)
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                        lock (ProcessSelectionFromServer.somePublicStaticObject)
                        {
                            foreach (JsonSelectionList jsl in ProcessSelectionFromServer.lastSelection)
                            {
                                if (jsl == null) continue;
                                if (jsl.state == "closed") continue;
                                if (jsl.listOfTagToPull == null) continue;
                                GrantedUser user = null;
                                if (jsl.user_id.HasValue)
                                {
                                    user = ctx.GrantedUsers.GetByServerId(jsl.user_id.Value);
                                }

                                SelectionViewModel svm = new SelectionViewModel();
                                svm.IsSelected = false;
                                svm.PullItemId = jsl.selection_id;
                                svm.ServerPullItemId = jsl.selection_id;

                                TimeSpan ts = DateTime.Now - jsl.created_at;
                                if (ts.TotalDays < 1)
                                    svm.PullItemDate = jsl.created_at.ToString("hh:mm tt");
                                else if (ts.TotalDays == 2)
                                    svm.PullItemDate = "Yesterday\r\n" + jsl.created_at.ToString("hh:mm tt");
                                else
                                    svm.PullItemDate = jsl.created_at.ToString("MMM dd") + "\r\n" + jsl.created_at.ToString("hh:mm tt");

                                svm.Description = string.IsNullOrEmpty(jsl.description) ? " " : jsl.description;

                                if (user != null)
                                    svm.User = user.FirstName + " " + user.LastName;
                                svm.TotalToPull = jsl.listOfTagToPull.Count;

                                svm.lstTopull = new List<string>();
                                svm.lstTagpulled = new List<string>();
                                int nbInDevice = 0;

                                foreach (var uid in jsl.listOfTagToPull)
                                {
                                    if (string.IsNullOrEmpty(uid)) continue;
                                    if (DevicesHandler.ListTagPerDrawer.ContainsKey(uid))
                                    {
                                        svm.lstTopull.Add(uid);
                                        nbInDevice++;
                                    }
                                }

                                if (nbInDevice > 0)
                                {
                                    svm.TotalToPullInDevice = nbInDevice;
                                    Selection.Add(svm);
                                }
                            }
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                        }
                        //wh.Stop();
                        HideProcessWindow();
                        LogToFile.LogMessageToFile("------- Stop Getting Selection --------");                       
                    }
                }
                else
                {
                    mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                    try
                    {
                        /*  if (wh != null)
                          {
                              wh.Stop();
                          }*/
                        HideProcessWindow();
                    }
                    catch
                    { }
                    MessageDialogResult messageResult = await mainview0.ShowMessageAsync(" Information", "Error while updating selection ...");


                }
                #endif
            }
            catch { }
            finally
            {
                try
                {
                    /* if (wh != null)
                     {
                         wh.Stop();
                     }*/
                    HideProcessWindow();
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
                IsInPutItemFastMode = false;
                if (SelectionSelected == null)
                    return;

                if (RfidStatus == false)
                {
                    IsFlyoutCassettePositionOpen = false;
                    return;
                }              


                CassettesSelection tmpCassette = new CassettesSelection();
                _SelectedBaseObjects = new List<ItemDetail>();
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
                    ItemDetail theBo = (from c in TagDetails
                                        where c.TagId.Equals(uid)
                                        select c).SingleOrDefault<ItemDetail>();

                    if (theBo != null)
                    {
                        _SelectedBaseObjects.Add(theBo);
                        if (!tmpCassette.ListControlNumber.Contains(theBo.TagId))
                        {
                            switch (theBo.DrawerId)
                            {
                                case 1:
                                    if (TmpListCtrlPerDrawer1.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[1].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 2:
                                    if (TmpListCtrlPerDrawer2.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[2].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 3:
                                    if (TmpListCtrlPerDrawer3.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[3].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 4:
                                    if (TmpListCtrlPerDrawer4.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[4].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 5:
                                    if (TmpListCtrlPerDrawer5.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[5].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 6:
                                    if (TmpListCtrlPerDrawer6.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[6].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 7:
                                    if (TmpListCtrlPerDrawer7.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[7].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                            }
                        }
                    }
                }

                tmpCassette.CassetteDrawer1Number = tmpCassette.TagToLight[1].Count == 0 ? "0" : tmpCassette.TagToLight[1].Count.ToString();
                tmpCassette.CassetteDrawer2Number = tmpCassette.TagToLight[2].Count == 0 ? "0" : tmpCassette.TagToLight[2].Count.ToString();
                tmpCassette.CassetteDrawer3Number = tmpCassette.TagToLight[3].Count == 0 ? "0" : tmpCassette.TagToLight[3].Count.ToString();
                tmpCassette.CassetteDrawer4Number = tmpCassette.TagToLight[4].Count == 0 ? "0" : tmpCassette.TagToLight[4].Count.ToString();
                tmpCassette.CassetteDrawer5Number = tmpCassette.TagToLight[5].Count == 0 ? "0" : tmpCassette.TagToLight[5].Count.ToString();
                tmpCassette.CassetteDrawer6Number = tmpCassette.TagToLight[6].Count == 0 ? "0" : tmpCassette.TagToLight[6].Count.ToString();
                tmpCassette.CassetteDrawer7Number = tmpCassette.TagToLight[7].Count == 0 ? "0" : tmpCassette.TagToLight[7].Count.ToString();

                BrushDrawer[1] = tmpCassette.TagToLight[1].Count == 0 ? _borderReady : _borderReadyToPull;
                BrushDrawer[2] = tmpCassette.TagToLight[2].Count == 0 ? _borderReady : _borderReadyToPull;
                BrushDrawer[3] = tmpCassette.TagToLight[3].Count == 0 ? _borderReady : _borderReadyToPull;
                BrushDrawer[4] = tmpCassette.TagToLight[4].Count == 0 ? _borderReady : _borderReadyToPull;
                BrushDrawer[5] = tmpCassette.TagToLight[5].Count == 0 ? _borderReady : _borderReadyToPull;
                BrushDrawer[6] = tmpCassette.TagToLight[6].Count == 0 ? _borderReady : _borderReadyToPull;
                BrushDrawer[7] = tmpCassette.TagToLight[7].Count == 0 ? _borderReady : _borderReadyToPull;

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
                    if  (adminUser == null)
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(null, "Error no Admin user in DB - Contact spacecode");
                        exp.ShowDialog();
                        return;
                    }
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
                IsInPutItemFastMode = false;
                if (RfidStatus == false)
                {
                    IsFlyoutCassettePositionOpen = false;
                    return;
                }

                List<ItemDetail> ListTagtoLight = TagDetails.Where(item => item.IsSelected == true).ToList();

                if (ListTagtoLight == null) return;
                if (ListTagtoLight.Count == 0) return;

                CassettesSelection tmpCassette = new CassettesSelection();
                _SelectedBaseObjects = new List<ItemDetail>();
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

                foreach (var item in ListTagtoLight)
                {
                    ItemDetail theBo = (from c in TagDetails
                                        where c.TagId.Equals(item.TagId)
                                        select c).SingleOrDefault<ItemDetail>();

                    if (theBo != null)
                    {
                        _SelectedBaseObjects.Add(theBo);
                        if (!tmpCassette.ListControlNumber.Contains(theBo.TagId))
                        {
                            switch (theBo.DrawerId)
                            {
                                case 1:
                                    if (TmpListCtrlPerDrawer1.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[1].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 2:
                                    if (TmpListCtrlPerDrawer2.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[2].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 3:
                                    if (TmpListCtrlPerDrawer3.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[3].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 4:
                                    if (TmpListCtrlPerDrawer4.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[4].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 5:
                                    if (TmpListCtrlPerDrawer5.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[5].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 6:
                                    if (TmpListCtrlPerDrawer6.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[6].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 7:
                                    if (TmpListCtrlPerDrawer7.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[7].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;

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
        async  void searchTxtGotCRfn()
        {
            try
            {
                if ((TagDetails == null) || (TagDetails.Count == 0)) return;

                bool bfind = false;

                mainview0.myDatagrid.SearchHelper.AllowFiltering = true;
                mainview0.myDatagrid.SearchHelper.ClearSearch();
                mainview0.myDatagrid.SearchHelper = new SearchHelperExt(this.mainview0.myDatagrid, "Column1");
                mainview0.myDatagrid.SearchHelper.Search(_txtSearchCtrl);   
               
                mainview0.myDatagrid.SearchHelper.SearchHighlightBrush = _selectionBrush;
                mainview0.myDatagrid.SearchHelper.SearchBrush = _selectionBrush;
                Thread.Sleep(50);
                bool bOut = mainview0.myDatagrid.SearchHelper.FindNext(_txtSearchCtrl);
                do
                {
                    var rowIndex = mainview0.myDatagrid.SearchHelper.CurrentRowColumnIndex.RowIndex;
                    var colIndex = mainview0.myDatagrid.SearchHelper.CurrentRowColumnIndex.ColumnIndex;
                    if (rowIndex >= 0 && colIndex != 0)  //find
                    {
                        bfind = true;
                        var record = mainview0.myDatagrid.View.Records[rowIndex - 1];
                        DataRowView drv = record.Data as DataRowView;
                        if (!mainview0.myDatagrid.SelectedItems.Contains(drv))
                            mainview0.myDatagrid.SelectedItems.Add(drv);
                    }
                    bOut = mainview0.myDatagrid.SearchHelper.FindNext(_txtSearchCtrl);
                    if (mainview0.myDatagrid.SearchHelper.CurrentRowColumnIndex.RowIndex != rowIndex) bOut = true;
                } while (bOut);

                if (!bfind)
                {
                    string info = string.Format("Working Order : {0}\r\nTag ID : {1}\r\nLast Device : {2}\r\nDrawer : {3}", _txtSearchCtrl, "xxxxxxxxxx", "yyyyyyyyyy", "?");
                    await mainview0.ShowMessageAsync("Information", info);
                }
                

                mainview0.TxtCtrlNumber.SelectAll();


            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error getting Stone info");
                exp.ShowDialog();
            }
        }

        public RelayCommand searchTxtBarcodeCR { get; set; }
        async void searchTxtBarcodeCRfn()
        {            
           
            if (!string.IsNullOrEmpty(txtBarcode))
            {               
                JsonSku mySku = await ProcessSelectionFromServer.GetSkuInfo(txtBarcode);
                if (mySku != null)
                {
                    if (!mySku.status) // error
                    {
                        txtStatus = string.Format("Error : Code {0} - {1}", mySku.errors.code, mySku.errors.msg);
                        txtRefNumber = string.Empty;
                        txtTagId = string.Empty;
                        txtDrawer = string.Empty;
                        txtDevice = string.Empty;
                        txtLastDate = string.Empty;
                    }
                    else
                    {
                        txtStatus = mySku.data.status;
                        txtRefNumber = mySku.data.refNumber;
                        txtTagId = mySku.data.rfidNumber;
                        if (txtStatus == "Present" || txtStatus == "Added")
                        {
                            txtDevice = WallName;
                            txtDrawer = mySku.data.drawer;
                            txtLastDate = mySku.data.updatedAt.ToShortDateString() + " " + mySku.data.updatedAt.ToLongTimeString();
                        }
                        else
                        {
                            txtDrawer = string.Empty;
                            txtDevice = string.Empty;
                            txtLastDate = string.Empty;
                        }
                    }
                    SelectionLifeTimeTimer.IsEnabled = true;
                    SelectionLifeTimeTimer.Stop();
                    SelectionLifeTimeTimer.Start();
                    LightBarcodeTag(txtTagId);
                }
            } 
            mainview0.TxtBarcodeCtrl.SelectAll();
            mainview0.TxtBarcodeCtrl.Focus();
        }

        void LightBarcodeTag(string tagId)
        {
            try
            {
                IsInPutItemFastMode = false;
                if (RfidStatus == false)
                {
                    IsFlyoutCassettePositionOpen = false;
                    return;
                }
                CassettesSelection tmpCassette = new CassettesSelection();
                _SelectedBaseObjects = new List<ItemDetail>();
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


                if (!string.IsNullOrEmpty(tagId))
                {
                    string uid = tagId;
                    ItemDetail theBo = (from c in TagDetails
                                        where c.TagId.Equals(uid)
                                        select c).SingleOrDefault<ItemDetail>();

                    if (theBo != null)
                    {
                        _SelectedBaseObjects.Add(theBo);
                        if (!tmpCassette.ListControlNumber.Contains(theBo.TagId))
                        {
                            switch (theBo.DrawerId)
                            {
                                case 1:
                                    if (TmpListCtrlPerDrawer1.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[1].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 2:
                                    if (TmpListCtrlPerDrawer2.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[2].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 3:
                                    if (TmpListCtrlPerDrawer3.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[3].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 4:
                                    if (TmpListCtrlPerDrawer4.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[4].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 5:
                                    if (TmpListCtrlPerDrawer5.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[5].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 6:
                                    if (TmpListCtrlPerDrawer6.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[6].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
                                case 7:
                                    if (TmpListCtrlPerDrawer7.Contains(theBo.TagId))
                                    {
                                        tmpCassette.TagToLight[7].Add(theBo.TagId);
                                        tmpCassette.ListControlNumber.Add(theBo.TagId);
                                    }
                                    break;
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


        public class SearchHelperExt:SearchHelper
        {
            string ColumnToSearch;
            public SearchHelperExt(SfDataGrid datagrid , string ColumnToSearch)
            : base(datagrid)
            {
                this.ColumnToSearch = ColumnToSearch;
            }

            protected override bool SearchCell(DataColumnBase column, object record, bool ApplySearchHighlightBrush)
            {

                if (column.GridColumn.MappingName == ColumnToSearch)
                    return base.SearchCell(column, record, ApplySearchHighlightBrush);
                return false;
            }
        }


        public RelayCommand btClearSelection { get; set; }
        void ClearSelection()
        {           
            IsInPutItemFastMode = false;

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

            //debug user
            //refreshUserFromServer();
            

            startTimer.Stop();
           startTimer.IsEnabled = false;

            // No serial in Configuration - Connect to get rfid serial      

#if UseInfinity

            if (!getInfinityUrl())
            {
                string Info = string.Format("Unable to get Configuration - Check SQL server - Application will stop \r\n Host : {0}\r\nLogin : {1}\r\nDbName : {2} ", RemoteDatabase.Host, RemoteDatabase.Login, RemoteDatabase.Name);
                await mainview0.ShowMessageAsync("INFORMATION", Info);
                System.Windows.Application.Current.Shutdown();
                return;
            }
           
               
            

            
            if (!string.IsNullOrEmpty(ReactInfinityUrl))  
            {
                if (!await ReactLogin())
                {
                    string Info = string.Format("Unable to log to React Infinity server");
                    await mainview0.ShowMessageAsync("INFORMATION", Info);
                    System.Windows.Application.Current.Shutdown();
                    return;
                }
            }

         

            InfinityTimer = new DispatcherTimer();
            InfinityTimer.Interval = new TimeSpan(0, 1, 0);
            InfinityTimer.Tick += InfinityTimer_Tick;
            InfinityTimer.IsEnabled = true;
            InfinityTimer.Start();
#endif
            getScanTimerValue();

            DevicesHandler.FindAndConnectDevice();
            if ((DevicesHandler.Device != null) &&(DevicesHandler.Device.IsConnected))
            {
                if (Properties.Settings.Default.RfidSerial != DevicesHandler.Device.SerialNumber)
                {
                    Properties.Settings.Default.RfidSerial = DevicesHandler.Device.SerialNumber;
                    Properties.Settings.Default.Save();
                    // Properties.Settings.Default.Upgrade();
                    Properties.Settings.Default.Reload();
                }
                DevicesHandler.Device.DisconnectReader();
            }  

         
            // Is Wall In database - refer to Rfid serial so need connection to get number
            // await mainview0.Dispatcher.BeginInvoke(new System.Action(async () =>
            // {

            // Test If serial exist in app
            bool bNeedQuit = false;

            ServerIp  = Properties.Settings.Default.ServerIp;
            if(String.IsNullOrEmpty(ServerIp))
            {
                bNeedQuit = true;
                await mainview0.ShowMessageAsync("Device Information", "No Server IP define! \r\n Please Configure Server!");
                SelectedTabIndex = 4;
            }

            if ((string.IsNullOrEmpty (Properties.Settings.Default.WallSerial)) || (string.IsNullOrEmpty(Properties.Settings.Default.WallName)))
            {
                bNeedQuit = true;
                await mainview0.ShowMessageAsync("Wall Information", "Wall Not define in app! \r\n Please Add device ");
                SelectedTabIndex = 4;           
            }

            if (bNeedQuit) return;

                Device myDev = await ProcessSelectionFromServer.GetCabinet(Properties.Settings.Default.WallSerial);
            if ((myDev == null) || (myDev.DeviceSerial != Properties.Settings.Default.WallSerial))
            {
                bNeedQuit = true;
                await mainview0.ShowMessageAsync("Wall Information", "Wall not found in server or server not found - You have to go in admin mode to setup device");
                SelectedTabIndex = 4;
            }
            else
            {
                string tmpIp = Utils.GetLocalIp();
                if (!string.IsNullOrEmpty(Properties.Settings.Default.NotificationIp))
                    tmpIp = Properties.Settings.Default.NotificationIp;

                if ((myDev.DeviceSerial != Properties.Settings.Default.WallSerial) || (myDev.DeviceName != Properties.Settings.Default.WallName) || (myDev.IpAddress != tmpIp))
                {
                    ConfFailure = false;
                    bool x  = await ReloadDevice();
                    if (!x)
                    {
                        ConfFailure = true;
                        startTimer.Stop();
                        startTimer.IsEnabled = false;
                        await mainview0.ShowMessageAsync("Device error", "Device configuration error - Contact Spacecode Support ");
                        startTimer.IsEnabled = true;
                        startTimer.Interval = new TimeSpan(0, 1, 0);
                        startTimer.Start();
                    }               
                }
                startTimer.Interval = new TimeSpan(0, 0, 5);
            }


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
               
                CreateProcessWindow();

            if (!string.IsNullOrEmpty(ReactInfinityUrl))
            {
               /* if (!await ReactRegisterDevices())
                {
                    string Info = string.Format("Unable to Register device(s) - Application will stop \r\n");
                    await mainview0.ShowMessageAsync("INFORMATION", Info);
                    System.Windows.Application.Current.Shutdown();
                    return;
                }*/

                CreateSocketIOClient(ReactInfinityUrl);
                Thread.Sleep(1000);
                if (!registerDevice())
                {
                    string Info = string.Format("Unable to Register device(s) - Application will stop \r\n");
                    await mainview0.ShowMessageAsync("INFORMATION", Info);
                    System.Windows.Application.Current.Shutdown();
                    return;
                }

                //debug product insertion
                /*List<string> TagListToSearch = new List<string>() { "3023403200" };
                var result = await InfinityServiceForReactHandler.GetSku(ReactInfinityUrl, ReactToken, TagListToSearch);               
                ProcessNewStone(TagListToSearch);*/
            }
            InitValue();
            await ProcessInfinityUser(null);
            doPing();


            ctx.Database.Connection.Close();
                 ctx.Dispose();

           // }));

            await mainview0.Dispatcher.BeginInvoke(new System.Action( () =>
            {
                //todo - we drive user (middleware or react? ) 
                refreshUserFromServer();
            }));
        }

        private void InfinityTimer_Tick(object sender, EventArgs e)
        {
            doPing();
        }

        private void AutoConnectTimer_Tick(object sender, EventArgs e)
        {
            AutoConnectTimer.IsEnabled = false;

            dayDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            dayTime = DateTime.Now.ToString("hh:mm tt");
            RfidStatus = DevicesHandler.DevicesConnected;
            #if UseInfinity 
                if (!IsDeviceRegistered)
                {
                    registerDevice();
                }
                if (!IsDeviceRegistered)
                {
                    AutoConnectTimer.IsEnabled = true;
                    return;
                }
            #endif

            if (RfidStatus)
            {
                TimeSpan ts = DateTime.Now - DevicesHandler.LastScanTime;
                LastScanInfo = string.Format("Last scan : {0} min(s) ago", (int)ts.TotalMinutes);

                //todo 
               if (ts.TotalSeconds > 3600) // no scan for one hour ; rerun it
               {
                    for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                    {
                        if (DevicesHandler.DevicesConnected)
                            DevicesHandler.IsDrawerWaitScan[loop] = true;                       
                    }
                }
            }

            // 2 drawers open not possible meaning power cut 
            if ((DevicesHandler.DrawerStatus[1] == DrawerStatusList.Open) && (DevicesHandler.DrawerStatus[2] == DrawerStatusList.Open))
                DevicesHandler.ReleaseDevices();

            if (!DevicesHandler.DevicesConnected )
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
        private volatile bool InLightOrRecheckprocess = false;
        private async void ScanTimer_Tick(object sender, EventArgs e)
        {
           // WaitHandler wh = null;
            //ProgressDialogController myConTroller = null;
            try
                {


                    if (TagDetails != null && mainview0.tiInventoryMode.IsVisible)
                    {
                        int nbtag = TagDetails.Where(item => item.IsSelected == true).ToList().Count();
                        txtNbSelectedItem = string.Format("Stone(s) Selected : {0}", nbtag);
                    }

                    if (InLightOrRecheckprocess)
                        return;
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
                    else if (RfidError)
                    {
                        OverallStatus = false;
                        WallStatusOperational = "RFID SCAN ERROR";
                    }
                    else if (!NetworkStatus)
                    {
                        OverallStatus = false;
                        WallStatusOperational = "SERVICE FAILURE";
                    }
                    else if (ConfFailure)
                    {
                        OverallStatus = false;
                        WallStatusOperational = "SETTING ERROR";
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
                    else if ((_recheckLightDrawer != -1) && (_lightDrawer == -1))
                    {
                    int _bckrecheckLightDrawer = _recheckLightDrawer;
                    if (!Properties.Settings.Default.DoRecheck)
                    {
                      
                        DevicesHandler.DrawerStatus[_bckrecheckLightDrawer] = DrawerStatusList.ScanPending;
                        DrawerStatus[_bckrecheckLightDrawer] = DevicesHandler.DrawerStatus[_bckrecheckLightDrawer];
                        BrushDrawer[_bckrecheckLightDrawer] = BrushDrawer[_recheckLightDrawer] = _borderScanPending;
                        _recheckLightDrawer = -1;

                        DevicesHandler.IsDrawerWaitScan[_bckrecheckLightDrawer] = true;
                    }
                    else
                    {

                        //wh = new WaitHandler();
                        // wh.Msg = "Please wait end of recheck before opening another drawer";
                        //wh.Start();
                        //myConTroller = await mainview0.ShowProgressAsync("Please wait end of recheck before opening another drawer", string.Format("Rechecking {0} tags in drawer {1}", SelectedCassette.TagToLight[_bckrecheckLightDrawer].Count, _bckrecheckLightDrawer), true);
                        //myConTroller.SetIndeterminate();                       
                        ShowProcessWindow("Please wait end of recheck before opening another drawer");
                       List<string> TagToLight = new List<string>(SelectedCassette.TagToLight[_bckrecheckLightDrawer]);

                        await Task.Run(() =>
                        {
                           InLightOrRecheckprocess = true;
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

                                    DevicesHandler.IsDrawerWaitScan[_bckrecheckLightDrawer] = true;
                                    if (nbToFind == TagToLight.Count)
                                   {
                                        DevicesHandler.StopLighting(_bckrecheckLightDrawer);
                                        if (DevicesHandler.IsDrawerWaitScan[_recheckLightDrawer])
                                        {
                                            DevicesHandler.DrawerStatus[_recheckLightDrawer] = DrawerStatusList.ScanPending;
                                            BrushDrawer[_recheckLightDrawer] = _borderScanPending;
                                        }
                                        else
                                        {
                                            DevicesHandler.DrawerStatus[_recheckLightDrawer] = DrawerStatusList.Ready;
                                            BrushDrawer[_recheckLightDrawer] = _borderReady;
                                        }
                                        DrawerStatus[_recheckLightDrawer] = DevicesHandler.DrawerStatus[_recheckLightDrawer];
                                    }
                                   else
                                   {
                                       DevicesHandler.DrawerStatus[_bckrecheckLightDrawer] = DrawerStatusList.InLight;
                                       DrawerStatus[_bckrecheckLightDrawer] = DevicesHandler.DrawerStatus[_bckrecheckLightDrawer];
                                       BrushDrawer[_bckrecheckLightDrawer] = _borderReadyToPull;
                                   }

                                  
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
                                   DevicesHandler.StopLighting(_recheckLightDrawer);                                  
                                    _recheckLightDrawer = -1;
                               }
                           }
                           else
                               wallStatus = "Wait user Action";
                           _recheckLightDrawer = -1;
                            InLightOrRecheckprocess = false;
                      });

                        List<string> TagTolightBck = null;                        
                        //Clone tag to light
                        if ((TagToLight != null) && (TagToLight.Count > 0 ))
                        {
                            TagTolightBck = new List<string>();
                            foreach (string uid in TagToLight)
                            {
                                TagTolightBck.Add(uid);
                                if (SelectionSelected != null)
                                    SelectionSelected.lstTagpulled.Add(uid); 
                            }
                        }
                        else
                        {
                         await  mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                            {
                                if ((SelectedTabIndex == 1) && (mainview0.tiBarcodeMode.Visibility == Visibility.Visible))
                                {
                                    //txtBarcode = string.Empty;
                                    mainview0.TxtBarcodeCtrl.SelectAll();
                                    mainview0.TxtBarcodeCtrl.Focus();
                                }
                            }));
                        }

                        /*****   Update API ****/
                        await Task.Run(() =>
                      {
                          if ((SelectionSelected != null) && (TagTolightBck != null) && (TagTolightBck.Count > 0))
                          {
                              ProcessSelectionFromServer.UpdateSelectionAsync(SelectionSelected.ServerPullItemId, TagTolightBck);

                              int nbtagPulled = TagTolightBck.Count + SelectionSelected.lstTagpulled.Count;
                              if ((SelectionSelected.TotalToPull == TagTolightBck.Count) && (SelectionSelected.TotalToPullInDevice == nbtagPulled))
                                  ProcessSelectionFromServer.DeleteSelectionAsync(SelectionSelected.ServerPullItemId);

                          }                          
                      });

                        mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                        try
                        {
                            // wh.Stop();
                            HideProcessWindow();
                        }
                        catch
                        { }
                    }
                    }
#endregion
#region Light
                    else if ((_lightDrawer != -1) && (_recheckLightDrawer == -1))
                    {
                        IsInPutItemFastMode = false;
                        int bckDrawer = _lightDrawer;
                        DevicesHandler.SetDrawerActive(bckDrawer);
                        if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                        {                        
                           
                            if (bDrawerToLight[bckDrawer] == true)
                            {

                            // myConTroller = await mainview0.ShowProgressAsync("Please wait - Not closed drawer until light process finish", string.Format("Lighting {0} tags in drawer {1}", SelectedCassette.TagToLight[_lightDrawer].Count, _lightDrawer), true);
                            //  myConTroller.SetIndeterminate();
                            /* wh = new WaitHandler();
                             wh.Msg = " Not closed drawer until light process finished";
                             wh.Start();*/
                            ShowProcessWindow("Not closed drawer until light process finished");
                            await  Task.Run(() =>
                            {
                                InLightOrRecheckprocess = true;
                                //Refresh list to light
                                LightSelectionFromList();
                                DevicesHandler.StopLighting(bckDrawer);
                                List<string> TagToLight = new List<string>(SelectedCassette.TagToLight[bckDrawer]);
                                List<string> TagToLightSos = new List<string>(SelectedCassette.TagToLight[bckDrawer]);
                                wallStatus = "Light " + TagToLight.Count + " stones(s) in drawer " + bckDrawer;
                                LightSelectionDrawer(TagToLight, bckDrawer);
                                if (TagToLight.Count > 0)
                                {
                                    Thread.Sleep(500);
                                    
                                    LightSelectionDrawer(TagToLightSos, bckDrawer);
                                    if (TagToLightSos.Count > 0)
                                        wallStatus = "Unable to light " + TagToLightSos.Count + " stones(s) in drawer " + bckDrawer;
                                }                             
                                   
                                bDrawerToLight[bckDrawer] = false;
                                bDrawerToRefreshLight[bckDrawer] = true;
                                InLightOrRecheckprocess = false;


                             });

                                mainview0.Dispatcher.Invoke(new System.Action(() => { }), DispatcherPriority.ContextIdle, null);
                            /* try
                             {
                                 if ((myConTroller != null) && (myConTroller.IsOpen))
                                 {
                                     await myConTroller.CloseAsync();

                                 }
                             }
                             catch
                             { }*/
                            //wh.Stop();
                            HideProcessWindow();
                            }
                            else if (bDrawerToRefreshLight[bckDrawer] == true)
                            {
                                DevicesHandler.StopLighting(bckDrawer);
                                Thread.Sleep(500);
                                DevicesHandler.Device.LEdOnAll(bckDrawer, 0, false);
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
#region Update selection
                else if (bNeedUpdateCriteriaAfterScan)
                    {
                        //Wait end of light process or recheck  
                        if ((SelectionSelected == null) &&(!IsWallInScan()) && (!InLightOrRecheckprocess) && (_recheckLightDrawer == -1) && (_lightDrawer == -1))
                        {
                        //comment for tiffany
                        #if (IsTiffany)
                        #else
                        #if UseInfinity
                               getStoneData();
                        #endif
                               getSelection();
                        #endif
                        bNeedUpdateCriteriaAfterScan = false;
                        }
                        else if (IsInPutItemFastMode)
                        {
                        //Comment for tiffany
                        #if (IsTiffany)
                        #else
                        #if UseInfinity
                              getStoneData();
                        #endif
                             getSelection();
                        #endif
                        bNeedUpdateCriteriaAfterScan = false;
                        }
                    }
#endregion
#region scan
                else if (!IsInPutItemFastMode  && IsWallReady())
                //else if ((!IsFlyoutCassettePositionOpen) && (IsWallReady()))
                {
                    //double elaspedTimeLastAction = (DateTime.Now - LastDeviceActionTime).TotalSeconds;
                    //if (elaspedTimeLastAction > 15.0)
                    //{
                        for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                        {
                            if (DevicesHandler.IsDrawerWaitScan[loop])
                            {
                                if ((DevicesHandler.Device != null) && (DevicesHandler.Device.IsConnected) && ((DevicesHandler.DrawerStatus[loop] == DrawerStatusList.Ready) || (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.ScanPending)))
                                {
                                    DevicesHandler.StartManualScan(loop);
                                    Thread.Sleep(500);                                   
                                break;
                                }
                            }
                        }
                    //}
                    if (bNeedUpdateCriteria)
                    {
                        if (!IsWaitingForScan())
                        {
                            bNeedUpdateCriteria = false;
                            bNeedUpdateCriteriaAfterScan = true;
                            await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                            {
                                if ((SelectedTabIndex == 1) && (mainview0.tiBarcodeMode.Visibility == Visibility.Visible))
                                {
                                    //txtBarcode = string.Empty;
                                    mainview0.TxtBarcodeCtrl.SelectAll();
                                    mainview0.TxtBarcodeCtrl.Focus();
                                }
                            }));
                        }
                    }
                }
#endregion
            }
            catch (IndexOutOfRangeException err)
            {
                /* try
                 {
                     if ((myConTroller != null) && (myConTroller.IsOpen))
                     {
                         await myConTroller.CloseAsync();
                     }
                 }
                 catch
                 { }*/
                try
                {
                    /* if (wh != null)
                     {
                         wh.Stop();
                     }*/
                    HideProcessWindow();
                }
                catch
                { }
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(err, "Out of range Exception - value = " + _autoLightDrawer);
                    exp.ShowDialog();
                }));
            }
            catch (Exception error)
            {
                /*try
                {
                    if ((myConTroller != null) && (myConTroller.IsOpen))
                    {
                        await myConTroller.CloseAsync();
                    }
                }
                catch
                { }*/
                try
                {
                    /* if (wh != null)
                     {
                         wh.Stop();
                     }*/
                    HideProcessWindow();
                }
                catch
                { }
                await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
               {
                   ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Scan Timer");
                   exp.ShowDialog();
               }));
            }
            finally
            {
                /*try
                {
                    if ((myConTroller != null) && (myConTroller.IsOpen))
                    {
                        await myConTroller.CloseAsync();
                    }
                }
                catch
                { }*/
                try
                {
                    /*if (wh != null)
                    {
                        wh.Stop();
                    }*/
                    HideProcessWindow();
                }
                catch
                { }

                ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                ScanTimer.IsEnabled = true;
                ScanTimer.Start();
            }
        }
        private void SelectionLifeTimeTimer_Tick(object sender, EventArgs e)
        {
            if ((SelectedTabIndex == 1) && (mainview0.tiBarcodeMode.Visibility == Visibility.Visible))
            {
                SelectionLifeTimeTimer.IsEnabled = false;
                SelectionLifeTimeTimer.Stop();
                if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                {

                    cancelLighting();
                    SelectionSelected = null;
                    SelectedCassette = null;
                }

                txtStatus = string.Empty;
                txtRefNumber = string.Empty;
                txtTagId = string.Empty;
                txtDrawer = string.Empty;
                txtDevice = string.Empty;
                txtLastDate = string.Empty;                 
                   
                txtBarcode = string.Empty;
                mainview0.TxtBarcodeCtrl.SelectAll();
                mainview0.TxtBarcodeCtrl.Focus();                 
                
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
                        bNeedUpdateCriteriaAfterScan = true;
                        //getCriteria();
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
                            //getCriteria();
                            bNeedUpdateCriteriaAfterScan = true;
                        }
                        break;
                    case "AddOrUpdateProduct":
                        //getCriteria();
                        bNeedUpdateCriteriaAfterScan = true;
                      
                        break;
                    case "StockOutProduct":
                       // getCriteria();
                        bNeedUpdateCriteriaAfterScan = true;
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
        #region Infinity 
        private async void doPing()
        {
            if (!string.IsNullOrEmpty(InfinityServerUrl))
            {
                InfinityService.EventInfo newEvent = new InfinityService.EventInfo();
                newEvent.reader = new Reader { serial = WallSerial };
                newEvent.reader.drawer = null;
                newEvent.user = null;
                newEvent.timestamp = UnixTimeStamp.ConvertToUnixTimestamp(DateTime.Now.ToUniversalTime());

                List<string> lstInventory = new List<string>();
                Event Event = new Event() { EventType = EventType.PING, stones = null };
                List<Event> lstEvent = new List<Event>();
                lstEvent.Add(Event);
                newEvent.events = lstEvent.ToArray();
                bool result = await InfinityServiceHandler.SendEvents(InfinityServerUrl, InfinityServerToken, WallSerial, newEvent);
                if (result)
                {
                    //if no network - send not notified scan
                    if (!NetworkStatus)
                    {
                        NetworkStatus = true;
                    }
                    else
                        NetworkStatus = true;
                    if (InfinityServiceHandler.LastReturnInfo != null)
                    {
                        if (InfinityServiceHandler.LastReturnInfo.hostUpdated == 1)
                        {
                            //Todo
                            bool ret = await InfinityServiceHandler.GetEndpoint(InfinityServerToken);
                            if (ret)
                            {
                                string newEndpoint = InfinityServiceHandler.LastEndPoint;
                                InfinityServerUrl = newEndpoint;
                                using (var ctx = RemoteDatabase.GetDbContext())
                                {
                                    var conf = ctx.Configurations.Where(c => c.Parameter == "RemoteServerUrl").SingleOrDefault();
                                    if (conf != null)
                                    {
                                        conf.Value = newEndpoint;
                                        ctx.Entry(conf).State = EntityState.Modified;
                                        ctx.SaveChanges();
                                    }
                                }
                            }
                        }

                        if (InfinityServiceHandler.LastReturnInfo.log != 0)
                        {
                            DateTime logTime = logTime = DateTime.Now;
                            if (InfinityServiceHandler.LastReturnInfo.log < 0)
                            {
                                logTime = DateTime.Now.AddDays(InfinityServiceHandler.LastReturnInfo.log);
                            }

                            string filename = LogToFile.GetTempPath() + string.Format("smartDrawerLog_{0}.txt", logTime.ToString("yyMMdd"));
                            await InfinityServiceHandler.SendLog(InfinityServerUrl, InfinityServerToken, WallSerial, filename);
                        }

                        if (InfinityServiceHandler.LastReturnInfo.clearDbase == 1)
                        {
                            ClearDatabase();
                        }
                        if (InfinityServiceHandler.LastReturnInfo.adminRights == 1)
                        {
                            isAdmin = true;
                            /*  UserViewableTimer = new DispatcherTimer();
                              UserViewableTimer.Tick += UserViewableTimer_Tick;
                              UserViewableTimer.Interval = new TimeSpan(0, 10, 0);
                              UserViewableTimer.IsEnabled = true;
                              UserViewableTimer.Start();*/
                        }
                        if (InfinityServiceHandler.LastReturnInfo.userList == 1)
                            ProcessInfinityUser(null);
                        if (InfinityServiceHandler.LastReturnInfo.addUsers == 1)
                            UpdateInfinityUsers();

                        if (InfinityServiceHandler.LastReturnInfo.setOperational == 1)
                        {
                            HideProcessWindow();
                            IsOpeningBusiness = false;
                            IsClosingBusiness = false;
                            IsClosingCompleted = false;
                        }

                        //TODO HOw We renew selection for Diamond match and transit for wall?
                        /*if (InfinityServiceHandler.LastReturnInfo.diamondMatch > 0)
                        {
                            bool bNeedUpateSel = true;
                            if ((Selection != null) && (Selection.Count > 0))
                            {
                                if (Selection[0].Ref == "REQUEST_MATCHING")
                                    bNeedUpateSel = false;
                            }
                            if (bNeedUpateSel)
                            {

                                SelectedTabIndex = 1;
                                getSelection();
                            }
                        }*/
                    }
                }
                else
                {
                    NetworkStatus = false;
                }
            }
        }
        /* private void ProcessInfinityUser(GrantedUser gu)
         {
             if (!string.IsNullOrEmpty(InfinityServerUrl))
             {
                 Task.Run(async () =>
             {
                 try
                 {
                     if (gu == null)
                     {
                         using (var ctx = RemoteDatabase.GetDbContext())
                         {
                             var lstDev = ctx.GrantedUsers
                             .Where(u => u.UserRankId > 1).ToList();
                             foreach (var dv in lstDev)
                             {
                                 UserInfo tmpUser = new UserInfo()
                                 {
                                     username = dv.Login,
                                     password = dv.Password,
                                     name = dv.FirstName + " " + dv.LastName,
                                     local_user_id = dv.GrantedUserId,
                                     extra = dv.BadgeNumber,
                                 };
                                 await InfinityServiceHandler.SendUser(InfinityServerUrl, InfinityServerToken, WallSerial, tmpUser);
                             }


                             //Add  Unknow user
                             UserInfo tmpUser2 = new UserInfo()
                             {
                                 username = "Unknown",
                                 password = "User",
                                 name = "Unknown User",
                                 local_user_id = 999999,
                                 extra = string.Empty,
                             };
                             await InfinityServiceHandler.SendUser(InfinityServerUrl, InfinityServerToken, WallSerial, tmpUser2);
                         }
                     }
                     else
                     {
                         UserInfo tmpUser = new UserInfo()
                         {
                             username = gu.Login,
                             password = gu.Password,
                             name = gu.FirstName + " " + gu.LastName,
                             local_user_id = gu.GrantedUserId,
                             extra = gu.BadgeNumber,
                         };
                         await InfinityServiceHandler.SendUser(InfinityServerUrl, InfinityServerToken, WallSerial, tmpUser);
                     }
                 }

                 catch (Exception error)
                 {
                     Application.Current.Dispatcher.Invoke(() =>
                     {
                         ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate User");
                         exp.ShowDialog();
                     });
                 }
             });
             }
         }*/
        private async Task<bool> ProcessInfinityUser(GrantedUser gu)
        {
           
            await Task.Run(async () =>
            {
                try
                {
                    if (!string.IsNullOrEmpty(InfinityServerUrl)) //Phase 2 - sebd user define
                    {

                        if (gu == null)
                        {
                            using (var ctx = RemoteDatabase.GetDbContext())
                            {
                                var lstDev = ctx.GrantedUsers
                                .Where(u => u.UserRankId > 1).ToList();
                                foreach (var dv in lstDev)
                                {
                                    UserInfo tmpUser = new UserInfo()
                                    {
                                        username = dv.Login,
                                        password = dv.Password,
                                        name = dv.FirstName + " " + dv.LastName,
                                        local_user_id = dv.GrantedUserId,
                                        extra = dv.BadgeNumber,
                                    };
                                    await InfinityServiceHandler.SendUser(InfinityServerUrl, InfinityServerToken, WallSerial, tmpUser);
                                }
                            }
                        }
                        else
                        {
                            UserInfo tmpUser = new UserInfo()
                            {
                                username = gu.Login,
                                password = gu.Password,
                                name = gu.FirstName + " " + gu.LastName,
                                local_user_id = gu.GrantedUserId,
                                extra = gu.BadgeNumber,
                            };
                            await InfinityServiceHandler.SendUser(InfinityServerUrl, InfinityServerToken, WallSerial, tmpUser);
                        }
                    }
                    if (!string.IsNullOrEmpty(ReactInfinityUrl)) // Phase 3 - get knwon user from server and store it
                    {
                        if (gu == null)
                        {
                            var result = await InfinityServiceForReactHandler.GetAllUsers(ReactInfinityUrl, ReactToken);
                            if (result)
                            {
                                if (InfinityServiceForReactHandler.LastUserInfo != null)
                                {
                                    using (var ctx = RemoteDatabase.GetDbContext())
                                    {
                                        foreach (var user in InfinityServiceForReactHandler.LastUserInfo.data)
                                        {
                                            var original = ctx.GrantedUsers.Where(u => u.Login == user.email).SingleOrDefault();
                                            if (original == null)  //need to create 
                                            {
                                                GrantedUser newUser = new GrantedUser()
                                                {
                                                    ServerGrantedUserId = 0,
                                                    Login = user.email,
                                                    Password = PasswordHashing.Sha256Of("123456"),
                                                    FirstName = user.firstName,
                                                    LastName = user.lastName,
                                                    BadgeNumber = user.badgeId,
                                                    UpdateAt = user.updatedAt,
                                                    UserRankId = 3,
                                                    userId = user._id,
                                                };
                                                ctx.GrantedUsers.Add(newUser);
                                                ctx.SaveChanges(); //to get new userID;
                                                if (user.fingerPrint != null)
                                                {
                                                    foreach (var fp in user.fingerPrint)
                                                    {
                                                        ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                                        {
                                                            Index = (int)fp.index,
                                                            GrantedUserId = newUser.GrantedUserId,
                                                            Template = fp.data
                                                        });
                                                    }
                                                }
                                                // Grant on all devices                                                
                                                var TmpDev = ctx.Devices.GetBySerialNumber(WallSerial);
                                                if (TmpDev != null)
                                                    ctx.GrantedAccesses.AddOrUpdateAccess(newUser, TmpDev, ctx.GrantTypes.All());
                                                 
                                            }
                                            else  //need to update
                                            {
                                                TimeSpan ts = user.updatedAt - original.UpdateAt;
                                                if (ts.TotalSeconds > 1.0)
                                                {

                                                    original.Login = user.email;
                                                    original.Password = PasswordHashing.Sha256Of("123456");
                                                    original.LastName = user.firstName;
                                                    original.FirstName = user.lastName;
                                                    original.BadgeNumber = user.badgeId;
                                                    original.UpdateAt = user.updatedAt;
                                                    original.userId = user._id;
                                                    ctx.Entry(original).State = EntityState.Modified;

                                                    if (original.Fingerprints != null)
                                                        ctx.Fingerprints.RemoveRange(ctx.Fingerprints.Where(x => x.GrantedUserId == original.GrantedUserId));
                                                    ctx.SaveChanges();
                                                    if (user.fingerPrint != null)
                                                    {
                                                        foreach (var fp in user.fingerPrint)
                                                        {
                                                            ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                                            {
                                                                Index = (int)fp.index,
                                                                GrantedUserId = original.GrantedUserId,
                                                                Template = fp.data
                                                            });
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                        ctx.SaveChanges();
                                    }
                                }
                            }
                        }
                        else
                        {
                            ReactUpdateUserBadgeAndFp ruubf = new ReactUpdateUserBadgeAndFp();
                            ruubf._id = gu.userId;
                            ruubf.badgeId = gu.BadgeNumber;
                            List<InfinityServiceForReact.Fingerprint> listFp = new List<InfinityServiceForReact.Fingerprint>();
                            if (gu.Fingerprints != null)
                            {
                                foreach (var fp in gu.Fingerprints)
                                {
                                    InfinityServiceForReact.Fingerprint tmpFp = new InfinityServiceForReact.Fingerprint()
                                    {
                                        index = fp.Index,
                                        data = fp.Template,
                                    };
                                    listFp.Add(tmpFp);
                                }
                            }

                            ruubf.fingerPrint = listFp.ToArray();
                            var result = await InfinityServiceForReactHandler.UpdateUser(ReactInfinityUrl, ReactToken, ruubf);
                            if (result)
                            {
                                if (InfinityServiceForReactHandler.LastReturnUpdateUser != null)
                                {
                                    if (InfinityServiceForReactHandler.LastReturnUpdateUser.status == false)
                                    {
                                        await mainview0.ShowMessageAsync("Error", "Unable to update user on server");
                                    }
                                }
                            }
                            else
                                await mainview0.ShowMessageAsync("Error", "Unable to update user on server");
                        }
                    }
                }
                catch (Exception error)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate User");
                        exp.ShowDialog();
                    });
                }
            });
            return true;
        }
        private void UpdateInfinityUsers()
        {
            if (!string.IsNullOrEmpty(InfinityServerUrl))
            {
                Task.Run(async () =>
            {
                try
                {
                    bool ret = await InfinityServiceHandler.GetRemoteUsers(InfinityServerUrl, InfinityServerToken, WallSerial);
                    if (ret)
                    {
                        using (var ctx = RemoteDatabase.GetDbContext())
                        {
                            foreach (var us in InfinityServiceHandler.LastRemoteUserInfo)
                            {
                                var original = ctx.GrantedUsers.GetByLogin(us.username);
                                if (original != null)
                                {
                                    original.Login = us.username;
                                    if (!string.IsNullOrWhiteSpace(us.password))
                                        original.Password = us.password;

                                    if (!string.IsNullOrWhiteSpace(us.name))
                                    {
                                        string[] tmpUser = us.name.Split(' ');
                                        if (tmpUser.Length == 2)
                                        {
                                            original.FirstName = tmpUser[0];
                                            original.LastName = tmpUser[1];
                                        }
                                    }
                                    original.BadgeNumber = us.badge;
                                    ctx.Entry(original).State = EntityState.Modified;
                                    ctx.SaveChanges();


                                    if (!string.IsNullOrWhiteSpace((string)us.fingerprints))
                                    {
                                        var fingerprints = ctx.Fingerprints.Where(fp => fp.GrantedUserId == original.GrantedUserId).ToList();
                                        if (fingerprints != null)
                                            foreach (var tmp in fingerprints)
                                                ctx.Fingerprints.Remove(tmp);
                                        ctx.SaveChanges();
                                        string[] tmpFp = us.fingerprints.ToString().Split(',');
                                        int nIndex = 0;
                                        if (tmpFp.Length > 0)
                                        {
                                            foreach (string newFp in tmpFp)
                                                ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                                {
                                                    Index = (int)nIndex++,
                                                    GrantedUserId = original.GrantedUserId,
                                                    Template = newFp
                                                });
                                            ctx.SaveChanges();
                                        }

                                    }

                                }
                                else
                                {
                                    string[] tmpUser = us.name.Split(' ');
                                    GrantedUser newUser = null;
                                    newUser = new GrantedUser()
                                    {
                                        ServerGrantedUserId = 0,
                                        Login = us.username,
                                        Password = us.password,
                                        FirstName = tmpUser.Length == 2 ? tmpUser[0] : us.name,
                                        LastName = tmpUser.Length == 2 ? tmpUser[1] : us.name,
                                        BadgeNumber = us.badge,
                                        UpdateAt = DateTime.Now,
                                        UserRankId = 3,
                                    };

                                    ctx.GrantedUsers.Add(newUser);
                                    ctx.SaveChanges();

                                    if (!string.IsNullOrWhiteSpace((string)us.fingerprints))
                                    {
                                        var fingerprints = ctx.Fingerprints.Where(fp => fp.GrantedUserId == (newUser.GrantedUserId)).ToList();
                                        if (fingerprints != null)
                                            foreach (var tmp in fingerprints)
                                                ctx.Fingerprints.Remove(tmp);
                                        ctx.SaveChanges();
                                        string[] tmpFp = us.fingerprints.ToString().Split(',');
                                        int nIndex = 0;
                                        if (tmpFp.Length > 0)
                                        {
                                            foreach (string newFp in tmpFp)
                                                ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                                {
                                                    Index = (int)nIndex++,
                                                    GrantedUserId = original.GrantedUserId,
                                                    Template = newFp
                                                });
                                            ctx.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception error)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Populate User");
                        exp.ShowDialog();
                    });
                }
            });
            }
        }
        private void ClearDatabase()
        {
            try
            {

                LogToFile.LogMessageToFile("------- Start Clear Database --------");
                using (var ctx = RemoteDatabase.GetDbContext())
                {
                    ctx.EventDrawerDetails.Clear();
                    ctx.Inventories.Clear();
                    ctx.InventoryProducts.Clear();
                    ctx.Products.Clear();
                    ctx.RfidTags.Clear();
                    ctx.SaveChanges();


                    //create empty scan for device
                    var newInventory = new SmartDrawerDatabase.DAL.Inventory
                    {
                        DeviceId = DevicesHandler.GetDeviceEntity().DeviceId,
                        AccessTypeId = 1,
                        TotalAdded = 0,
                        TotalPresent = 0,
                        TotalRemoved = 0,
                        InventoryDate = DateTime.Now
                    };
                    ctx.Inventories.Add(newInventory);
                    ctx.SaveChanges();

                    DevicesHandler.ReleaseDevices();
                    LogToFile.LogMessageToFile("------- End Clear Database --------");

                }
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End Clear Database --------");
            }
        }
        public async Task<bool> ProcessNewStone(List<string> TagList)
        {            
            await Task.Run(() =>
            {
                if (!string.IsNullOrEmpty(InfinityServerUrl))  // do for phase 2
                {
                    if ((InfinityServiceHandler.LastStonesListNotScan != null) && (InfinityServiceHandler.LastStonesListNotScan.Count() > 0))
                    {
                        foreach (string tagId in TagList)
                        {
                            StoneInfo sto = (from si in InfinityServiceHandler.LastStonesListNotScan
                                             where si.tag_id == tagId
                                             select si).SingleOrDefault();

                            if (sto != null)
                            {
                                using (var ctx = RemoteDatabase.GetDbContext())
                                {
                                    var rfidTag = ctx.RfidTags.FirstOrDefault(tag => tag.TagUid == sto.tag_id);
                                    if (rfidTag == null) // create tag with stone info
                                    {
                                        // Create Unknown tag
                                        RfidTag newTag = new RfidTag() { TagUid = sto.tag_id };
                                        ctx.RfidTags.Add(newTag);
                                        Product p = new Product()
                                        {
                                            RfidTag = newTag,
                                            ProductInfo0 = sto.report_number,
                                            ProductInfo1 = sto.carat_weight,
                                            ProductInfo2 = sto.shape,
                                            ProductInfo3 = sto.color,
                                            ProductInfo4 = sto.clarity,
                                            ProductInfo5 = sto.cut,
                                        };
                                        ctx.Products.Add(p);
                                        ctx.SaveChanges();
                                    }
                                    else //product to update it
                                    {
                                        Product pToUpdate = ctx.Products.Where(po => po.RfidTag.TagUid == sto.tag_id).Include(po => po.RfidTag).FirstOrDefault();
                                        if (pToUpdate != null)
                                        {
                                            pToUpdate.ProductInfo0 = sto.report_number;
                                            pToUpdate.ProductInfo1 = sto.carat_weight;
                                            pToUpdate.ProductInfo2 = sto.shape;
                                            pToUpdate.ProductInfo3 = sto.color;
                                            pToUpdate.ProductInfo4 = sto.clarity;
                                            pToUpdate.ProductInfo5 = sto.cut;
                                            ctx.Entry(pToUpdate).State = EntityState.Modified;
                                            ctx.SaveChanges();
                                        }
                                        else
                                        {
                                            Product p = new Product()
                                            {
                                                RfidTag = rfidTag,
                                                ProductInfo0 = sto.report_number,
                                                ProductInfo1 = sto.carat_weight,
                                                ProductInfo2 = sto.shape,
                                                ProductInfo3 = sto.color,
                                                ProductInfo4 = sto.clarity,
                                                ProductInfo5 = sto.cut,
                                            };
                                            ctx.Products.Add(p);
                                            ctx.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(ReactInfinityUrl)) //Do for react
                {
                    LogToFile.LogMessageToFile("------- Start Process new stone --------");

                    if ((InfinityServiceForReactHandler.LastSkuInfo != null) && (InfinityServiceForReactHandler.LastSkuInfo.data.Count() > 0))
                    {
                        using (var ctx2 = RemoteDatabase.GetDbContext())
                        {
                            foreach (string tagId in TagList)
                            {
                                SkuData sd = GetStoneInfoForReact(tagId);

                                if (sd != null)
                                {

                                    var rfidTag = ctx2.RfidTags.FirstOrDefault(tag => tag.TagUid == sd.rfId.rfid);
                                    if (rfidTag == null) // create tag with stone info
                                    {
                                        // Create Unknown tag
                                        RfidTag newTag = new RfidTag() { TagUid = sd.rfId.rfid };
                                        ctx2.RfidTags.Add(newTag);
                                        Product p = new Product()
                                        {
                                            RfidTag = newTag,
                                            ProductInfo0 = sd.clientRefId,
                                            ProductInfo1 = sd.weight.ToString("0.00"),
                                            ProductInfo2 = sd.shape,
                                            ProductInfo3 = sd.colorCategory,
                                            ProductInfo4 = sd.clarity,
                                            ProductInfo5 = sd.cut,
                                            ProductInfo6 = sd._id,
                                            ProductInfo7 = sd.colorType,
                                            ProductInfo8 = sd.infinityRefId,
                                            ProductInfo9 = sd.pwvImport,
                                            ProductInfo10 = sd.dmGuid,
                                        };
                                        ctx2.Products.Add(p);
                                        ctx2.SaveChanges();

                                    }
                                    else //product to update it - no udpate diamond info
                                    {

                                        Product pToUpdate = ctx2.Products.Where(po => po.RfidTag.TagUid == sd.rfId.rfid).Include(po => po.RfidTag).FirstOrDefault();
                                        if (pToUpdate != null)
                                        {
                                            LogToFile.LogMessageToFile("Product  found :" + sd.clientRefId);
                                            pToUpdate.ProductInfo0 = sd.clientRefId;
                                            pToUpdate.ProductInfo1 = sd.weight.ToString("0.00");
                                            pToUpdate.ProductInfo2 = sd.shape;
                                            pToUpdate.ProductInfo3 = sd.colorCategory;
                                            pToUpdate.ProductInfo4 = sd.clarity;
                                            pToUpdate.ProductInfo5 = sd.cut;
                                            pToUpdate.ProductInfo6 = sd._id;
                                            pToUpdate.ProductInfo7 = sd.colorType;
                                            pToUpdate.ProductInfo8 = sd.infinityRefId;
                                            pToUpdate.ProductInfo9 = sd.pwvImport;
                                            pToUpdate.ProductInfo10 = sd.dmGuid;

                                            ctx2.Entry(pToUpdate).State = EntityState.Modified;
                                            ctx2.SaveChanges();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            });
           
            LogToFile.LogMessageToFile("------- End Process new stone --------");
            return true;
        }
        private StoneInfo GetStoneInfo(string TagId)
        {
            if (InfinityServiceHandler.LastStonesListNotScan == null) return null;
            StoneInfo si = null;
            si = (from sto in InfinityServiceHandler.LastStonesListNotScan
                  where sto.tag_id == TagId
                  select sto).SingleOrDefault();

            return si;
        }
        private SkuData GetStoneInfoForReact(string TagId)
        {
            SkuData sd = null;
            if (InfinityServiceForReactHandler.LastSkuInfo != null)
            {
                sd = (from sto in InfinityServiceForReactHandler.LastSkuInfo.data
                      where sto.rfId.rfid == TagId
                      select sto).SingleOrDefault();
            }
            return sd;
        }
        Dictionary<string, int> UnknownTags = new Dictionary<string, int>();
      
        private void ProcessNewStone()
        {
            if (!string.IsNullOrEmpty(InfinityServerUrl))
            {
                if ((InfinityServiceHandler.LastStonesListNotScan != null) && (InfinityServiceHandler.LastStonesListNotScan.Count() > 0))
                {
                    foreach (var sto in InfinityServiceHandler.LastStonesListNotScan)
                    {
                        using (var ctx = RemoteDatabase.GetDbContext())
                        {
                            // find if Tag exist in local db
                            var rfidTag = ctx.RfidTags.FirstOrDefault(tag => tag.TagUid == sto.tag_id);
                            if (rfidTag == null) // create tag with stone info
                            {
                                /* Create Unknown tag*/
                                RfidTag newTag = new RfidTag() { TagUid = sto.tag_id };
                                ctx.RfidTags.Add(newTag);
                                Product p = new Product()
                                {
                                    RfidTag = newTag,
                                    ProductInfo0 = sto.report_number,
                                    ProductInfo1 = sto.carat_weight,
                                    ProductInfo2 = sto.shape,
                                    ProductInfo3 = sto.color,
                                    ProductInfo4 = sto.clarity,
                                    ProductInfo5 = sto.cut,
                                };
                                ctx.Products.Add(p);
                                ctx.SaveChanges();
                            }
                            else //product to update it
                            {
                                Product pToUpdate = ctx.Products.Where(po => po.RfidTag.TagUid == sto.tag_id).Include(po => po.RfidTag).FirstOrDefault();
                                if (pToUpdate != null)
                                {
                                    pToUpdate.ProductInfo0 = sto.report_number;
                                    pToUpdate.ProductInfo1 = sto.carat_weight;
                                    pToUpdate.ProductInfo2 = sto.shape;
                                    pToUpdate.ProductInfo3 = sto.color;
                                    pToUpdate.ProductInfo4 = sto.clarity;
                                    pToUpdate.ProductInfo5 = sto.cut;
                                    ctx.Entry(pToUpdate).State = EntityState.Modified;
                                    ctx.SaveChanges();
                                }
                                else
                                {
                                    Product p = new Product()
                                    {
                                        RfidTag = rfidTag,
                                        ProductInfo0 = sto.report_number,
                                        ProductInfo1 = sto.carat_weight,
                                        ProductInfo2 = sto.shape,
                                        ProductInfo3 = sto.color,
                                        ProductInfo4 = sto.clarity,
                                        ProductInfo5 = sto.cut,
                                    };
                                    ctx.Products.Add(p);
                                    ctx.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
        }
        private void ProcessUnreferencedStones(List<string> TagList, string firstname, string lastName)
        {
            if (!string.IsNullOrEmpty(InfinityServerUrl))
            {
                Task.Run(async () =>
            {
                AlertInfo alert = new AlertInfo();
                alert.timestamp = UnixTimeStamp.ConvertToUnixTimestamp(DateTime.Now.ToUniversalTime());
                alert.type = 2;
                alert.comment = lastName + ',' + firstname + ",";
                alert.comment += string.Join(",", TagList);
                await InfinityServiceHandler.SendAlert(InfinityServerUrl, InfinityServerToken, WallSerial, alert);
            });
            }
        }
        private InfinityService.EventInfo ProcessEvent(Inventory inv, bool bClosingBusiness, bool bOpeningBusiness, bool bIsClosingComplete)
        {
            // List<string> TagInwall = GetListOfallOtherdrawer(inv.DrawerNumber, inv.InventoryDate);

            InfinityService.EventInfo newEvent = new InfinityService.EventInfo();
            newEvent.reader = new Reader { serial = inv.Device.DeviceSerial };
            if (inv.Device.DeviceTypeId == 15) // un wall
                newEvent.reader.drawer = inv.DrawerNumber;
            else
                newEvent.reader.drawer = null;

            if (inv.GrantedUser != null)
            {
                newEvent.user = new User() { id = inv.GrantedUserId.Value, name = inv.GrantedUser.FirstName + " " + inv.GrantedUser.LastName };
            }
            else
            {
                newEvent.user = new User() { id = 999999, name = "Unknown User" };
            }

            newEvent.timestamp = UnixTimeStamp.ConvertToUnixTimestamp(inv.InventoryDate.ToUniversalTime());


            List<string> lstArrival = new List<string>();
            List<string> lstIn = new List<string>();
            List<string> lstOut = new List<string>();
            List<string> lstInventory = new List<string>();

            foreach (var tag in inv.InventoryProducts)
            {
                switch (tag.MovementType)
                {
                    case (int)MovementType.Added:
                        lstInventory.Add(tag.RfidTag.TagUid);
                        if (UnknownTags.Keys.Contains(tag.RfidTag.TagUid))
                        {
                            if (!lstArrival.Contains(tag.RfidTag.TagUid))
                                lstArrival.Add(tag.RfidTag.TagUid);
                        }
                        else
                        {
                            if (!lstIn.Contains(tag.RfidTag.TagUid))
                                lstIn.Add(tag.RfidTag.TagUid);
                        }
                        break;
                    case (int)MovementType.Removed:
                        if (!lstOut.Contains(tag.RfidTag.TagUid))
                            lstOut.Add(tag.RfidTag.TagUid);
                        break;
                    case (int)MovementType.Present:
                        if (!lstInventory.Contains(tag.RfidTag.TagUid))
                            lstInventory.Add(tag.RfidTag.TagUid);
                        break;
                }
            }

            /* if (TagInwall.Count > 0)
             {
                 foreach (string uid in TagInwall)
                 {
                     if (!lstInventory.Contains(uid))
                     lstInventory.Add(uid);
                 }
             }*/

            List<Event> lstEvent = new List<Event>();

            if (lstArrival.Count > 0)
            {
                Event ArrivalEvent = new Event() { EventType = EventType.ARRIVAL, stones = lstArrival.ToArray() };
                lstEvent.Add(ArrivalEvent);
            }
            if (lstIn.Count > 0)
            {
                Event InEvent = new Event() { EventType = EventType.IN, stones = lstIn.ToArray() };
                lstEvent.Add(InEvent);
            }
            if (lstOut.Count > 0)
            {
                if (bIsClosingComplete)
                {
                    Event OutEvent = new Event() { EventType = EventType.VAULT, stones = lstOut.ToArray() };
                    lstEvent.Add(OutEvent);
                }
                else
                {
                    Event OutEvent = new Event() { EventType = EventType.OUT, stones = lstOut.ToArray() };
                    lstEvent.Add(OutEvent);
                }
            }
            if (bIsClosingComplete)
            {
                if (lstInventory.Count > 0)
                {
                    Event InventoryEvent = new Event() { EventType = EventType.CLOSEBIZ, stones = lstInventory.ToArray() };
                    lstEvent.Add(InventoryEvent);
                }
                else
                {
                    Event InventoryEvent = new Event() { EventType = EventType.CLOSEBIZ };
                    lstEvent.Add(InventoryEvent);
                }
            }
            else if (bOpeningBusiness)
            {
                if (lstInventory.Count > 0)
                {
                    Event InventoryEvent = new Event() { EventType = EventType.OPENBIZ, stones = lstInventory.ToArray() };
                    lstEvent.Add(InventoryEvent);
                }
                else
                {
                    Event InventoryEvent = new Event() { EventType = EventType.OPENBIZ };
                    lstEvent.Add(InventoryEvent);
                }
            }
            else if (bClosingBusiness)
            {
                if (lstInventory.Count > 0)
                {
                    Event InventoryEvent = new Event() { EventType = EventType.CLOSEBIZ, stones = lstInventory.ToArray() };
                    lstEvent.Add(InventoryEvent);
                }
                else
                {
                    Event InventoryEvent = new Event() { EventType = EventType.CLOSEBIZ };
                    lstEvent.Add(InventoryEvent);
                }
            }
            else
            {
                if (bFirstScan)
                {
                    bFirstScan = false;
                    if (lstInventory.Count > 0)
                    {
                        Event InventoryEvent = new Event() { EventType = EventType.RESTART, stones = lstInventory.ToArray() };
                        lstEvent.Add(InventoryEvent);
                    }
                    else
                    {
                        Event InventoryEvent = new Event() { EventType = EventType.RESTART };
                        lstEvent.Add(InventoryEvent);
                    }
                }
                else
                {
                    if (lstInventory.Count > 0)
                    {
                        Event InventoryEvent = new Event() { EventType = EventType.INVENTORY, stones = lstInventory.ToArray() };
                        lstEvent.Add(InventoryEvent);
                    }
                    else
                    {
                        Event InventoryEvent = new Event() { EventType = EventType.INVENTORY };
                        lstEvent.Add(InventoryEvent);
                    }
                }
            }
            newEvent.events = lstEvent.ToArray();
            return newEvent;

        }
        private async void ProcessInfinity(int drawerId)
        {
            try
            {
                LogToFile.LogMessageToFile("------- Start Process infinity  --------");
                if (!NetworkStatus)
                {
                    LogToFile.LogMessageToFile("------- Quit Process infinity - no Network  --------");
                    return;
                }

                using (var ctx = RemoteDatabase.GetDbContext())
                {
                    var NotSendInventory = ctx.Inventories.GetNotNotifiedInventory(drawerId);
                    if (NotSendInventory != null)
                    {
                        LogToFile.LogMessageToFile("Nb inventory to Proceed : " + NotSendInventory.Count);
                        foreach (Inventory inv in NotSendInventory)
                        {
                            // Send it
                            InfinityService.EventInfo tmpEvent = ProcessEvent(inv, IsClosingBusiness, IsOpeningBusiness, IsClosingCompleted);
                            if (tmpEvent.events.Count() > 0)
                            {
                                bool result = await InfinityServiceHandler.SendEvents(InfinityServerUrl, InfinityServerToken, WallSerial, tmpEvent);

                                if (result)
                                {
                                    if (InfinityServiceHandler.LastReturnInfo != null)
                                    {
                                        if (InfinityServiceHandler.LastReturnInfo.setOperational == 1)
                                        {
                                            HideProcessWindow();
                                            IsOpeningBusiness = false;
                                            IsClosingBusiness = false;
                                            IsClosingCompleted = false;
                                        }
                                        else if (InfinityServiceHandler.LastReturnInfo.deviceStatus != "operational")
                                        {
                                            if ((!IsClosingBusiness) && (!IsOpeningBusiness) && !(IsClosingCompleted))
                                            {
                                                IsOpeningBusiness = false;
                                                IsClosingBusiness = true;
                                                IsClosingCompleted = true;
                                            }
                                        }
                                    }
                                    ctx.Inventories.UpdateInventoryForNotification(inv);
                                    ctx.SaveChanges();
                                }
                                else
                                {
                                    // error network 
                                    LogToFile.LogMessageToFile("------- Network error --------");
                                    NetworkStatus = false;
                                    return;
                                }
                            }
                            else // If no event, no tag in scan, not send it but notify local
                            {
                                ctx.Inventories.UpdateInventoryForNotification(inv);
                                ctx.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExceptionMessageBox msg = new ExceptionMessageBox(ex, "Error Send Events");
                    msg.Show();
                });
            }
        }

        //phase 3

        private async Task<bool> ReactLogin()
        {
            bool ret = false;
            if (await InfinityServiceForReactHandler.Login(ReactInfinityUrl, ReactDeviceUser, ReactDevicePassword))
            {
                if (InfinityServiceForReactHandler.LastReturnLogin != null)
                {
                    ReactToken = InfinityServiceForReactHandler.LastReturnLogin.data.token;
                    return true;
                }
            }
            return ret;
        }

        private async Task<bool> ReactRegisterDevices()
        {
            bool ret = true;             
         
            bool tmpRet = await InfinityServiceForReactHandler.RegisterDevice(ReactInfinityUrl, ReactToken, WallSerial);
            if (!tmpRet)
            {
                ret = false;
            }
            else
            {
                if (InfinityServiceForReactHandler.LastReturnRegisterDevice != null)
                {
                    if (!string.IsNullOrEmpty(InfinityServiceForReactHandler.LastReturnRegisterDevice.data.token))
                        DevicesHandler.ReactInfinityToken = InfinityServiceForReactHandler.LastReturnRegisterDevice.data.token;
                    else
                        ret = false;
                }
                else
                    ret = false;
            }
            
            
            return ret;
        }

        public Socket clientSocket;
        public bool IsClientSocketConnected = false;
        public void CreateSocketIOClient(string url)
        {

            clientSocket = IO.Socket(url);
            clientSocket.On(Socket.EVENT_CONNECT, () =>
            {
                IsClientSocketConnected = true;
                LogToFile.LogMessageToFile("Info React - Connect to " + url);
                NetworkStatus = true;
            });
            clientSocket.On(Socket.EVENT_ERROR, (data) =>
            {
                LogToFile.LogMessageToFile("Receive Error : " + data);
            });
            clientSocket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                LogToFile.LogMessageToFile("Receive Connect Error : " + data);
            });
            clientSocket.On(Socket.EVENT_CONNECT_TIMEOUT, (data) =>
            {
                LogToFile.LogMessageToFile("Receive Connect Timeout : " + data);
            });
            clientSocket.On(Socket.EVENT_RECONNECT, (data) =>
            {
                LogToFile.LogMessageToFile("Receive Reconnect : " + data);
            });

            clientSocket.On(Socket.EVENT_DISCONNECT, () =>
            {
                IsClientSocketConnected = false;
                IsDeviceRegistered = false;
                LogToFile.LogMessageToFile("Info React - Disconnect from " + url);
                NetworkStatus = false;
            });

            clientSocket.On("success", (data) =>
            {
                LogToFile.LogMessageToFile("Receive Success : " + data);
            });
            clientSocket.On("failure", (data) =>
            {
                LogToFile.LogMessageToFile("Receive Failure : " + data);
            });
            /*  clientSocket.On("diamondMatchRegistration", (data) =>
              {
                  LogToFile.LogMessageToFile("Receive diamondMatchRegistration : " + data);
                  _lastDMRegistrationData = RetDMSocket.DeserializedJsonList(data.ToString());
                  eventDMRegistration.Set();

              });
              clientSocket.On("diamondMatch", (data) =>
              {
                  LogToFile.LogMessageToFile("Receive  diamondMatch : " + data);
                  _lastDMMatchData = RetDMSocket.DeserializedJsonList(data.ToString());
                  eventDMMatch.Set();
              });*/

            clientSocket.On("ledTrigger", (data) =>
            {
                LogToFile.LogMessageToFile("Receive  ledTrigger : " + data);

            });
            clientSocket.On("registerDevice", (data) =>
            {
                LogToFile.LogMessageToFile("Receive  registerDevice : " + data);
                _lastRegistrationDeviceData = RetDMSocket2.DeserializedJsonList(data.ToString());              
                eventRegistrationDevice.Set();
            });

        }

        public bool IsDeviceRegistered = false;
        private static EventWaitHandle eventRegistrationDevice = new EventWaitHandle(false, EventResetMode.AutoReset);
        private RetDMSocket2 _lastRegistrationDeviceData = null;
        //private ReturnRegisterDevice _lastRegistrationDeviceData = null;
        public bool registerDevice()
        {
            IsDeviceRegistered = false ;
            RegisterDeviceInfo rdi = new RegisterDeviceInfo() { serialNumber = WallSerial };
            if (EmitRegisterDevice(rdi))
            {
                DevicesHandler.ReactInfinityToken = _lastRegistrationDeviceData.body.token;              
                IsDeviceRegistered = true;
            }
            else
                LogToFile.LogMessageToFile("Info React - Unable to register device ");
            return IsDeviceRegistered;
        }
        public bool EmitRegisterDevice(RegisterDeviceInfo rdi)
        {
            if ((clientSocket == null) || (!IsClientSocketConnected))
                CreateSocketIOClient(ReactInfinityUrl);

            if ((clientSocket != null) && (IsClientSocketConnected))
            {
                _lastRegistrationDeviceData = null;
                eventRegistrationDevice.Reset();
                string data = RegisterDeviceInfo.SerializedJsonAlone(rdi);
                LogToFile.LogMessageToFile("Send React - registerDevice : " + data);
                var ret = clientSocket.Emit("registerDevice", data);

                if (eventRegistrationDevice.WaitOne(5000, false))
                {
                    if (_lastRegistrationDeviceData != null)
                    {
                        if (_lastRegistrationDeviceData.status == "success")
                            return true;
                        else
                            return false;
                    }
                }
                else
                    return false;
            }
            return false;
        }
       

        private async void ProcessInfinityReact(int drawerId)
        {
            try
            {               
                using (var ctx = RemoteDatabase.GetDbContext())
                { 
                    var NotSendInventory = ctx.Inventories.GetNotNotifiedInventory(drawerId);
                    if (NotSendInventory != null)
                    {
                        foreach (Inventory inv in NotSendInventory)
                        {
                            // Send it
                            ReactEventInfo tmpEvent = ProcessEventForReact(inv);
                            if (tmpEvent.events.Count() > 0)
                            {
                                if ((clientSocket == null) || (!IsClientSocketConnected))
                                    CreateSocketIOClient(ReactInfinityUrl);

                                if ((clientSocket != null) && (IsClientSocketConnected))
                                {

                                    string data = ReactEventInfo.SerializedJsonAlone(tmpEvent);
                                    //string data2 = JsonConvert.SerializeObject(tmpEvent);
                                    LogToFile.LogMessageToFile("Send React - Event createRawActivity : " + data);
                                    var ret = clientSocket.Emit("createRawActivity", data);
                                }

                                ctx.Inventories.UpdateInventoryForNotification(inv);
                                ctx.SaveChanges();

                            }
                            else // If no event, no tag in scan, not send it but notify local
                            {
                                ctx.Inventories.UpdateInventoryForNotification(inv);
                                ctx.SaveChanges();
                            }
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ExceptionMessageBox msg = new ExceptionMessageBox(ex, "Error Process Infinity For React");
                    msg.Show();
                });
            }
        }

        private ReactEventInfo ProcessEventForReact(Inventory inv)
        {
            ReactEventInfo newEvent = new ReactEventInfo();
            newEvent.token = DevicesHandler.ReactInfinityToken;
            newEvent.inventory_id = inv.InventoryId;
            newEvent.reader = new ReactReader { serial = WallSerial };
            if (inv.Device.DeviceTypeId == 15) // un wall
                newEvent.reader.drawer = inv.DrawerNumber;
            else
                newEvent.reader.drawer = 0;

            if (inv.GrantedUser != null)
            {
                newEvent.user = inv.GrantedUser.userId;
            }
            else
            {
                newEvent.user = "5f92d7c7f9cdf76de85734a3";
            }

            newEvent.timestamp = ReactUnixTimeStamp.ConvertToUnixTimestamp(inv.InventoryDate.ToUniversalTime());

            List<string> lstIn = new List<string>();
            List<string> lstOut = new List<string>();
            List<string> lstInventory = new List<string>();

            foreach (var tag in inv.InventoryProducts)
            {
                switch (tag.MovementType)
                {
                    case (int)MovementType.Added:
                        lstInventory.Add(tag.RfidTag.TagUid);
                        lstIn.Add(tag.RfidTag.TagUid);
                        break;
                    case (int)MovementType.Removed:
                        lstOut.Add(tag.RfidTag.TagUid);
                        break;
                    case (int)MovementType.Present:
                        lstInventory.Add(tag.RfidTag.TagUid);
                        break;

                }
            }
            List<ReactEvent> lstEvent = new List<ReactEvent>();
            //if (lstIn.Count > 0)
            if (1 == 1)
            {
                ReactEvent InEvent = new ReactEvent() { EventType = ReactEventType.IN, stones = lstIn.ToArray() };
                lstEvent.Add(InEvent);
            }

            //if (lstOut.Count > 0)
            if (1 == 1)
            {
                ReactEvent OutEvent = new ReactEvent() { EventType = ReactEventType.OUT, stones = lstOut.ToArray() };
                lstEvent.Add(OutEvent);
            }

            // if (lstInventory.Count > 0)
            if (1 == 1)
            {
                ReactEvent InventoryEvent = new ReactEvent() { EventType = ReactEventType.INVENTORY, stones = lstInventory.ToArray() };
                lstEvent.Add(InventoryEvent);
            }


            newEvent.events = lstEvent.ToArray();
            return newEvent;
        }



        #endregion
        #region Device

        int CptErrorRfid = 0;

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
                RfidError = false;

                for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                {
                   
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
                                DevicesHandler.DrawerStatus[loop] = DrawerStatusList.ScanPending;
                                DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                                BrushDrawer[loop] = _borderScanPending;
                                bFirstScan = true;
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
        private async void DevicesHandler_DrawerClosed(object sender, DrawerEventArgs e)
        {
            LastDeviceActionTime = DateTime.Now;
            if (IsInPutItemFastMode)
            {
                try
                {
                    FastModeContinuousReading = false;
                    DevicesHandler.StopScan(e.DrawerId);
                    Thread.Sleep(500);
                    DevicesHandler.ProcessEndInventory();                                    
                    InventoryHandler.HandleNewScanCompleted(e.DrawerId);                             
                    PendingFastModeOperation = false;

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

                    bDrawerToRefreshLight[e.DrawerId] = false;
                    if (DevicesHandler.IsDrawerWaitScan[e.DrawerId])
                    {
                        DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.ScanPending;
                        BrushDrawer[e.DrawerId] = _borderScanPending;
                    }
                    else
                    {
                        DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                        BrushDrawer[e.DrawerId] = _borderReady;
                    }
                    DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                    CountTotalStones();
                    bNeedUpdateCriteria = true;


                }

                catch (Exception error)
                {
                    await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Drawer Closed");
                        exp.ShowDialog();
                    }));
                }

                finally
                {
                    if (!ScanTimer.IsEnabled)
                    {
                        ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                        ScanTimer.IsEnabled = true;
                        ScanTimer.Start();
                    }
                }
            }
            else
            {
                try
                {
                    //Wait end of light process or recheck
                    if ((InLightOrRecheckprocess) || (_recheckLightDrawer != -1))
                    {
                        do
                        {
                            Thread.Sleep(1000);
                        }
                        while (InLightOrRecheckprocess);
                    }

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

                    bDrawerToRefreshLight[e.DrawerId] = false;

                    if ((SelectedCassette != null) && (SelectedCassette.ListControlNumber.Count > 0))
                    {
                        if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InLight)
                        {
                            bDrawerToRefreshLight[e.DrawerId] = false;
                            DevicesHandler.StopLighting(e.DrawerId);
                            _recheckLightDrawer = e.DrawerId;

                            wallStatus = "Recheck stones in drawer " + e.DrawerId;
                            if (DevicesHandler.IsDrawerWaitScan[e.DrawerId])
                            {
                                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.ScanPending;
                                BrushDrawer[e.DrawerId] = _borderScanPending;
                            }
                            else
                            {
                                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                                BrushDrawer[e.DrawerId] = _borderReady;
                            }
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                        }
                        else
                        {
                            if (DevicesHandler.IsDrawerWaitScan[e.DrawerId])
                            {
                                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.ScanPending;
                                BrushDrawer[e.DrawerId] = _borderScanPending;
                            }
                            else
                            {
                                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                                BrushDrawer[e.DrawerId] = _borderReady;
                            }
                            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                        }
                    }
                    else
                    {
                        if (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.InLight)
                            DevicesHandler.StopLighting(e.DrawerId);
                        if (DevicesHandler.IsDrawerWaitScan[e.DrawerId])
                        {
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.ScanPending;
                            BrushDrawer[e.DrawerId] = _borderScanPending;
                        }
                        else
                        {
                            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                            BrushDrawer[e.DrawerId] = _borderReady;
                        }
                        DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];

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
                        DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.ScanPending;
                        BrushDrawer[e.DrawerId] = _borderScanPending;
                        DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                    }
                }

                catch (Exception error)
                {
                    await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in Drawer Closed");
                        exp.ShowDialog();
                    }));
                }

                finally
                {
                    if (!ScanTimer.IsEnabled)
                    {
                        ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                        ScanTimer.IsEnabled = true;
                        ScanTimer.Start();
                    }

                    if ((SelectedTabIndex == 1) && (mainview0.tiBarcodeMode.Visibility == Visibility.Visible))
                    {
                        SelectionLifeTimeTimer.IsEnabled = true;
                        SelectionLifeTimeTimer.Stop();
                        SelectionLifeTimeTimer.Start();
                    }
                }
            }
        }    
        private async void DevicesHandler_DrawerOpened(object sender, DrawerEventArgs e)
        {
            LastDeviceActionTime = DateTime.Now;
            while (PendingFastModeOperation == true)
                Thread.Sleep(1000);

            if (IsInPutItemFastMode)
            {
                PendingFastModeOperation = true;
                IsAutoLightDrawerChecked = false;
                DevicesHandler.IsDrawerWaitScan[e.DrawerId] = true;
                try
                {
                    _lastDrawerOpen = e.DrawerId;
                    if (DevicesHandler.DevicesConnected)
                        DevicesHandler.lockAllTags(e.DrawerId);
                    Thread.Sleep(500);
                    FastModeContinuousReading = true;
                    DevicesHandler.lstAcumulate.Clear();
                    DevicesHandler.bNeedUpdateAccumulationInventory = false;
                    DevicesHandler.InitialDataForAccumulate[e.DrawerId] = CloneReaderData.CloneObject(DevicesHandler.DrawerInventoryData[e.DrawerId]);
                    DevicesHandler.StartManualScan(e.DrawerId, false);
                }
                catch (Exception error)
                {
                    await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                    {
                        ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in DrawerOpened");
                        exp.ShowDialog();
                    }));
                }
                finally
                {
                    if (!ScanTimer.IsEnabled)
                    {
                        ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                        ScanTimer.IsEnabled = true;
                        ScanTimer.Start();
                    }
                }
            }
            else
            {
                try
                {

                    // wait open drawer to let time automatic trimming (max field for comfirmation)
                    Thread.Sleep(1000);
                    if (_lastDrawerOpen == -1)
                    {
                        //Wait end of light process or recheck
                        if ((InLightOrRecheckprocess) || (_recheckLightDrawer != -1))
                        {
                            do
                            {
                                Thread.Sleep(1000);
                            }
                            while (InLightOrRecheckprocess);
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
                        await Task.Factory.StartNew(() =>
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
                        //Wait end of light process or recheck
                        if ((InLightOrRecheckprocess) || (_recheckLightDrawer != -1))
                        {
                            do
                            {
                                Thread.Sleep(1000);
                            }
                            while (InLightOrRecheckprocess);
                        }

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
                            else if ((DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.Ready) || (DevicesHandler.DrawerStatus[e.DrawerId] == DrawerStatusList.ScanPending))
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


                            await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                            {
                                string info = string.Format("Drawer {0} already open - please Close drawer {1} to continue", _lastDrawerOpen, e.DrawerId);
                                mainview0.ShowMessageAsync("Wall Information", info);
                            }));
                        }
                    }


                }
                catch (Exception error)
                {
                    await mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                   {
                       ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in DrawerOpened");
                       exp.ShowDialog();
                   }));
                }
                finally
                {
                    if (!ScanTimer.IsEnabled)
                    {
                        ScanTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
                        ScanTimer.IsEnabled = true;
                        ScanTimer.Start();
                    }
                    if ((SelectedTabIndex == 1) && (mainview0.tiBarcodeMode.Visibility == Visibility.Visible))
                    {
                        SelectionLifeTimeTimer.IsEnabled = false;
                        SelectionLifeTimeTimer.Stop();
                    }
                }
            }

        }
        private void DeviceHandler_ScanStarted(object sender, DrawerEventArgs e)
        {
            try
            {
                RfidError = false;
                CptErrorRfid = 0;
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
            wallStatus = "Drawer " + e.DrawerId + " failed scan started";
            /*try
            {
                if (CptErrorRfid == 0 ) //first error try to relaunch scan
                {
                    CptErrorRfid++;
                    Thread.Sleep(500);
                    wallStatus = "Drawer " + e.DrawerId + " failed scan started " + CptErrorRfid;
                    if (DevicesHandler.Device.IsConnected)
                         DevicesHandler.StartManualScan(e.DrawerId, !IsInPutItemFastMode);
                }
                else
                {
                    RfidError = true;
                    wallStatus = "Drawer " + e.DrawerId + " failed scan started";
                    if (DevicesHandler.DrawerStatus[e.DrawerId] != DrawerStatusList.Open)
                    {
                        DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.NotReady;
                        DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                    }
                    BrushDrawer[e.DrawerId] = _borderNotReady;
                }
               
            }
            catch (Exception error)
            {
                mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error in FailStartSscan");
                    exp.ShowDialog();
                }));
            }*/
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
                    #if UseInfinity
                    if (!string.IsNullOrEmpty(InfinityServerUrl))
                        ProcessInfinity(e.DrawerId);
                    if (!string.IsNullOrEmpty(ReactInfinityUrl))
                        ProcessInfinityReact(e.DrawerId);
                    #endif

                });
                DevicesHandler.IsDrawerWaitScan[e.DrawerId] = false;
                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.Ready;
                DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                BrushDrawer[e.DrawerId] = _borderReady;
                CountTotalStones();
                bNeedUpdateCriteria = true;
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
                if (IsInPutItemFastMode) return;

                wallStatus = "Drawer " + e.DrawerId + " scan cancelled";
                DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.ScanPending;
                DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
                BrushDrawer[e.DrawerId] = _borderScanPending;

                DevicesHandler.DrawerTagQty[e.DrawerId] = DevicesHandler.DrawerInventoryData[e.DrawerId].strListTag.Count;
                DrawerTagQty[e.DrawerId] = DevicesHandler.DrawerTagQty[e.DrawerId].ToString("000");
                CountTotalStones();

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
            if (IsInPutItemFastMode)
            {      
                DevicesHandler.DrawerTagQty[e.DrawerId] = DevicesHandler.InitialDataForAccumulate[e.DrawerId].strListTag.Count;
                DrawerTagQty[e.DrawerId] = DevicesHandler.DrawerTagQty[e.DrawerId].ToString();
                CountTotalStones();
            }
            else
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
        }
        private void DevicesHandler_ScanAccucompleted(object sender, DrawerEventArgs e)
        {
            Thread.Sleep(50);
            if (FastModeContinuousReading)
                DevicesHandler.StartManualScan(e.DrawerId, false);
           
        }
        private void DevicesHandler_ScanAccuStarted(object sender, DrawerEventArgs e)
        {
            RfidError = false;
            _autoLockCpt = 120;
            wallStatus = "Drawer " + e.DrawerId + " scan started";
            DevicesHandler.DrawerStatus[e.DrawerId] = DrawerStatusList.InScan;
            DrawerStatus[e.DrawerId] = DevicesHandler.DrawerStatus[e.DrawerId];
            BrushDrawer[e.DrawerId] = _borderInScan;
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
            int cptDrawerReady = 0;

            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if ((DevicesHandler.DrawerStatus[loop] == DrawerStatusList.Ready) || (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.ScanPending))
                    cptDrawerReady++;
            }
            return cptDrawerReady == DevicesHandler.NbDrawer ? true : false;
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
                    DevicesHandler.DrawerStatus[loop] = DrawerStatusList.ScanPending;
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
                    BrushDrawer[loop] = _borderScanPending;
                    //DrawerTagQty[loop] = DevicesHandler.GetTagFromDictionnary(1, DevicesHandler.ListTagPerPreviousDrawer).Count.ToString();
                    DevicesHandler.DrawerTagQty[loop] = DevicesHandler.DrawerInventoryData[loop].strListTag.Count;
                    DrawerTagQty[loop] = DevicesHandler.DrawerTagQty[loop].ToString("000");

                }
            }
            CountTotalStones();
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
                    if (DevicesHandler.IsDrawerWaitScan[loop])
                    {
                        DevicesHandler.DrawerStatus[loop] = DrawerStatusList.ScanPending;
                        BrushDrawer[loop] = _borderScanPending;
                    }
                    else
                    {
                        DevicesHandler.DrawerStatus[loop] = DrawerStatusList.Ready;
                        BrushDrawer[loop] = _borderReady;
                    }
                    DrawerStatus[loop] = DevicesHandler.DrawerStatus[loop];
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
                            BrushDrawer[loop] = _borderReadyToPull;
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

        private void ReadDrawerDescription()
        {
            const string DirectoryName = @"C:\Temp\SmartDrawerLog\";
            string FileName = "DescDrawer.json";
            string PathFile = (string.Format("{0}{1}", DirectoryName, FileName));
            if (File.Exists(PathFile))
            {
                try
                {
                    Dictionary<string, string> Dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(PathFile));

                    if (Dictionary != null)
                    {
                        if (Dictionary.ContainsKey("1"))
                            descDrawer1 = Dictionary["1"];
                        if (Dictionary.ContainsKey("2"))
                            descDrawer2 = Dictionary["2"];
                        if (Dictionary.ContainsKey("3"))
                            descDrawer3 = Dictionary["3"];
                        if (Dictionary.ContainsKey("4"))
                            descDrawer4 = Dictionary["4"];
                        if (Dictionary.ContainsKey("5"))
                            descDrawer5 = Dictionary["5"];
                        if (Dictionary.ContainsKey("6"))
                            descDrawer6 = Dictionary["6"];
                        if (Dictionary.ContainsKey("7"))
                            descDrawer7 = Dictionary["7"];
                    }

                }
                catch
                {

                }
            }
        }

        private void InitValue()
        {

            ReadDrawerDescription();

            DevicesHandler.DeviceConnected += DeviceHandler_DeviceConnected;
            DevicesHandler.DeviceDisconnected += DeviceHandlerOnDeviceDisconnected;
            DevicesHandler.TagRead += DeviceHandler_TagRead;
            DevicesHandler.ScanAccuStarted += DevicesHandler_ScanAccuStarted;
            DevicesHandler.ScanStarted += DeviceHandler_ScanStarted;
            DevicesHandler.ScanCompleted += DeviceHandler_ScanCompleted;
            DevicesHandler.ScanAccucompleted += DevicesHandler_ScanAccucompleted;
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

            LastDeviceActionTime = DateTime.Now;
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
            //Properties.Settings.Default.Reset();

#if (IsTiffany)
{
    SelectedTabIndex = 1;
}
#else
{
    SelectedTabIndex = 0;
}
#endif

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }          

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
            searchTxtBarcodeCR = new RelayCommand(() => searchTxtBarcodeCRfn());
           btClearSelection = new RelayCommand(() => ClearSelection());
            LogoutCommand = new RelayCommand(() => logout());
            btLightFilteredTagSelection = new RelayCommand(() => LightSelectionFromList());
            btRemoveSelection = new RelayCommand(() => removeSelection());
            BtRefreshSelection = new RelayCommand(() => refreshSelection());
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

            if (SelectedTabIndex == 4) //in admin mode
            {
                if (EditedUser != null)
                {
                    EditedUser.BadgeId = badgeID;
                    RaisePropertyChanged(() => EditedUser);
                }
            }
            else
            {
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
#if (IsTiffany)
            {
                mainview0.tiAdminMode.Visibility = Visibility.Visible;
                mainview0.tiBarcodeMode.Visibility = Visibility.Visible;
                mainview0.tiDrawerMode.Visibility = Visibility.Collapsed;
                mainview0.tiSelectionMode.Visibility = Visibility.Collapsed;
                mainview0.tiInventoryMode.Visibility = Visibility.Collapsed;
            }
#else
            {
                mainview0.tiAdminMode.Visibility = Visibility.Visible;
                mainview0.tiBarcodeMode.Visibility = Visibility.Collapsed;
                mainview0.tiDrawerMode.Visibility = Visibility.Collapsed;
                mainview0.tiSelectionMode.Visibility = Visibility.Visible;
                mainview0.tiInventoryMode.Visibility = Visibility.Collapsed;

                using (var ctx = RemoteDatabase.GetDbContext())
                {
                    int nbCol = ctx.Columns.Count();
                    if (nbCol > 1)
                        mainview0.tiInventoryMode.Visibility = Visibility.Visible;
                }
            }
#endif


            btUserVisibility = Visibility.Collapsed;
            btAdminVisibility = Visibility.Hidden;
            isAdmin = false;

            NetworkStatus = false;
            GpioStatus = false;
            RfidStatus = false;

            dayDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");
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

            SelectionLifeTimeTimer = new DispatcherTimer();
            SelectionLifeTimeTimer.Interval = new TimeSpan(0, 1, 0);
            SelectionLifeTimeTimer.Tick += SelectionLifeTimeTimer_Tick;
            SelectionLifeTimeTimer.IsEnabled = false;
            SelectionLifeTimeTimer.Stop();

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