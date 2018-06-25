using SDK_SC_RfidReader;
using SecurityModules.FingerprintReader;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.StaticHelpers;
using SmartDrawerWpfApp.StaticHelpers.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model.DeviceModel
{
    public class DevicesHandler
    {
        private static SmartDrawerDatabase.DAL.Device _deviceEntity;
        private static Object thisLock = new Object();

        public const int NbDrawer = 7;
        public const int NbInventoryToKeep = 50;
        public static Dictionary<string, int> DeviceList { get; private set; }

        private static bool isVersionV3 = true;

        public static RfidReader Device;
        public static FingerprintReader FPReader { get; private set; }
        public static ReaderData[] DrawerInventoryData = new ReaderData[NbDrawer + 1];        

        public static GpioCardLib GpioCardObject = new GpioCardLib();
        //private static clBadgeReader AccessBadgeObject;

        private static string[] _DrawerStatus = new string[NbDrawer + 1];
        public static string[] DrawerStatus
        {
            get { return _DrawerStatus; }
            set { _DrawerStatus = value; }
        }

        private static int[] _DrawerTagQty = new int[NbDrawer + 1];
        public static int[] DrawerTagQty
        {
            get { return _DrawerTagQty; }
            set { _DrawerTagQty = value; }
        }

        private static int _CurrentActiveRfidDrawer = 0;
        public static int CurrentActiveRfidDrawer
        {
            get { return _CurrentActiveRfidDrawer; }
            set { _CurrentActiveRfidDrawer = value; }
        }

        public static string LastScanAccessTypeName { get; set; }
        public static DateTime LastScanTime = DateTime.Now;

        private static int _LastDrawerOpened = 0;
        private static int _LastDrawerClosed = 0;

        private static bool isWallLocked = true;
        public static bool IsWallLocked { get { return isWallLocked; } }


        public static void ResetDEviceEntity()
        {
            _deviceEntity = null;
        }

        public static async Task<SmartDrawerDatabase.DAL.Device> GetDeviceEntityAsync()
        {
            if (_deviceEntity != null)
            {
                return _deviceEntity;
            }
            if ((Device == null) || string.IsNullOrWhiteSpace(Device.SerialNumber))
            {
                return null;
            }            
            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                _deviceEntity = ctx.Devices
                                .Where(d => d.RfidSerial == Device.SerialNumber)
                                .Include(d => d.DeviceType)
                                .FirstOrDefault();
                ctx.Database.Connection.Close();
                ctx.Dispose();
                return _deviceEntity;
            }
            catch (Exception error)
            {
                ///Trace.TraceError("{0} Unable to get data from local DB [Device].", DateTime.Now.ToString("g"));
                //return null;              
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "GetDeviceEntityAsync");
                exp.ShowDialog();
                return null;
            }
        }
        public static SmartDrawerDatabase.DAL.Device GetDeviceEntity()
        {
            if (_deviceEntity != null)
            {
                return _deviceEntity;
            }
            if ((Device == null) || string.IsNullOrWhiteSpace(Device.SerialNumber))
            {
                return null;
            }
            try
            {
                var ctx =  RemoteDatabase.GetDbContext();
                _deviceEntity = ctx.Devices
                                .Where(d => d.RfidSerial == Device.SerialNumber)
                                .Include(d => d.DeviceType)
                                .FirstOrDefault();
                ctx.Database.Connection.Close();
                ctx.Dispose();
                return _deviceEntity;
            }
            catch (Exception error )
            {
                ExceptionMessageBox msg = new ExceptionMessageBox(error, "Error GetDeviceEntity");
                msg.ShowDialog();          
                return null;  
            }
        }

        private static readonly ReaderWriterLockSlim MethodLock = new ReaderWriterLockSlim();

        private static Dictionary<string, int> _listTagPerDrawer = new Dictionary<string, int>();
        private static Dictionary<string, int> _listTagPreviousPerDrawer = new Dictionary<string, int>();
        private static Dictionary<string, int> _listTagAddedPerDrawer = new Dictionary<string, int>();
        private static Dictionary<string, int> _listTagRemovedPerDrawer = new Dictionary<string, int>();

        public static Dictionary<string, int> ListTagPerDrawer
        {
            get { return _listTagPerDrawer; }
            set { _listTagPerDrawer = value; }
        }
        public static Dictionary<string, int> ListTagPerPreviousDrawer
        {
            get { return _listTagPreviousPerDrawer; }
            set { _listTagPreviousPerDrawer = value; }
        }
        public static Dictionary<string, int> ListTagRemovedPerDrawer
        {
            get { return _listTagRemovedPerDrawer; }
            set { _listTagRemovedPerDrawer = value; }
        }
        public static Dictionary<string, int> ListTagAddedPerDrawer
        {
            get { return _listTagAddedPerDrawer; }
            set { _listTagAddedPerDrawer = value; }
        }

        public static bool bUseRfidSuccessively = true;
        private static bool[] isDrawerWaitScan = new bool[NbDrawer + 1];
        public static bool[] IsDrawerWaitScan { get { return isDrawerWaitScan; } set { isDrawerWaitScan = value; } }

        #region deviceEvent
        public delegate void DeviceEventHandler(object sender, DrawerEventArgs e);
        public delegate void FpEventHandler(object sender, FingerprintReaderEventArgs args);
        /// <summary>
        /// Local device is instantiated & connected
        /// </summary>
        public static event DeviceEventHandler DeviceConnected;

        /// <summary>
        /// Local device sent a Disconnection event
        /// </summary>
        public static event DeviceEventHandler DeviceDisconnected;

        /// <summary>
        /// Local device is not configured in application settings
        /// </summary>
        //public static event DeviceEventHandler DeviceNotInitialized;

        /// <summary>
        /// Local device started a scan
        /// </summary>
        public static event DeviceEventHandler ScanStarted;

        /// <summary>
        /// Local device started a scanNEw Tags read</summary>
        public static event DeviceEventHandler TagRead;

        /// <summary>
        /// Local device completed a scan
        /// </summary>
        public static event DeviceEventHandler ScanCompleted;

        /// <summary>
        /// Current scan has been cancelled
        /// </summary>
        public static event DeviceEventHandler ScanCancelledByHost;

        /// <summary>
        /// drawer opened
        /// </summary>
        public static event DeviceEventHandler DrawerOpened;

        /// <summary>
        /// drawer closed
        /// </summary>
        public static event DeviceEventHandler DrawerClosed;

        public static event DeviceEventHandler WallLocked;
        public static event DeviceEventHandler WallUnLocked;

        /// <summary>
        /// Fail start scan
        /// </summary>
        public static event DeviceEventHandler FailStartSscan;

        /// <summary>
        /// Fail start scan
        /// </summary>
        public static event DeviceEventHandler GpioConnected;
        public static event DeviceEventHandler BadgeRead;
        public static event FpEventHandler FpAuthenticationReceive;
        #endregion
        #region DevicesProperty

        public static string Drawer1SerialNumber { get; set; }
        public static string Drawer2SerialNumber { get; set; }
        public static string Drawer3SerialNumber { get; set; }
        public static string Drawer4SerialNumber { get; set; }
        public static string Drawer5SerialNumber { get; set; }
        public static string Drawer6SerialNumber { get; set; }
        public static string Drawer7SerialNumber { get; set; }

        public static bool IsDrawer1Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer1SerialNumber);
            }
        }
        public static bool IsDrawer2Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer2SerialNumber);
            }
        }
        public static bool IsDrawer3Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer3SerialNumber);
            }
        }
        public static bool IsDrawer4Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer4SerialNumber);
            }
        }
        public static bool IsDrawer5Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer5SerialNumber);
            }
        }
        public static bool IsDrawer6Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer6SerialNumber);
            }
        }
        public static bool IsDrawer7Configured
        {
            get
            {
                return !String.IsNullOrEmpty(Drawer7SerialNumber);
            }
        }

        public static bool DevicesConnected
        {
            get
            {
                try
                {
                    bool bConnect = true;
                    if (Device != null)
                    {
                        if (!Device.IsConnected)
                            bConnect = false;
                    }
                    else
                    {
                        bConnect = false;
                    }

                    return bConnect;
                }
                catch
                    (NullReferenceException)
                {
                    return false;
                }
                catch
                {
                    //Should not arrive
                }
                return false;
            }
        }
        #endregion
        #region Device Connection

        public static void TryInitializeLocalDeviceAsync()
        {
            ThreadPool.QueueUserWorkItem(delegate
            {
                if (!DevicesConnected)
                    FindAndConnectDevice();
                if (!GpioCardObject.IsConnected)
                    FindAndConnectGPIO();
                //ConnectAccessBadge("COM2");

                // fingerprint reader
                var fpReadersSerialNumbers = FingerprintReader.GetPluggedReadersSerialNumbers();
                if (fpReadersSerialNumbers.Count > 0)
                {
                    FPReader = new FingerprintReader(fpReadersSerialNumbers.First());
                    FPReader.StartCapture();
                    FPReader.FingerprintReaderEvent += FPReader_FingerprintReaderEvent;
                }

            });
        }
        private static void FPReader_FingerprintReaderEvent(object sender, FingerprintReaderEventArgs args)
        {
            var fpReader = sender as FingerprintReader;
            if (fpReader == null)
            {
                // cannot happen
                return;
            }
            /*if (args.EventType != FingerprintReaderEventArgs.EventTypeValue.FPReaderReadingComplete)
            {
                return;
            }*/

            var handler = FpAuthenticationReceive;
            if (handler != null)
            {
                var evt = new FingerprintReaderEventArgs(args.EventType, args.IsMaster);
                handler(sender, evt);
            }          
        }
        public static void FindAndConnectDevice()
        {
            lock (thisLock)
            {
                //Connect RFID             
                List<string> comPortList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
                if ((comPortList != null) && (comPortList.Count > 0))
                {
                    foreach (string comPort in comPortList)
                    {
                        RfidReader tmpReader = new RfidReader();
                        tmpReader.NotifyEvent += deviceForDiscover_NotifyEvent;
                        tmpReader.ConnectReader(comPort);
                        eventEndDiscover.WaitOne(500, false);
                        if (tmpReader.IsConnected)
                        {
                            Device = tmpReader;
                            Device.NotifyEvent += Device_NotifyEvent;
                            InitDeviceList(tmpReader.SerialNumber);
                            LastScanAccessTypeName = AccessType.Manual;
                            FireEvent(DeviceConnected, tmpReader.SerialNumber, 0);
                        }
                        else
                        {
                            tmpReader.DisconnectReader();
                            tmpReader.Dispose();
                        }
                    }
                }
            }
        }
        public static void FindAndConnectGPIO()
        {
            //Connect GPIO          
            if (GpioCardObject.IsConnected)
                GpioCardObject.CloseSerialPort();

            GpioCardObject.NotifyGpioEvent += GpioCardObject_NotifyGpioEvent;
            List<string> portComList = GpioCardObject.GetDevicePortCom();
            if (portComList.Count > 0)
                GpioCardObject.OpenSerialPort(portComList[0]);

            if (GpioCardObject.IsConnected)
                FireEvent(GpioConnected, "0", 0);

            if (GpioCardObject.IsConnected)
            {
                GpioCardObject.ClearOut(1);
                GpioCardObject.ClearOut(2);
            }
        }
        private static void GpioCardObject_NotifyGpioEvent(string arg)
        {
            /* if (_LastDrawerOpened != 0)
             {
                 if (GpioCardObject.InStatus[_LastDrawerOpened] == 0)
                 {                   
                     _LastDrawerClosed = _LastDrawerOpened;
                     _LastDrawerOpened = 0;
                     FireEvent(DrawerClosed, DeviceList.FirstOrDefault(x => x.Value == _LastDrawerClosed).Key, _LastDrawerClosed);
                     return;
                 }
             }

             for (int loop = 1; loop <= NbDrawer; loop++)
             {
                 if (GpioCardObject.InStatus[loop] == 1)
                 {
                     if (_LastDrawerOpened != loop)
                     {
                         _LastDrawerOpened = loop;
                         _LastDrawerClosed = 0;
                         FireEvent(DrawerOpened, DeviceList.FirstOrDefault(x => x.Value == _LastDrawerOpened).Key, _LastDrawerOpened);
                     }
                     break;
                 }
             }*/
            
            for (int loop = 1; loop <= NbDrawer; loop++)
            {
                if (GpioCardObject.InStatus[loop] != GpioCardObject.PreviousInStatus[loop])
                {                    
                    if ((GpioCardObject.InStatus[loop] == 0) && (GpioCardObject.PreviousInStatus[loop] == 1))
                        FireEvent(DrawerClosed, DeviceList.FirstOrDefault(x => x.Value == loop).Key, loop);
                }
            }           

            for (int loop = 1; loop <= NbDrawer; loop++)
            {
                if ((GpioCardObject.InStatus[loop] == 1) && (GpioCardObject.PreviousInStatus[loop] == 0))
                {                  
                    FireEvent(DrawerOpened, DeviceList.FirstOrDefault(x => x.Value == loop).Key, loop);
                   
                }
            }

            if (GpioCardObject.PreviousInStatus[8] == -1)
            {
                if (GpioCardObject.InStatus[8] == 1) isWallLocked = true;
                else isWallLocked = false;
            }

            if ((GpioCardObject.InStatus[8] == 1) && (GpioCardObject.PreviousInStatus[8] == 0))
            {
                isWallLocked = false;
                FireEvent(WallUnLocked, "0", 0);
            }
            if ((GpioCardObject.InStatus[8] == 0) && (GpioCardObject.PreviousInStatus[8] == 1))
            {
                isWallLocked = true;
                FireEvent(WallLocked, "0", 0);
            }
        }
        /*private static void ConnectAccessBadge(string portCom)
        {
            if (AccessBadgeObject != null)
                AccessBadgeObject.closePort();

            AccessBadgeObject = new clBadgeReader(portCom, true);
            AccessBadgeObject.NotifyBadgeReaderEvent += AccessBadgeObject_NotifyBadgeReaderEvent;
        }
        private static void AccessBadgeObject_NotifyBadgeReaderEvent(object sender, string badgeID)
        {
            FireEvent(BadgeRead, badgeID, 0);
        }*/

        private static EventWaitHandle eventEndDiscover = new AutoResetEvent(false);
        private static void deviceForDiscover_NotifyEvent(object sender, rfidReaderArgs args)
        {
            switch (args.RN_Value)
            {
                case rfidReaderArgs.ReaderNotify.RN_Connected:
                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                case rfidReaderArgs.ReaderNotify.RN_FailedToConnect:
                    eventEndDiscover.Set();
                    break;
            }
        }

        #endregion
        #region deviceFunction

        public static void SetDrawerActive(int drawerId)
        {

            if (CurrentActiveRfidDrawer == drawerId) return;
            if ((Device != null) && (Device.IsConnected))
            {
                _CurrentActiveRfidDrawer = drawerId;
                if (DrawerInventoryData[_CurrentActiveRfidDrawer] == null)
                    DrawerInventoryData[_CurrentActiveRfidDrawer] = new ReaderData();
                Device.ReaderData = DrawerInventoryData[_CurrentActiveRfidDrawer];
                Device.StopField();
                Device.SendSwitchCommand(true, (byte)_CurrentActiveRfidDrawer, false);
            }
        }
        public static void InitDeviceList(string serialRfid)
        {
            // Create Dummy serial for V1 compatibility
            Drawer1SerialNumber = serialRfid + "_1";
            Drawer2SerialNumber = serialRfid + "_2";
            Drawer3SerialNumber = serialRfid + "_3";
            Drawer4SerialNumber = serialRfid + "_4";
            Drawer5SerialNumber = serialRfid + "_5";
            Drawer6SerialNumber = serialRfid + "_6";
            Drawer7SerialNumber = serialRfid + "_7";

            DeviceList = new Dictionary<string, int>();

            if (IsDrawer1Configured)
                DeviceList.Add(Drawer1SerialNumber, 1);
            if (IsDrawer2Configured)
                DeviceList.Add(Drawer2SerialNumber, 2);
            if (IsDrawer3Configured)
                DeviceList.Add(Drawer3SerialNumber, 3);
            if (IsDrawer4Configured)
                DeviceList.Add(Drawer4SerialNumber, 4);
            if (IsDrawer5Configured)
                DeviceList.Add(Drawer5SerialNumber, 5);
            if (IsDrawer6Configured)
                DeviceList.Add(Drawer6SerialNumber, 6);
            if (IsDrawer7Configured)
                DeviceList.Add(Drawer7SerialNumber, 7);
        }
        public static void ReleaseDevices()
        {
            try
            {
                if (Device != null)
                {
                    Device.DisconnectReader();
                    Device.Dispose();
                }

                if (GpioCardObject != null)
                {
                    GpioCardObject.CloseSerialPort();
                    GpioCardObject = new GpioCardLib();
                }              

            }
            catch
            {

            }
        }
        public static bool IsWallInScan()
        {
            bool bInScan = false;           
            if (Device != null)
            {
                if (Device.IsInScan)
                {
                    bInScan = true;
                }
            }
            return bInScan;
        }
        public static void StartManualScan(int drawerId)
        {
            if ((Device != null) && (Device.IsConnected))
            {
                Device.StopField();
                SetDrawerActive(drawerId);
                Device.RequestScanSelectedAxis(true, true);
            }
        }
        public static void StopScan(int drawerId)
        {
            if ((Device != null) && (Device.IsConnected))
                Device.RequestEndScan();
        }
        public static void LightAll(int drawerId)
        {
            if (Device == null) return;
            if (Device.IsInScan)
                StopScan(drawerId);

            SetDrawerActive(drawerId);
            Device.DeviceBoard.setAntenna(true);
            Thread.Sleep(10);
            Device.DeviceBoard.sendSyncPulse();
            Thread.Sleep(50);
            ushort Rcor;
            Device.DeviceBoard.sendCommand((byte)SDK_SC_RfidReader.DeviceBase.LowlevelBasicOrder.KD, out Rcor);
            Thread.Sleep(10);
            Device.DeviceBoard.sendCommand((byte)SDK_SC_RfidReader.DeviceBase.LowlevelBasicOrder.KZ_SPCE2, out Rcor);
            Thread.Sleep(10);
            Device.DeviceBoard.sendCommand((byte)SDK_SC_RfidReader.DeviceBase.LowlevelBasicOrder.KB, out Rcor);
            Thread.Sleep(10);
            Device.DeviceBoard.sendCommand((byte)SDK_SC_RfidReader.DeviceBase.LowlevelBasicOrder.KL, out Rcor);
            Thread.Sleep(10);

        }
        public static void LightTags(int drawerId, List<string> tagList, bool bLight)
        {
            if (Device == null) return;

            if (Device.IsInScan)
                StopScan(drawerId);

            SetDrawerActive(drawerId);
           // Device.ConfirmAndLightWithKD(1, tagList, bLight);

             Device.DeviceBoard.setBridgeState(false, 30 , 30);
             Device.StartLedOn(drawerId);
             Device.DeviceBoard.setBridgeState(false, 50, 50);
             Device.StartLedOn(drawerId);
             Device.DeviceBoard.setBridgeState(false, 100, 100);
             Device.StartLedOn(drawerId);
             Device.DeviceBoard.setBridgeState(false, 167, 167);

             Device.ConfirmAndLightWithKD(1, tagList, bLight);
             if (tagList.Count > 0)
             {
                 Device.DeviceBoard.setBridgeState(false, 100, 100);
                 Device.ConfirmAndLight(drawerId, tagList);
             }
             if (tagList.Count > 0)
             {
                 Device.DeviceBoard.setBridgeState(false, 50, 50);
                 Device.ConfirmAndLight(drawerId, tagList);
             }
             if (tagList.Count > 0)
             {
                 Device.DeviceBoard.setBridgeState(false, 30, 30);
                 Device.ConfirmAndLight(drawerId, tagList);
             }
             Device.DeviceBoard.setBridgeState(false, 167, 167);
             
        }
        public static void StopLighting(int drawerId)
        {
            if (Device == null) return;
            Device.StopField();
        }
        public static void UnlockWall()
        {
            if (Device == null) return;

            if (!isVersionV3)
                Device.OpenDoor();
            else
            {
                if (GpioCardObject != null)
                {
                    if (GpioCardObject.IsConnected)
                    {
                        GpioCardObject.SetOut(1);
                        Thread.Sleep(500);
                        GpioCardObject.ClearOut(1);
                    }
                }
            }
        }
        public static void LockWall()
        {
            if (Device == null) return;

            if (!isVersionV3)
                Device.CloseDoor();
            else
            {
                if (GpioCardObject != null)
                {
                    if (GpioCardObject.IsConnected)
                    {
                        GpioCardObject.SetOut(2);
                        Thread.Sleep(500);
                        GpioCardObject.ClearOut(2);
                    }
                }
            }
        }

        #endregion
        #region Inventory
        public static void RemoveTagFromListForDrawer(int drawerId)
        {
            // clone List to previous
            _listTagPreviousPerDrawer = new Dictionary<string, int>(_listTagPerDrawer);

            //remove for drawer
            foreach (var item in _listTagPerDrawer.Where(kvp => kvp.Value == drawerId).ToList())
            {
                _listTagPerDrawer.Remove(item.Key);
            }
            //remove for added
            foreach (var item in  _listTagAddedPerDrawer.Where(kvp => kvp.Value == drawerId).ToList())
            {
                _listTagAddedPerDrawer.Remove(item.Key);
            }
            //remove for removed

            foreach (var item in _listTagRemovedPerDrawer.Where(kvp => kvp.Value == drawerId).ToList())
            {
                _listTagRemovedPerDrawer.Remove(item.Key);
            }
        }
        public static List<string> GetTagFromDictionnary(int drawerId, Dictionary<string, int> dic)
        {
            List<string> listTags = dic
                .Where(kvp => kvp.Value == drawerId)
                .Select(kvp => kvp.Key)
                .ToList();
            return listTags;

        }
        public static void AddTagListForDrawer(int drawerId, ArrayList tagList)
        {
            foreach (string tagId in tagList)
            {
                if (!_listTagAddedPerDrawer.ContainsKey(tagId)) // Add to present if not in Added
                {
                    if (_listTagPerDrawer.ContainsKey(tagId))
                        _listTagPerDrawer[tagId] = drawerId;
                    else
                        _listTagPerDrawer.Add(tagId, drawerId);
                }
            }
        }
        public static void UpdateAddedTagToDrawer(int drawerId, ArrayList tagList)
        {
            List<string> prevListPresent = GetTagFromDictionnary(drawerId, _listTagPreviousPerDrawer);
            foreach (string tagId in tagList)
            {
                if (!prevListPresent.Contains(tagId))
                {
                    if (_listTagAddedPerDrawer.ContainsKey(tagId))
                        _listTagAddedPerDrawer[tagId] = drawerId;
                    else
                        _listTagAddedPerDrawer.Add(tagId, drawerId);
                }
            }
        }
        public static void UpdateremovedTagToDrawer(int drawerId, ArrayList tagList)
        {
            List<string> prevListPresent = GetTagFromDictionnary(drawerId, _listTagPreviousPerDrawer);
            foreach (string tagId in prevListPresent)
            {
                if (!tagList.Contains(tagId))
                    if (_listTagRemovedPerDrawer.ContainsKey(tagId))
                        _listTagRemovedPerDrawer[tagId] = drawerId;
                    else
                        _listTagRemovedPerDrawer.Add(tagId, drawerId);

            }
        }
        #endregion
        #region Device Notification
        private static void Device_NotifyEvent(object sender, rfidReaderArgs args)
        {
            if (_CurrentActiveRfidDrawer == 0) return;
            switch (args.RN_Value)
            {
                case rfidReaderArgs.ReaderNotify.RN_Connected:
                    for (int loop = 1; loop <= NbDrawer; loop++)
                        isDrawerWaitScan[loop] = false;
                    FireEvent(DeviceConnected, args.SerialNumber, _CurrentActiveRfidDrawer);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_Disconnected:
                    FireEvent(DeviceDisconnected, args.SerialNumber, _CurrentActiveRfidDrawer);

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanStarted:
                    FireEvent(ScanStarted, args.SerialNumber, _CurrentActiveRfidDrawer);
                    FireEvent(TagRead, args.SerialNumber, _CurrentActiveRfidDrawer);

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan:
                    FireEvent(FailStartSscan, args.SerialNumber, _CurrentActiveRfidDrawer);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_TagAdded:

                    if (Properties.Settings.Default.bReadDft)
                    {
                        FireEvent(TagRead, args.SerialNumber, _CurrentActiveRfidDrawer);
                    }
                    else
                    {
                        if (isValidUidFormat(args.Message))
                        {
                            FireEvent(TagRead, args.SerialNumber, _CurrentActiveRfidDrawer);
                        }
                        else if (Device.ReaderData.strListTag.Contains(args.Message))
                        {
                            Device.ReaderData.strListTag.Remove(args.Message);
                            Device.ReaderData.nbTagScan = Device.ReaderData.strListTag.Count;
                        }
                    }

                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanCompleted:
                    MethodLock.EnterReadLock();
                    try
                    {
                        DrawerInventoryData[_CurrentActiveRfidDrawer] = Device.ReaderData;
                        DrawerTagQty[_CurrentActiveRfidDrawer] = Device.ReaderData.strListTag.Count;
                        RemoveTagFromListForDrawer(_CurrentActiveRfidDrawer);
                     
                        AddTagListForDrawer(_CurrentActiveRfidDrawer, Device.ReaderData.strListTag);
                        UpdateAddedTagToDrawer(_CurrentActiveRfidDrawer, Device.ReaderData.strListTag);                                          
                        UpdateremovedTagToDrawer(_CurrentActiveRfidDrawer, Device.ReaderData.strListTag);
                        LastScanTime = DateTime.Now;

                        Thread.Sleep(10);
                    }
                    finally
                    {
                        MethodLock.ExitReadLock();
                    }
                    FireEvent(TagRead, args.SerialNumber, _CurrentActiveRfidDrawer);  // Update drawer tag counter;
                    FireEvent(ScanCompleted, args.SerialNumber, _CurrentActiveRfidDrawer);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost:
                    FireEvent(ScanCancelledByHost, args.SerialNumber, _CurrentActiveRfidDrawer);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_Door_Opened:
                    FireEvent(DrawerOpened, args.SerialNumber, _CurrentActiveRfidDrawer);
                    break;
                case rfidReaderArgs.ReaderNotify.RN_Door_Closed:
                    FireEvent(DrawerClosed, args.SerialNumber, _CurrentActiveRfidDrawer);
                    break;
            }
        }
        private static void FireEvent(DeviceEventHandler eventHandler, string serial, int drawerId)
        {
            var handler = eventHandler;
            if (handler != null)
            {
                var evt = new DrawerEventArgs(serial, drawerId);
                eventHandler(Device, evt);
            }
        }
        #endregion
        private static bool isValidUidFormat(string uidToTest)
        {
            // if ((uidToTest.Length == 12) && ((uidToTest.StartsWith("1007") || (uidToTest.StartsWith("1102")))))
            if (uidToTest.Length == 12)
                return true;
            else
                return false;
        }

    }
}
