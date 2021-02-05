using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using SDK_SC_RfidReader.DeviceBase;

namespace SDK_SC_RfidReader
{
    /// <summary>
    /// Enumeration of door statut
    /// </summary>
    public enum Door_Status
    {
        /// <summary>
        /// Value for open
        /// </summary>
        Door_Open,
        /// <summary>
        /// value for close
        /// </summary>
        Door_Close,
    }
    /// <summary>
    /// 
    /// </summary>
    public enum Lock_Status
    {
        /// <summary>
        /// Value for open
        /// </summary>
        Lock_Open,
        /// <summary>
        /// value for close
        /// </summary>
        Lock_Close,
    }

       public enum TagType
    {
        TT_UNDEFINED = 0xFF,
        TT_R8 = 0x00,
        TT_SPCE2_RO = 0x01,
        TT_SPCE2_RW = 0x02,
        TT_SPCE2_DEC = 0x03,
    }

    /// <summary>
    /// This Class content all the variables used to store the result and status of a reader inventory.
    /// </summary>
    [Serializable]
    public class ReaderData
    {
        
        /// <summary>
        /// Variable to stored number of tag in the last inventory
        /// </summary>
        public int nbTagScan = 0;
        /// <summary>
        /// List collection of tag in string format
        /// </summary>
        public ArrayList strListTag = new ArrayList();
       
        /// <summary>
        ///  Variable of the status of the reader following RespScan enumeration
        ///  RS_ScanSucceed = 0x00, Value RS_ScanSucceed if scan ending correctly
        ///  RS_FailedToConnect = 0x01,  Value RS_FailedToConnect if no connection are valid or not connection are found
        ///  RS_FailedToStartScan = 0x02, Value RS_FailedToStartScan occurs when bad voltage detect, on the board, board not ready,
        ///  RS_FinishScanTimeOut = 0x03, Value RS_FinishScanTimeOut occurs when time to inventory is too long.
        ///  RS_ErrorDuringScan = 0x04, Values  RS_ErrorReadUID occurs when a Tag present in the field send recursively a bad uid number
        ///  This value mentionned that the scan cannot finish normally due to a tag error 
        ///  RS_ReaderReady = 0x05,  /// Value to enable next reader action
        /// </summary>
        public RespScan RspScan;
        /// <summary>
        /// String to  describe the reason of the last failure in case of failure notification. 
        /// </summary>
        public string FailureReason;        
        /// <summary>
        /// Bool to notify a failure.
        /// </summary>
        public bool bFail;
        /// <summary>
        /// Bool to notify a scan in process.
        /// </summary>
        public bool bScan;
        /// <summary>
        /// Byte to store maximum correlation when no tag answer - indicate max noise level.
        /// </summary>
        public byte MaximumCorrelationWhithoutResponse;
        /// <summary>
        ///  Byte to store average correlation when no tag answer
        /// </summary>
        public byte AverageCorrelationWhithoutResponse;
        /// <summary>
        /// Byte to store minimum correlation with tag answer - indicate Min tag answer.
        /// </summary>
        public byte MinimumCorrelationWithResponse;
        /// <summary>
        /// Byte to store Threshold value. This value must be between 5 and 200.
        /// The value  must be greater than MaximumCorrelationWhithoutResponse to not detect noise and below
        /// MinimumCorrelationWithResponse to assume to detect all the tag.
        /// </summary>
        public byte Threshold;
        /// <summary>
        /// String to store Serial Number in String Format
        /// </summary>
        public string DeviceID = null;

        public List<TagInfo> ListTagInfo = new List<TagInfo>(); 

    }

    public class TagInfo
    {
        public TagType TagType;
        public UInt64 TagIdSerial; // Previous Method of notitifaction
        public string TagIdOctal;  // FOR ALL should be 42 digit with famili and CRC
        public string TagId_R8_RO; // For R8 and R0 - 10 digits octals
        public string TagId_RW;    // For RW
        public string TagId_DEC;  // For Decimal 12 digits ( convert 14 octal after the FAMILY 1)
        
    }

