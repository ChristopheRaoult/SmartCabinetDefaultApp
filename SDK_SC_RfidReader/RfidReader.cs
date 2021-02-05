using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Text;
using System.Threading;
using SDK_SC_RfidReader.DeviceBase;
using System.Collections;
using System.IO;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Mail;
using System.Data.SqlClient;

namespace SDK_SC_RfidReader
{
    /// <summary>
    /// Spacecode RFID reader notify event to inform host.
    /// This is a delegate function to define the notify function.
    /// </summary>
    /// <param name="sender">Class sender of the event</param>
    /// <param name="arg">Class rfidReaderArgs that contains event to notify </param>
    public delegate void NotifyHandlerDelegate(Object sender, rfidReaderArgs arg);
    /// <summary>
    /// Delegate for debug message in textbox
    /// </summary>
    /// <param name="device"></param>
    /// <param name="message"></param>
    /// <param name="modifier"></param>
    public delegate void ShowMessageDelegate(IDeviceRfidBoard device, string message, string modifier);

    /// <summary>
    /// Main reader class for driving Spacecode rfid board.
    /// Class define method to send High level command to drive the RFID board.
    /// </summary>
    public class RfidReader : IRfidReader, IDisposable
    {
        private ushort _status;
        /// <summary>
        /// internal light value
        /// </summary>
        public int LightValue { get; private set; }
        /// <summary>
        /// door status
        /// </summary>
        public Door_Status Door_Status { get; private set; }
        /// <summary>
        /// lock status
        /// </summary>
        public Lock_Status Lock_Status { get; private set; }


        private int initialFrequency = 334;
        private int currentFrequency;
        private bool _bUpdateFrequencyWithDrawer = false;
        public bool  UpdateFrequencyWithDrawer { 
            get { return _bUpdateFrequencyWithDrawer; } 
            set {_bUpdateFrequencyWithDrawer = value;}}

        private readonly Object _logLock = new Object();

        private const int MaxDigit = 10;

        private readonly uint[] _countsPresent = new uint[256];
        private readonly uint[] _countsMissing = new uint[256];
        private byte _threshold;
        private bool _driveLight;
        private uint _scanPourcent;
        private int _voltage12V;
        private volatile bool _eventUsbArrived;

        private bool _bPowerOn;
        /// <summary>
        /// Threshold backup value
        /// </summary>
        public byte ThresholdBackup;
        private byte _checkKZout = 0;

        private bool _debugReader;
        /// <summary>
        /// Property to get and set debug window activation
        /// </summary>
        public bool DebugReader { set { _debugReader = value; } get { return _debugReader; } }


        private bool _enabledLog;
        /// <summary>
        /// Property to enable or not the log 
        /// </summary>
        public bool EnabledLog { set { _enabledLog = value; } get { return _enabledLog; } }

        private bool _sendLogMail;
        /// <summary>
        /// Property to enable to send log by mail as the default receiver
        /// </summary>
        public bool SendLogMail { set { _sendLogMail = value; } get { return _sendLogMail; } }

        private StatusForm _statusForm;

        /// <summary>
        /// Variable contenant l'objet Thread pendant flash
        /// </summary>
        public Thread FlashThread = null;

        private string _logPath;

        private int _previousMs = -1;
        /// <summary>
        /// Property to get end scan statut (=4 if all axis well ended)
        /// </summary>
        /// <returns></returns>
        public byte GetCheckKZout()
        {
            return _checkKZout;
        }

        private DateTime _lastEventDoorDate;
        private DateTime _currentEventDoorDate;
        private DateTime _lastConnectionDate;

        private readonly ArrayList _comBluetooth = new ArrayList();
        private readonly ArrayList _comList = new ArrayList();
        private ArrayList _comListPresent = new ArrayList();
        private ArrayList _lastScanSerial = new ArrayList();

      

        private byte _confirmationStatut;
        /// <summary>
        /// Property to retrieve statut of a confirmation tag request.
        /// </summary>
        /// <returns></returns>
        public byte GetConfirmationStatus()
        {
            byte backup = _confirmationStatut;
            _confirmationStatut = 0;
            return backup;
        }

        //private static Mutex _mutex;
        //private const string MutexName = "SDK_RFID.1mutex";

        /// <summary>
        /// Array List of the devices in format "serial;portcom"
        /// </summary>
        public ArrayList ListOfSerialPluggedDevices { get { return _comList; } }


        /// <summary>
        /// Variable function for notify event.
        /// </summary>
        public event NotifyHandlerDelegate NotifyEvent;

        /// <summary>
        /// Property to retrieve serail number of the board
        /// </summary>
        public string SerialNumber { get; private set; }

        /// <summary>
        /// Property to retrieve the connection status.
        /// </summary>
        /// <value>True is connected, false if not connected.</value>
        public bool IsConnected { get; private set; }

        private volatile bool _isInScan;
        /// <summary>
        /// Tells if device is currently performing a scan
        /// </summary>
        public bool IsInScan { get { return _isInScan; } }

        private volatile bool _isInWaitTagMode;
        /// <summary>
        /// Tells if device is currently in Wait mode
        /// </summary>
        public bool IsInWaitTagMode { get { return _isInWaitTagMode; } }

        /// <summary>
        /// Property to set scan timeout in millisecond.This time while stop the scan process thread and request a end of 
        /// scanning.
        /// </summary>
        public uint ScanTimeout { get; set; }


        /// <summary>
        /// Property to set or get the serial port name "ex COM1".
        /// </summary>
        public string StrCom { get; set; }

        /// <summary>
        /// Property to get and set errBoard class
        /// </summary>
        public ErrorBoard ErrBoard { get { return DeviceBoard.deviceChannel.ErrBoard; } }

        /// <summary>
        /// 
        /// </summary>
        public DeviceRfidBoard DeviceBoard { get; private set; }

        private EventThread _eventThread;

        /// <summary>
        /// General Reader data class variable.
        /// This class contain all the reader information and result.
        /// Each time a notification occurs, the variable in relation is uptate in this structure.
        /// </summary>
        public ReaderData ReaderData;
        public ReaderData PreviousReaderData;

        /// <summary>
        /// Property to get firmware version
        /// </summary>
        public string FirmwareVersion
        {
            get
            {
                if ((IsConnected) && (_bPowerOn))
                {
                    return DeviceBoard.FirmwareVersion;
                }
                else
                    return null;
            }
        }
        /// <summary>
        /// Property to get Hardware version
        /// </summary>
        public string HardwareVersion
        {
            get
            {
                if ((IsConnected) && (_bPowerOn))
                {
                    return DeviceBoard.HardwareVersion;
                }
                else
                    return null;
            }
        }


        public void NotifyRelock()
        {
            rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Locked_Before_Open, "Relock Before Open");
            if (NotifyEvent != null)
            {                
                NotifyEvent(this, notifyEvent);
            }
        }

        public int statusWrite = -1;
        private readonly EventWaitHandle _eventWrite = new AutoResetEvent(false);
        private readonly EventWaitHandle _eventScanStart = new AutoResetEvent(false);
        private readonly EventWaitHandle _eventScanEnd = new AutoResetEvent(false);
        private readonly EventWaitHandle _eventConfirmation = new AutoResetEvent(false);
        private readonly EventWaitHandle _eventCancelByHost = new AutoResetEvent(false);
        private Thread _scanThread;
        private ThreadArgs _threadArgs;

        private USBWatcher _watcher;
        private readonly bool _bWithUSBWatcher;

        private const string SenderAdress = "alert@spacecode-rfid.com";
        private const string LoginName = "alert@spacecode-rfid.com";
        private const string Password = "rfidnet";
        private const string SMTPServer = "mail.spacecode-rfid.com";
        private const int SMTPPort = 587;
        private const int SMTPActive = 1;

        /*private void initMutex()
        {
            try
            {
                _mutex = Mutex.OpenExisting(MutexName, System.Security.AccessControl.MutexRights.FullControl);

            }
            catch (WaitHandleCannotBeOpenedException)
            {
                _mutex = new Mutex(false, MutexName);
            }
            catch
            {
                _mutex = new Mutex(false, MutexName);
                //throw new Exception();
            }
        }*/

       

        /// <summary>
        /// Constructor of rfidReaderClass.
        /// </summary>
        public RfidReader(bool bWithUSBWatcher)
        {
            ScanTimeout = 600000;
            Lock_Status = Lock_Status.Lock_Close;
            Door_Status = Door_Status.Door_Close;

            AddMessage(0, "Create New Object", "INF :", DateTime.Now);

            _bWithUSBWatcher = true; // High CPU usage
            IsConnected = false;
            _isInScan = false;
            DeviceBoard = null;
            SerialNumber = null;
            StrCom = null;
            ReaderData = new ReaderData();
            ReaderData.RspScan = RespScan.RS_ReaderNotReady;
            _lastEventDoorDate = DateTime.Now;
            _currentEventDoorDate = DateTime.Now;

            _comListPresent.Clear();

            //initMutex();

            //using (_watcher = new USBWatcher())
            _watcher = new USBWatcher();
            {
                _watcher.DeviceEvent += new EventHandler(OnDeviceEvent);
                _watcher.Start(5);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RfidReader()
        {
            ScanTimeout = 600000;
            Lock_Status = Lock_Status.Lock_Close;
            Door_Status = Door_Status.Door_Close;

            AddMessage(0, "Create New Object", "INF :", DateTime.Now);

            this._bWithUSBWatcher = true; // for autoconnection
            IsConnected = false;
            _isInScan = false;
            DeviceBoard = null;
            SerialNumber = null;
            StrCom = null;
            ReaderData = new ReaderData();
            ReaderData.RspScan = RespScan.RS_ReaderNotReady;
            _lastEventDoorDate = DateTime.Now;
            _currentEventDoorDate = DateTime.Now;

            _comListPresent.Clear();

            //initMutex();


            //using (_watcher = new USBWatcher())
            _watcher = new USBWatcher();
            {
                _watcher.DeviceEvent += new EventHandler(OnDeviceEvent);
                _watcher.Start(5);
            }
        }

        /// <summary>
        /// Destructor of rfidReader Class.
        /// Launch a disconnect function to end the reception thread.
        /// </summary>
        ~RfidReader()
        {
            AddMessage(0, "Destroy Object", "INF :", DateTime.Now);
            if (_debugReader)
                _statusForm.Close();
            if (IsInScan)
            {
                RequestEndScan();
                Thread.Sleep(2000);
            }
            if (_isInWaitTagMode)
            {
                SetWaitForTag(false);
                Thread.Sleep(200);
            }
            if (IsConnected)
            DisconnectReader();
            if (_watcher != null)
            {
                _watcher.Dispose();
                _watcher = null;
            }

        }
        /// <summary>
        /// Function to dispose the reader object.
        /// Have to be launch before quit application to well remove the USB Watcher
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                AddMessage(0, "Dispose Object", "INF :", DateTime.Now);
                if (IsInScan)
                {
                    RequestEndScan();
                    Thread.Sleep(2000);
                }
                if (_isInWaitTagMode)
                {
                    SetWaitForTag(false);
                    Thread.Sleep(200);
                }
                if (IsConnected)
                    DisconnectReader();
                if (_watcher != null)
                {
                    _watcher.Dispose();
                    _watcher = null;
                }
            }
        }

