using System;
using System.Collections;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Interface of rfidReader class.
    /// </summary>
    interface IRfidReader
    {
        /// <summary>
        /// Property to get SerialNumber of the board
        /// </summary>
        string SerialNumber { get; }
        /// <summary>
        /// Property to know if the board is in scan process
        /// </summary>
        bool IsInScan { get; }

        /// <summary>
        /// property to retrieve software version format x.xx
        /// </summary>
        string FirmwareVersion { get; }
        /// <summary>
        /// property to retrieve hardware version format x.xx
        /// </summary>
        string HardwareVersion { get; }     
        /// <summary>
        /// Property to retrieve the connection status.
        /// </summary>
        /// <value>True is connected, false if not connected.</value>
        bool IsConnected {get;}
        /// <summary>
        /// Property to set or get the serial port name "ex COM1".
        /// </summary>
        string StrCom { get; set; }
        /// <summary>
        /// Property to set scan timeout in millisecond.This time while stop the scan process thread and request a end of 
        /// scanning.
        /// </summary>
        uint ScanTimeout { set; }

        /// <summary>
        /// Property to retrieve the Door Status State (ie close or open)
        /// </summary>
        Door_Status Door_Status { get; }
        /// <summary>
        ///  Property to retrieve the Lock Status State (ie close or open)
        /// </summary>
        Lock_Status Lock_Status { get; }
        /// <summary>
        /// Arry content the Serial plugged device after a request to the Discover device function
        /// </summary>
        ArrayList ListOfSerialPluggedDevices { get ; }


        /// <summary>
        /// 
        /// </summary>
        void Dispose();
        /// <summary>
        /// Function to switch on ot off the IR sensor and the movement detection sensor.
        /// </summary>
        /// <param name="bStart"></param>
        void SetIRSensorON(bool bStart);
        /// <summary>
        /// Method to retriev device plugged
        /// </summary>
        /// <returns>List array of serial port valid</returns>
        void DiscoverPluggedDevices();

        /// <summary>
        /// Method to connect a reader to a serial port.
        /// The result of the connection generate a notification ReaderNotify.RN_Connected if succeed
        /// or ReaderNotify.RN_FailedToConnect if failed.
        /// </summary>
        /// <param name="strCom">string name of the port com to connect</param>
        /// <returns>true is succeed, false otherwise</returns>
        bool ConnectReader(string strCom);
        /// <summary>
        /// Method to disconnect reader to serial port and stop listen thread.
        /// This Method generate a notification ReaderNotify.RN_Disconnected if succeed.
        /// </summary>
        /// <returns>true if succeed, false otherwise</returns>
        bool DisconnectReader();
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
        /// <param name="bUnlockTagAllAxis">bool to unlock tag at start scan . Must be true by default </param>
        void RequestScan(bool ResetList, bool AsynchronousEvent, bool bUseKR, bool bUnlockTagAllAxis);
        /// <summary>
        /// Same method as above but with a switch relay board control on one from 8
        /// </summary>
        /// <param name="ChannelNumber"></param>
        /// <param name="ResetList"></param>
        /// <param name="AsynchronousEvent"></param>
        /// <param name="bUseKR"></param>
        /// <param name="bUnlockTagAllAxis"></param>
        void RequestScan(byte ChannelNumber, bool ResetList, bool AsynchronousEvent, bool bUseKR, bool bUnlockTagAllAxis);

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
        void RequestScan3D(bool bUseKR, bool bUnlockTagAllAxis);

        /// <summary>
        /// Method to request end of the scanning thread.
        /// This Method generate a notification ReaderNotify.RN_ScanCancelByHost if succeed.
        /// </summary>
        bool RequestEndScan();
        /// <summary>
        /// Method to retrieve list of stored tag in reader board.
        /// Tag list are present in readerData.strListTag list and number of tag are in  readerData.nbTagScan
        /// </summary>
        void ReadScannedTag();
        /// <summary>
        /// Method to flash the firmware.
        /// Use this method with caution with only recommendation of spacecode.
        /// A bad use could result of a non function of the reader.
        /// </summary>
        /// <param name="hexFileNamePath">String to the path of the firmware file</param>
        void FlashFirmware(string hexFileNamePath);
        /// <summary>
        /// Method to retrieve correlation threshold.
        /// This value represent the level above which a correaltion is assume done by the tag.
        /// </summary>
        /// <returns>value of the threshold</returns>
        byte GetThresholdValue();
        /// <summary>
        /// Method to set and store threshold value.
        /// This value must be between 5 and 200.
        /// The value  must be greater than MaximumCorrelationWhithoutResponse to not detect noise and below
        /// MinimumCorrelationWithResponse to aasume to detect all the tag.
        /// </summary>
        /// <param name="Threshold">threshold value to set</param>
        void SetThresholdValue(byte Threshold);
        /// <summary>
        /// Method to request a correlation sampling process to measure noise level.
        /// This Method generate a notification ReaderNotify.RN_ThresholdMaxNoise when scan completed.
        /// The result are store in datareader class.
        /// </summary>
        /// <returns>true is order successively send.</returns>
        bool FindThreshold();

        /// <summary>
        /// Method to select one axis in the reader (for debug purpose only)
        /// </summary>
        /// <param name="bSet">True to set the relay false otherwise</param>
        /// <param name="RelaisNumber">Relay to drive for 1 to 8</param>
        /// <param name="ResetAllBeforSet">true to reset all relay before </param>
        void SendSwitchCommand(bool bSet, byte RelaisNumber, bool ResetAllBeforSet);

        /// <summary>
        /// Function to set the light value
        /// </summary>
        /// <param name="power">Scale the light form 0 to 300</param>
        void SetLightPower(UInt16 power);
        /// <summary>
        /// Function to open the door
        /// </summary>
        void OpenDoor();
        /// <summary>
        /// Function to close the Door
        /// </summary>
        void CloseDoor();
        //void InitWusb();
        int getSupply12V();
        void LEdOnAll(int axis, int timeout , bool bChangeChannel = true);
        int ConfirmAndLed(int axis, Hashtable tagLedState);
        void StopField();
        /// <summary>
        /// Function to set the alarm on IP device
        /// </summary>
        /// <param name="host">name or IP of the alarm device</param>
        /// <param name="bStrobeOn">true to set the stoble light on</param>
        /// <param name="bHeavySoundOn">true to set the heavy alarm on 114dB</param>
        /// <param name="bLoudSoundOn">true to set the light alarm on 90dB</param>
        /// <returns></returns>
        bool SetIpAlarm(string host, bool bStrobeOn, bool bHeavySoundOn, bool bLoudSoundOn);

        bool SetWaitForTag(bool bOnOff);

    }
}