    /// <summary cref="rfidReaderArgs">
    /// Class that define methods and variables to notify event of the rfid Reader.
    /// </summary>
    public class rfidReaderArgs : System.EventArgs
    {
        /// <summary>
        /// Enumeration of possible notification
        /// </summary>
        public enum ReaderNotify
        {
            /// <summary>
            /// Notification value when connectReader method failed.
            /// </summary>
            RN_FailedToConnect = 0x00,
            /// <summary>
            /// Notification value when reader connected.
            /// </summary>
            RN_Connected = 0x01,
            /// <summary>
            /// Notification value when reader disconnected.
            /// </summary>
            RN_Disconnected = 0x02,
            /// <summary>
            /// Notification value when inventory started.
            /// </summary>
            RN_ScanStarted = 0x03,
            /// <summary>
            /// Notification value when inventory completed.
            /// </summary>
            RN_ScanCompleted = 0x04,
            /// <summary>
            /// Notification value when tag added.
            /// </summary>
            RN_TagAdded = 0x05,
            /// <summary>
            /// Notification value when tag removed.
            /// </summary>
            RN_TagRemoved = 0x06,
            /// <summary>
            /// Notification value when read tag list completed.
            /// </summary>
            RN_ReadTagCompleted = 0x07,
            /// <summary>
            /// Notification value reader not ready.
            /// </summary>
            RN_ReaderNotReady = 0x08,
            /// <summary>
            /// Notification value when failed to start Scan.
            /// </summary>
            RN_ReaderFailToStartScan = 0x09,
            /// <summary>
            /// Notification when scan time overtake scan timeout value.
            /// </summary>
            RN_ReaderScanTimeout = 0x0A,
            /// <summary>
            /// Notification when error during scan occurs.
            /// </summary>
            RN_ErrorDuringScan = 0x0B,
            /// <summary>
            /// Notification when firmware update.
            /// </summary>
            RN_FirmwareMessage = 0x0C,
            /// <summary>
            /// Notify scan stop by request from the host.
            /// </summary>
            RN_ScanCancelByHost = 0x0D,
            /// <summary>
            /// Notify result of noise aquisition completed.
            /// </summary>
            RN_ThresholdMaxNoise = 0x0E,
            /// <summary>
            /// Notify failed to disconnect device
            /// </summary>
            RN_FailedToDisconnected = 0x0F,
            /// <summary>
            /// Notify that reader alraedy disconnected
            /// </summary>
            RN_AlreadyDisconnected = 0x20,
            /// <summary>
            /// Notify When the search of plugged devices is completed
            /// </summary>
            RN_DiscoverPluggedDevicesCompleted = 0x21,
            /// <summary>
            /// Notify that cable or plug is removed 
            /// </summary>
            RN_UsbCableUnplug = 0x22,
            /// <summary>
            /// Notify that power is OFF
            /// </summary>
            RN_Power_OFF = 0x23,
            /// <summary>
            /// Notify that power is ON
            /// </summary>
            RN_Power_ON = 0x24,
            /// <summary>
            /// Notification when door opened
            /// </summary>
            RN_Door_Opened = 0x40,
            /// <summary>
            /// notification for door closed
            /// </summary>
            RN_Door_Closed = 0x41,
            /// <summary>
            /// 
            /// </summary>
            RN_Scan_Pourcent = 0x42,           

            /// <summary>
            /// Notification flash started
            /// </summary>
            RN_FirmwareStarted = 0x43,
            /// <summary>
            /// Notification firlware flashe succeed to end
            /// </summary>
            RN_FirmwareSuccedToFinish = 0x44,
            /// <summary>
            /// Firmware flashed failed to end
            /// </summary>
            RN_FirmwareFailedToFinish = 0x45,
            /// <summary>
            /// Firmware corrupted file
            /// </summary>
            RN_FirmwareCorruptedHexFile = 0x46,
            /// <summary>
            /// Notification for debug and assume thread scan finish
            /// </summary>
            RN_ThreadScanFinish = 0x50,
            /// <summary>
            /// Notification for debug and assume thread scan join the main thread
            /// </summary>
            RN_ThreadScanJoin = 0x51,
            /// <summary>
            /// Notification for new usb serial port plug
            /// </summary>
            RN_SerialPortPlugged = 0x52,

            /// <summary>
            /// Notification for infra red event sensor
            /// </summary>
            RN_IntrusionDetected = 0x53,
            /// <summary>
            /// Notification for movement/acceloremeter 
            /// </summary>
            RN_MovementDetected = 0x54,
            /// <summary>
            /// Notification alarm door stay open after Timer
            /// </summary>
            RN_DoorOpenTooLong = 0x55,
            RN_Locked_Before_Open = 0x56,
            /// <summary>
            /// Notification when tag Detected
            /// </summary>
            RN_TagPresenceDetected = 0x10,
            RN_ActiveChannnelChange = 0x11,

            RN_TestInfo = 0x60,
            RN_CorrelationSample = 0x61,
            RN_CorrelationSamplesComplete = 0x62,
            RN_TagCharacterizationComplete = 0x63,
            RN_TagScanCompleted = 0x64,
        }
        private string serialNumber;
        private ReaderNotify rnValue;
        private string message;
        /// <summary>
        /// Constructor of rfidReaderArgs
        /// </summary>
        /// <param name="RNValue">Notification to launch</param>
        /// <param name="message">Message to displayed</param>
        ///  <param name="serialNumber">SerialNumber of the  Board</param>
        public rfidReaderArgs(string serialNumber,ReaderNotify RNValue, string message)
        {
            this.serialNumber = serialNumber;
            this.rnValue = RNValue;
            this.message = message;
        }
        /// <summary>
        /// Property to retreive serial number
        /// </summary>
        public string SerialNumber
        {
            get { return serialNumber; }
        }

        /// <summary>
        /// Property to retrieve message.
        /// </summary>
        /// <returns>a string of the message.</returns>
        public string Message
        {
            get { return message; }
        }
        /// <summary>
        /// Property to retrieve notification type
        /// </summary>
        /// <returns>The enumeration ReaderNotify</returns>
        public ReaderNotify RN_Value
        {
            get { return rnValue; }
        }
    }
    
    /// <summary>
    /// Class to store last error message
    /// </summary>
    public class ErrorBoard
    {
        /// <summary>
        /// Time of the error
        /// </summary>
        public DateTime dt = DateTime.Now;
        /// <summary>
        /// Message of the error
        /// </summary>
        public string message = "No error";
    }

    
}
