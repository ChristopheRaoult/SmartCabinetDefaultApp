using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Class DeviceRfidBoard attach to the interface IDeviceRfidBoard
    /// This class contain variable and method for control a spacecode RFID board 
    /// </summary>
    public class DeviceRfidBoard : IDeviceRfidBoard,IDisposable
    {
      

        /// <summary>
        /// Delegate method for display message from communication channel
        /// </summary>
        /// <param name="device">the interface of the device from which the message come</param>
        /// <param name="message">The message to displayed</param>
        /// <param name="modifier">The modifier for parse message as come in or out or if it's a asynchronous message</param>
        public delegate void ShowMessageDelegate(IDeviceRfidBoard device, string message, string modifier);

        /// <summary>
        /// Variable uint which contain serial number of the device.
        /// </summary>
        public uint deviceId = 0x000000;
        /// <summary>
        /// Variable constant for limit maximum duty cycle value in the board.
        /// </summary>
        public const ushort MAX_DutyCycle = 167;
        /// <summary>
        /// Variable constant to define minimum phase accepted during phase shift.
        /// </summary>
        public const int MIN_ValidPhaseShift = -70;
        /// <summary>
        /// Variable constant to define maximum phase accepted during phase shift.
        /// </summary>
        public const int MAX_ValidPhaseShift = 70;
        /// <summary>
        /// Variable constant to define lower limit of the threshold value
        /// </summary>
        public const int MIN_CorrelationThreshold = 5;
        /// <summary>
        /// Variable constant to define upper limit of the threshold value
        /// </summary>
        public const int MAX_CorrelationThreshold = 250;

        /// <summary>
        /// Variable bool if the board is connect 
        /// </summary>
        private bool bConnected = false;
        /// <summary>
        /// Variable IDeviceChannel that content the communication layer interface
        /// </summary>
        public IDeviceChannel deviceChannel;
        /// <summary>
        /// Delegate method for display message that are send and receive through the communication layer
        /// </summary>
        protected readonly ShowMessageDelegate showOutbound;

        /// <summary>
        /// Variable uint major value of the hardware version.
        /// </summary>
        protected uint hardwareVersionMajor;
        /// <summary>
        /// Variable uint minor value of the hardware version.
        /// </summary>
        protected uint hardwareVersionMinor;
        /// <summary>
        /// Variable uint major value of the software version.
        /// </summary>
        protected uint softwareVersionMajor;
        /// <summary>
        /// Variable uint minor value of the software version.
        /// </summary>
        protected uint softwareVersionMinor;
        /// <summary>
        /// Variable DeviceTypeMajorType
        /// </summary>
        public DeviceTypeMajorType deviceTypeMajorType;

        private string firmwareVersion;
        /// <summary>
        /// property to get software version
        /// </summary>
        public string FirmwareVersion { get { return firmwareVersion; } }

        private string hardwareVersion;
        /// <summary>
        /// property to get software version
        /// </summary>
        public string HardwareVersion { get { return hardwareVersion; } }

        /// <summary>
        /// Property to retrieve serial number of the board
        /// </summary>
        public uint DeviceId { get { return deviceId; } set { deviceId = value; } }
        /// <summary>
        /// Variable string contain the serial port used.
        /// </summary>
        private string strSerialPortCom;

        private bool _disposed = false;
        /// <summary>
        /// 
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
            if (!_disposed)
            {
                if (disposing)
                {
                    if (deviceChannel != null)
                    {
                        deviceChannel.cancelPendingOperations();
                        deviceChannel.CloseSerialPort();
                        _disposed = true;
                    }                   
                }
            }
        }

        /// <summary>
        /// Constructor of DeviceRfidBoard
        /// </summary>
        /// <param name="deviceId">DeviceID is a dummy variable to recover device ID of the board </param>
        /// <param name="strSerialPortCom">String of the associated serial port plug to the device</param>
        /// <param name="showOutbound">Delegate for displaying message from serial port</param>
        public DeviceRfidBoard(uint deviceId,
                               string strSerialPortCom,
                               ShowMessageDelegate showOutbound)
        {
            this.deviceId = deviceId;
            this.strSerialPortCom = strSerialPortCom;
            this.showOutbound = showOutbound;
            firmwareVersion = null;

        }
        /// <summary>
        /// Method to connect device to the serial port string pass in the constructor
        /// If the connection succeed recover all the parameter of the board.
        /// </summary>
        /// <returns>true is succed to connect, false otherwise</returns>
        public bool ConnectBoard()
        {
            deviceChannel = null;
            deviceChannel = new RS232DeviceChannel(0x23200232);
            bConnected = deviceChannel.OpenSerialPort(strSerialPortCom, out deviceId);
            if (bConnected)
            {
                deviceChannel.TheDeviceId = deviceId;
                PbRspGetHelloWorld Helloworld = getHelloWorld();
                this.deviceId = Helloworld.DeviceInfo.deviceId;
                hardwareVersionMajor = Helloworld.DeviceInfo.description.hardwareVersionMajor;
                hardwareVersionMinor = Helloworld.DeviceInfo.description.hardwareVersionMinor;
                softwareVersionMajor = Helloworld.DeviceInfo.description.softwareVersionMajor;
                softwareVersionMinor = Helloworld.DeviceInfo.description.softwareVersionMinor;
                firmwareVersion = softwareVersionMajor.ToString() + "." + softwareVersionMinor.ToString("00");
                hardwareVersion = hardwareVersionMajor.ToString() + "." + hardwareVersionMinor.ToString("00");

                switch (hardwareVersionMajor)
                {
                    case 1: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_DIAMOND_SMART_BOX; break;
                    case 2: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_JEWELLERY_CABINET; break;
                    case 3: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_MEDICAL_CABINET; break;
                    case 4: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_DIAMOND_SMART_BOX_V2; break;
                    case 5: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_MONO_AXE_READER; break;
                    case 6: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_FLAT_3D_SHELVES; break;
                    case 7: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_DIAMOND_SAS; break;
                    case 8: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_FRIDGE_1D; break;
                    case 9: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_MINI_SAS; break;
                    case 10: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_RFID_BLOOD_FRIDGE; break;
                    default: deviceTypeMajorType = DeviceTypeMajorType.DEVICE_UNKNOWN; break;
                }
            }
            return (bConnected);
        }
        /// <summary>
        /// Method to disconnect device from the seriap port
        /// </summary>
        /// <returns>return true id succeed false otherwise</returns>
        public bool DisconnectBoard()
        {
            if (deviceChannel.CloseSerialPort())
            {
                bConnected = false;
                deviceChannel = null;
            }
            return (bConnected);
        }
        /// <summary>
        /// Property for retrieved  connection status.
        /// </summary>
        public bool IsConnected { get { return bConnected; } }

        /// <summary>
        /// Method for marshal a byte array to a response struct define in DefRfidClass
        /// </summary>
        /// <param name="message">Object struct to receive message </param>
        /// <param name="responseType">Type of message to marshal</param>
        /// <returns>The struc or class with variable filled with response array byte</returns>
        protected object marshalMessage(object message, Type responseType)
        {

            if (deviceChannel == null) return null;

            showOutbound(this, message.ToString(), "> ");

            // Use reflection to get the message content size.
            Type messageType = message.GetType();
            System.Reflection.FieldInfo sizeField = messageType.GetField("SIZE",
                System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            uint contentSize = (uint)sizeField.GetValue(message);

            // Pin the message content to be sent to unmanaged code, then send it and wait for the response.
            byte[] messageContent = new byte[contentSize];
            GCHandle pinnedMessageContent = GCHandle.Alloc(messageContent, GCHandleType.Pinned);
            Marshal.StructureToPtr(message, pinnedMessageContent.AddrOfPinnedObject(), true);
            ResponseType genericResponse = deviceChannel.sendMessageWaitResponse(deviceId, messageContent);
            pinnedMessageContent.Free();

            // If the response is null or the response indicates failure, the request failed.
            if (genericResponse == null)
            {
                //throw new SerialException(ResponseCodeType.PBRC_TIMEOUT, "Board not responding.");
                showOutbound(this, "Board not responding. Generic response is null", "< ");
                if (deviceChannel != null)
                {
                    if (deviceChannel.ErrBoard != null)
                    {
                        deviceChannel.ErrBoard.dt = DateTime.Now;
                        deviceChannel.ErrBoard.message = message.ToString();
                    }
                }
                return null;
            }
            // If the response indicates failure, throw an exception. The calling code may or may
            // not handle the exception.
            if (genericResponse.serialMessage == null) return null;
            if (genericResponse.serialMessage[0] != (byte)ResponseCodeType.PBRC_SUCCESS)
            {
                //showResponseDelegate(this, genericResponse.ToString());
                string str = string.Format("Response failure: {0}; message was {1}",
                    (ResponseCodeType)genericResponse.serialMessage[0], message);
                showOutbound(this, str, "< ");
              /*  throw new SerialException((ResponseCodeType)genericResponse.serialMessage[0],
                    string.Format("Response failure: {0}; message was {1}",
                    (ResponseCodeType)genericResponse.serialMessage[0], message));*/
                return null;
            }

            // If caller doesn't want to extract the response contents, return the generic response object.
            if (responseType == null)
            {
                //showResponseDelegate(this, genericResponse.ToString());
                showOutbound(this, genericResponse.ToString(), "< ");
                return genericResponse;
            }

            object response = Utilities.MarshalToStruct(genericResponse.serialMessage, responseType);

            showOutbound(this, response.ToString(), "< ");
            return response;
        }

        /// <summary>
        /// Method for requestr a particular action from rfidCommandtype with responsetype not defined
        /// </summary>
        /// <param name="rfidCommand">The command to send</param>
        /// <param name="param1">General purpose param depending  of the request action</param>
        /// <param name="param2">General purpose param depending  of the request action</param>
        /// <param name="param3">General purpose param depending  of the request action</param>
        /// <returns></returns>
        private object rfidAction(RfidCommandType rfidCommand,
           byte param1, ushort param2, ushort param3)
        {
            PbMsgRfidCommand message = new PbMsgRfidCommand(rfidCommand,
                param1, param2, param3);
            return marshalMessage(message, typeof(PbRspRfidCommand));
        }
        /// <summary>
        /// Method for request a particular action from rfidCommandtype with responsetype defined
        /// </summary>
        /// <param name="rfidCommand">The command to send</param>
        /// <param name="responseType">Class type expected in return</param>
        /// <param name="param1">General purpose param depending  of the request action</param>
        /// <param name="param2">General purpose param depending  of the request action</param>
        /// <param name="param3">General purpose param depending  of the request action</param>
        /// <returns>Return struct or class of the requested command</returns>
        private object rfidAction(RfidCommandType rfidCommand, Type responseType,
            byte param1, ushort param2, ushort param3)
        {
            PbMsgRfidCommand message = new PbMsgRfidCommand(rfidCommand,
                param1, param2, param3);
            return marshalMessage(message, responseType);
        }
        /// <summary>
        /// Method for request a board to discover and give it's parameter (serial number and version)
        /// </summary>
        /// <returns>a PbRspGetHelloWorld class with serial number and version filled</returns>
        public PbRspGetHelloWorld getHelloWorld()
        {
            PbMsgGetHelloWorld message = new PbMsgGetHelloWorld();
            return (PbRspGetHelloWorld)marshalMessage(message, typeof(PbRspGetHelloWorld));
        }
        
       
        /// <summary>
        /// Method for request clear the known tag list before the next tag scan. Only applies to one tag scan. 
        /// </summary>
        public void clearKnownTagsBeforeTagScan()
        {
            rfidAction(RfidCommandType.RBDC_ClearTagListBeforeTagScan, 0, 0, 0);
        }
        /// <summary>
        /// Method for set dutycycle value of full and half bridge and the mode of create the field
        /// </summary>
        /// <param name="bEnableHalfBridge">if set , board is in half bridge (low field mode)</param>
        /// <param name="fullBridgeDutyCycle">dutycycle value for the fullbridge mode</param>
        /// <param name="halfBridgeDutyCycle">dutycycle value for the halfbridge mode</param>
        public void setBridgeState(bool bEnableHalfBridge, uint fullBridgeDutyCycle, uint halfBridgeDutyCycle)
        {
            rfidAction(RfidCommandType.RBDC_SetBridgeState, (byte)(bEnableHalfBridge ? 1 : 0),
                (ushort)fullBridgeDutyCycle, (ushort)halfBridgeDutyCycle);
        }
        /// <summary>
        /// Method to request end scan
        /// </summary>
        /// <param name="bEnableStatsAsyncEvent">bool to enable async event</param>
        /// <param name="bStopScan">bool to request end scan</param>
        /// <returns>true if succeed</returns>
        public bool control(bool bEnableStatsAsyncEvent, bool bStopScan)
        {
            PbMsgRfidControl message = new PbMsgRfidControl(bEnableStatsAsyncEvent, bStopScan);

            return (marshalMessage(message, null) != null); // Success if response is not null.
        }

      

        /*public bool startTagScan(bool bClearTagListHasChangedFlag, bool bAsynchronousTagUpdates)
        {
            PbMsgRequestTagScan message = new PbMsgRequestTagScan(bClearTagListHasChangedFlag,
                bAsynchronousTagUpdates);
            try
            {
                PbRspRequestTagScan response = (PbRspRequestTagScan)marshalMessage(message, typeof(PbRspRequestTagScan));
                return true;
            }
            catch (SerialException)
            {
                return false; // The request failed.
            }
        }*/
        /// <summary>
        /// Method for send an inventory request
        /// </summary>
        /// <param name="bClearTagListHasChangedFlag">if set, list of known tag is cleared</param>
        /// <param name="bAsynchronousTagUpdates">if set, tag are notify during scan asynchronously , if false the function getNextTag must be use for retrieve tag UID</param>
        /// <param name="bUnlockTagAllAxis">If set perform unlock all tag prior to scan</param>
        /// <param name="bUseKR">If set perform an inventory with KR</param>
        /// <returns>true if request succeed : Be careful the start of the invnetory will be notify asynchronously</returns>
        public bool startTagScan(bool bClearTagListHasChangedFlag, bool bAsynchronousTagUpdates, bool bUnlockTagAllAxis, bool bUseKR)
        {
            PbMsgRequestTagScan message = new PbMsgRequestTagScan(bClearTagListHasChangedFlag,
                bAsynchronousTagUpdates, bUnlockTagAllAxis, bUseKR);
            try
            {
                PbRspRequestTagScan response = (PbRspRequestTagScan)marshalMessage(message, typeof(PbRspRequestTagScan));
                if (response != null)
                {
                    if ((response.doorStatus & (byte)DoorStatusFlags.DSF_DoorIsOpen) != 0)
                        return false;
                    else
                        return true;
                }
                else
                    return false;
            }
            catch (SerialException)
            {
                showOutbound(this, "Board not responding 1st in Start Scan Function", "< ");

                deviceChannel.ErrBoard.dt = DateTime.Now;
                deviceChannel.ErrBoard.message = "Erreur send Scan 1 time";
                return false;
            }
            
              /* Thread.Sleep(1000);
               ConnectBoard();
               Thread.Sleep(1000);
               message = new PbMsgRequestTagScan(bClearTagListHasChangedFlag,
               bAsynchronousTagUpdates, bUnlockTagAllAxis, bUseKR);
               try
               {
                   PbRspRequestTagScan response = (PbRspRequestTagScan)marshalMessage(message, typeof(PbRspRequestTagScan));
                   return true;
               }
               catch (SerialException)
               {
                   showOutbound(this, "Board not responding 2nd in Start Scan Function", "< ");
                   deviceChannel.ErrBoard.dt = DateTime.Now;
                   deviceChannel.ErrBoard.message = "Erreur send Scan 2 time";
                   return false;
               }*/
            
        }
       

         // Initiate a tag characterization session to gather sampleCount samples with Full-Bridge
        // with the specified duty cycle. The resulting data may be retrieved using
        // getCharacterizeTagResults() after the BDET_TagCharacterizationComplete backdoor event
        // is received.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dutyCycle"></param>
        /// <param name="sampleCount"></param>
        /// <param name="bStopOnBadPhase"></param>
        /// <returns></returns>
        public bool characterizeTag(uint dutyCycle, uint sampleCount, bool bStopOnBadPhase)
        {
            PbMsgRfidBackDoorTagCharacterization message = new PbMsgRfidBackDoorTagCharacterization(
                dutyCycle, sampleCount, MIN_ValidPhaseShift, MAX_ValidPhaseShift, bStopOnBadPhase);
            return (marshalMessage(message, null) != null);
        }

        /// <summary>
        /// Get the tag characterization data from a tag characterization session initiated
        /// with a call to characterizeTag().
        /// </summary>
        /// <returns>The tag characterization data</returns>
        public PbRspRfidBackDoorGetTagCharacterizationResults getCharacterizeTagResults()
        {
            PbMsgRfidBackDoor message = new PbMsgRfidBackDoor(
                RfidCommandType.RBDC_GetTagCharacterizationResults, 0, 0, 0);
            return (PbRspRfidBackDoorGetTagCharacterizationResults)marshalMessage(message,
                typeof(PbRspRfidBackDoorGetTagCharacterizationResults));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bStart"></param>
        /// <param name="sampleCount"></param>
        /// <param name="bGetPhaseShift"></param>
        /// <returns></returns>
        public bool tagPresenceTest(bool bStart, uint sampleCount, bool bGetPhaseShift)
        {
            // Initiate a tag presence test. Response is BackDoorEventType.BDET_CorrelationSample.
            PresenceTestFlagsType param1 = 0;
            if (bStart)
                param1 |= PresenceTestFlagsType.PTF_EnableTest;
            if (bGetPhaseShift)
                param1 |= PresenceTestFlagsType.PTF_GetPhaseShift;
            return (backDoor(RfidCommandType.RBDC_TagPresenceTest, (byte)param1,
                (byte)sampleCount, 0) != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bEnable"></param>
        public void enableCorrelationEvent(bool bEnable)
        {
            backDoor(RfidCommandType.RBDC_EnableCorrelationEvent, (byte)(bEnable ? 1 : 0), 0, 0);
        }

        /// <summary>
        /// Method to request next tag in list known tagin Stat Scan function
        /// </summary>
        /// <param name="bFirstTag">If set, the first tag is requested and put all the tag uin the list in no reported status</param>
        /// <param name="tagID">variable to store the uid retrieved</param>
        /// <param name="tagIndex">Index of the tag in the list</param>
        /// <param name="tagCount">number of tag in the list</param>
        /// <returns>true is succeed to request</returns>
        /// 
        public bool getNextTag(bool bFirstTag, out UInt64 tagID, out uint tagIndex, out uint tagCount)
        {
            PbMsgRfidGetNextTag message = new PbMsgRfidGetNextTag();
            message.bGetFirst = bFirstTag;

            while (true)
            {
                PbRspRfidGetNextTag getNextTagResponse =
                    (PbRspRfidGetNextTag)marshalMessage(message, typeof(PbRspRfidGetNextTag));

                if ((getNextTagResponse == null) || (getNextTagResponse.completionStatus != (byte)ResponseCodeType.PBRC_SUCCESS))
                {
                    Debug.Fail("Unexpected response for PbMsgRfidGetNextTag: " +
                        ((getNextTagResponse == null) ? "null" : ((ResponseCodeType)getNextTagResponse.completionStatus).ToString()));
                    tagID = 0;
                    tagIndex = 0;
                    tagCount = 0;
                    return false;
                }

                tagID = SerialRFID.SerialNumber(getNextTagResponse.serialNumber);
                tagIndex = getNextTagResponse.tagIndex;
                tagCount = getNextTagResponse.currentTagCount;

                // If another tag has responded, return it to the caller.
                if ((getNextTagResponse.tagInfo & (uint)TagInfoFlags.TIF_ReachedEndOfTagList) == 0)
                    return true;

                // If the device is still scanning tags, loop to try again, else return false.
                if ((getNextTagResponse.scanStatus & (uint)ScanStatusFlags.SSF_ScanningTagsNow) == 0)
                    return false;
            }
        }
        /// <summary>
        /// Method for request the failure of tag that have been report failed tag
        /// </summary>
        /// <param name="failedTagIndex">Index of tag to retreive</param>
        /// <param name="tagID">>variable to store the uid retrieved</param>
        /// <param name="serialDigitsRead">Number of digit read before failure</param>
        /// <param name="failureReason">The reason of the failure in string format</param>
        /// <returns>true is succeed</returns>
        public bool getFailedTag(uint failedTagIndex, out UInt64 tagID, out uint serialDigitsRead, out string failureReason)
        {
            PbMsgRfidGetTagFailure message = new PbMsgRfidGetTagFailure(failedTagIndex);
            PbRspRfidGetTagFailure response =
                (PbRspRfidGetTagFailure)marshalMessage(message, typeof(PbRspRfidGetTagFailure));
            if ((response == null) || (response.completionStatus != (byte)ResponseCodeType.PBRC_SUCCESS))
            {
                Debug.Fail("Unexpected response for PbMsgRfidGetNextTag: " +
                    ((response == null) ? "null" : ((ResponseCodeType)response.completionStatus).ToString()));
                tagID = 0;
                serialDigitsRead = 0;
                failureReason = "";
                return false;
            }

            tagID = SerialRFID.SerialNumber(response.digitSequence);
            serialDigitsRead = response.digitsRead;
            switch ((TagFailureEnumType)response.failureType)
            {
                case TagFailureEnumType.TFFT_MissingDigit: failureReason = "Digit Missing"; break;
                case TagFailureEnumType.TFFT_ChecksumFailure: failureReason = "Checksum"; break;
                default: Debug.Fail("Unexpected failure type."); failureReason = "** Unknown **"; break;
            }

            return true;
        }
        /// <summary>
        /// Method for format a scan failure reason
        /// </summary>
        /// <returns>The string of the failure of the non start scan</returns>
        public string tagScanFailureReason()
        {
            PbRspGetStatus status = (PbRspGetStatus)getStatus();
            if (status == null)
                return "Device not responding";
            if ((status.scanStatus & (byte)ScanStatusFlags.SSF_ScanningTagsNow) != 0)
                return "Device already scanning tags";
            if ((status.scanStatus & (byte)ScanStatusFlags.SSF_TagScanWasCanceled) != 0)
                return "Tag scan canceled";
            if ((status.systemStatus & (byte)SystemStatusFlags.SSF_12VoltsFailure) != 0)
                return "12V power failure";
            if ((status.systemStatus & (byte)SystemStatusFlags.SSF_5VoltsFailure) != 0)
                return "5V power failure";
            if ((status.systemStatus & (byte)SystemStatusFlags.SSF_AntennaNotConnected) != 0)
                return "Antenna disconnected";
            return "Unknown";
        }
        /// <summary>
        /// Method to request status of the board
        /// </summary>
        /// <returns>The class PbRspGetStatus with the info status filled</returns>
        public object getStatus()
        {
            PbMsgGetStatus message = new PbMsgGetStatus();
            return marshalMessage(message, typeof(PbRspGetStatus));
        }
        private object backDoor(RfidCommandType backDoorCommand,
            byte param1, ushort param2, ushort param3)
        {
            PbMsgRfidBackDoor message = new PbMsgRfidBackDoor(backDoorCommand,
                param1, param2, param3);
            return marshalMessage(message, typeof(PbRspRfidBackDoor));
        }

        private object backDoor(RfidCommandType backDoorCommand, Type responseType,
            byte param1, ushort param2, ushort param3)
        {
            PbMsgRfidBackDoor message = new PbMsgRfidBackDoor(backDoorCommand,
                param1, param2, param3);
            return marshalMessage(message, responseType);
        }
        /// <summary>
        /// Method to request Threshold value
        /// </summary>
        /// <returns>value of the threshold - must be between 5 and 200</returns>
        public byte getCorrelationThreshold()
        {
            PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
                RfidCommandType.RBDC_GetCorrelationThreshold, 0, 0, 0);
            if (response == null)
                return 0;
            return response.responseByte1;
        }
        /// <summary>
        /// Method to retreive 12v value
        /// </summary>
        /// <returns>int of the batterie value from ADC</returns>
        public int getSupply12V()
        {
            PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
                RfidCommandType.RBDC_GetSupply12V, 0, 0, 0);
            if (response == null)
                return 0;
            return response.responseWord1;
           
        }

        public int getNumberMaxChannel()
        {
            
             PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
                RfidCommandType.RDBC_GetNumberMaxChannel, 0, 0, 0);
            return response == null ? 0 : response.responseWord1;
        }

        /// <summary>
        /// Method to set the threshold noise value
        /// </summary>
        /// <param name="threshold">value of the threshold -must be between 5 and 200</param>
        public void setCorrelationThreshold(byte threshold)
        {
            backDoor(RfidCommandType.RBDC_SetCorrelationThreshold, threshold, 0, 0);
        }
        /// <summary>
        /// Method to store threshold value in ROM
        /// </summary>
        public void saveCorrelationThresholdToROM()
        {
            backDoor(RfidCommandType.RBDC_SaveCorrelationThresholdToRom, 0, 0, 0);
        }

        /// <summary>
        /// Method to request a noise and tag answer aquisition
        /// </summary>
        /// <param name="bStartSampling">bool to start and stop sampling</param>
        /// <param name="sampleCount">Number of samples to obtain</param>
        /// <returns></returns>
        public bool sampleCorrelationSeries(bool bStartSampling, ushort sampleCount)
        {
            return (backDoor(RfidCommandType.RBDC_SampleCorrelation, (byte)(bStartSampling ? 1 : 0), sampleCount, 0) != null);
        }
        /// <summary>
        /// Method to request unique sample
        /// </summary>
        /// <param name="bNonResponseOnly">true if without response requested</param>
        /// <param name="sampleCount">number of sample</param>
        /// <returns>true is succeed to launch</returns>
        public bool sampleCorrelationSeriesSimple(bool bNonResponseOnly, ushort sampleCount)
        {
            return (backDoor(RfidCommandType.RBDC_SampleCorrelationSimple, (byte)(bNonResponseOnly ? 1 : 0), sampleCount, 0) != null);
        }
        /// <summary>
        /// Method to retrieve correlation samples
        /// </summary>
        /// <param name="bGetMissingCounts">bool to retreive correlation tag present or not </param>
        /// <param name="nonEmptyGroups"></param>
        /// <param name="counts">Values retrieved</param>
        public void getCorrelationCounts(bool bGetMissingCounts, uint nonEmptyGroups, uint[] counts)
        {
            // The correlation sample session has finished. Gather the results.
            byte getMissingCounts = (byte)(bGetMissingCounts ? 1 : 0);
            uint groupBit = 1;
            for (ushort correlationOffset = 0; correlationOffset < 256; correlationOffset += 16)
            {
                if ((nonEmptyGroups & groupBit) != 0)
                {
                    PbMsgRfidBackDoor backDoorMessage = new PbMsgRfidBackDoor(
                        RfidCommandType.RBDC_GetCorrelationCounts,
                        getMissingCounts, correlationOffset, 0);
                    PbRspRfidBackdoorGetCorrelationCounts backDoorCorrelationSamples =
                        (PbRspRfidBackdoorGetCorrelationCounts)marshalMessage(backDoorMessage,
                        typeof(PbRspRfidBackdoorGetCorrelationCounts));
                    ushort[] correlationCounts = backDoorCorrelationSamples.correlationCounts;
                    MyDebug.Assert(backDoorCorrelationSamples.correlationOffset == correlationOffset,"Error in getCorrelationCounts" );
                    for (uint i = 0; i < correlationCounts.Length; i++)
                        counts[correlationOffset + i] += correlationCounts[i];
                }
                groupBit <<= 1;
            }
        }
        /// <summary>
        /// Method to control switch board through RS485.
        /// </summary>
        /// <param name="bSet">bool to set or clear the relay</param>
        /// <param name="RelaisNumber">byte number of the relais to drive  (1-8) ; 9 for all</param>
        public void sendSwitchCommand(byte bSet, byte RelaisNumber)
        {
            backDoor(RfidCommandType.RBDC_SendSwitchCommand, bSet, RelaisNumber, 0);
        }
        /// <summary>
        /// Methos for lock/unlock the door/drawer
        /// </summary>
        /// <param name="bOnOff">is true send Lock otherwise send unlock command</param>
        public bool LockDoor(bool bOnOff)
        {
            if (bOnOff)
            {
                PbMsgDrawerLock message = new PbMsgDrawerLock((ushort)DoorValue.DV_Master);
                return (marshalMessage(message, null) != null); // Success if response is not null.
            }
            else
            {
                PbMsgDrawerUnlock message = new PbMsgDrawerUnlock((ushort)DoorValue.DV_Master);
                return (marshalMessage(message, null) != null); // Success if response is not null.
            } 

        }
        /// <summary>
        /// Method to lock/Unlock the door
        /// </summary>
        /// <param name="door"> door to lock</param>
        /// <param name="bOnOff">lock or unlock</param>
        /// <returns></returns>
        public bool LockDoor(DoorValue door, bool bOnOff)
        {
            if (bOnOff)
            {
                PbMsgDrawerLock message = new PbMsgDrawerLock((ushort)door);
                return (marshalMessage(message, null) != null); // Success if response is not null.
            }
            else
            {
                PbMsgDrawerUnlock message = new PbMsgDrawerUnlock((ushort)door);
                return (marshalMessage(message, null) != null); // Success if response is not null.
            }

        }

        /// <summary>
        /// Method to set power of the light
        /// </summary>
        /// <param name="Duty">Power to set between 0 (off) and 300 max</param>
        public bool SetLightDuty(UInt16 Duty)
        {
            pbMsgSetLightDuty message = new pbMsgSetLightDuty(Duty);
            return (marshalMessage(message, null) != null); // Success if response is not null.
        }
        /// <summary>
        /// Method to load uid code for confirmation
        /// </summary>
        /// <param name="blockNumber">1 for MSB, 2 for LSB</param>
        /// <param name="mySerial"></param>
        public void setTagUidDigit(byte blockNumber, TagIdType mySerial)
        {
            ushort word1, word2;
            unsafe
            {
                if (blockNumber == 1)
                {
                    word1 = mySerial.word[3];
                    word2 = mySerial.word[2];
                }
                else
                {
                    word1 = mySerial.word[1];
                    word2 = mySerial.word[0];
                }
            }
            backDoor(RfidCommandType.RBDC_SetTagUidDigit, blockNumber, word1, word2);

        }       
        /// <summary>
        /// Function to start confirmation process
        /// </summary>
        /// <returns>true is succeed</returns>
        public bool startConfirmation()
        {
            return (backDoor(RfidCommandType.RBDC_StartConfirmation, 0, 0, 0) != null);
        }
        /// <summary>
        /// Function to end confirmation process
        /// </summary>
        /// <returns>true id succeed</returns>
        public bool endConfirmation()
        {
            return (backDoor(RfidCommandType.RBDC_EndConfirmation, 0, 0, 0) != null);
        }
        /// <summary>
        /// Function to send confirmation command of the loaded UID
        /// </summary>
        /// <param name="nbDigits">Number of digits to check</param>
        /// <returns>True is command succed - result will be notified through asynchronous operation</returns>
        public bool confirmLoadedUID(byte nbDigits)
        {
            return (backDoor(RfidCommandType.RBDC_ConfirmUID, (byte)(nbDigits-1), 0, 0) != null);
        }

        /// <summary>
        /// Set Infra red sensor On or Off
        /// </summary>
        /// <param name="bStart">Value to put on or off</param>
        /// <returns>true if succeed</returns>
        public bool setInfraRedSensor(bool bStart)
        {
            return (backDoor(RfidCommandType.RBDC_SetAlarmInfraOn, (byte)(bStart ? 1 : 0), 0, 0) != null);
        }
        /// <summary>
        /// Function to switch on/off the field
        /// </summary>
        /// <param name="bStart">true to on, false to stop</param>
        /// <returns>true if function succeed </returns>
        public bool setAntenna(bool bStart)
        {
            return (backDoor(RfidCommandType.RBDC_SetAntennaOn, (byte)(bStart ? 1 : 0), 0, 0) != null);
        }
        /// <summary>
        /// Function to request a synchronisation
        /// </summary>
        /// <returns>true id succeed</returns>
        public bool sendSyncPulse()
        {
            return (backDoor(RfidCommandType.RBDC_SendSynchronization, 0, 0, 0) != null);
        }
        /// <summary>
        /// Function to send basic order
        /// </summary>
        /// <param name="theCmd">command to lanch</param>
        /// <param name="Rcor">N/A</param>
        /// <returns>true id command succeed</returns>
        public byte sendCommand(byte theCmd, out ushort Rcor)
        {
            Rcor = 0;
            PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
               RfidCommandType.RBDC_SendLowLevelOrder, theCmd, 0, 0);

            if (response == null) return 0;
            Rcor = response.responseWord1;
            return response.responseByte1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bStart"></param>
        /// <returns></returns>
        public bool calibrate(bool bStart)
        {
            return (backDoor(RfidCommandType.RBDC_Calibrate, (byte)(bStart ? 1 : 0), 0, 0) != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="startingIndex"></param>
        /// <param name="lockSignal"></param>
        /// <param name="getVoltage"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public bool getCarrierSignal(ushort startingIndex, bool lockSignal, bool getVoltage,
            sbyte[] values)
        {
            PbRspRfidBackdoorGetCarrierSignal response =
                (PbRspRfidBackdoorGetCarrierSignal)backDoor(
                RfidCommandType.RBDC_GetCarrierSignal,
                typeof(PbRspRfidBackdoorGetCarrierSignal),
                (byte)(lockSignal ? 1 : 0), startingIndex, (ushort)(getVoltage ? 1 : 0));

            if (response != null)
            {
                for (uint i = 0; i < 32; i++)
                    values[i] = response.signalValues[i];
                return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool getCorrelationCounts()
        {
            return (backDoor(RfidCommandType.RBDC_GetCorrelationCounts, 0, 0, 0) != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strSerialNumber"></param>
        /// <returns></returns>
        public bool setSerialNumber(string strSerialNumber)
        {
            bool ret = false;

            try
            {
                if (strSerialNumber.Length != 8) return ret;

                //Put serial as Bob's cardinal way

                string strBigEndian = strSerialNumber.Substring(6, 2) + strSerialNumber.Substring(4, 2) + strSerialNumber.Substring(2, 2) + strSerialNumber.Substring(0, 2);

                uint newSerialNumber = Convert.ToUInt32(strBigEndian, 16); ;
                backDoor(RfidCommandType.RBDC_SetDeviceSerialNumber, 0,
                    (ushort)(newSerialNumber & 0xFFFF), (ushort)((newSerialNumber >> 16) & 0xFFFF));
                ret = true;
            }
            catch { }
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public uint getSerialNumber()
        {
            PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
                RfidCommandType.RBDC_GetDeviceSerialNumber,
                typeof(PbRspRfidBackDoor), 0, 0, 0);

            if (response != null)
                return response.responseLong1;
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bEnableHalfBridge"></param>
        /// <param name="fullBridgeDutyCycle"></param>
        /// <param name="halfBridgeDutyCycle"></param>
        public void getBridgeState(out bool bEnableHalfBridge, out uint fullBridgeDutyCycle, out uint halfBridgeDutyCycle)
        {
            PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
                RfidCommandType.RBDC_GetBridgeState,
                typeof(PbRspRfidBackDoor), 0, 0, 0);
            bEnableHalfBridge = ((response.responseByte1 & 1) != 0);
            fullBridgeDutyCycle = response.responseWord1;
            halfBridgeDutyCycle = response.responseLong1;
            if (fullBridgeDutyCycle == 0) // Previous versions of firmware returned only the half-bridge duty cycle.
                fullBridgeDutyCycle = DeviceRfidBoard.MAX_DutyCycle;
        }
        /// <summary>
        /// 
        /// </summary>
        public void saveBridgeDutyCyclesToROM()        {
            
                backDoor(RfidCommandType.RBDC_SaveBridgeDutyCyclesToRom, 0, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="period"></param>
        /// <param name="Vant"></param>
        public void GetCarrierFrequency(out UInt16 period, out UInt16 Vant)
        {
            PbRspRfidBackDoor response = (PbRspRfidBackDoor)backDoor(
                RfidCommandType.RBDC_GetCarrierFrequency,
                typeof(PbRspRfidBackDoor), 0, 0, 0);
            period = response.responseWord1;
            Vant = (UInt16)response.responseLong1;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool FindGoodFrequency()
        {
            return (backDoor(RfidCommandType.RBDC_FindGoodFrequency, 0, 0, 0) != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IncreaseCarrierFrequency()
        {
            return (backDoor(RfidCommandType.RBDC_IncreaseCarrierFrequency, 0, 0, 0) != null);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool DecreaseCarrierFrequency()
        {
            return (backDoor(RfidCommandType.RBDC_DecreaseCarrierFrequency, 0, 0, 0) != null);
        }

        static UInt16[] long2doubleInt(UInt32 a)
        {
            UInt16 MSB = (UInt16)(a >> 16);
            UInt16 LSB = (UInt16)(a & 0xFFFF);
            return new UInt16[] { MSB, LSB };
        }

        public bool WriteBlock(byte Block, UInt32 data)
        {
            UInt16[] val = long2doubleInt(data);
            return (backDoor(RfidCommandType.RBDC_WriteBlock, Block, val[0], val[1]) != null);
            // notify return asynchronously in AsyncEventType.PBET_BackDoorInfo
        }

        //Confirmation full mem
        public bool ConfirmationFullMemory(byte nbDigitToConfirm, byte[] TagDigitToConfirm)
        {
            PbMsgRfidBackDoorConfirmationFullMem message = new PbMsgRfidBackDoorConfirmationFullMem(nbDigitToConfirm, TagDigitToConfirm);
            return (marshalMessage(message, null) != null);
        }

               
    }
}