        /// <summary>
        /// Method to retrieve correlation threshold.
        /// This value represent the level above which a correaltion is assume done by the tag.
        /// </summary>
        /// <returns>value of the threshold</returns>
        public byte GetThresholdValue()
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "GetThresholdValue()", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                _threshold = DeviceBoard.getCorrelationThreshold();
                ReaderData.Threshold = _threshold;
                return _threshold;
            }
            return 0;
        }
        /// <summary>
        /// Method to retrieve value of the 12V of the board
        /// 14v 1023 - 13.5v 1023 - 13v 1023 - 12.5 1023   - 12 992 - 11.5 951 11 911
        /// </summary>
        /// <returns>A value of the ADC 12v</returns>
        public int getSupply12V()
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "getSupply12V()", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                _voltage12V = DeviceBoard.getSupply12V();

                return _voltage12V;
            }
            return 0;
        }
        /// <summary>
        /// Method to set and store threshold value.
        /// This value must be between 5 and 200.
        /// The value  must be greater than MaximumCorrelationWhithoutResponse to not detect noise and below
        /// MinimumCorrelationWithResponse to aasume to detect all the tag.
        /// </summary>
        /// <param name="Threshold">threshold value to set</param>
        public void SetThresholdValue(byte Threshold)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "SetThresholdValue(" + Threshold.ToString() + ")", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                ReaderData.Threshold = Threshold;
                DeviceBoard.setCorrelationThreshold(Threshold);
                DeviceBoard.saveCorrelationThresholdToROM();
            }
        }
        /// <summary>
        /// Method to flash the firmware.
        /// Use this method with caution with only recommendation of spacecode.
        /// A bad use could result of a non function of the reader.
        /// </summary>
        /// <param name="hexFileNamePath">String to the path of the firmware file</param>
        public void FlashFirmware(string hexFileNamePath)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "FlashFirmware(" + hexFileNamePath + ")", "CMD :", DateTime.Now);


            if ((IsConnected) && (_bPowerOn))
            {
                try
                {
                    ThresholdBackup = GetThresholdValue();
                    if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, "getCorrelationThreshold", "CMD :", DateTime.Now);
                    string fileName = Path.GetFileNameWithoutExtension(hexFileNamePath);
                    string[] splitFileName = fileName.Split('_');

                    if (splitFileName.Length == 3)
                    {
                        string strHardwareVersion = splitFileName[2].Substring(1);

                        double currentHardwareVersion;
                        double fileHardwareVersion;
                        bool ret1 = false;
                        bool ret2 = false;

                        ret1 = double.TryParse(DeviceBoard.HardwareVersion.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out currentHardwareVersion);
                        ret2 = double.TryParse(strHardwareVersion.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture, out fileHardwareVersion);

                        if (ret1 & ret2)
                        {
                            if (currentHardwareVersion != fileHardwareVersion)
                            {
                                rfidReaderArgs notifyEvent;

                                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareCorruptedHexFile, " Firmware flash failed due to corrupted file");
                                if (NotifyEvent != null)
                                {
                                    if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, "Bad Hardware Version Value", "NFY :", DateTime.Now);
                                    NotifyEvent(this, notifyEvent);
                                }
                                return;
                            }

                        }
                        else
                        {
                            rfidReaderArgs notifyEvent;
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareCorruptedHexFile, " Firmware flash failed due to corrupted file");
                            if (NotifyEvent != null)
                            {
                                if (DeviceBoard != null)
                                AddMessage(DeviceBoard.deviceId, "Unable to parse hardware version", "NFY :", DateTime.Now);
                                NotifyEvent(this, notifyEvent);
                            }
                            return;
                        }

                    }
                }
                catch (Exception ex)
                {
                    throw new Exception();
                }


                HexLoader hexLoader = HexLoader.LoadHexFile(hexFileNamePath, false);
                if (hexLoader != null)
                {
                    if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, "Create firmware thread", "NFY :", DateTime.Now);
                    FlashThread = new Thread(flashFirmwareThread);
                    FlashThread.Name = "Flash Firmware Thread";
                    FlashThread.IsBackground = true;
                    FlashThread.Start(hexLoader);
                    /*rfidReaderArgs notifyEvent;
                    addMessage(deviceBoard.deviceId,  rfidReaderArgs.ReaderNotify.RN_FirmwareStarted.ToString(), "NFY :");
                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareStarted, " Firmware flash started");
                    if (NotifyEvent != null)
                    {
                        NotifyEvent(this, notifyEvent);
                           
                    }*/

                    // flashFirmwareThread(hexLoader);
                }
                else
                {
                    rfidReaderArgs notifyEvent;
                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareCorruptedHexFile, " Firmware flash failed due to corrupted file");
                    if (NotifyEvent != null)
                    {
                        if (DeviceBoard != null)
                        AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_FirmwareCorruptedHexFile.ToString(), "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
                }

            }
        }

        /// <summary>
        /// Thread to flash firmware
        /// </summary>
        /// <param name="hexLoaderObject"></param>
        private void flashFirmwareThread(object hexLoaderObject)
        {

            rfidReaderArgs notifyEvent;
            try
            {
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Enter firmware thread", "NFY :", DateTime.Now);

                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareStarted, "Firmware Flashing Started");
                if (NotifyEvent != null)
                {
                    if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_FirmwareStarted.ToString(), "NFY :", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }
                // Check the current mode of the firmware.
                string response = DeviceBoard.deviceChannel.sendMessageWaitResponse("?");

                if (response == null)
                {

                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareFailedToFinish, "Firmware not responding. on ?");
                    if (NotifyEvent != null)
                    {
                        if (DeviceBoard != null)
                        AddMessage(DeviceBoard.deviceId, "Firmware not responding. on ?", "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }

                    return;
                }
                if (!response.StartsWith("Update|V"))
                {
                    response = DeviceBoard.deviceChannel.sendMessageWaitResponse("UPDATE");
                    if (response == null)
                    {

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareFailedToFinish, "Firmware not responding. on UPDATE");
                        if (NotifyEvent != null)
                        {
                            if (DeviceBoard != null)
                            AddMessage(DeviceBoard.deviceId, "Firmware not responding. on UPDATE", "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }

                        return;
                    }
                    if (!response.StartsWith("Update"))
                    {

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareFailedToFinish, "Unable to switch to BootLoader mode.");
                        if (NotifyEvent != null)
                        {
                            if (DeviceBoard != null)
                            AddMessage(DeviceBoard.deviceId, "Unable to switch to BootLoader mode.", "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }

                        return;
                    }
                    response = DeviceBoard.deviceChannel.sendMessageWaitResponse("?");
                }
                // Make sure the bootloader is running.
                if (!response.StartsWith("Update|V"))
                {

                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareFailedToFinish, "Firmware not responding. on ?");
                    if (NotifyEvent != null)
                    {
                        if (DeviceBoard != null)
                        AddMessage(DeviceBoard.deviceId, "Firmware not responding. on ?", "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }

                    return;
                }


                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareMessage, "Device in BootLoader mode.");
                if (NotifyEvent != null)
                {
                    if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, "Device in BootLoader mode.", "NFY :", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }
                System.Threading.Thread.Sleep(100);
                // Iterate through the program code to flash each program row.
                HexLoader hexLoader = hexLoaderObject as HexLoader;
                uint rowIndex;
                byte[] row;
                bool bFirst = true;
                uint cpt = 0;
                while (hexLoader.EnumerateRows(bFirst, out row, out rowIndex))
                {
                    bFirst = false;
                    StringBuilder messageBuilder = new StringBuilder(string.Format("DOWNLOAD|{0:X6}|", rowIndex), 256);
                    foreach (byte rowByte in row)
                        messageBuilder.AppendFormat("{0:X2}", rowByte);
                    DeviceBoard.deviceChannel.sendMessageExpectResponse(messageBuilder.ToString(), string.Format("Download|Ok|{0:X6}", rowIndex));


                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareMessage, string.Format("Flashed {0} of {1} rows.", cpt++, hexLoader.TotalRowCount));
                    if (NotifyEvent != null)
                    {
                        if (DeviceBoard != null)
                        AddMessage(DeviceBoard.deviceId, string.Format("Flashed {0} of {1} rows.", cpt, hexLoader.TotalRowCount), "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
                }
                DeviceBoard.deviceChannel.sendMessageExpectResponse(string.Format("FINALIZE|{0:X2}|{1:X4}|{2:X4}", HexLoader.FirstApplicationRowIndex, hexLoader.ApplicationRowCount, hexLoader.CRC), "Finalize|Ok");
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, " Firmware flash succeed", "NFY :", DateTime.Now);


                Thread.Sleep(500);
                SetThresholdValue(ThresholdBackup);
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Leave firmware thread", "NFY :", DateTime.Now);

                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareSuccedToFinish, " Firmware flash succeed");
                if (NotifyEvent != null) NotifyEvent(this, notifyEvent);

                return;

            }
            catch (Exception exception)
            {
                //MessageBox.Show(exception.ToString(), "Firmware Flash Failed");
                if (DeviceBoard == null) return;

                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FirmwareFailedToFinish, " Firmware flash failed :" + exception.Message);
                if (NotifyEvent != null)
                {
                    if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, " Firmware flash failed :" + exception.Message, "NFY :", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }
                //throw new Exception();
            }


        }


        /// <summary>
        /// Method to retrieve serial port where a device is plugged
        /// </summary>
        /// <returns>Array of valid serial port</returns>
        /*public void DiscoverPluggedDevices()
        {
            addMessage(0, "DiscoverPluggedDevices()", "CMD :", DateTime.Now);
            comList.Clear();
            comBluetooth.Clear();
            // search for bluetooth port

            const string Win32_SerialPort = "Win32_SerialPort";
            SelectQuery q = new SelectQuery(Win32_SerialPort);
            ManagementObjectSearcher mos = new ManagementObjectSearcher(q);
            addMessage(0, "Search bluetooth port ", " -> ", DateTime.Now);
            foreach (object cur in mos.Get())
            {
                ManagementObject mo = (ManagementObject)cur;
                object id = mo.GetPropertyValue("DeviceID");
                object id2 = mo.GetPropertyValue("Name");
                if (id2.ToString().Contains("Bluetooth")) comBluetooth.Add(id.ToString());
            }
            addMessage(0, comBluetooth.Count.ToString() + " bluetooth port found", " -> ", DateTime.Now);


            addMessage(0, "Search serial port ", " -> ", DateTime.Now);
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            addMessage(0, ports.Length.ToString() + " serial port found", " -> ", DateTime.Now);
            foreach (string s in ports)
            {
                if (!comBluetooth.Contains(s))
                {
                    addMessage(0, "Search Device on  port " + s, " -> ", DateTime.Now);
                    DiscoverDevices Dut = new DiscoverDevices();
                    string serial;
                    if (Dut.TestSerialPort(s, out serial))
                    {
                        addMessage(0, "Device " + serial + " found on  port " + s, " <- ", DateTime.Now);
                        comList.Add(serial + ";" + s);

                    }
                    else
                        addMessage(0, "No device found on  port " + s, " <- ", DateTime.Now);
                }
                else
                {
                    addMessage(0, "Bluetooth port " + s + " not tested", " -> ", DateTime.Now);
                    addMessage(0, "  ", "  ", DateTime.Now);
                }
            }
            rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_DiscoverPluggedDevicesCompleted, "End of Search reader process");
            if (NotifyEvent != null)
            {
                addMessage(0, rfidReaderArgs.ReaderNotify.RN_DiscoverPluggedDevicesCompleted.ToString(), "NFY :", DateTime.Now);
                NotifyEvent(this, notifyEvent);
            }

        }*/

        public void DiscoverPluggedDevices()
        {
            AddMessage(0, "DiscoverPluggedDevices()", "CMD :", DateTime.Now);
            _comList.Clear();
            AddMessage(0, "Search serial port ", " -> ", DateTime.Now);
            List<string> ports = GetDevicePortCom();
            AddMessage(0, ports.Count.ToString() + " serial port found", " -> ", DateTime.Now);
            foreach (string s in ports)
            {
                AddMessage(0, "Search Device on  port " + s, " -> ", DateTime.Now);
                DiscoverDevices Dut = new DiscoverDevices();
                string serial;
                if (Dut.TestSerialPort(s, out serial))
                {
                    AddMessage(0, "Device " + serial + " found on  port " + s, " <- ", DateTime.Now);
                    _comList.Add(serial + ";" + s);

                }
                else
                    AddMessage(0, "No device found on  port " + s, " <- ", DateTime.Now);
            }
            rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_DiscoverPluggedDevicesCompleted, "End of Search reader process");
            if (NotifyEvent != null)
            {
                AddMessage(0, rfidReaderArgs.ReaderNotify.RN_DiscoverPluggedDevicesCompleted.ToString(), "NFY :", DateTime.Now);
                NotifyEvent(this, notifyEvent);
            }
        }

        private List<string> GetDevicePortCom()
        {
            List<string> comPortList = new List<string>(System.IO.Ports.SerialPort.GetPortNames());
            try
            {
                const string VID = "0403";
                const string PID = "6001";
                //ArrayList comPortList = new ArrayList(System.IO.Ports.SerialPort.GetPortNames());

                string pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
                Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
                List<string> comports = new List<string>();
                RegistryKey rk1 = Registry.LocalMachine;
                RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
                foreach (String s3 in rk2.GetSubKeyNames())
                {
                    RegistryKey rk3 = rk2.OpenSubKey(s3);
                    //KB filter on FTDIBUS only
                    if (!rk3.Name.Equals(@"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Enum\FTDIBUS"))
                        continue;
                    foreach (String s in rk3.GetSubKeyNames())
                    {
                        if (_rx.Match(s).Success)
                        {
                            RegistryKey rk4 = rk3.OpenSubKey(s);
                            foreach (String s2 in rk4.GetSubKeyNames())
                            {
                                RegistryKey rk5 = rk4.OpenSubKey(s2);
                                RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");

                                string prt = (string)rk6.GetValue("PortName");
                                if (!string.IsNullOrEmpty(prt))
                                {
                                    if (comPortList.Contains(prt))
                                        comports.Add(prt);
                                }
                            }
                        }
                    }
                }
                return comports;
            }
            catch
            {
                return comPortList;
            }
        }

        /// <summary>
        /// Method to connect a reader to a serial port.
        /// The result of the connection generate a notification ReaderNotify.RN_Connected if succeed
        /// or ReaderNotify.RN_FailedToConnect if failed.
        /// </summary>
        /// <param name="strPortCom">string name of the port com to connect</param>
        /// <returns>true is succeed, false otherwise</returns>
        public bool ConnectReader(string strPortCom)
        {

            if (string.IsNullOrEmpty(strPortCom))
            {
                rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FailedToConnect, "Com port is null");
                if (NotifyEvent != null)
                 NotifyEvent(this, notifyEvent);
                return false;               
            }

            if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "ConnectReader(" + strPortCom + ")", "CMD :", DateTime.Now);
            else
                AddMessage(0, "ConnectReader(" + strPortCom + ")", "CMD :", DateTime.Now);
            if (!IsConnected)
            {
                if (DeviceBoard != null) DeviceBoard = null;
                DeviceBoard = new DeviceRfidBoard(0x23200232, strPortCom, showOutbound);
                if (DeviceBoard.ConnectBoard())
                {

                    if ((_debugReader) && (_statusForm == null))
                    {
                        _statusForm = new StatusForm(this);
                        _statusForm.Show();
                    }

                    _eventUsbArrived = false;
                    StrCom = strPortCom;
                    _eventThread = new EventThread(DeviceBoard.deviceChannel, ReceiveAsynchronousEvent);
                    ReaderData.RspScan = RespScan.RS_ReaderReady;
                    IsConnected = true;
                    ReaderData.DeviceID = string.Format("{0:X8}", DeviceBoard.DeviceId);
                    SerialNumber = ReaderData.DeviceID;
                    _bPowerOn = true;
                    ScanTimeout = (uint)setTimeoutVsHardware(HardwareVersion);
                    CloseDoor();
                    Door_Status = SDK_SC_RfidReader.Door_Status.Door_Close;
                    _lastConnectionDate = DateTime.Now;

                    SetLightPower(100);
                    Thread.Sleep(500);
                    SetLightPower(0);

                    rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Connected, "Reader Connected to " + StrCom);
                    if (NotifyEvent != null)
                    {
                        if (DeviceBoard == null) return false;
                        AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Connected.ToString(), "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);
                    }
                    return true;
                }
                else
                {
                    if (_eventThread != null) _eventThread.stopThread();
                    DeviceBoard.deviceChannel.cancelPendingOperations();
                    Thread.Sleep(200);
                    _eventThread = null;
                    DeviceBoard.DisconnectBoard();
                    ReaderData.RspScan = RespScan.RS_FailedToConnect;
                    StrCom = null;
                    SerialNumber = null;
                    IsConnected = false;
                    rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FailedToConnect, "Reader Connection failed on  " + StrCom);
                    if (NotifyEvent != null)
                    {
                        if (DeviceBoard == null) return false;
                        AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_FailedToConnect.ToString(), "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);

                    }
                    return false;
                }
            }
            return false;
        }
        private ArrayList GetSerialPortPlugged()
        {
            ArrayList PortList = new ArrayList();

            RegistryKey hklm = Registry.LocalMachine;
            RegistryKey system = hklm.OpenSubKey("HARDWARE");

            RegistryKey DeviceMap = system.OpenSubKey("DEVICEMAP");
            RegistryKey SerialComm = DeviceMap.OpenSubKey("SERIALCOMM");

            if (SerialComm != null)
            {
                string[] section = SerialComm.GetValueNames();

                foreach (string str in section)
                {
                    PortList.Add(SerialComm.GetValue(str).ToString());
                }
            }
            return PortList;
        }

        private void OnDeviceEvent(object sender, EventArgs e)
        {
            try
            {
                //mutex.WaitOne(Timeout.Infinite, false);

                _eventUsbArrived = true;

                rfidReaderArgs notifyEvent;
                ArrayList tmpComList = GetSerialPortPlugged();

                if (!IsConnected) // test to reconnect
                {
                    foreach (string port in tmpComList)
                    {
                        if (!_bWithUSBWatcher) continue;
                        if (_comListPresent.Contains(port)) continue;
                        if (_comBluetooth.Contains(port)) break;

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_SerialPortPlugged, port);
                        if ((NotifyEvent != null) && (DeviceBoard != null))
                        {

                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_SerialPortPlugged.ToString(), "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);

                        }
                        Thread.Sleep(200);
                        DiscoverDevices dut = new DiscoverDevices();
                        string serial;

                        if (!dut.TestSerialPort(port, out serial)) continue;
                        if (serial != this.SerialNumber) continue;

                        ConnectReader(port);
                        _comListPresent.Clear();
                        _comListPresent = tmpComList;
                        return;
                    }
                }
                else // board connected
                {
                    if (!tmpComList.Contains(StrCom))
                    {
                        // MessageBox.Show("A device is disconnect on port" + strCom + "\r\n Kill the com thread and the object", Properties.ResStrings.strInfo);

                        _comListPresent.Clear();
                        _comListPresent = tmpComList;


                        if (_eventThread != null) _eventThread.stopThread();
                        DeviceBoard.deviceChannel.cancelPendingOperations();
                        Thread.Sleep(200);
                        _eventThread = null;
                        if (DeviceBoard.IsConnected) DeviceBoard.DisconnectBoard();
                        IsConnected = false;
                        _isInScan = false;
                        ReaderData.RspScan = RespScan.RS_ReaderNotReady;
                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Disconnected, "Reader Disconnected to " + StrCom);
                        if ((NotifyEvent != null) && (DeviceBoard != null))
                        {
                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Disconnected.ToString(), "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }
                        Thread.Sleep(200);
                        // MessageBox.Show("Launch the notification power cut due to usb unplugged", Properties.ResStrings.strInfo);
                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_UsbCableUnplug, "Usb Removed");
                        if ((NotifyEvent != null) && (DeviceBoard != null))
                        {
                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_UsbCableUnplug.ToString(), "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }
                    }

                }
                // Thread.Sleep(1000);
            }
            finally
            {
                //mutex.ReleaseMutex();
            }

        }
        /// <summary>
        /// Method to disconnect reader to serial port and stop listen thread.
        /// This Method generate a notification ReaderNotify.RN_Disconnected if succeed.
        /// </summary>
        /// <returns>true if succeed, false otherwise</returns>
        public bool DisconnectReader()
        {
            if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "DisConnectReader", "CMD :", DateTime.Now);
            else
                AddMessage(0, "DisConnectReader", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                try
                {                    
                    if (IsInScan)
                    {
                        RequestEndScan();
                        Thread.Sleep(2000);
                    }
                    if (_isInWaitTagMode)
                    {
                        SetWaitForTag(false);
                        Thread.Sleep(200);
                    }

                    if (_driveLight) SetLightPower(0);
                    if (_eventThread != null) _eventThread.stopThread();
                    DeviceBoard.deviceChannel.cancelPendingOperations();
                    Thread.Sleep(200);
                    _eventThread = null;
                    if (DeviceBoard.IsConnected) DeviceBoard.DisconnectBoard();
                    DeviceBoard = null;
                    IsConnected = false;
                    _isInScan = false;
                    _isInWaitTagMode = false;
                    ReaderData.DeviceID = null;
                    _driveLight = false;
                    ReaderData.RspScan = RespScan.RS_ReaderNotReady;
                    rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Disconnected, "Reader Disconnected to " + StrCom);
                    if (NotifyEvent != null)
                    {
                        AddMessage(0, rfidReaderArgs.ReaderNotify.RN_Disconnected.ToString(), "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);

                    }


                    return true;
                }
                catch (Exception e)
                {

                    rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_FailedToDisconnected, "Fail to Disconnected to " + StrCom + " : " + e.Message.ToString());
                    if (NotifyEvent != null)
                    {
                        AddMessage(0, rfidReaderArgs.ReaderNotify.RN_FailedToDisconnected.ToString(), "NFY :", DateTime.Now);
                        NotifyEvent(this, notifyEvent);

                    }
                    //throw new Exception();
                    return false;
                }
            }
            else
            {
                rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_AlreadyDisconnected, "Reader already Disconnected");
                if (NotifyEvent != null)
                {
                    AddMessage(0, rfidReaderArgs.ReaderNotify.RN_AlreadyDisconnected.ToString(), "NFY :", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }
                return false;
            }

        }
        /// <summary>
        /// Method to request end of the scanning thread.
        /// This Method generate a notification ReaderNotify.RN_ScanCancelByHost if succeed.
        /// </summary>
        public bool RequestEndScan()
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "RequestEndScan()", "CMD :", DateTime.Now);

            ReaderData = SerializeHelper.CloneObject(PreviousReaderData);
            ReaderData.RspScan = RespScan.RS_ReaderReady;

            if ((IsConnected) && (_bPowerOn) && (_isInScan))
            {
                
                _eventCancelByHost.Reset();
                if (DeviceBoard.control(false, true))
                {
                    if (!_eventCancelByHost.WaitOne(3000, false))
                    {
                        if (_scanThread != null)
                        {
                            _scanThread.Abort();
                            _scanThread.Join(1000);
                        }
                        _isInScan = false;
                         ReaderData.RspScan = RespScan.RS_ReaderReady;
                        rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "Host Stop Scan");
                        if (NotifyEvent != null)
                        {
                            showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                            NotifyEvent(this, notifyEvent);
                        }
                        return true;
                    }
                    else
                    {
                        _isInScan = false;
                        ReaderData.RspScan = RespScan.RS_ReaderReady;
                        rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "Host Stop Scan");
                        if (NotifyEvent != null)
                        {
                            showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                            NotifyEvent(this, notifyEvent);
                        }
                        return true;
                    }
                }
            }
            else
            {
                //  Set Forscaning to false in firmaware
                if (IsConnected)
                    DeviceBoard.control(false, true);

            }
            _isInScan = false;
            ReaderData.RspScan = RespScan.RS_ReaderReady;
            rfidReaderArgs notifyEventEnd = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "Host Stop Scan");
            if (NotifyEvent != null)
            {
                showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                NotifyEvent(this, notifyEventEnd);
            }                  
            return false;

        }
        /// <summary>
        /// Method to request tag inventory, failed in reader not ready (a scan is active, no connection available)        
        /// This Method generate a notification ReaderNotify.RN_ScanStarted when scan starts.
        /// This Method generate a notification ReaderNotify.RN_ScanCompleted when scan is completed.
        /// This Method generate a notification ReaderNotify.RN_ErrorDuringScan when an error occurs. 
        /// The error reason is present in readerData.FailureReason.
        /// If asynchronous event set, this method generate a notification ReaderNotify.RN_TagAdded when new tag found;
        /// and ReaderNotify.RN_TagRemoved when tag removed from previous state.
        /// If ResetList is set, only ReaderNotify.RN_TagAdded will be generate.
        /// If not use the asynchronous event, the reader inventory can be retrieve by ReadScannedTag() at the end of the scan.
        /// In any case readerData contain list of tag and status of the reader for the this inventory.
        /// </summary>
        /// <param name="ResetList">bool to reset the known tag list in board</param>
        /// <param name="AsynchronousEvent">bool to set asynchronous event flag</param>
        /// <param name="bUseKR">bool to set KR in  inventory mode . Must be true by default</param>
        /// <param name="bUnlockTagAllAxis">bool to set reverse orientation inventory . Must be true by default </param>
        public void RequestScan(bool ResetList, bool AsynchronousEvent, bool bUseKR, bool bUnlockTagAllAxis)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "RequestScan(" +
                       ResetList.ToString() + "," +
                       AsynchronousEvent.ToString() + "," +
                       bUseKR.ToString() + "," +
                       bUnlockTagAllAxis.ToString() + ")", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                _scanPourcent = 0;
                if (ReaderData.RspScan == RespScan.RS_ReaderReady)
                {
                    if (ResetList)
                    {
                        PreviousReaderData = SerializeHelper.CloneObject(ReaderData);
                        ReaderData = null;
                        ReaderData = new ReaderData();
                        ReaderData.RspScan = RespScan.RS_ReaderReady;
                        ReaderData.DeviceID = SerialNumber;
                    }
                }
                _eventScanStart.Reset();
                _eventScanEnd.Reset();
                _eventConfirmation.Reset();
                _threadArgs = new ThreadArgs(DeviceBoard,
                                            bUseKR,
                                            bUnlockTagAllAxis,
                                            AsynchronousEvent,
                                            ResetList,
                                            ReaderData,
                                            _eventScanStart,
                                            _eventScanEnd,
                                            _eventConfirmation,
                                            NotifyEvent,
                                            (int)ScanTimeout,
                                            SerialNumber,
                                            _isInScan,
                                            _lastScanSerial,
                                            GetConfirmationStatus,
                                            GetCheckKZout,
                                            showOutbound,
                                            true);
                _scanThread = new Thread(new ThreadStart(_threadArgs.Run));
                _scanThread.IsBackground = true;
                _scanThread.Start();
            }

        }
        /// <summary>
        /// Same as  RequestScan but with choose of one channel.
        /// </summary>
        /// <param name="ChannelNumber">byte of the channel number to scan</param>
        /// <param name="ResetList"></param>
        /// <param name="AsynchronousEvent"></param>
        /// <param name="bUseKR"></param>
        /// <param name="bUnlockTagAllAxis"></param>
        public void RequestScan(byte ChannelNumber, bool ResetList, bool AsynchronousEvent, bool bUseKR, bool bUnlockTagAllAxis)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "RequestScan(" +
                      ChannelNumber.ToString() + "," +
                      ResetList.ToString() + "," +
                      AsynchronousEvent.ToString() + "," +
                      bUseKR.ToString() + "," +
                      bUnlockTagAllAxis.ToString() + ")", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                _scanPourcent = 0;
                if (ChannelNumber >= 8) return;
                if (ChannelNumber <= 0) return;
                if (ReaderData.RspScan == RespScan.RS_ReaderReady)
                {
                    if (ResetList)
                    {
                        PreviousReaderData = SerializeHelper.CloneObject(ReaderData);

                        ReaderData = null;
                        ReaderData = new ReaderData();
                        ReaderData.RspScan = RespScan.RS_ReaderReady;
                        ReaderData.DeviceID = SerialNumber;
                    }
                }

                SendSwitchCommand(true, ChannelNumber, true);
                _eventScanStart.Reset();
                _eventScanEnd.Reset();
                _eventConfirmation.Reset();
                _threadArgs = new ThreadArgs(DeviceBoard,
                                            bUseKR,
                                            bUnlockTagAllAxis,
                                            AsynchronousEvent,
                                            ResetList,
                                            ReaderData,
                                            _eventScanStart,
                                            _eventScanEnd,
                                            _eventConfirmation,
                                            NotifyEvent,
                                            (int)ScanTimeout,
                                            SerialNumber,
                                            _isInScan,
                                            _lastScanSerial,
                                            GetConfirmationStatus,
                                            GetCheckKZout,
                                            showOutbound,true);
                _scanThread = new Thread(new ThreadStart(_threadArgs.Run));
                _scanThread.IsBackground = true;
                _scanThread.Start();
            }

        }
        /// <summary>
        /// Method to request tag inventory, failed in reader not ready (a scan is active, no connection available)    
        /// This Mode impose to use reset list, the board must be cleared known tags each time the relay switch
        /// This Mode impose also to be in asynchonous Mode
        /// This Method generates a notification ReaderNotify.RN_ScanStarted when scan starts.
        /// This Method generates a notification ReaderNotify.RN_ScanCompleted when scan is completed.
        /// This Method generates a notification ReaderNotify.RN_ErrorDuringScan when an error occurs. 
        /// The error reason is present in readerData.FailureReason.
        /// This method generates a notification ReaderNotify.RN_TagAdded when new tag found;        
        /// ReaderData contains list of tag and status of the reader for the this inventory.
        /// </summary>
        /// <param name="bUseKR">bool to set KR in  inventory mode . Must be true by default</param>
        /// <param name="bUnlockTagAllAxis">bool to set to unlock all tags . Must be true by default, can be put to false when door not open more than 5 min</param>
        public void RequestScan3D(bool bUseKR, bool bUnlockTagAllAxis)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "RequestScan3D(" +
                        bUseKR.ToString() + "," +
                        bUnlockTagAllAxis.ToString() + ")", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                _scanPourcent = 0;


               

                    PreviousReaderData = SerializeHelper.CloneObject(ReaderData);
                    ReaderData = null;
                    ReaderData = new ReaderData();
                    ReaderData.RspScan = RespScan.RS_ReaderReady;
                    ReaderData.DeviceID = SerialNumber;
            


                _eventScanStart.Reset();
                _eventScanEnd.Reset();
                _eventConfirmation.Reset();
                _threadArgs = new ThreadArgs(DeviceBoard,
                                        bUseKR,
                                        bUnlockTagAllAxis,
                                        true,
                                        true,
                                        ReaderData,
                                        _eventScanStart,
                                        _eventScanEnd,
                                        _eventConfirmation,
                                        NotifyEvent,
                                        (int)ScanTimeout,
                                        SerialNumber,
                                        _isInScan,
                                        _lastScanSerial,
                                        GetConfirmationStatus,
                                        GetCheckKZout,
                                        showOutbound,true);


                _scanThread = new Thread(new ThreadStart(_threadArgs.Run));
                _scanThread.IsBackground = true;
                _scanThread.Start();

                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Thread start request", "CMD :", DateTime.Now);
            }
        }
        public void RequestScanSelectedAxis(bool bUseKR, bool bUnlockTagAllAxis )
        {
            if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "RequestScan3D(" +
                            bUseKR.ToString() + "," +
                            bUnlockTagAllAxis.ToString() + ")", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                _scanPourcent = 0;




                PreviousReaderData = SerializeHelper.CloneObject(ReaderData);
                ReaderData = null;
                ReaderData = new ReaderData();
                ReaderData.RspScan = RespScan.RS_ReaderReady;
                ReaderData.DeviceID = SerialNumber;



                _eventScanStart.Reset();
                _eventScanEnd.Reset();
                _eventConfirmation.Reset();
                _threadArgs = new ThreadArgs(DeviceBoard,
                                        bUseKR,
                                        bUnlockTagAllAxis,
                                        false,
                                        true,
                                        ReaderData,
                                        _eventScanStart,
                                        _eventScanEnd,
                                        _eventConfirmation,
                                        NotifyEvent,
                                        (int)ScanTimeout,
                                        SerialNumber,
                                        _isInScan,
                                        _lastScanSerial,
                                        GetConfirmationStatus,
                                        GetCheckKZout,
                                        showOutbound, true);


                _scanThread = new Thread(new ThreadStart(_threadArgs.Run));
                _scanThread.IsBackground = true;
                _scanThread.Start();

                if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, "Thread start request", "CMD :", DateTime.Now);
            }
        }
        /// <summary>
        /// Method to request tag inventory, failed in reader not ready (a scan is active, no connection available)    
        /// This Mode impose to use reset list, the board must be cleared known tags each time the relay switch
        /// This Mode impose also to be in asynchonous Mode
        /// This Method generates a notification ReaderNotify.RN_ScanStarted when scan starts.
        /// This Method generates a notification ReaderNotify.RN_ScanCompleted when scan is completed.
        /// This Method generates a notification ReaderNotify.RN_ErrorDuringScan when an error occurs. 
        /// The error reason is present in readerData.FailureReason.
        /// This method generates a notification ReaderNotify.RN_TagAdded when new tag found;        
        /// ReaderData contains list of tag and status of the reader for the this inventory.
        /// </summary>
        /// <param name="bUseKR">bool to set KR in  inventory mode . Must be true by default</param>
        /// <param name="bUnlockTagAllAxis">bool to set to unlock all tags . Must be true by default, can be put to false when door not open more than 5 min</param>
        /// <param name="UseMutex">Bool to scan simultaneously or not the devices</param>
        public void RequestScan3D(bool bUseKR, bool bUnlockTagAllAxis,bool UseMutex)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "RequestScan3D(" +
                        bUseKR.ToString() + "," +
                        bUnlockTagAllAxis.ToString() + ")", "CMD :", DateTime.Now);

            if ((!IsConnected) || (!_bPowerOn)) return;

            _scanPourcent = 0;


           
                PreviousReaderData = SerializeHelper.CloneObject(ReaderData);

                ReaderData = null;
                ReaderData = new ReaderData();
                ReaderData.RspScan = RespScan.RS_ReaderReady;
                ReaderData.DeviceID = SerialNumber;
           

            _eventScanStart.Reset();
            _eventScanEnd.Reset();
            _eventConfirmation.Reset();
            _threadArgs = new ThreadArgs(DeviceBoard,
                bUseKR,
                bUnlockTagAllAxis,
                true,
                true,
                ReaderData,
                _eventScanStart,
                _eventScanEnd,
                _eventConfirmation,
                NotifyEvent,
                (int)ScanTimeout,
                SerialNumber,
                _isInScan,
                _lastScanSerial,
                GetConfirmationStatus,
                GetCheckKZout,
                showOutbound, UseMutex);


            _scanThread = new Thread(new ThreadStart(_threadArgs.Run));
            _scanThread.IsBackground = true;
            _scanThread.Start();

            if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Thread start request", "CMD :", DateTime.Now);
        }

        /// <summary>
        /// Get Statut from rfid board
        /// </summary>
        /// <param name="strStatut"></param>
        /// <returns></returns>
        public bool getStatus(out string strStatut)
        {
            string strTmp = null;
            try
            {
                if (DeviceBoard != null)
                {
                    strTmp = DeviceBoard.getStatus().ToString();
                }
                strStatut = strTmp;
                return true;
            }
            catch
            {
                strStatut = strTmp;
                return false;
                //throw new Exception();
            }
        }


        /// <summary>
        /// Method to retrieve list of stored tag in reader board.
        /// Tag list are present in readerData.strListTag list and number of tag are in  readerData.nbTagScan
        /// </summary>
        public void ReadScannedTag()
        {

            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "ReadScannedTag()", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                bool bFirstTag = true;
                uint tagIndex;
                uint tagCount;
                UInt64 tagID;
                ReaderData.strListTag.Clear();

                ReaderData.nbTagScan = 0;

                while (DeviceBoard.getNextTag(bFirstTag, out tagID, out tagIndex, out tagCount))
                {
                    bFirstTag = false;
                    string codeToAdd = SerialRFID.SerialNumberAsString(tagID);
                    ReaderData.strListTag.Add(codeToAdd.Substring(0, MaxDigit));

                }
                ReaderData.nbTagScan = ReaderData.strListTag.Count;
                rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ReadTagCompleted, "Tag Read Completed.");
                if (NotifyEvent != null)
                {
                    if (DeviceBoard != null)
                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_ReadTagCompleted.ToString(), "NFY :", DateTime.Now);
                    NotifyEvent(this, notifyEvent);
                }
            }
        }
        /// <summary>
        /// Method to control switch board through RS485
        /// </summary>
        /// <param name="bSet">Set or clear relay</param>
        /// <param name="RelaisNumber">relais number to control (1 to 8) ; 9 for all</param>
        /// <param name="ResetAllBeforSet">if true clear all Relay befor set one.</param>
        public void SendSwitchCommand(bool bSet, byte RelaisNumber, bool ResetAllBeforSet)
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "SendSwitchCommand(" +
                    bSet.ToString() + "," +
                    RelaisNumber.ToString() + "," +
                    ResetAllBeforSet.ToString() + ")", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                byte AsciiRelaisValue;
                if (bSet)
                {
                    if (ResetAllBeforSet)
                    {
                        AsciiRelaisValue = (byte)(9 + 0x30);
                        DeviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
                        Thread.Sleep(50);
                    }
                    AsciiRelaisValue = (byte)(RelaisNumber + 0x30);
                    DeviceBoard.sendSwitchCommand(1, AsciiRelaisValue);
                    Thread.Sleep(50);
                }
                else
                {
                    AsciiRelaisValue = (byte)(RelaisNumber + 0x30);
                    DeviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
                    Thread.Sleep(50);
                }
              /*  AsciiRelaisValue = (byte)(RelaisNumber + 0x30);
                DeviceBoard.sendSwitchCommand(1, AsciiRelaisValue);
                Thread.Sleep(500);*/
            }
        }
        /// <summary>
        /// Method to set Light power
        /// </summary>
        /// <param name="power">power value to set (0-300)</param>
        public void SetLightPower(UInt16 power)
        {           
            if ((IsConnected) && (_bPowerOn) && (!_isInScan))
            {
                LightValue = (int)power;
                DeviceBoard.SetLightDuty(power);
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "SetLightPower(" +
                  power.ToString() + ")", "CMD :", DateTime.Now);
            }
        }
        /// <summary>
        /// Method to switch on IR sensor
        /// </summary>
        /// <param name="bStart"></param>
        public void SetIRSensorON(bool bStart)
        {
            if ((IsConnected) && (_bPowerOn))
            {
                DeviceBoard.setInfraRedSensor(bStart);
            }
        }
        /// <summary>
        /// Function to set alarm and strob
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="bStrobeOn"></param>
        /// <param name="bHeavySoundOn"></param>
        /// <param name="bLoudSoundOn"></param>
        public bool SetIpAlarm(string hostname, bool bStrobeOn, bool bHeavySoundOn, bool bLoudSoundOn)
        {
            try
            {
                if (!PingAlarm(hostname)) return false;

                string httpLink = "http://" + hostname + "/preset.htm?";

                if (bStrobeOn)
                    httpLink += "led1=1";
                else
                    httpLink += "led1=0";

                if (bHeavySoundOn)
                    httpLink += "&led2=1";
                else
                    httpLink += "&led2=0";
                if (bLoudSoundOn)
                    httpLink += "&led3=1";
                else
                    httpLink += "&led3=0";

                ASCIIEncoding encoding = new ASCIIEncoding();

                byte[] buffer = encoding.GetBytes(httpLink);
                // Prepare web request...
                HttpWebRequest myRequest =
                      (HttpWebRequest)WebRequest.Create(httpLink);

                // We use POST ( we can also use GET )
                myRequest.Method = "POST";
                // Set the content type to a FORM
                myRequest.ContentType = "application/x-www-form-urlencoded";
                // Get length of content
                myRequest.ContentLength = buffer.Length;
                // Get request stream
                Stream newStream = myRequest.GetRequestStream();
                // Send the data.
                newStream.Write(buffer, 0, buffer.Length);
                // Close stream
                newStream.Close();

                HttpWebResponse loWebResponse = (HttpWebResponse)myRequest.GetResponse();

                Encoding enc = System.Text.Encoding.GetEncoding(1252);

                StreamReader loResponseStream =
                   new StreamReader(loWebResponse.GetResponseStream(), enc);

                string lcHtml = loResponseStream.ReadToEnd();

                loWebResponse.Close();
                loResponseStream.Close();

                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Write to  " + hostname + " succeed :" + httpLink, "ALA :", DateTime.Now);

            }
            catch (Exception exp)
            {
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Write to  " + hostname + " failed :" + exp.Message, "ALA :", DateTime.Now);
                //throw new Exception();
            }
            return true;
        }

        private bool PingAlarm(string Host)
        {
            const int Timeout = 120;
            String Data = "[012345678901234567890123456789]";
            byte[] buffer = Encoding.ASCII.GetBytes(Data);
            Ping Sender = new Ping();
            PingReply Reply = Sender.Send(Host, Timeout, buffer);

            if (Reply.Status == IPStatus.Success)
            {
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Ping " + Host + " Succeed", "ALA :", DateTime.Now);
                return true;
            }
            else
            {
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Ping " + Host + " failed", "ALA :", DateTime.Now);
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CalibrateDialog()
        {
            SDK_SC_RfidReader.UtilsWindows.CalibrationGraphDialog diagCal =
                  new SDK_SC_RfidReader.UtilsWindows.CalibrationGraphDialog(DeviceBoard);
            diagCal.Show();
        }

        SDK_SC_RfidReader.UtilsWindows.FindThresholdDialog diagThreshold = null;
        /// <summary>
        /// 
        /// </summary>
        public void FindThresholdDialog()
        {
            diagThreshold = new SDK_SC_RfidReader.UtilsWindows.FindThresholdDialog(DeviceBoard);
            diagThreshold.ShowDialog();
            diagThreshold = null;
        }
        /// <summary>
        /// 
        /// </summary>
        public void ConversionDialog()
        {
            SDK_SC_RfidReader.UtilsWindows.ConversionsDialog diagConv =
                  new SDK_SC_RfidReader.UtilsWindows.ConversionsDialog();
            diagConv.Show();
        }
        SDK_SC_RfidReader.UtilsWindows.TagSetsDialog diagTagSet = null;
        /// <summary>
        /// 
        /// </summary>
        public void TagSetDialog()
        {
            diagTagSet = new SDK_SC_RfidReader.UtilsWindows.TagSetsDialog(DeviceBoard);
            diagTagSet.ShowDialog();
            diagTagSet = null;
        }

        SDK_SC_RfidReader.UtilsWindows.DoorAndLightDialog diagDoorLight = null;
        /// <summary>
        /// 
        /// </summary>
        public void DoorAndLightDiag()
        {
            diagDoorLight = new SDK_SC_RfidReader.UtilsWindows.DoorAndLightDialog(DeviceBoard);
            diagDoorLight.ShowDialog();
            diagDoorLight = null;
        }

        private void SwitchCurrentAxis(int axis)
        {
           // byte asciiRelaisValue = (9 + 0x30);
           // DeviceBoard.sendSwitchCommand(0, asciiRelaisValue); // clearCounts all
           // Thread.Sleep(100);
            DeviceBoard.setAntenna(false);
            Thread.Sleep(10);
            byte asciiRelaisValue = (byte)(axis + 0x30);
            DeviceBoard.sendSwitchCommand(1, asciiRelaisValue);
            Thread.Sleep(50);
        }


        public void LockAllTag(int axis)
        {
            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            DeviceBoard.sendSyncPulse();
            ushort Rcor;
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out Rcor);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out Rcor);
            Thread.Sleep(10);
            DeviceBoard.setAntenna(false);
        }

        public void UnlockAllTag(int axis)
        {
            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);         
            DeviceBoard.sendSyncPulse();           
            ushort Rcor;
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);           
            Thread.Sleep(10);
            DeviceBoard.setAntenna(false);
        }

        public void StartLedOn(int axis)
        {
            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            DeviceBoard.sendSyncPulse();
            ushort Rcor;
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
        }
        public bool StartLedOn2(int axis)
        {
            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            DeviceBoard.sendSyncPulse();
            ushort Rcor;
            DeviceBoard.sendCommand((byte) LowlevelBasicOrder.KD, out Rcor);
            DeviceBoard.sendCommand((byte) LowlevelBasicOrder.KZ_SPCE2, out Rcor);
            _eventConfirmation.WaitOne(500);
            Thread.Sleep(50);
            if (_status == 1)
                return true;
            return false;
        }
        public void StopField()
        {
            if (DeviceBoard != null)
                DeviceBoard.setAntenna(false);
            Thread.Sleep(10);
        }
        public void LEdOnAll(int axis , int timeout, bool bChangeChannel = true)
        {
            if (bChangeChannel)
                SwitchCurrentAxis(axis);

            DeviceBoard.setAntenna(true);
            Thread.Sleep(50);
            //DeviceBoard.sendSyncPulse();
            ushort Rcor;
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out Rcor);
            Thread.Sleep(50);
            if (timeout > 0)
            Thread.Sleep(timeout * 1000);
        }
               

        public bool ConfirmTagUID(int axis, string tagUID)
        {     
            /*ushort Rcor;
            _status = 0;

            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            Thread.Sleep(50);
            DeviceBoard.sendSyncPulse();
            Thread.Sleep(50);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            Thread.Sleep(50);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out Rcor);
            _eventConfirmation.WaitOne(500);        
            Thread.Sleep(50);

            if (_status != 1) return false;

                _confirmationStatut = 0;
                LoadUIDForConfirmation(tagUID);
                DeviceBoard.confirmLoadedUID(24);
            _eventConfirmation.WaitOne(1000);

            Thread.Sleep(50);*/
              _status = 0;
            ushort Rcor;

            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            DeviceBoard.sendSyncPulse();
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out Rcor);
            Thread.Sleep(100);           

            if (_status == 1)
            {
                _confirmationStatut = 0;

                if (tagUID.Length > 20)
                {
                    return ConfirmTagUIDFullMem(tagUID, 42);
                }
                else
                {
                    LoadUIDForConfirmation(tagUID);
                    DeviceBoard.confirmLoadedUID(24);
                    _eventConfirmation.WaitOne(100);
                    Thread.Sleep(100);
                    return _confirmationStatut == 1;
                }
            }
            return false;

        }

        //public string getTagUidOct(string TagUidAlpha)
        public string getTagUidOct(string TagUidAlpha)
        {
            try
            {
                if (ReaderData != null)
                {
                    if (ReaderData.ListTagInfo != null)
                    {
                        foreach (TagInfo ti in ReaderData.ListTagInfo)
                        {
                            int len = TagUidAlpha.Length;
                            if (ti.TagType == TagType.TT_R8) continue;
                            if (ti.TagType == TagType.TT_SPCE2_RO)
                            {
                                if (ti.TagId_R8_RO.Substring(0, len).Equals(TagUidAlpha))
                                    return ti.TagIdOctal;
                            }
                            else if (ti.TagType == TagType.TT_SPCE2_RW)
                            {
                                if (ti.TagId_RW.Equals((TagUidAlpha)))
                                    return ti.TagIdOctal;
                            }
                            else if (ti.TagType == TagType.TT_SPCE2_DEC)
                            {
                                if (ti.TagId_DEC.Equals((TagUidAlpha)))
                                    return ti.TagIdOctal;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
               
            }
           
            return null;

        }


        public int ConfirmAndLed(int axis, Hashtable ListTagUID)
        {     
            ushort Rcor;
            _status = 0;
            List<string> tagFound = new List<string>();
          
            SwitchCurrentAxis(axis);         
            DeviceBoard.setAntenna(true);
            DeviceBoard.sendSyncPulse();
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);           

            foreach (DictionaryEntry entry in ListTagUID)
            {
                if ((bool)entry.Value) continue;
               
                //string tagUID = SerialRFID.ConvertAlphaNumToOct(alphaUID.Substring(0,10));
                string tagUID = getTagUidOct((string) entry.Key);
                if (string.IsNullOrEmpty(tagUID))
                    tagUID = SerialRFID.ConvertAlphaNumToOct((string)entry.Key,TagType.TT_SPCE2_RO);
                if (string.IsNullOrEmpty(tagUID)) continue;
                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out Rcor);
                _eventConfirmation.WaitOne(500);
                Thread.Sleep(50);
                if (_status == 1)
                {
                    _confirmationStatut = 0;
                    if (tagUID.Length > 20)
                    {
                        if (ConfirmTagUIDFullMem(tagUID, 42))
                        {
                            _status = 0;
                            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out Rcor);
                            _eventConfirmation.WaitOne(500);

                            if (_status == 1)
                            {
                                _status = 0;
                                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out Rcor);
                                tagFound.Add((string)entry.Key);
                            }
                        }
                    }
                    else
                    {
                        LoadUIDForConfirmation(tagUID);
                        DeviceBoard.confirmLoadedUID(24);
                        _eventConfirmation.WaitOne(500);
                        if (_confirmationStatut == 1)
                        {
                            _status = 0;
                            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out Rcor);
                            _eventConfirmation.WaitOne(500);

                            if (_status == 1)
                            {
                                _status = 0;
                                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out Rcor);
                                tagFound.Add((string)entry.Key);
                            }
                        }
                    }
                }
            }

            foreach (string uid in tagFound) ListTagUID[uid] = true;

            return tagFound.Count;
        }
        public void ConfirmAndLight(int axis, List<string> tagList)
        {
            ushort rcor;
            _status = 0;
            SwitchCurrentAxis(axis);
            Thread.Sleep(50);
            DeviceBoard.setAntenna(true);
            Thread.Sleep(50);
            DeviceBoard.sendSyncPulse();
            Thread.Sleep(50);

           

            List<string> tagFound = new List<string>();

            foreach (string tagId in tagList)
            {
                _status = 0;
                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out rcor);
                _eventConfirmation.WaitOne(500);
                Thread.Sleep(10);
                if (_status == 1)
                {
                    _confirmationStatut = 0;
                    //string alphaUID = tagId + "§0000000000";
                    string tagUID = getTagUidOct((string)tagId);
                    if (string.IsNullOrEmpty(tagUID))
                        tagUID = SerialRFID.ConvertAlphaNumToOct((string)tagId, TagType.TT_SPCE2_RO);
                    if (string.IsNullOrEmpty(tagUID)) continue;

                    if (tagUID.Length > 20)
                    {

                        if (ConfirmTagUIDFullMem(tagUID, 42))
                        {
                            _status = 0;
                            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out rcor);
                            _eventConfirmation.WaitOne(500);
                            Thread.Sleep(10);
                            if (_status == 1)
                            {
                                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out rcor);
                                tagFound.Add(tagId);
                            }
                        }
                    }
                    else
                    {
                        LoadUIDForConfirmation(tagUID);
                        DeviceBoard.confirmLoadedUID(24);
                        _eventConfirmation.WaitOne(500);
                        Thread.Sleep(10);
                        if (_confirmationStatut == 1)
                        {
                            _status = 0;
                            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out rcor);
                            _eventConfirmation.WaitOne(500);
                            Thread.Sleep(10);
                            if (_status == 1)
                            {
                                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out rcor);
                                tagFound.Add(tagId);
                            }
                        }
                    }
                }
            }

            foreach (string tagId in tagFound) tagList.Remove(tagId);
        }
        public void ConfirmAndLightWithKD(int axis, List<string> tagList , bool bLightTag = true)
        {
            ushort rcor;
            _status = 0;
           
            DeviceBoard.setAntenna(true);
            Thread.Sleep(50);
            DeviceBoard.sendSyncPulse();
            Thread.Sleep(50);
            ushort Rcor;
            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            Thread.Sleep(50);
            List<string> tagFound = new List<string>();

            bool bQuit = false;

            foreach (string tagId in tagList)
            {
                if (bQuit) break;
                _status = 0;
                _eventConfirmation.Reset();
                DeviceBoard.sendSyncPulse();
                Thread.Sleep(10);
                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out rcor);
                Thread.Sleep(10);
                _eventConfirmation.WaitOne(500);
                Thread.Sleep(10);
                if (_status == 0)
                {
                    //retry KZ
                    _status = 0;
                    _eventConfirmation.Reset();
                    DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out rcor);
                    Thread.Sleep(10);
                    _eventConfirmation.WaitOne(500);
                    Thread.Sleep(10);
                    if (_status == 0) bQuit = true;
                }
                if (_status == 1)
                {
                    _confirmationStatut = 0;
                    //string alphaUID = tagId + "§0000000000";
                    string tagUID = getTagUidOct((string)tagId);
                    if (string.IsNullOrEmpty(tagUID))
                        tagUID = SerialRFID.ConvertAlphaNumToOct((string)tagId, TagType.TT_SPCE2_RO);
                    if (string.IsNullOrEmpty(tagUID)) continue;

                    if (tagUID.Length > 20)
                    {
                        if (ConfirmTagUIDFullMem(tagUID, 42))
                        {
                            _status = 0;
                            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out rcor);
                            Thread.Sleep(10);
                            _eventConfirmation.WaitOne(500);
                            Thread.Sleep(10);
                            if ((_status == 1) && (bLightTag))
                            {
                                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out rcor);
                                Thread.Sleep(25);
                                tagFound.Add(tagId);
                            }
                        }
                    }
                    else
                    {
                        LoadUIDForConfirmation(tagUID);
                        DeviceBoard.confirmLoadedUID(24);
                        _eventConfirmation.WaitOne(500);
                        Thread.Sleep(10);
                        if (_confirmationStatut == 1)
                        {
                            _status = 0;
                            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out rcor);
                            _eventConfirmation.WaitOne(500);
                            Thread.Sleep(10);
                            if ((_status == 1) && (bLightTag))
                            {
                                DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KL, out rcor);
                                Thread.Sleep(25);
                                tagFound.Add(tagId);
                            }
                        }
                    }
                }
                
            }

            foreach (string tagId in tagFound) tagList.Remove(tagId);
        }
        public int[] WriteBlock(byte nbBlock, string[] dataHexa, string tagToWrite, int axis)
        {
            int[] arrayStatus = new int[6];
            for ( int i = 0 ; i < 6 ; ++i) arrayStatus[i] = -1;

            statusWrite = -1;
            _status = 0;
            ushort Rcor;

            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            Thread.Sleep(50);

            DeviceBoard.sendSyncPulse();
            Thread.Sleep(50);

            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            Thread.Sleep(50);     

            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out Rcor);
            Thread.Sleep(100);

            arrayStatus[0] = _status;

            if (_status == 1)
            {
                _confirmationStatut = 0;
                LoadUIDForConfirmation(tagToWrite);
                DeviceBoard.confirmLoadedUID(24);
                _eventConfirmation.WaitOne(100);      

                Thread.Sleep(100);
                arrayStatus[1] = _confirmationStatut;

                if (_confirmationStatut == 1)
                {
                    _status = 0;   

                    for (int loop = 0; loop < nbBlock; ++loop)
                    {
                        byte block = (byte)(loop + 1);

                        uint data = UInt32.Parse(dataHexa[loop], NumberStyles.HexNumber);
                        DeviceBoard.WriteBlock(block, data);
                        Thread.Sleep(100);

                        arrayStatus[loop+2] = statusWrite;
                    }
                }
            }            

            DeviceBoard.setAntenna(false);
            Thread.Sleep(50);
            return arrayStatus;           
        }

        public int[] WriteBlockFullMem(byte nbBlock, string[] dataHexa, string tagToWrite, int axis)
        {
            int[] arrayStatus = new int[6];
            for (int i = 0; i < 6; ++i) arrayStatus[i] = -1;

            statusWrite = -1;
            _status = 0;
            ushort Rcor;

            SwitchCurrentAxis(axis);
            DeviceBoard.setAntenna(true);
            Thread.Sleep(50);

            DeviceBoard.sendSyncPulse();
            Thread.Sleep(50);

            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
            Thread.Sleep(50);

            DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out Rcor);
            Thread.Sleep(100);

            arrayStatus[0] = _status;

            if (_status == 1)
            {
                if (ConfirmTagUIDFullMem(tagToWrite,42))
                {
                    _status = 0;

                    for (int loop = 0; loop < nbBlock; ++loop)
                    {
                        byte block = (byte)(loop + 1);

                        uint data = UInt32.Parse(dataHexa[loop], NumberStyles.HexNumber);
                        DeviceBoard.WriteBlock(block, data);
                        Thread.Sleep(100);

                        arrayStatus[loop + 2] = statusWrite;
                    }
                }
            }

            DeviceBoard.setAntenna(false);
            Thread.Sleep(50);
            return arrayStatus;
        }


        private void LoadUIDForConfirmation(string strUIDtoLoad)
        {
            int len = strUIDtoLoad.Length;
            //CR 42digits
            byte[] TagDigits = new byte[24];
            uint index = 0;
            uint chunkDigitIndex = 0;
            string uidwithCRC = strUIDtoLoad;
            for (int loop = 0; loop < len; loop++)
            {
                if (chunkDigitIndex < 5)
                {
                    chunkDigitIndex++;
                    string ch = strUIDtoLoad[loop].ToString(); ;
                    TagDigits[index++] = byte.Parse(ch);
                }
                else
                {
                    chunkDigitIndex = 0;
                    TagDigits[index++] = byte.Parse("8"); //Put a surely bad checksum Must be not take into account
                    chunkDigitIndex++;
                    string ch = strUIDtoLoad[loop].ToString(); ;
                    TagDigits[index++] = byte.Parse(ch);
                }
            }

            TagIdType mySerial = new TagIdType();
            unsafe
            {
                //CR 42digits
                ConvertTagDigitsTo64(&mySerial, 24, TagDigits);
                //ConvertTagDigitsTo64(&mySerial, 42, TagDigits);

            }

            DeviceBoard.setTagUidDigit(1, mySerial);
            DeviceBoard.setTagUidDigit(2, mySerial);

        }


        public bool ConfirmTagUIDFullMem(string strUIDtoConfirm, byte nbDigits)
        {
            byte[] TagDigitToConfirm = new byte[16];


            // Adapt nbdigit versus UID

            if (strUIDtoConfirm.StartsWith("1")) // it's a GIA
                nbDigits = 18;
            else if (strUIDtoConfirm.StartsWith("3")) // it's a RW or RO
                nbDigits = 12;
            else if (strUIDtoConfirm.StartsWith("2")) // it's a RW so 42 digits or until EOF - for now let full mem
                nbDigits = 42;


            // convert string in bit
            string strBin = string.Empty;
            foreach (char ch in strUIDtoConfirm)
            {
                strBin += Convert.ToString(((int)ch - 48), 2).PadLeft(3, '0');
            }
            strBin = strBin.PadRight(128, '0');
            // form 16 bytes from str bin

            for (int loop = 0; loop < 16; loop++)
            {
                string byteInStr = strBin.Substring(loop * 8, 8);
                TagDigitToConfirm[loop] = Convert.ToByte(byteInStr, 2);
            }
            _eventConfirmation.Reset();
            _confirmationStatut = 0;
            DeviceBoard.ConfirmationFullMemory(nbDigits, TagDigitToConfirm);
            DeviceBoard.confirmLoadedUID(nbDigits);
            Thread.Sleep(10);
            _eventConfirmation.WaitOne(500);
            Thread.Sleep(10);
            return _confirmationStatut == 1;
        }

        public void ConfirmList(int axis, List<string> tagList, TagType tagType)
        {
            ushort rcor;
            _status = 0;

            SwitchCurrentAxis(axis);
            Thread.Sleep(10);
            DeviceBoard.setAntenna(true);
            Thread.Sleep(10);
            DeviceBoard.sendSyncPulse();
            Thread.Sleep(10);
            ushort Rcor;
           // DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KD, out Rcor);
           //Thread.Sleep(50);
            List<string> tagFound = new List<string>();

            foreach (string tagId in tagList)
            {
                _status = 0;
                if (tagType == TagType.TT_R8)
                    DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ, out rcor);
                else
                    DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KZ_SPCE2, out rcor);
                _eventConfirmation.WaitOne(500);
                Thread.Sleep(10);
                if (_status == 1)
                {
                    if (ConfirmTagUIDFullMem(tagId,12))
                    {
                        _status = 0;
                        DeviceBoard.sendCommand((byte)LowlevelBasicOrder.KB, out rcor);
                        _eventConfirmation.WaitOne(500);
                        Thread.Sleep(10);
                        if (_status == 1)
                        {
                            rfidReaderArgs notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, tagId);
                            if (NotifyEvent != null)
                            {
                                if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + tagId, "NFY :", DateTime.Now);
                                NotifyEvent(this, notifyEvent);
                            }
                            tagFound.Add(tagId);
                        }
                    }
                }
            }

            foreach (string tagId in tagFound) tagList.Remove(tagId);
        }

        public static unsafe void ConvertTagDigitsTo64(TagIdType* pSerialNumber, uint digitCount, byte[] TagDigits)
        {
            UInt16 word = 0;
            byte* pByte = (byte*)&pSerialNumber->word[0] + 7; // Point to the high-order byte of serialNumber.
            uint shift = 9; // First digit starts at bit 59 of the 64-bit serial number, or bit 11 of the high word.
            uint chunkDigitIndex = 0;
            uint digitIndex = 0;
            while (true)
            {
                if (chunkDigitIndex < 5)
                {
                    chunkDigitIndex++;

                    // If digitIndex is less than the requested digits to copy, copy the digit, else
                    // assume the copied digit is 0. If done with all 24 digits, break out of loop.
                    if (digitIndex < digitCount)
                        word |= (UInt16)((int)TagDigits[digitIndex] << (int)shift);
                    //CR 42digits
                    else if (digitIndex == 24)
                        //else if (digitIndex == 42)
                        break;

                    if (shift < 3)
                    {
                        *pByte-- = ((byte*)&word)[1];
                        shift += 8;
                        word = (UInt16)((int)word << 8);
                    }
                    shift -= 3;
                }
                else
                    chunkDigitIndex = 0; // Skip checksum digits.

                digitIndex++;
            }
            *pByte = ((byte*)&word)[1];
        }


        #region Test


        const uint SampleCount = 435;
		const ushort CorrelationSampleCount = 10;
		const uint PhaseShiftSampleCount = 10;
		const uint MIN_AllowableCorrelation = 90;
		const uint MAX_AllowableSigma = 10;
		const uint MAX_AllowableOutOfPhase = 1;

        private bool bCorrelationTooLow;
		private bool bSigmaTooWide;
        private bool bGoodPhaseShift;

        double mean;
        double sigma;
        int minimum;
        int maximum;
        private PbRspRfidBackDoorGetTagCharacterizationResults tagCharacterizeResponse;

        public double Sigma
        {
            get { return sigma; }
            private set { sigma = value; }
        }
        public double Mean
        {
            get { return mean; }
            private set { mean = value; }
        }
        public int Maximum
        {
            get { return maximum; }
            private set { maximum = value; }
        }

        public int Minimum
        {
            get { return minimum; }
            private set { minimum = value; }
        }
        public PbRspRfidBackDoorGetTagCharacterizationResults TagCharacterizeResponse
        {
            get { return tagCharacterizeResponse; }
            private set { tagCharacterizeResponse = value; }
        }

        private bool bInTest = false;
        private readonly EventWaitHandle _eventTestScan = new AutoResetEvent(false);
        private readonly EventWaitHandle _eventTestCorrelation = new AutoResetEvent(false);
        private readonly EventWaitHandle _eventTestCharacterize = new AutoResetEvent(false);

        public enum TestCode
        {
            Unknown = 0xff,
            Success = 0x00,
            TestFailed = 0x01,
            ErrorScan = 0x02,
            NoTagRead = 0x03,
            TooManyTags = 0x04,
            ErrorSampleCorrelation = 0x05,
            ErrorCharacterization = 0x06,
            TagReadOk = 0x07,
        }

        public TestCode TestTagAndLed(int timeoutLed, out string findTagId)
        {
            bInTest = true;
            findTagId = string.Empty;
            ResetReaderData();
            DeviceBoard.setBridgeState(false, 167,167);
            Thread.Sleep(50);
            DeviceBoard.clearKnownTagsBeforeTagScan();
            DeviceBoard.startTagScan(true, true, true, false);

            _eventTestScan.Reset();
            if (_eventTestScan.WaitOne(2000, false))
            {
                if (ReaderData.strListTag.Count == 1)
                {
                    findTagId = (string)ReaderData.strListTag[0];
                    LEdOnAll(1, timeoutLed, false);
                    StopField();
                    return TestCode.Success;
                }
                else if (ReaderData.strListTag.Count > 1)
                return TestCode.TooManyTags;
                else
                 return TestCode.NoTagRead;
            }
            return TestCode.ErrorScan;
        }
        public TestCode TestTagGetUid(out string findTagId)
        {

            bInTest = true;
            findTagId = string.Empty;
            ResetReaderData();
            DeviceBoard.setBridgeState(false, 167,167);
            Thread.Sleep(50);
            DeviceBoard.clearKnownTagsBeforeTagScan();
            DeviceBoard.startTagScan(true, true, true, false);

            _eventTestScan.Reset();
            if (_eventTestScan.WaitOne(2000, false))
            {
                if (ReaderData.strListTag.Count == 1)
                {
                    findTagId = (string)ReaderData.strListTag[0];
                    return TestCode.Success;
                }
                else if (ReaderData.strListTag.Count > 1)
                    return TestCode.TooManyTags;
                else
                    return TestCode.NoTagRead;
            }
            return TestCode.ErrorScan;
        }
       /* public int TestTag3Dcu(string tagid, uint normalDcu, uint minDcu, uint maxDcu)
        {
            string uidUnderTest;
            int retVal = 0;
            if (TestTagGetUid(out uidUnderTest) == TestCode.Success)
            {
                if (uidUnderTest == tagid)
                {
                 
                    bool bTestNormal = TestTagAtDcu(normalDcu);
                  
                    Thread.Sleep(200);
                    bool bTestMin = TestTagAtDcu(minDcu);
                    Thread.Sleep(200);

                    bool bTestMax = TestTagAtDcu(maxDcu);
                    Thread.Sleep(200);

                    if (bTestMin) retVal = retVal +  0x01;
                    if (bTestNormal) retVal = retVal +  0x02;
                    if (bTestMax) retVal = retVal +  0x04;

                    return retVal;
                }
                return retVal;
            }
            return retVal;
           
        }*/
        public TestCode TestTagAtDcu(uint Dcu , bool bTestCorr)
        {
            bInTest = true;
            ResetReaderData();
            DeviceBoard.setBridgeState(false, Dcu, Dcu);
            Thread.Sleep(50);
            DeviceBoard.clearKnownTagsBeforeTagScan();
            DeviceBoard.startTagScan(true, true, true, false);

            _eventTestScan.Reset();
            _eventTestCorrelation.Reset();
            if (_eventTestScan.WaitOne(2000, false))
            {
                if (ReaderData.strListTag.Count == 1)
                {
                    if (bTestCorr)
                    {
                        DeviceBoard.sampleCorrelationSeries(true, CorrelationSampleCount);
                        if (_eventTestCorrelation.WaitOne(5000, false))
                        {
                            if ((bCorrelationTooLow) || (bSigmaTooWide)) return TestCode.TestFailed;
                            return TestCode.Success;
                        }
                        return TestCode.ErrorSampleCorrelation;
                    }
                    else
                        return TestCode.TagReadOk;
                    
                }
                else if (ReaderData.strListTag.Count > 1)
                    return TestCode.TooManyTags;
                else
                    return TestCode.NoTagRead;
            }
            return TestCode.ErrorScan;
        }
        public TestCode TestCharacterizeTagAtDcu(uint Dcu)
        {
            bInTest = true;
            ResetReaderData();
            DeviceBoard.setBridgeState(false, Dcu, Dcu);
            Thread.Sleep(50);
            DeviceBoard.clearKnownTagsBeforeTagScan();
            DeviceBoard.startTagScan(true, true, true, false);

            _eventTestScan.Reset();
            _eventTestCharacterize.Reset();
            if (_eventTestScan.WaitOne(2000, false))
            {
                if (ReaderData.strListTag.Count == 1)
                {
                    DeviceBoard.characterizeTag(Dcu, PhaseShiftSampleCount, false);
                    if (_eventTestCharacterize.WaitOne(5000, false))
                    {
                        if (!bGoodPhaseShift) return TestCode.TestFailed;
                        return TestCode.Success;
                    }
                    return TestCode.ErrorCharacterization;
                }
                else if (ReaderData.strListTag.Count > 1)
                    return TestCode.TooManyTags;
                else
                    return TestCode.NoTagRead;
            }
            return TestCode.ErrorCharacterization;
        }
        private void CorrelationSampleSeriesReceived(uint nonEmptyGroups, uint groupsMissingFlags)
		{
			uint[] countsPresent = new uint[256];
			DeviceBoard.getCorrelationCounts(false, nonEmptyGroups, countsPresent);
			
			findMean(countsPresent, out mean, out sigma, out minimum, out maximum);
			
			bCorrelationTooLow = (mean < MIN_AllowableCorrelation);
			bSigmaTooWide = (sigma > MAX_AllowableSigma);
		
		}
        private void TagCharacterizationReceived()
        {
           tagCharacterizeResponse = DeviceBoard.getCharacterizeTagResults();
          
           bGoodPhaseShift = (tagCharacterizeResponse.outPhaseCount <= MAX_AllowableOutOfPhase);
                       
            
        }

        private void ResetReaderData()
        {
            PreviousReaderData = SerializeHelper.CloneObject(ReaderData);

            ReaderData = null;
            ReaderData = new ReaderData();
            ReaderData.RspScan = RespScan.RS_ReaderReady;
            ReaderData.DeviceID = SerialNumber;
        }

        #endregion


        /// <summary>
        /// Main Asynchronous receive function to handle event.
        /// </summary>
        /// <param name="asyncEventMessage">the asynchronous message involved</param>
        private void ReceiveAsynchronousEvent(AsyncEventMessage asyncEventMessage)
        {
            try
            {
                if (diagDoorLight != null)
                {
                    diagDoorLight.handleAsyncEvent( asyncEventMessage);
                    return;
                }

                if (diagTagSet != null)
                {
                    diagTagSet.handleAsyncEvent(asyncEventMessage);
                    return;
                }

                rfidReaderArgs notifyEvent;
                switch (asyncEventMessage.asyncEventType)
                {
                    case AsyncEventType.PBET_RfidScanStateChanged:
                        {
                            showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                            PBAE_RfidScanStateChanged scanStateChangedMessage = (PBAE_RfidScanStateChanged)Utilities.MarshalToStruct(
                            asyncEventMessage.serialMessage, typeof(PBAE_RfidScanStateChanged));

                            // Switch on the new scan state.
                            switch ((ScanStatusType)scanStateChangedMessage.scanStatus)
                            {
                                case ScanStatusType.SS_TagScanStarted: // Cabinet scan has begun.
                                    {
                                        _checkKZout = 1;
                                        _isInScan = true;
                                        _eventScanStart.Set();
                                        break;
                                    }
                                case ScanStatusType.SS_TagScanCompleted: // Cabinet scan has completed                            {
                                    {
                                      
                                        _lastScanSerial = new ArrayList(ReaderData.strListTag);

                                        if (bInTest)
                                        {
                                            _isInScan = false;
                                            _eventTestScan.Set();
                                            return;
                                        }
                                        _eventScanEnd.Set();
                                        
                                        if (NotifyEvent != null)
                                        {
                                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagScanCompleted, null);
                                            NotifyEvent(this, notifyEvent);
                                        }
                                        
                                        _isInScan = false;
                                        Thread log = new Thread(ZipAndBackupLog);
                                        log.IsBackground = true;
                                        log.Start();

                                        break;
                                    }
                                case ScanStatusType.SS_TagScanCanceledByHost:
                                    {
                                        ReaderData.RspScan = RespScan.RS_ReaderReady;
                                        _checkKZout = 255; // notify cancel by host
                                        _eventScanEnd.Set();
                                        _eventCancelByHost.Set();
                                        _isInScan = false;
                                        Thread log = new Thread(ZipAndBackupLog);
                                        log.IsBackground = true;
                                        log.Start();

                                        break;
                                    }
                                case ScanStatusType.SS_TagScanFailedByUnrecoverableError:
                                    {
                                        /*readerData.FailureReason = ((UnrecoverableErrorType)scanStateChangedMessage.info).ToString().Substring(3);

                                        string strError = null;
                                        for (uint failedTagIndex = 0; failedTagIndex < scanStateChangedMessage.tagFailureCount; failedTagIndex++)
                                        {
                                            UInt64 tagID;
                                            uint serialDigitsRead;
                                            string failureReason;
                                            if (!deviceBoard.getFailedTag(failedTagIndex, out tagID, out serialDigitsRead, out failureReason))
                                                break;
                                            string digitSequence = SerialRFID.SerialNumberAsString(tagID).Substring(0, (int)serialDigitsRead) + "...";
                                            strError += digitSequence + " : " + failureReason + "\r\n";
                                        }
                                        MessageBox.Show(strError);*/

                                        ReaderData.bFail = false;
                                        _isInScan = false;
                                        _eventScanEnd.Set();
                                        Thread log = new Thread(ZipAndBackupLog);
                                        log.IsBackground = true;
                                        log.Start();

                                        break;
                                    }
                                case ScanStatusType.SS_TagScanSendPourcent:
                                    {
                                        _scanPourcent++;
                                        int Pourcent = (int)(double)((100.0 / 74.0) * _scanPourcent);
                                        if (Pourcent >= 100) Pourcent = 100;
                                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Scan_Pourcent, Pourcent.ToString());
                                        if (NotifyEvent != null)
                                        {
                                            if (DeviceBoard != null)
                                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Scan_Pourcent.ToString() + ":" + Pourcent.ToString(), "NFY :", DateTime.Now);
                                            NotifyEvent(this, notifyEvent);
                                        }
                                        break;
                                    }
                                case ScanStatusType.SS_TagConfirmation:
                                    {

                                        _confirmationStatut = scanStateChangedMessage.info;
                                        if (DeviceBoard != null)
                                        AddMessage(DeviceBoard.deviceId, " Confirmation return : " + _confirmationStatut.ToString(), "NFY :", DateTime.Now);
                                       
                                        _eventConfirmation.Set();
                                        break;
                                    }
                            }
                            break;
                        }


                    case AsyncEventType.PBET_TagFullMem_R8:
                    case AsyncEventType.PBET_TagFullMem_RO:
                        {
                            showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                            PBAE_RfidTagFullMem tagFullMessage = (PBAE_RfidTagFullMem)Utilities.MarshalToStruct(
                                        asyncEventMessage.serialMessage, typeof(PBAE_RfidTagFullMem));

                            tagFullMessage.TagId[5] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 0);
                            tagFullMessage.TagId[11] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 6);
                            tagFullMessage.TagId[17] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 12);
                            tagFullMessage.TagId[23] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 18);
                            tagFullMessage.TagId[29] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 24);
                            tagFullMessage.TagId[35] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 30);
                            tagFullMessage.TagId[41] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 36);


                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < 42; i++)
                                sb.AppendFormat("{0}", tagFullMessage.TagId[i]);

                            string codeToAddOct = sb.ToString();

                            //code to add  10 digits with family without CRC
                            string codeToAdd = sb.ToString(0, 11).Remove(5,1);
                            

                            if (!ReaderData.strListTag.Contains(codeToAdd))
                            {
                                ReaderData.strListTag.Add(codeToAdd);
                                ReaderData.nbTagScan = ReaderData.strListTag.Count;

                                TagInfo ti = new TagInfo();
                                if (asyncEventMessage.asyncEventType == AsyncEventType.PBET_TagFullMem_R8)
                                    ti.TagType = TagType.TT_R8;
                                else
                                    ti.TagType = TagType.TT_SPCE2_RO;
                                ti.TagIdOctal = codeToAddOct;
                                ti.TagId_R8_RO = codeToAdd;
                                ti.TagId_RW = null;
                                ti.TagId_DEC = null;
                                ReaderData.ListTagInfo.Add(ti);

                            } // Change for ST - Warning DT

                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, codeToAdd);
                            if (NotifyEvent != null)
                            {
                                if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + codeToAdd, "NFY :", DateTime.Now);
                                NotifyEvent(this, notifyEvent);
                            }
                            // for DT put } here

                            _checkKZout = 0; // raz cpt  test sortie

                            break;
                        }
                    case AsyncEventType.PBET_TagFullMem_RW:
                    {
                        showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                        PBAE_RfidTagFullMem tagFullMessage = (PBAE_RfidTagFullMem)Utilities.MarshalToStruct(
                                    asyncEventMessage.serialMessage, typeof(PBAE_RfidTagFullMem));

                        tagFullMessage.TagId[5] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 0);
                        tagFullMessage.TagId[11] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 6);
                        tagFullMessage.TagId[17] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 12);
                        tagFullMessage.TagId[23] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 18);
                        tagFullMessage.TagId[29] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 24);
                        tagFullMessage.TagId[35] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 30);
                        tagFullMessage.TagId[41] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 36);


                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < 42; i++)
                            sb.AppendFormat("{0}", tagFullMessage.TagId[i]);

                        string codeToAddOct = sb.ToString();
                        string codeToAddAlpha = SerialRFID.CodeMemFullToString(codeToAddOct);

                        string codeToAdd = codeToAddAlpha;
                        if (codeToAddAlpha.Contains("§"))
                            codeToAdd = codeToAddAlpha.Substring(0, codeToAddAlpha.IndexOf('§'));

                        if (!ReaderData.strListTag.Contains(codeToAdd))
                        {
                            ReaderData.strListTag.Add(codeToAdd);
                            ReaderData.nbTagScan = ReaderData.strListTag.Count;
                            TagInfo ti = new TagInfo();
                            ti.TagType = TagType.TT_SPCE2_RW;
                            ti.TagIdSerial = 0;
                            ti.TagIdOctal = codeToAddOct;
                            ti.TagId_R8_RO = null;
                            ti.TagId_RW = codeToAdd;
                            ti.TagId_DEC = null;
                            ReaderData.ListTagInfo.Add(ti);


                        } // Change for ST - Warning DT

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, codeToAdd);
                        if (NotifyEvent != null)
                        {
                            if (DeviceBoard != null)
                                AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + codeToAdd, "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }
                        // for DT put } here

                        _checkKZout = 0; // raz cpt  test sortie

                        break;
                    }
                    case AsyncEventType.PBET_TagFullMem_DEC:
                    {
                        showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                        PBAE_RfidTagFullMem tagFullMessage = (PBAE_RfidTagFullMem)Utilities.MarshalToStruct(
                                    asyncEventMessage.serialMessage, typeof(PBAE_RfidTagFullMem));

                        tagFullMessage.TagId[5] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 0);
                        tagFullMessage.TagId[11] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 6);
                        tagFullMessage.TagId[17] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 12);
                        tagFullMessage.TagId[23] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 18);
                        tagFullMessage.TagId[29] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 24);
                        tagFullMessage.TagId[35] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 30);
                        tagFullMessage.TagId[41] = SerialRFID.ComputeCrc(tagFullMessage.TagId, 36);


                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < 42; i++)
                            sb.AppendFormat("{0}", tagFullMessage.TagId[i]);

                        string codeToAddOct = sb.ToString();
                        string codeToAdd = SerialRFID.CodeMemFullToDec(codeToAddOct);

                        if (!ReaderData.strListTag.Contains(codeToAdd))
                        {
                            ReaderData.strListTag.Add(codeToAdd);
                            ReaderData.nbTagScan = ReaderData.strListTag.Count;
                            TagInfo ti = new TagInfo();
                            ti.TagType = TagType.TT_SPCE2_DEC;
                            ti.TagIdSerial = 0;
                            ti.TagIdOctal = codeToAddOct;
                            ti.TagId_R8_RO = null;
                            ti.TagId_RW = null;
                            ti.TagId_DEC = codeToAdd;
                            ReaderData.ListTagInfo.Add(ti);


                        } // Change for ST - Warning DT

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, codeToAdd);
                        if (NotifyEvent != null)
                        {
                            if (DeviceBoard != null)
                                AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + codeToAdd, "NFY :", DateTime.Now);
                            NotifyEvent(this, notifyEvent);
                        }
                        // for DT put } here

                        _checkKZout = 0; // raz cpt  test sortie

                        break;
                    }
                    //case AsyncEventType.PBET_TagAdded:
                    case AsyncEventType.PBET_TagAddedR8:
                        {
                            showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                            PBAE_RfidTagAdded tagAddedMessage = (PBAE_RfidTagAdded)Utilities.MarshalToStruct(asyncEventMessage.serialMessage, typeof(PBAE_RfidTagAdded));
                            string codeToAddOct = SerialRFID.SerialNumberAsString(SerialRFID.SerialNumber(tagAddedMessage.serialNumber));
                            string codeToAdd =  codeToAddOct.Substring(0, MaxDigit);
                           
                            // KB : Error scan 03 12 2012
                            if (!ReaderData.strListTag.Contains(codeToAdd))
                            {
                                ReaderData.strListTag.Add(codeToAdd);
                                ReaderData.nbTagScan = ReaderData.strListTag.Count;

                                TagInfo ti = new TagInfo();
                                ti.TagType = TagType.TT_R8;
                                ti.TagIdSerial = SerialRFID.SerialNumber(tagAddedMessage.serialNumber);
                                ti.TagIdOctal = codeToAddOct;
                                ti.TagId_R8_RO = null;
                                ti.TagId_RW = null;
                                ti.TagId_DEC = null;
                                ReaderData.ListTagInfo.Add(ti);

                            } // Change for ST - Warning DT

                                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, codeToAdd);
                                if (NotifyEvent != null)
                                {
                                    if (DeviceBoard != null)
                                        AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + codeToAdd, "NFY :", DateTime.Now);
                                    NotifyEvent(this, notifyEvent);
                                }                               
                            // for DT put } here
                           
                            _checkKZout = 0; // raz cpt  test sortie
                            break;
                        }
                   // case AsyncEventType.PBET_TagRemoved:
                    case AsyncEventType.PBET_TagAddedSPCE2_RO:
                        {
                            showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                            PBAE_RfidTagAdded tagAddedMessage = (PBAE_RfidTagAdded)Utilities.MarshalToStruct(asyncEventMessage.serialMessage, typeof(PBAE_RfidTagAdded));
                            // CR
                            //string codeToAddAlpha = SerialRFID.SerialNumberAsAlphaString(SerialRFID.SerialNumber(tagAddedMessage.serialNumber),TagType.TT_SPCE2_RO);
                            string codeToAddOct = SerialRFID.SerialNumberAsString(SerialRFID.SerialNumber(tagAddedMessage.serialNumber));

                            string codeToAdd = codeToAddOct.Substring(0, MaxDigit);

                            // KB : Error scan 03 12 2012
                            if (!ReaderData.strListTag.Contains(codeToAdd))
                                {
                                    ReaderData.strListTag.Add(codeToAdd);
                                    ReaderData.nbTagScan = ReaderData.strListTag.Count;

                                    TagInfo ti = new TagInfo();
                                    ti.TagType = TagType.TT_SPCE2_RO;
                                    ti.TagIdSerial = SerialRFID.SerialNumber(tagAddedMessage.serialNumber);
                                    ti.TagIdOctal = codeToAddOct;
                                    ti.TagId_R8_RO = codeToAddOct;
                                    ti.TagId_RW = null;
                                    ti.TagId_DEC = null;
                                    ReaderData.ListTagInfo.Add(ti);


                                } // Change for ST - Warning DT

                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, codeToAdd);
                                    if (NotifyEvent != null)
                                    {
                                        if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + codeToAdd, "NFY :", DateTime.Now);
                                        NotifyEvent(this, notifyEvent);
                                    }
                            // for DT put } here

                            _checkKZout = 0; // raz cpt  test sortie
                            break;
                        }
                    case AsyncEventType.PBET_TagAddedSPCE2_RW:
                        {
                            showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                            PBAE_RfidTagAdded tagAddedMessage = (PBAE_RfidTagAdded)Utilities.MarshalToStruct(asyncEventMessage.serialMessage, typeof(PBAE_RfidTagAdded));
                            // CR
                            string codeToAddAlpha = SerialRFID.SerialNumberAsAlphaString(SerialRFID.SerialNumber(tagAddedMessage.serialNumber), TagType.TT_SPCE2_RW);
                            string codeToAddOct = SerialRFID.SerialNumberAsString(SerialRFID.SerialNumber(tagAddedMessage.serialNumber));
                            
                            string codeToAdd = string.Empty;
                         
                            if (codeToAddAlpha.Contains("§"))
                                codeToAdd = codeToAddAlpha.Substring(0, codeToAddAlpha.IndexOf('§'));
                            else
                                codeToAdd = codeToAddAlpha.Substring(0, 10);
                           
                            
                            
                            // KB : Error scan 03 12 2012
                            if (!ReaderData.strListTag.Contains(codeToAdd))
                            {
                                ReaderData.strListTag.Add(codeToAdd);
                                ReaderData.nbTagScan = ReaderData.strListTag.Count;

                                TagInfo ti = new TagInfo();
                                ti.TagType = TagType.TT_SPCE2_RW;
                                ti.TagIdSerial = SerialRFID.SerialNumber(tagAddedMessage.serialNumber);
                                ti.TagIdOctal = codeToAddOct;
                                ti.TagId_R8_RO = null;
                                ti.TagId_RW = codeToAdd;
                                ti.TagId_DEC = null;
                                ReaderData.ListTagInfo.Add(ti);


                            } // Change for ST - Warning DT

                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, codeToAdd);
                            if (NotifyEvent != null)
                            {
                                if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagAdded.ToString() + ":" + codeToAdd, "NFY :", DateTime.Now);
                                NotifyEvent(this, notifyEvent);
                            }
                            // for DT put } here

                            _checkKZout = 0; // raz cpt  test sortie
                            break;
                        }
                    case AsyncEventType.PBET_BackDoorInfo:
                        {
                            showOutbound(DeviceBoard, asyncEventMessage.ToString(), "*");
                            PBAE_BackDoorInfo backDoorPacket = (PBAE_BackDoorInfo)Utilities.MarshalToStruct(
                            asyncEventMessage.serialMessage, typeof(PBAE_BackDoorInfo));

                            if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_LowLevelOrder)
                            {
                                _status = backDoorPacket.value1;
                                _eventConfirmation.Set();
                            }
                            if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_WriteBlock)
                            {
                                statusWrite = backDoorPacket.value1; ;
                                _eventWrite.Set();
                            }


                            if (backDoorPacket.backDoorEventType == (byte) BackDoorEventType.BDET_CorrelationSample)
                            {
                                    if (NotifyEvent != null)
                                    {
                                        int phaseShift = backDoorPacket.value2;
                                        if (phaseShift > 180)
                                            phaseShift -= 360;
                                        notifyEvent = new rfidReaderArgs(SerialNumber,
                                            rfidReaderArgs.ReaderNotify.RN_CorrelationSample,
                                            backDoorPacket.value1.ToString() + ";" + phaseShift.ToString());
                                        NotifyEvent(this, notifyEvent);
                                    }
                                
                            }

                            if (backDoorPacket.backDoorEventType == (byte) BackDoorEventType.BDET_CorrelationSamplesComplete)   
                              {

                                  if (bInTest)
                                  {
                                      CorrelationSampleSeriesReceived(backDoorPacket.value1, backDoorPacket.value2);
                                      _eventTestCorrelation.Set();
                                  }
                                  else
                                  {
                                      if (NotifyEvent != null)
                                      {
                                          notifyEvent = new rfidReaderArgs(SerialNumber,
                                              rfidReaderArgs.ReaderNotify.RN_CorrelationSamplesComplete,
                                              backDoorPacket.value1.ToString() + ";" + backDoorPacket.value2);
                                          NotifyEvent(this, notifyEvent);
                                      }
                                  }
                              }
                              if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_TagCharacterizationComplete)   
                              {
                                  if (bInTest)
                                  {
                                      TagCharacterizationReceived();
                                      _eventTestCharacterize.Set();
                                  }
                                  else
                                  {
                                      if (NotifyEvent != null)
                                      {
                                          notifyEvent = new rfidReaderArgs(SerialNumber,
                                              rfidReaderArgs.ReaderNotify.RN_TagCharacterizationComplete, null);
                                          NotifyEvent(this, notifyEvent);
                                      }
                                  }
                              }  
                        

                            if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_CorrelationSamplesComplete)
                            {
                                if (diagThreshold != null)
                                {
                                    diagThreshold.ProcessData(backDoorPacket);
                                }
                                else
                                {
                                    ClearCounts();
                                    DeviceBoard.getCorrelationCounts(true, backDoorPacket.value2, _countsMissing);
                                    DeviceBoard.getCorrelationCounts(false, backDoorPacket.value1, _countsPresent);
                                    double mean, sigma;
                                    int minimum, maximum, tmp;
                                    findMean(_countsMissing, out mean, out sigma, out tmp, out maximum);
                                    findMean(_countsPresent, out mean, out sigma, out minimum, out tmp);
                                    ReaderData.MaximumCorrelationWhithoutResponse = (byte)maximum;
                                    ReaderData.MinimumCorrelationWithResponse = (byte)minimum;
                                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ThresholdMaxNoise, maximum.ToString() + "/" + minimum.ToString());
                                    if (NotifyEvent != null)
                                    {
                                        if (DeviceBoard != null)
                                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_ThresholdMaxNoise.ToString() + ":" + maximum.ToString() + "/" + minimum.ToString(), "NFY :", DateTime.Now);
                                        NotifyEvent(this, notifyEvent);
                                    }
                                }
                            }
                            if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_AxisChange)
                            {
                                if ((backDoorPacket.value1 == 10) && (backDoorPacket.value2 == 1))
                                    _checkKZout++;

                                if (backDoorPacket.value1 < 10) //notification axis
                                {
                                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ActiveChannnelChange, backDoorPacket.value1.ToString());
                                    if (NotifyEvent != null)
                                        NotifyEvent(this, notifyEvent);
                                }
                            }
                            if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_CorrelationSample)
                            {                               
                                if (backDoorPacket.value1 > ThresholdBackup)
                                {                                
                                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_TagPresenceDetected, backDoorPacket.value1.ToString());
                                    if (NotifyEvent != null)
                                    {
                                        if (DeviceBoard != null)
                                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_TagPresenceDetected.ToString() + ":" + backDoorPacket.value1.ToString(), "NFY :", DateTime.Now);
                                        NotifyEvent(this, notifyEvent);
                                    }
                                }
                            }



                            break;
                        }
                    case AsyncEventType.PBET_PowerOff:
                        {
                            // MessageBox.Show("Power cut detect, Kill reception thread and object", Properties.ResStrings.strInfo);
                            ReaderData.RspScan = RespScan.RS_ReaderNotReady;
                            _bPowerOn = false;
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Power_OFF, "Power Off");
                            if (NotifyEvent != null)
                            {
                                if (DeviceBoard != null)
                                AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Power_OFF.ToString(), "NFY :", DateTime.Now);
                                NotifyEvent(this, notifyEvent);
                            }

                            Thread.Sleep(2000); // wait for usb event, time > to watcher interval
                            if (!_eventUsbArrived) //just a Power off
                            {
                                IsConnected = false;
                                if (_isInScan) _eventScanEnd.Set();
                                _isInScan = false;
                                ReaderData.DeviceID = null;
                                _driveLight = false;
                                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Disconnected, "Reader Disconnected to " + StrCom);
                                if (NotifyEvent != null)
                                {
                                    if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Disconnected.ToString(), "NFY :", DateTime.Now);
                                    NotifyEvent(this, notifyEvent);
                                }
                            }

                            break;
                        }
                   
                    case AsyncEventType.PBET_PowerOn:
                        {
                            Thread.Sleep(1000);
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Power_ON, "Power ON");
                            if (NotifyEvent != null)
                            {
                                NotifyEvent(this, notifyEvent);
   
                                if (DeviceBoard != null)
                                AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Power_ON.ToString(), "NFY :", DateTime.Now);
                            }
                            _bPowerOn = true;
                            IsConnected = true;
                            ReaderData.RspScan = RespScan.RS_ReaderReady;
                            SetLightPower(300);
                            Thread.Sleep(1000);
                            SetLightPower(0);
                            Thread.Sleep(1000);
                            _lastConnectionDate = DateTime.Now;
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Connected, "Reader Connected to " + StrCom);
                            if (NotifyEvent != null)
                            {
                                if (DeviceBoard != null)
                                AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Connected.ToString(), "NFY :", DateTime.Now);
                                NotifyEvent(this, notifyEvent);
                            }

                            break;
                        }
                    case AsyncEventType.PBET_AlarmInfra:

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_IntrusionDetected, "Infra red Intrusion detected");
                        if (NotifyEvent != null)
                        {
                            NotifyEvent(this, notifyEvent);
                            if (DeviceBoard != null)
                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_IntrusionDetected.ToString(), "NFY :", DateTime.Now);
                        }
                        break;
                    case AsyncEventType.PBET_AlarmMove:

                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_MovementDetected, "Movement sensor detected");
                        if (NotifyEvent != null)
                        {
                            NotifyEvent(this, notifyEvent);
                            if (DeviceBoard != null)
                            AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_MovementDetected.ToString(), "NFY :", DateTime.Now);
                        }
                        break;

                    case AsyncEventType.PBET_DoorOpened:
                        {
                           /* if (IsInScan)
                            {
                                AddMessage(DeviceBoard.deviceId, "IsInScan a True - Event Open not pass", "NFY :", DateTime.Now);
                                break;
                            }*/
                            Door_Status = SDK_SC_RfidReader.Door_Status.Door_Open;
                            _currentEventDoorDate = DateTime.Now;
                            if (DeviceBoard != null)
                            AddMessage(DeviceBoard.deviceId, " Door Status: " + Door_Status.ToString(), "NFY :", DateTime.Now);

                            TimeSpan ts = _currentEventDoorDate - _lastEventDoorDate;
                            double TimeInSecond = ts.TotalSeconds;
                            _lastEventDoorDate = _currentEventDoorDate;

                            //reset for autoscan 
                            _lastScanSerial.Clear();

                            if ((TimeInSecond > 1.0) && (IsConnected) && (_bPowerOn))
                            {
                               // if (_isInScan)
                                //    RequestEndScan();
                                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Door_Opened, "Door Opened!");
                                if (NotifyEvent != null)
                                {
                                    if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Door_Opened.ToString(), "NFY :", DateTime.Now);
                                    NotifyEvent(this, notifyEvent);
                                }
                            }
                            else
                            {
                                Thread.Sleep(3000);
                                if ((IsConnected) && (_bPowerOn))
                                {
                                   // if (_isInScan)
                                   //     RequestEndScan();
                                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Door_Opened, "Door Opened!");
                                    if (NotifyEvent != null)
                                    {
                                        if (DeviceBoard != null)
                                        AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Door_Opened.ToString(), "NFY :", DateTime.Now);
                                        NotifyEvent(this, notifyEvent);
                                    }
                                }
                            }
                        }
                        break;

                    case AsyncEventType.PBET_DoorClosed:
                        {
                            if (IsInScan)
                            {
                                AddMessage(DeviceBoard.deviceId, "IsInScan a True - Event Close not pass", "NFY :", DateTime.Now);
                                break;
                            }

                            Door_Status = SDK_SC_RfidReader.Door_Status.Door_Close;
                            _currentEventDoorDate = DateTime.Now;
                            if (DeviceBoard != null)
                            AddMessage(DeviceBoard.deviceId, " Door Status: " + Door_Status.ToString(), "NFY :", DateTime.Now);
                            TimeSpan ts = _currentEventDoorDate - _lastEventDoorDate;
                            double TimeInSecond = ts.TotalSeconds;
                            _lastEventDoorDate = _currentEventDoorDate;

                            //check last connection to prevent send first false open when power come back
                            TimeSpan ts2 = _currentEventDoorDate - _lastConnectionDate;
                            double TimeInSecondLastConnection = ts2.TotalSeconds;
                            if ((TimeInSecond > 1.0) && (IsConnected) && (_bPowerOn) && (TimeInSecondLastConnection > 3.0))
                            {
                                if (_isInScan)
                                    RequestEndScan();
                                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Door_Closed, "Door Closed!");
                                if (NotifyEvent != null)
                                {
                                    if (DeviceBoard != null)
                                    AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Door_Closed.ToString(), "NFY :", DateTime.Now);
                                    NotifyEvent(this, notifyEvent);
                                }
                            }
                            else
                            {
                                Thread.Sleep(3000);
                                if ((IsConnected) && (_bPowerOn))
                                {
                                    if (_isInScan)
                                        RequestEndScan();
                                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Door_Closed, "Door Closed!");
                                    if (NotifyEvent != null)
                                    {
                                        if (DeviceBoard != null)
                                        AddMessage(DeviceBoard.deviceId, rfidReaderArgs.ReaderNotify.RN_Door_Closed.ToString(), "NFY :", DateTime.Now);
                                        NotifyEvent(this, notifyEvent);
                                    }
                                }
                            }

                        }
                        break;
                   
                }
            }
            catch (Exception exp)
            {
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, exp.Source + " : " + exp.TargetSite + " : " + exp.Message, "EXP :", DateTime.Now);
                //throw new Exception();
            }
        }
        /// <summary>
        /// Delegate function to display comunication message if textbox declared
        /// </summary>
        /// <param name="device">device to display</param>
        /// <param name="message">message to dispaly</param>
        /// <param name="modifier">modifier to parse message type  </param>
        private void showOutbound(IDeviceRfidBoard device, string message, string modifier)
        {

            AddMessage(device.DeviceId, message, modifier, DateTime.Now);
        }
        /// <summary>
        /// Method to add message to logs
        /// </summary>
        /// <param name="serialNumber">device serial number</param>
        /// <param name="message">message to display</param>
        /// <param name="direction">modifier to parse message</param>
        /// <param name="dt"> Date de demande de log</param>
        private void AddMessage(uint serialNumber, string message, string direction, DateTime dt)
        {
            string logmes = string.Format(" [{0:X8}] {1} {2} \r\n", serialNumber, direction, message);


            if (_enabledLog)
            {
                lock (_logLock)
                {

                    DateTime TCurrent = dt;
                    DateTime TRef = new DateTime(TCurrent.Year,
                                                    TCurrent.Month,
                                                    TCurrent.Day,
                                                    12, 0, 0);

                    int res = DateTime.Compare(TCurrent, TRef);
                    if (res < 0)
                        _logPath = @"c:\temp\RfidTracking\log\ReaderLog" + string.Format("[{0:X8}]_{1}_AM.txt", serialNumber, DateTime.Now.Date.ToString("dd_MM_yyyy"));

                    else
                        _logPath = @"c:\temp\RfidTracking\log\ReaderLog" + string.Format("[{0:X8}]_{1}_PM.txt", serialNumber, DateTime.Now.Date.ToString("dd_MM_yyyy"));
                    WriteToLog(_logPath, logmes, dt);

                }
            }

        }
        private void ZipAndBackupLog()
        {
            lock (_logLock)
            {
                try
                {
                    string[] fichiers = Directory.GetFiles(@"c:\temp\RfidTracking\log\", "*.txt", SearchOption.AllDirectories);
                   // DateTime dt = DateTime.Now.AddHours(-12.0);
                    DateTime dt = DateTime.Now;
                    dt = dt - new TimeSpan(12, 0, 0);

                    for (int i = 0; i < fichiers.Length; i++)
                    {
                        FileInfo fic = new FileInfo(fichiers[i]);
                        if ((fic.CreationTime.CompareTo(dt) < 0) && (fic.Name.Contains(SerialNumber))) //zip fichier > 12h et avec serial
                        {
                            string pathZip = fic.FullName + ".gz";
                            FileZip oZip = new FileZip(fic.FullName, pathZip, Action.Zip);
                            File.Delete(fic.FullName);

                        }
                    }
                    if (_sendLogMail)
                        SendMailLog();
                    CleanLog();

                }

                catch (Exception exp)
                {
                    //MessageBox.Show("Error ZIP : " + exp.Message);
                    if (DeviceBoard.deviceId != 0x0000)
                        AddMessage(DeviceBoard.deviceId, "Error ZIP : " + exp.Message, "ERR :", DateTime.Now);
                    //throw new Exception();
                }

            }
        }
        private void CleanLog()
        {

            string[] fichiers = Directory.GetFiles(@"c:\temp\RfidTracking\log\", "*.*", SearchOption.AllDirectories);

            //DateTime dt = DateTime.Now.AddDays(-7.0);
            DateTime dt = DateTime.Now;
            dt = dt - new TimeSpan(7, 0, 0, 0);

            for (int i = 0; i < fichiers.Length; i++)
            {
                FileInfo fic = new FileInfo(fichiers[i]);

                if (fic.CreationTime.CompareTo(dt) < 0)
                    File.Delete(fichiers[i]);
            }

        }
        private void SendMailLog()
        {

            if (SMTPActive == 0) return;
            if (string.IsNullOrEmpty(SenderAdress)) return;
            if (string.IsNullOrEmpty(LoginName)) return;
            if (string.IsNullOrEmpty(Password)) return;
            if (string.IsNullOrEmpty(SMTPServer)) return;
            if (SMTPPort == 0) return;
            try
            {
                bool mailcreated = false;
                //create the mail message
                MailMessage mail = null;
                string mailbody = null;

                ArrayList zipFileAttach = new ArrayList();

                string[] fichiers = Directory.GetFiles(@"c:\temp\RfidTracking\log\", "*.gz", SearchOption.AllDirectories);
                for (int i = 0; i < fichiers.Length; i++)
                {
                    FileInfo fic = new FileInfo(fichiers[i]);
                    if (fic.Name.Contains(SerialNumber))
                    {
                        if (mailcreated)
                        {
                            mailbody += "Log file : " + fic.Name + "\r\n\r\n<br />";
                            zipFileAttach.Add(fic.FullName);
                        }
                        else
                        {
                            mailcreated = true;
                            mail = new MailMessage();
                            //set the addresses
                            mail.From = new MailAddress(SenderAdress);


                            mail.To.Add("christophe.raoult@spacecode-rfid.com");
                            mail.To.Add("eric.gout@spacecode-rfid.com");

                            mail.Subject = "Fichier Log Databox : : [" + SerialNumber + "]";
                            mailbody = string.Empty;
                            mailbody += "Log file : " + fic.Name + "\r\n\r\n<br />";
                            zipFileAttach.Add(fic.FullName);
                        }
                    }
                }


                if (mailcreated)
                {

                    //first we create the Plain Text part
                    AlternateView plainView = AlternateView.CreateAlternateViewFromString(mailbody, null, "text/plain");

                    //then we create the Html part
                    //to embed images, we need to use the prefix 'cid' in the img src value
                    //the cid value will map to the Content-Id of a Linked resource.
                    //thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'
                    AlternateView htmlView;

                    htmlView = AlternateView.CreateAlternateViewFromString(mailbody + "<br />", null, "text/html");

                    //add the views
                    mail.AlternateViews.Add(plainView);
                    mail.AlternateViews.Add(htmlView);


                    //Attach file
                    Attachment[] myAttachment = new Attachment[zipFileAttach.Count];
                    int indexFile = 0;
                    foreach (string fi in zipFileAttach)
                    {
                        myAttachment[indexFile] = new Attachment(fi);
                        mail.Attachments.Add(myAttachment[indexFile++]);
                    }


                    //send the message
                    SmtpClient SmtpServer = new SmtpClient(SMTPServer);
                    SmtpServer.Port = SMTPPort;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(LoginName, Password);
                    SmtpServer.Send(mail);

                    foreach (Attachment att in myAttachment)
                        att.Dispose();
                    mail.Dispose();

                    foreach (string fi in zipFileAttach)
                    {
                        if (File.Exists(fi + ".bck"))
                            File.Delete(fi + ".bck");

                        if (File.Exists(fi))
                            File.Move(fi, fi + ".bck");
                    }

                }
            }
            catch (Exception exp)
            {
                // MessageBox.Show("Error send Log Mail : " + exp.Message);
                if (DeviceBoard.deviceId != 0x00000)
                    AddMessage(DeviceBoard.deviceId, " Error send Log Mail : " + exp.Message, "ERR :" + exp.InnerException, DateTime.Now);
                //throw new Exception();
            }
        }
        private int setTimeoutVsHardware(string HardwareVersion)
        {
            int timeout = 600000; 
            try
            {
                string[] tmpHw = HardwareVersion.Split('.');

                switch (tmpHw[0])
                {
                    case "7": timeout = 300000; break;
                    case "2": timeout = 900000; break;
                    default: timeout = 180000; break;                       
                }
            }
            catch
            {
                //throw new Exception();
            }
            return timeout;
        }
       

        /// <summary>
        /// Test presence  column in DB
        /// </summary>
        /// <param name="dataReader"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static bool IsColumnExists(SqlDataReader dataReader, string columnName)
        {
            bool retVal = false;
            try
            {
                dataReader.GetSchemaTable().DefaultView.RowFilter = string.Format("ColumnName= '{0}'", columnName);
                if (dataReader.GetSchemaTable().DefaultView.Count > 0)
                {
                    retVal = true;
                }
            }
            catch
            {  //throw;}
                return retVal;
            }
            return retVal;
        }

        private static string Chr(int p_intByte)
        {

            byte[] bytBuffer = BitConverter.GetBytes(p_intByte);

            return Encoding.Unicode.GetString(bytBuffer);

        }

        private void WriteToLog(string path, string message, DateTime dt)
        {
            try
            {
                StreamWriter log;
                if (!File.Exists(path))
                {
                    FileInfo fi = new FileInfo(path);
                    string DirectoryPath = fi.DirectoryName;
                    if (!Directory.Exists(DirectoryPath))
                        Directory.CreateDirectory(DirectoryPath);
                    log = new StreamWriter(path);
                }
                else
                {
                    log = File.AppendText(path);
                }
                // Write to the file: 

                int currentMS = dt.Millisecond;
                if (currentMS == _previousMs)
                {
                    currentMS++;
                    if (currentMS > 999) currentMS = 0;
                }
                _previousMs = currentMS;

                log.Write(dt.ToString("s") + ":" + currentMS.ToString("000") + message);
                // Close the stream:
                log.Close();
            }
            catch (UnauthorizedAccessException exp)
            {
                ErrBoard.dt = DateTime.Now;
                ErrBoard.message = "Erreur  Log in reader: " + exp.Message;
            }
            catch (ArgumentException exp)
            {
                ErrBoard.dt = DateTime.Now;
                ErrBoard.message = "Erreur  Log in reader: " + exp.Message;
            }
            catch(Exception exp)
            {
                ErrBoard.dt = DateTime.Now;
                ErrBoard.message = "Erreur  Log in reader: " + exp.Message;
                //throw new Exception();
            }

        }

        /// <summary>
        /// Method to request a correlation sampling process to measure noise level.
        /// This Method generate a notification ReaderNotify.RN_ThresholdMaxNoise when scan completed.
        /// The result are store in datareader class.
        /// </summary>
        /// <returns>true is order successively send.</returns>
        public bool FindThreshold()
        {
            if (DeviceBoard != null)
            AddMessage(DeviceBoard.deviceId, "FindThreshold()", "CMD :", DateTime.Now);
            if ((IsConnected) && (_bPowerOn))
            {
                DeviceBoard.setBridgeState(false, (uint)167, (uint)167); //Field to max
                return (DeviceBoard.sampleCorrelationSeries(true, 128));
            }
            return false;
        }
        private void ClearCounts()
        {
            for (uint i = 0; i < _countsPresent.Length; i++)
                _countsPresent[i] = 0;
            for (uint i = 0; i < _countsMissing.Length; i++)
                _countsMissing[i] = 0;
        }

        private void findMean(uint[] samples, out double mean, out double sigma, out int minimum, out int maximum)
        {
            double sum = 0.0;
            double sumSquare = 0.0;
            uint sampleCount = 0;
            minimum = 255;
            maximum = 0;

            for (int sample = 0; sample < 256; sample++)
            {
                if (samples[sample] > 0)
                {
                    if (sample < minimum)
                        minimum = sample;
                    if (sample > maximum)
                        maximum = sample;

                    sampleCount += samples[sample];
                    sum += samples[sample] * sample;
                    sumSquare += samples[sample] * sample * sample;
                }
            }

            mean = sum / sampleCount;
            double nMeanSquare = sampleCount * (mean * mean);
            sigma = Math.Sqrt((sumSquare - nMeanSquare) / sampleCount);
        }
        /// <summary>
        /// Funtion to unlock the latch mechanism
        /// </summary>
        public void OpenDoor()
        {

            if ((!IsConnected) && (StrCom != null))
                ConnectReader(StrCom);


            if ((IsConnected) && (_bPowerOn))
            {
                if (DeviceBoard != null)
                {
                    AddMessage(DeviceBoard.deviceId, "OpenDoor()", "CMD :", DateTime.Now);
                    AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
                }
                if (DeviceBoard.LockDoor(false))
                    Lock_Status = SDK_SC_RfidReader.Lock_Status.Lock_Open;
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
            }
            else
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "OpenDoor() failed", "ERR :", DateTime.Now);
        }
        /// <summary>
        /// Method to open Master Door in SAS
        /// </summary>
        public void OpenDoorMaster()
        {

            if ((!IsConnected) && (StrCom != null))
                ConnectReader(StrCom);


            if ((IsConnected) && (_bPowerOn))
            {
                if (DeviceBoard != null)
                {
                    AddMessage(DeviceBoard.deviceId, "OpenDoor() MASTER", "CMD :", DateTime.Now);
                    AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
                }
                if (DeviceBoard.LockDoor(DoorValue.DV_Master, false))
                    Lock_Status = SDK_SC_RfidReader.Lock_Status.Lock_Open;
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
            }
            else
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "OpenDoor() failed", "ERR :", DateTime.Now);
        }

        /// <summary>
        /// Methode to open slave door in SAS
        /// </summary>
        public void OpenDoorSlave()
        {

            if ((!IsConnected) && (StrCom != null))
                ConnectReader(StrCom);


            if ((IsConnected) && (_bPowerOn))
            {
                if (DeviceBoard != null)
                {
                    AddMessage(DeviceBoard.deviceId, "OpenDoor() Slave", "CMD :", DateTime.Now);
                    AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
                }
                if (DeviceBoard.LockDoor(DoorValue.DV_Slave, false))
                    Lock_Status = SDK_SC_RfidReader.Lock_Status.Lock_Open;
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
            }
            else
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "OpenDoor() failed", "ERR :", DateTime.Now);
        }


        /// <summary>
        /// Function to lock the latch mechanism
        /// </summary>
        public void CloseDoor()
        {

            if ((IsConnected) && (_bPowerOn))
            {
                if (DeviceBoard != null)
                {
                    AddMessage(DeviceBoard.deviceId, "CloseDoor()", "CMD :", DateTime.Now);
                    AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
                }
                if (DeviceBoard.LockDoor(true))
                    Lock_Status = SDK_SC_RfidReader.Lock_Status.Lock_Close;
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
            }
            else
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "CloseDoor() failed", "ERR :", DateTime.Now);

        }
        /// <summary>
        /// Method to close master lock
        /// </summary>
        public void CloseDoorMaster()
        {

            if ((IsConnected) && (_bPowerOn))
            {
                if (DeviceBoard != null)
                {
                    AddMessage(DeviceBoard.deviceId, "CloseDoor() Master", "CMD :", DateTime.Now);
                    AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
                }
                if (DeviceBoard.LockDoor(DoorValue.DV_Master,true))
                    Lock_Status = SDK_SC_RfidReader.Lock_Status.Lock_Close;
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
            }
            else
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "CloseDoor() failed", "ERR :", DateTime.Now);

        }
        /// <summary>
        /// Method to close slave lock in SAS
        /// </summary>
        public void CloseDoorSlave()
        {

            if ((IsConnected) && (_bPowerOn))
            {
                if (DeviceBoard != null)
                {
                    AddMessage(DeviceBoard.deviceId, "CloseDoor() Slave", "CMD :", DateTime.Now);
                    AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
                }
                if (DeviceBoard.LockDoor(DoorValue.DV_Slave, true))
                    Lock_Status = SDK_SC_RfidReader.Lock_Status.Lock_Close;
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "Lock Status : " + Lock_Status.ToString(), "INF :", DateTime.Now);
            }
            else
                if (DeviceBoard != null)
                AddMessage(DeviceBoard.deviceId, "CloseDoor() failed", "ERR :", DateTime.Now);

        }

    
        /// <summary>
        /// Method to put the wait tag mode on
        /// </summary>
        /// <param name="bOnOff"></param>
        /// <returns></returns>
       
        public bool SetWaitForTag(bool bOnOff)
        {
          
            if (bOnOff)
            {                
                ThresholdBackup = GetThresholdValue();
                SendSwitchCommand(true, 1, true);
                DeviceBoard.enableCorrelationEvent(true);              
            }            

            bool ret = DeviceBoard.tagPresenceTest(bOnOff, 0, false);           
            if (ret) _isInWaitTagMode = bOnOff;
            return (ret);
        }
    
    }

    /// <summary>
    /// Class to exchange data with scan thread.
    /// </summary>
    public class ThreadArgs
    {
        /// <summary>
        /// Shared reader data
        /// </summary>
        public ReaderData ReaderData;
        /// <summary>
        /// Event to notify start of scan
        /// </summary>
        public EventWaitHandle EventScanStart;
        /// <summary>
        /// Event to notify end of scan
        /// </summary>
        public EventWaitHandle EventScanEnd;
        /// <summary>
        /// 
        /// </summary>
        public EventWaitHandle EventConfirmation;
        /// <summary>
        /// Bool to notify Dynamic inventory mode
        /// </summary>
        public bool BUseKR;
        /// <summary>
        /// Bool to notify dual field scan
        /// </summary>
        public bool BUnlockTagAllAxis;
        /// <summary>
        /// Bool to notify asynchronous info during scan
        /// </summary>
        public bool AsynchronousEvent;
        /// <summary>
        /// Bool to notify to reset list
        /// </summary>
        public bool ResetList;
        /// <summary>
        /// Shared device board class
        /// </summary>
        public DeviceRfidBoard DeviceBoard;
        /// <summary>
        /// Delegate to notify event
        /// </summary>
        public event NotifyHandlerDelegate NotifyEvent;
        /// <summary>
        /// int to fix timeout in  ms of the inventory
        /// </summary>
        public int ScanTimeout;
        /// <summary>
        /// String of the serial Nummber of the board
        /// </summary>
        public string SerialNumber;

        /// <summary>
        /// Boolean to true when reader is in scan, false otherwise
        /// </summary>
        public bool IsInScan;

        /// <summary>
        /// Array  of last scan tag on all axis
        /// </summary>
        public ArrayList LastScanSerial;
        /// <summary>
        /// Boolean to block the scan simultaneously from many devices
        /// </summary>
        public bool UseMutex = false;

        /// <summary>
        /// Delegate to recover confirmation statut value
        /// </summary>
        /// <returns>byte of the confirmation value</returns>
        public delegate byte GetConfirmationDelegate();

        /// <summary>
        ///  Delegate to recover end scan well ended value
        /// </summary>
        /// <returns>byte of the number of axis well ended</returns>
        public delegate byte GetCheckKZOutDelegate();
        private readonly GetCheckKZOutDelegate _getCheckKZOutDelegate;

        //private static Mutex _mutex;
        //private const string MutexName = "SDK_RFID.0mutex";

        private readonly ShowMessageDelegate _showOutbound;


        /// <summary>
        /// main constructor
        /// </summary>
        /// <param name="deviceBoard"></param>
        /// <param name="bUseKR"></param>
        /// <param name="bUnlockTagAllAxis"></param>
        /// <param name="asynchronousEvent"></param>
        /// <param name="resetList"></param>
        /// <param name="readerData"></param>
        /// <param name="eventScanStart"></param>
        /// <param name="eventScanEnd"></param>
        /// <param name="eventConfirmation"></param>
        /// <param name="notifyEvent"></param>
        /// <param name="scanTimeout"></param>
        /// <param name="serialNumber"></param>
        /// <param name="isInScan"></param>
        /// <param name="lastScanSerial"></param>
        /// <param name="getConfirmationDelegate"></param>
        /// <param name="getCheckKZOutDelegate"></param>
        /// <param name="showOutbound"></param>
        /// <param name="useMutex"></param>
        public ThreadArgs(DeviceRfidBoard deviceBoard,
                           bool bUseKR,
                           bool bUnlockTagAllAxis,
                           bool asynchronousEvent,
                           bool resetList,
                           ReaderData readerData,
                           EventWaitHandle eventScanStart,
                           EventWaitHandle eventScanEnd,
                           EventWaitHandle eventConfirmation,
                           NotifyHandlerDelegate notifyEvent,
                           int scanTimeout,
                           string serialNumber,
                           bool isInScan,
                           ArrayList lastScanSerial,
                           GetConfirmationDelegate getConfirmationDelegate,
                           GetCheckKZOutDelegate getCheckKZOutDelegate,
                           ShowMessageDelegate showOutbound,
                           bool useMutex )
        {
            DeviceBoard = deviceBoard;
            BUseKR = bUseKR;
            BUnlockTagAllAxis = bUnlockTagAllAxis;
            AsynchronousEvent = asynchronousEvent;
            ResetList = resetList;
            ReaderData = readerData;
            EventScanStart = eventScanStart;
            EventScanEnd = eventScanEnd;
            EventConfirmation = eventConfirmation;
            NotifyEvent = notifyEvent;
            ScanTimeout = scanTimeout;
            SerialNumber = serialNumber;
            IsInScan = isInScan;
            LastScanSerial = lastScanSerial;
            _getCheckKZOutDelegate = getCheckKZOutDelegate;
            _showOutbound = showOutbound;
            UseMutex = useMutex;
            //initMutex();
        }

        /*private void initMutex()
        {
            try
            {
                _mutex = Mutex.OpenExisting(MutexName, System.Security.AccessControl.MutexRights.FullControl);

            }
            catch (WaitHandleCannotBeOpenedException)
            {
                _mutex = new Mutex(false, MutexName);
            }
            catch
            {
                _mutex = new Mutex(false, MutexName);
                //throw new Exception();
            }
        }*/




        /// <summary>
        /// Main run function to scan Tags in a threads
        /// </summary>
        public void Run()
        {
          
            rfidReaderArgs notifyEvent;
            try
            {
                //if (UseMutex)
                //    _mutex.WaitOne(Timeout.Infinite, false);               
                #region RFID3D

                bool bEndConfirmation = false;
                ArrayList backupList = new ArrayList(LastScanSerial);
               
                if (!IsInScan) 
                //if (!IsInScan)
                {
                    IsInScan = true;
                    _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                    if (ResetList)
                    {
                        DeviceBoard.clearKnownTagsBeforeTagScan();
                    }
                    ReaderData.bScan = true;
                    ReaderData.bFail = false;
                    bool ret = DeviceBoard.startTagScan(true, AsynchronousEvent, BUnlockTagAllAxis, BUseKR);
                    if (ret == false)
                    {

                        //if ((DeviceBoard.deviceTypeMajorType == DeviceTypeMajorType.DEVICE_RFID_MONO_AXE_READER) ||
                          //   (DeviceBoard.deviceTypeMajorType == DeviceTypeMajorType.DEVICE_RFID_FLAT_3D_SHELVES))
                        if (true)
                        {
                            DeviceBoard.control(true, true);
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan, "Failed to start scan!");
                            if (NotifyEvent != null)
                            {
                                _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan.ToString(), "NFY :");
                                NotifyEvent(this, notifyEvent);

                            }
                        }
                        else
                        {  //Ie pb rebond de porte - assume que c'est une porte mal fermée - renvoi pour DT realnce le scan
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_Door_Opened, "Door open after startscan!");
                            if (NotifyEvent != null)
                            {
                                _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_Door_Opened.ToString(), "NFY :");
                                NotifyEvent(this, notifyEvent);

                            }
                        }
                        goto end;
                    }
                    if (EventScanStart.WaitOne(2000,false))
                    {
                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanStarted, "Scan started!");
                        if (NotifyEvent != null)
                        {
                            _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanStarted.ToString(), "NFY :");
                            NotifyEvent(this, notifyEvent);
                        }

                        if (EventScanEnd.WaitOne((int)ScanTimeout,false))
                        {
                            if (ReaderData.bFail)
                            {
                                ReaderData.FailureReason = DeviceBoard.tagScanFailureReason();
                                ReaderData.RspScan = RespScan.RS_ErrorDuringScan;
                                ReaderData.bScan = false;
                                IsInScan = false;
                                _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan, "Scan has error");
                                if (NotifyEvent != null)
                                {
                                    _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan.ToString(), "NFY :");
                                    NotifyEvent(this, notifyEvent);
                                }

                            }
                            else
                            {
                                 if (_getCheckKZOutDelegate() == 255)
                                    {
                                        ReaderData.RspScan = RespScan.RS_ReaderReady;
                                        ReaderData.bScan = false;
                                        IsInScan = false;
                                        _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "Host Stop Scan");
                                        if (NotifyEvent != null)
                                        {
                                            _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                                            NotifyEvent(this, notifyEvent);
                                        }
                                    }
                                 else
                                 {
                                    ReaderData.RspScan = RespScan.RS_ScanSucceed;
                                    ReaderData.bScan = false;
                                    IsInScan = false;
                                    _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCompleted, "Scan completed! / " + _getCheckKZOutDelegate().ToString());
                                    if (NotifyEvent != null)
                                    {
                                        _showOutbound(DeviceBoard, "RN_Scancompleted / " + _getCheckKZOutDelegate().ToString(), "NFY :");
                                        NotifyEvent(this, notifyEvent);
                                    }
                                    ReaderData.RspScan = RespScan.RS_ReaderReady;
                                 }
                               /* if (getCheckKZOutDelegate() == KZTabClass.KZValue[deviceBoard.deviceTypeMajorType]) // Verifie scan terminé correctement fonction du nombre axe donc du type de reader
                                {
                                    readerData.RspScan = RespScan.RS_ScanSucceed;
                                    readerData.bScan = false;
                                    isInScan = false;
                                    showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                    notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCompleted, "Scan completed! / " + getCheckKZOutDelegate().ToString());
                                    if (NotifyEvent != null)
                                    {
                                        showOutbound(deviceBoard, "RN_Scancompleted / " + getCheckKZOutDelegate().ToString(), "NFY :");
                                        NotifyEvent(this, notifyEvent);
                                    }
                                    readerData.RspScan = RespScan.RS_ReaderReady;
                                }
                                else
                                    if (getCheckKZOutDelegate() == 255)
                                    {
                                        readerData.RspScan = RespScan.RS_ReaderReady;
                                        readerData.bScan = false;
                                        isInScan = false;
                                        showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                        notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "Host Stop Scan");
                                        if (NotifyEvent != null)
                                        {
                                            showOutbound(deviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                                            NotifyEvent(this, notifyEvent);
                                        }
                                    }
                                    else
                                    {

                                        if ( (deviceBoard.deviceTypeMajorType == DeviceTypeMajorType.DEVICE_RFID_MONO_AXE_READER) ||
                                             (deviceBoard.deviceTypeMajorType == DeviceTypeMajorType.DEVICE_RFID_FLAT_3D_SHELVES))
                                        {
                                            readerData.RspScan = RespScan.RS_ScanSucceed;
                                            readerData.bScan = false;
                                            isInScan = false;
                                            notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCompleted, "Scan completed! / " + getCheckKZOutDelegate().ToString());
                                            if (NotifyEvent != null)
                                            {
                                                showOutbound(deviceBoard, "RN_Scancompleted / " + getCheckKZOutDelegate().ToString(), "NFY :");
                                                NotifyEvent(this, notifyEvent);
                                            }
                                            readerData.RspScan = RespScan.RS_ReaderReady;
                                        }
                                        else
                                        {
                                            #region Confirmation
                                            int nbaxe = KZTabClass.KZValue[deviceBoard.deviceTypeMajorType] - 1;
                                            // error KZ search for remaining with confirmation;
                                            showOutbound(deviceBoard, "RN_Scancompleted with existing KZ / " + getCheckKZOutDelegate().ToString(), "NFY :");


                                            //Search tag not inventory
                                            foreach (string myserial in readerData.strListTag)
                                            {
                                                if (backupList.Contains(myserial))
                                                    backupList.Remove(myserial);
                                            }

                                            //il y a des tags a trouver.
                                            if (backupList.Count > 0)
                                            {
                                                showOutbound(deviceBoard, "Essai Confirmation de " + backupList.Count.ToString() + "Tag(s)", "NFY :");

                                                for (int loop = 1; loop <= nbaxe; loop++)
                                                {
                                                    ArrayList tmpList = new ArrayList(backupList);
                                                    if ((tmpList.Count > 0) && (!bEndConfirmation))
                                                    {
                                                        showOutbound(deviceBoard, "Essai Confirmation sur Axe " + loop.ToString(), "NFY :");
                                                        SendSwitchCommand(true, (byte)loop, true);
                                                        deviceBoard.startConfirmation();
                                                        if (eventScanStart.WaitOne(2000,false))
                                                        {
                                                            foreach (string myserial in tmpList)
                                                            {
                                                                ConfirmTagUID(myserial, 12);
                                                                if (!eventConfirmation.WaitOne(1000,false))
                                                                {
                                                                    bEndConfirmation = true;
                                                                    break;
                                                                }
                                                                if (getConfirmationDelegate() == 1)
                                                                {
                                                                    showOutbound(deviceBoard, "Confirmation de " + myserial + " OK", "NFY :");
                                                                    backupList.Remove(myserial);
                                                                    notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_TagAdded, myserial);
                                                                    if (NotifyEvent != null) NotifyEvent(this, notifyEvent);
                                                                }
                                                                else
                                                                    showOutbound(deviceBoard, "Confirmation de " + myserial + " Not OK", "NFY :");
                                                            }
                                                            deviceBoard.endConfirmation();
                                                            Thread.Sleep(10);
                                                        }
                                                        else
                                                        {
                                                            deviceBoard.endConfirmation();
                                                            Thread.Sleep(10);
                                                            readerData.RspScan = RespScan.RS_FailedToStartScan;
                                                            readerData.bScan = false;
                                                            isInScan = false;
                                                            showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                                            notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan, "Reader Failed To start Scan!");
                                                            if (NotifyEvent != null)
                                                            {
                                                                showOutbound(deviceBoard, rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan.ToString(), "NFY :");
                                                                NotifyEvent(this, notifyEvent);
                                                            }
                                                            readerData.RspScan = RespScan.RS_ReaderReady;
                                                        }
                                                    }
                                                }
                                                if (!bEndConfirmation)
                                                {
                                                    if (backupList.Count == 0) // toutes retrouvées
                                                    {
                                                        readerData.RspScan = RespScan.RS_ScanSucceed;
                                                        readerData.bScan = false;
                                                        isInScan = false;
                                                        showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                                        notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCompleted, "Scan completed!");
                                                        if (NotifyEvent != null) NotifyEvent(this, notifyEvent);
                                                        readerData.RspScan = RespScan.RS_ReaderReady;
                                                    }
                                                    else
                                                    {
                                                        readerData.RspScan = RespScan.RS_ErrorDuringScan;
                                                        readerData.bScan = false;
                                                        isInScan = false;
                                                       // showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                                       // notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan, "Scan has error / " + getCheckKZOutDelegate().ToString());
                                                       // if (NotifyEvent != null)
                                                       // {
                                                       //     showOutbound(deviceBoard, rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan.ToString(), "NFY :");
                                                       //     NotifyEvent(this, notifyEvent);
                                                       // }
                                                        //KB : Compteur KZ pas fiable si seuil bas ( 18 pass axes sans erreur pour JSC)
                                                        showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                                        notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCompleted, "Scan completed! / " + getCheckKZOutDelegate().ToString());
                                                        if (NotifyEvent != null)
                                                        {
                                                            showOutbound(deviceBoard, "RN_Scancompleted / " + getCheckKZOutDelegate().ToString(), "NFY :");
                                                            NotifyEvent(this, notifyEvent);
                                                        }
                                                        readerData.RspScan = RespScan.RS_ReaderReady;
                                                       
                                                    }
                                                }
                                                else
                                                {
                                                    readerData.RspScan = RespScan.RS_ScanSucceed;
                                                    readerData.bScan = false;
                                                    isInScan = false;
                                                    showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                                    notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "Host Stop Scan");
                                                    if (NotifyEvent != null)
                                                    {
                                                        showOutbound(deviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                                                        NotifyEvent(this, notifyEvent);
                                                    }
                                                    readerData.RspScan = RespScan.RS_ReaderReady;
                                                }
                                            }
                                            else //erreur pas de tag renvoi erreur
                                            {
                                                readerData.RspScan = RespScan.RS_ErrorDuringScan;
                                                readerData.bScan = false;
                                                isInScan = false;
                                                ///notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan, "Scan has error / " + getCheckKZOutDelegate().ToString());
                                                //if (NotifyEvent != null)
                                                //{
                                                //    showOutbound(deviceBoard, rfidReaderArgs.ReaderNotify.RN_ErrorDuringScan.ToString(), "NFY :");
                                               //     NotifyEvent(this, notifyEvent);
                                                //}
                                                //readerData.RspScan = RespScan.RS_ReaderReady;
                                                showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                                                notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCompleted, "Scan completed! / " + getCheckKZOutDelegate().ToString());
                                                if (NotifyEvent != null)
                                                {
                                                    showOutbound(deviceBoard, "RN_Scancompleted / " + getCheckKZOutDelegate().ToString(), "NFY :");
                                                    NotifyEvent(this, notifyEvent);
                                                }
                                                readerData.RspScan = RespScan.RS_ReaderReady;
                                            }
                                            #endregion
                                        }
                                    }*/
                            }
                        }
                        else
                        {
                            DeviceBoard.control(false, true);
                            ReaderData.RspScan = RespScan.RS_FinishScanTimeOut;
                            ReaderData.bScan = false;
                            IsInScan = false;
                            _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                            notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout, "Timeout scanning!");
                            if (NotifyEvent != null)
                            {
                                _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ReaderScanTimeout.ToString(), "NFY :");
                                NotifyEvent(this, notifyEvent);
                            }
                        }
                    }
                    else
                    {
                        /*readerData.RspScan = RespScan.RS_FailedToStartScan;
                        readerData.bScan = false;
                        isInScan = false;
                        showOutbound(deviceBoard, "IsInScan = " + isInScan.ToString(), "INF :");
                        notifyEvent = new rfidReaderArgs(serialNumber, rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan, "Reader Failed To start Scan!");
                        if (NotifyEvent != null)
                        {
                            showOutbound(deviceBoard, rfidReaderArgs.ReaderNotify.RN_ReaderFailToStartScan.ToString(), "NFY :");
                            NotifyEvent(this, notifyEvent);
                        }   */

                        ReaderData.RspScan = RespScan.RS_ScanSucceed;
                        ReaderData.bScan = false;
                        IsInScan = false;
                        _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                        notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost, "reader himself Stop Scan ");
                        if (NotifyEvent != null)
                        {
                            _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ScanCancelByHost.ToString(), "NFY :");
                            NotifyEvent(this, notifyEvent);
                        }
                        ReaderData.RspScan = RespScan.RS_ReaderReady;
                    }
                }
                else
                {
                    notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ReaderNotReady, "Reader Not ready");
                    if (NotifyEvent != null)
                    {
                        _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ReaderNotReady.ToString(), "NFY :");
                        NotifyEvent(this, notifyEvent);
                    }
                }

end:            ReaderData.bScan = false;
                IsInScan = false;
                _showOutbound(DeviceBoard, "IsInScan = " + IsInScan.ToString(), "INF :");
                notifyEvent = new rfidReaderArgs(SerialNumber, rfidReaderArgs.ReaderNotify.RN_ThreadScanFinish, "Leave scan thread");
                if (NotifyEvent != null)
                {
                    _showOutbound(DeviceBoard, rfidReaderArgs.ReaderNotify.RN_ThreadScanFinish.ToString(), "NFY :");
                    NotifyEvent(this, notifyEvent);
                }
                #endregion

            }
            finally
            {
                //if (UseMutex)
                 //   _mutex.ReleaseMutex();
            }

        }
        /// <summary>
        /// Main run function to scan Tags in 3D in a threads
        /// </summary>
        public void Run3D()
        {
            Run();


        }
        /// <summary>
        /// Function to preform an autoscan
        /// </summary>
        public void RunAutoscan()
        {

        }

        /// <summary>
        /// Method to control switch board through RS485
        /// </summary>
        /// <param name="bSet">Set or clear relay</param>
        /// <param name="RelaisNumber">relais number to control (1 to 8) ; 9 for all</param>
        /// <param name="ResetAllBeforSet">if true clear all Relay befor set one.</param>
        public void SendSwitchCommand(bool bSet, byte RelaisNumber, bool ResetAllBeforSet)
        {
            byte AsciiRelaisValue;
            if (bSet)
            {
                if (ResetAllBeforSet)
                {
                    AsciiRelaisValue = (byte)(9 + 0x30);
                    DeviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
                    Thread.Sleep(400);
                }
                AsciiRelaisValue = (byte)(RelaisNumber + 0x30);
                DeviceBoard.sendSwitchCommand(1, AsciiRelaisValue);
                Thread.Sleep(500);
            }
            else
            {
                AsciiRelaisValue = (byte)(RelaisNumber + 0x30);
                DeviceBoard.sendSwitchCommand(0, AsciiRelaisValue); // clearCounts all
            }
        }
    }
}

