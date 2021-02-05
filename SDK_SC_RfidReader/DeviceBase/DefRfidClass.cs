using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace SDK_SC_RfidReader.DeviceBase
{
    #region Enumeration
        /// <summary>
        ///  Enumeration of each response value possible for a request of starting a scan.
        /// </summary>
        /// 
        public enum DoorValue
        {
            /// <summary>
            /// Value for door master
            /// </summary>
            DV_Master = 0x00,
            /// <summary>
            /// Value for Door Slave
            /// </summary>
            DV_Slave = 0x01,
        }
    /// <summary>
    /// Enumeration of Scan resonse
    /// </summary>
        public enum RespScan
        {
            /// <summary>
            ///  Value RS_ScanSucceed if scan ending correctly
            /// </summary>
            RS_ScanSucceed = 0x00,
            /// <summary>
            ///  Value RS_FailedToConnect if no connection are valid or not connection are found
            /// </summary>
            RS_FailedToConnect = 0x01,
            /// <summary>
            ///  Value RS_FailedToStartScan occurs when bad voltage detect, on the board, board not ready,
            /// </summary>
            RS_FailedToStartScan = 0x02,
            /// <summary>
            /// Value RS_FinishScanTimeOut occurs when time to inventory is too long.This time is define in the firmware of the board
            /// </summary>
            RS_FinishScanTimeOut = 0x03,
            /// <summary>
            /// Values  RS_ErrorReadUID occurs when a Tag present in the field send recursively a bad uid number
            /// This value mentionned that the scan cannot finish normally due to a tag error 
            /// </summary>
            RS_ErrorDuringScan = 0x04,
            /// <summary>
            /// Value to enable next reader action
            /// </summary>
            RS_ReaderReady = 0x05,
            /// <summary>
            /// Value to disable reader, can be test to see if reader available for next action
            /// </summary>
            RS_ReaderNotReady = 0x06,
           

        }

        /// <summary>
        /// Enumeration of the message type, As only one channel is possible this value is unique.
        /// </summary>
        public enum PbMessageType
        {
            /// <summary>
            /// None value
            /// </summary>
            ASYNC_None = 0x00,
            /// <summary>
            /// Value to define a classic asynchronous event on the communication channel
            /// </summary>
            ASYNC_MSG_FROM_PB = 9,
        };

        /// <summary>
        /// Enumeration of all the avalaible command.
        /// </summary>
        public enum CommandType
        {
            /// <summary>
            /// None value
            /// </summary>
            PBCMD_None = 0x00,
            /// <summary>
            /// EX command for all the command that are asynchronous 
            /// response of startScan, tagAdded, TagRemove,EndScan
            /// </summary>
            PBCMD_AsyncEvent = 0x5845,     // "EX"
            /// <summary>
            /// HW command for discovering new board
            /// </summary>
            PBCMD_HelloWorld = 0x5748,     // "HW"
            /// <summary>
            /// BD command use for send all rfid Process , search for noise, adjust noise threshold
            /// </summary>
            PBCMD_RfidCommand = 0x4442,     // "BD" 
            /// <summary>
            /// RC command to request a full inventory
            /// </summary>
            PBCMD_RequestTagCheck = 0x4352, // "RC" Request Tag Check   
            /// <summary>
            /// RT command for recovery in non asynchronous operation , uid of the next stored tag
            /// </summary>
            PBCMD_RequestNextTag = 0x5452,  // "RT" Request Next Tag
            /// <summary>
            /// TF command for recovery the failure error when a tag error is detected.
            /// </summary>
            PBMCD_GetTagFailure = 0x4654, // "TF" Get Tag Failure
            /// <summary>
            /// ST command for recover board State ans Status.
            /// </summary>
            PBCMD_GetStatus = 0x5453, // "ST"
            /// <summary>
            /// Request stop scan
            /// </summary>
            PBCMD_Control = 0x4F43, // "CO"
            /// <summary>
            /// Request Back door to diagnostic functionality.
            /// </summary>
            PBCMD_BackDoor = 0x4442, // "BD" 
            /// <summary>
            /// Request for lock the door / drawer
            /// </summary>
            PBCMD_DrawerLock = 0x4C44, // "DL"
            /// <summary>
            /// REquest to unlock door /drawer
            /// </summary>
            PBCMD_DrawerUnlock = 0x5544, // "DU"
            /// <summary>
            /// REquest to set Power of the light
            /// </summary>
            PBCMD_DrawerSetLight = 0x5344, //
           
        };

        /// <summary>
        /// Enumeration of all the kind of command.
        /// </summary>
        public enum MessageIdType
        {
            /// <summary>
            /// None value
            /// </summary>
            PBMID_None = 0x00,
            /// <summary>
            /// Value  PBMID_CommandMessage for all command message with a response structure define
            /// </summary>
            PBMID_CommandMessage = 0x02,
            /// <summary>
            /// value PBMID_Acknowledge for all messages which return a Acknowlegment.
            /// </summary>
            PBMID_Acknowledge = 0x03,
            /// <summary>
            /// value PBMID_Completion  for all messages which return a Completion
            /// </summary>
            PBMID_Completion = 0x05,
        };

        /// <summary>
        /// Enumeration of all the Different Type of Device.
        /// </summary>
        public enum DeviceTypeMajorType
        {
            /// <summary>
            /// Value when device Unknow
            /// </summary>
            DEVICE_UNKNOWN = 0,
            /// <summary>
            /// Value for board in Diamond reader 1 module
            /// </summary>
            DEVICE_RFID_DIAMOND_SMART_BOX = 1,
            /// <summary>
            /// Value for board in Jewellery reader 1 module
            /// </summary>
            DEVICE_RFID_JEWELLERY_CABINET = 2,
            /// <summary>
            /// Value for board in Diamond reader 2 modules
            /// </summary>
            DEVICE_RFID_MEDICAL_CABINET = 3,
            /// <summary>
            /// Value for Narco Box
            /// </summary>
            DEVICE_RFID_DIAMOND_SMART_BOX_V2 = 4,
            /// <summary>
            /// Value for Mono_Axe
            /// </summary>
            DEVICE_RFID_MONO_AXE_READER = 5,
            /// <summary>
            /// Value for 3D shelves 55x65cm
            /// </summary>
            DEVICE_RFID_FLAT_3D_SHELVES = 6,
            /// <summary>
            /// Value 
            /// </summary>
            DEVICE_RFID_DIAMOND_SAS = 7,

            DEVICE_RFID_FRIDGE_1D = 8,

            DEVICE_RFID_MINI_SAS = 9,

            DEVICE_RFID_BLOOD_FRIDGE = 10,
           
        };

       

        /// <summary>
        /// Enumeration of Minor type , Declare for future use.
        /// </summary>
        public enum DeviceTypeMinorType
        {
            /// <summary>
            /// None value
            /// </summary>
            DT_None = 0x00,
            /// <summary>
            /// Value for Rfid device
            /// </summary>
            DT_RfidDeviceTypeMinor = 1,
        };

        /// <summary>
        /// Enumeration of all Asynchronous event.
        /// </summary>
        public enum AsyncEventType
        {
            /// <summary>
            /// 
            /// </summary>
            PBET_DrawerOpened = 0x00, // Drawer opened
            /// <summary>
            /// 
            /// </summary>
            PBET_DrawerClosed = 0x01, // Drawer closed

            /// <summary>
            /// Value unknown AsyncEvent
            /// </summary>
            PBET_ErrorAsync = 0x66,
            /// <summary>
            /// Value : 0x3B :  Unexpected device reset, e.g. watchdog reset.
            /// </summary>
            PBET_DeviceResetAbnormally = 0x3B, 
            /// <summary>
            /// Value : 0x3C :  Scan started, finished, or was canceled. see PBAE_RfidScanStateChanged.
            /// </summary>
            PBET_RfidScanStateChanged = 0x3C,
            /// <summary>
            /// Value 0x3E :  An RFID tag was added.
            /// </summary>
            //PBET_TagAdded = 0x3E, 
            /// <summary>
            /// Value 0x3F :  An RFID tag was removed.
            /// </summary>
            //PBET_TagRemoved = 0x3F, 
            /// <summary>
            /// Value 0xBD :  Back Door information response.
            /// </summary>
            PBET_TagAddedR8       = 0x3E,  // Tag R8
            PBET_TagAddedSPCE2_RO = 0x3F,  // Tag SPCE2 Read Only ( not rewrite - first digit = 3)
            PBET_TagAddedSPCE2_RW = 0x3D,  // Tag SPCE2 Read Write( rewrite - first digit != 3)

            PBET_TagFullMem_R8 = 0x50,  //notify full Memory For R8
            PBET_TagFullMem_RO = 0x51,  //notify full Memory For SPC E2 blank ( begin by 3)
            PBET_TagFullMem_RW = 0x52,  //notify full Memory For SPC E2 Rewrite ( begin by 2)
            PBET_TagFullMem_DEC = 0x53,  //notify full Memory For SPC E2 Decimal format ( begin by 1)

           // PBET_TagFullMem = 0x50,  //notify full Memory

            PBET_BackDoorInfo = 0xBD,
            /// <summary>
            /// Value 0xFF : power is cut or usb cable is removed
            /// </summary>  
            PBET_PowerOff = 0xFF,
            /// <summary>
            /// Value 0xEE : power is ON
            /// </summary> 
            PBET_PowerOn = 0xEE,
            /// <summary>
            /// Value 0x01 : DoorClosed
            /// </summary>
            PBET_DoorClosed = 0x01,
            /// <summary>
            /// Value 0x00 : Door is open
            /// </summary>
            PBET_DoorOpened = 0x00,
            /// <summary>
            /// Value Ox40 if infra red event
            /// </summary>
            PBET_AlarmInfra = 0x40,
            /// <summary>
            /// Value Ox41 if Move event
            /// </summary>
            PBET_AlarmMove = 0x41, 

        };

        /// <summary>
        /// Enumeration of all Response Code possible.
        /// </summary>
        public enum ResponseCodeType
        {
            /// <summary>
            /// Value  PBRC_SUCCESS :  operation was successful
            /// </summary>
            PBRC_SUCCESS = 0x00,  
            /// <summary>
            /// Value PBRC_FAILURE :  general failure
            /// </summary>
            PBRC_FAILURE = 0x01,  
            /// <summary>
            /// Value  PBRC_TIMEOUT : general timeout
            /// </summary>
            PBRC_TIMEOUT = 0x02, 
            /// <summary>
            /// Value PBRC_ILLEGAL_VALUE :  illegal value for a parameter was given
            /// </summary>
            PBRC_ILLEGAL_VALUE = 0x03, 
            /// <summary>
            /// Value PBRC_ILLEGAL_NUM_ARGS :  wrong number of arguments
            /// </summary>
            PBRC_ILLEGAL_NUM_ARGS = 0x04,  
            /// <summary>
            /// Value PBRC_BOOTLOAD_MODE : Device in BootLoader mode.
            /// </summary>
            PBRC_BOOTLOAD_MODE = 0x05,  
        };

        /// <summary>
        /// Enumeration used for systemStatus field for various response messages.
        /// </summary>
        public enum SystemStatusFlags 
        {
            /// <summary>
            /// None value
            /// </summary>
            SSF_None = 0x00,
            /// <summary>
            /// Value  SSF_12VoltsFailure : Set when 12V supply on board is faulty.
            /// </summary>
            SSF_12VoltsFailure = 0x01, 
            /// <summary>
            /// Value SSF_AntennaNotConnected : Set when antenna is not connected.
            /// </summary>
            SSF_AntennaNotConnected = 0x02,
            /// <summary>
            /// Value SSF_5VoltsFailure : Set when 5V supply on board is faulty.
            /// </summary>
            SSF_5VoltsFailure = 0x04, 
        };

        /// <summary>
        /// Enumeration used for scanStatus field for various response messages.
        /// </summary>
        public enum ScanStatusFlags 
        {
            /// <summary>
            /// None value
            /// </summary>
            SSF_None = 0x00,
            /// <summary>
            /// Value SSF_ScanningTagsNow : Board in scan process.
            /// </summary>
            SSF_ScanningTagsNow = 0x01,
            /// <summary>
            /// Value  SSF_TagListHasChanged : Stored list has changed.
            /// </summary>
            SSF_TagListHasChanged = 0x02,
            /// <summary>
            /// Value :  SSF_TagScanWasCanceled : Scan canceled by the sender.
            /// </summary>
            SSF_TagScanWasCanceled = 0x04,
            /// <summary>
            /// Value SSF_TagScanRequired :  A tag scan is required because power-up or door was opened.
            /// </summary>
            SSF_TagScanRequired = 0x08, 
            /// <summary>
            /// Value SSF_TooManyTagsDetected : The number of tags exceeds the firmware limit.
            /// </summary>
            SSF_TooManyTagsDetected = 0x10, 
        };

        /// <summary>
        /// Enumeration for statistic message.
        /// </summary>
        public enum ControlModeFlags
        {
            /// <summary>
            /// None value
            /// </summary>
            CMF_None = 0x00,
            /// <summary>
            /// Value CMF_EnableStatsAsyncEvent : Enable asynchronous message on statistics change.
            /// </summary>
            CMF_EnableStatsAsyncEvent = 0x01, 
        };

        /// <summary>
        /// Enumeration for Control scan.
        /// </summary>
        public enum ControlControlFlags
        {
            /// <summary>
            /// None value
            /// </summary>
            CCF_None = 0x00,
            /// <summary>
            /// Value  CCF_StopScan : Set to abort current RFID scan.
            /// </summary>
            CCF_StopScan = 0x01, 
        };

        /// <summary>
        /// Enumeration for control the behaviour of the scan.
        /// </summary>
        public enum TagScanModeFlags
        {
            /// <summary>
            /// None value
            /// </summary>
            TSMF_None = 0x00,
            /// <summary>
            /// Value TSMF_ClearTagListHasChangedFlag :  If set, clear the Tag-List-Has-Changed flag.
            /// </summary>
            TSMF_ClearTagListHasChangedFlag = 0x01, 
            /// <summary>
            /// Value TSMF_SendAsyncMessages : If set, send asynchronous events for tag changes.
            /// </summary>
            TSMF_SendAsyncMessages = 0x02,
            /// <summary>
            /// Value TSMF_UnlockTagAllAxis :  If set, perform Reverse inventory.
            /// </summary>
            TSMF_UnlockTagAllAxis = 0x04, 
            /// <summary>
            /// Value TSMF_UseKR : If set , perform san with multiple field and multiple synchronization time
            /// </summary>
            TSMF_UseKR = 0x08,  
        };

        /// <summary>
        /// Enumeration  used for PbRspRfidGetNextTag.tagInfo.
        /// </summary>
        public enum TagInfoFlags 
        {
            /// <summary>
            /// None value
            /// </summary>
            TIF_None = 0x00,
            /// <summary>
            /// Value TIF_ReachedEndOfTagList : Indicates end of tag list reached. If scan pending, try again.
            /// </summary>
            TIF_ReachedEndOfTagList = 0x40,  
        };

        /// <summary>
        /// Enumeration used for PBAE_RfidScanStateChanged.scanStatus.
        /// </summary>
        public enum ScanStatusType  
        {
            /// <summary>
            /// Value  SS_TagScanStarted :  tag scan has begun.
            /// </summary>
            SS_TagScanStarted = 0, 
            /// <summary>
            /// Value SS_TagScanCompleted :  tag scan has completed successfully.
            /// </summary>
            SS_TagScanCompleted = 1, 
            /// <summary>
            /// Value SS_TagScanCanceledByHost : scan canceled by host request before completion.
            /// </summary>
            SS_TagScanCanceledByHost = 2, 
            /// <summary>
            /// Value SS_TagScanCanceledByDoorOpen : scan canceled before completion due to door open.
            /// </summary>
            SS_TagScanCanceledByDoorOpen = 3, 

            /// <summary>
            /// Value SS_TagScanFailedByUnrecoverableError :  scan aborted due to unrecoverable errors.
            /// </summary>
            SS_TagScanFailedByUnrecoverableError = 4,
            /// <summary>
            /// Value SS_TagScanSendPourcent : Send notify % during scan
            /// </summary>
            SS_TagScanSendPourcent = 5,
            /// <summary>
            /// Value SS_TagConfirmation : Send notify result of confirmation
            /// </summary>
            SS_TagConfirmation = 6,
        };

        /// <summary>
        /// Enumeration used for PBAE_RfidScanStateChanged.info when scanStatus is SS_TagScanFailedByUnrecoverableError.
        /// </summary>
        public enum UnrecoverableErrorType 
        {
            /// <summary>
            /// None value
            /// </summary>
            UE_None = 0x00,
            /// <summary>
            /// Value UE_ExcessiveNoise :  Too much noise in tag response; possibly RF interference.
            /// </summary>
            UE_ExcessiveNoise = 1, 
            /// <summary>
            /// Value UE_12VPowerSupply : 12V power supply has failed.
            /// </summary>
            UE_12VPowerSupply = 2, 
            /// <summary>
            /// Value UE_5VPowerSupply :  5V power supply has failed.
            /// </summary>
            UE_5VPowerSupply = 3, 
            /// <summary>
            /// Value UE_AntennaRemoved :  Antenna has been removed.
            /// </summary>
            UE_AntennaRemoved = 4, 
            /// <summary>
            /// Value UE_PersistentFailures :  Tag scan continues to fail, with no recognized reason.
            /// </summary>
            UE_PersistentFailures = 5, 
            /// <summary>
            /// Value UE_BrokenTagLock :  A tag consistently ignored its lock-tags command.
            /// </summary>
            UE_BrokenTagLock = 6, 
        };

        /// <summary>
        /// Enumeration  used for Tag Tester.
        /// </summary>
        public enum SetAntennaPolarityAndPowerType 
        {
            /// <summary>
            /// None value
            /// </summary>
            SAPP_None = 0x00,
            /// <summary>
            /// Value SAPP_EnableFeature :  Set to enable feature, else other bits ignored.
            /// </summary>
            SAPP_EnableFeature = 0x01, 
            /// <summary>
            /// Value  SAPP_AntennaPolarityReversed :  Set to reverse antenna polarity, else normal polarity.
            /// </summary>
            SAPP_AntennaPolarityReversed = 0x02,
            /// <summary>
            /// Value SAPP_SelectHighPower : Set to select >12V for drive board; clear to select 12V.
            /// </summary>
            SAPP_SelectHighPower = 0x04, 
        };

        /// <summary>
        ///  Enumeration  used for Tag Tester.
        /// </summary>
        public enum PresenceTestFlagsType 
        {
            /// <summary>
            /// None value
            /// </summary>
            PTF_None = 0x00,
            /// <summary>
            /// Value PTF_EnableTest : Set to enable feature, else other bits ignored.
            /// </summary>
            PTF_EnableTest = 0x01, 
            /// <summary>
            /// Value PTF_GetPhaseShift : Set to calculate and return phase shift.
            /// </summary>
            PTF_GetPhaseShift = 0x02, 
        };

        /// <summary>
        /// Enumeration for PBAE_DeviceResetAbnormally.resetReason.
        /// </summary>
        public enum ResetReasonType 
        {
            /// <summary>
            /// Value RR_FirmwareReset : The device was reset by firmware request.
            /// </summary>
            RR_FirmwareReset, 
            /// <summary>
            /// Value RR_PowerOn : The device has been powered on.
            /// </summary>
            RR_PowerOn, 
            /// <summary>
            /// Value RR_BrownOut : A brown-out reset has occurred.
            /// </summary>
            RR_BrownOut, 
            /// <summary>
            /// Value RR_MasterClear : Reset via the /MCLR processor pin.
            /// </summary>
            RR_MasterClear, 
            /// <summary>
            /// Value RR_Watchdog : A watchdog timeout reset has occurred.
            /// </summary>
            RR_Watchdog, 
            /// <summary>
            /// Value RR_IllegalOpcodeOrAddressMode :  An illegal opcode or an illegal address mode caused reset.
            /// </summary>
            RR_IllegalOpcodeOrAddressMode, 
        };

        /// <summary>
        /// Enumeration used for PBAE_RfidScanStateChanged.info when scanStatus is SS_TagScanCompleted.
        /// </summary>
        public enum TagScanCompleteInfoFlagsType 
        {
            /// <summary>
            /// None value
            /// </summary>
            TSCF_None = 0x00,
            /// <summary>
            /// Value TSCF_ChangesDetected :  Changes in the tag list were detected.
            /// </summary>
            TSCF_ChangesDetected = 0x01,
            /// <summary>
            /// Value TSCF : _TooManyTags : Too many tags were detected.
            /// </summary>
            TSCF_TooManyTags = 0x02, 
        };

        /// <summary>
        /// Enumeration used for PbRspRfidGetTagFailure.failureType.
        /// </summary>
        public enum TagFailureEnumType 
        {
            /// <summary>
            /// None value
            /// </summary>
            TFFT_None = 0x00,
            /// <summary>
            /// Value TFFT_MissingDigit : A digit is missing in the Tag Code.
            /// </summary>
            TFFT_MissingDigit = 0x01,
            /// <summary>
            /// Value TFFT_ChecksumFailure : The Uid hasn't a valid CRC
            /// </summary>
            TFFT_ChecksumFailure = 0x02,
        };
    
        /// <summary>
        /// Enumeration user for CommandType.PBCMD_rfidCommand.
        /// </summary>
         public enum RfidCommandType 
         {
             /// <summary>
             /// None value
             /// </summary>
             RBDC_None = 0x00,
             /// <summary>
             /// Value RBDC_Calibrate : Control calibration for the RFID antenna.
             /// </summary>
             RBDC_Calibrate = 0x01, 
             /// <summary>
             /// Value RBDC_TagPresenceTest : Do a tag-presence test.
             /// </summary>
             RBDC_TagPresenceTest = 0x02, 
             /// <summary>
             /// Value RBDC_SampleCorrelation : Sample a series of correlation values.
             /// </summary>
             RBDC_SampleCorrelation = 0x03,
             /// <summary>
             /// Value RBDC_ReadDigitTest : Do a read-digit test.
             /// </summary>
             RBDC_ReadDigitTest = 0x04, 
             /// <summary>
             /// Value RBDC_NoModuNoiseTest : Do a no-modulation noise test.
             /// </summary>
             RBDC_NoModuNoiseTest = 0x05,
             /// <summary>
             /// Value RBDC_EnableCorrelationEvent : Enable an asynchronous event to report correlation value.
             /// </summary>
             RBDC_EnableCorrelationEvent = 0x06,
             /// <summary>
             /// Value RBDC_ResetRfidStatistics : Clear all fields in the RFID statististics structure.
             /// </summary>
             RBDC_ResetRfidStatistics = 0x07, 
             /// <summary>
             /// Value  Clear all fields in the Communication statististics structure.
             /// </summary>
             RBDC_ResetCommunicationStatistics = 0x08, 
             /// <summary>
             /// Value RBDC_SetCorrelationThreshold : Set the correlation threshold to the specified value.
             /// </summary>
             RBDC_SetCorrelationThreshold = 0x09, 
             /// <summary>
             /// Value RBDC_GetCorrelationCounts_Retired : Get the correlation sample counts.
             /// </summary>
             RBDC_GetCorrelationCounts_Retired = 0x0A, 
             /// <summary>
             /// Value RBDC_GetCorrelationThreshold : Get the current correlation threshold.
             /// </summary>
             RBDC_GetCorrelationThreshold = 0x0B, 
             /// <summary>
             /// Value RBDC_SaveCorrelationThresholdToRom : Save the current correlation threshold to Flash ROM.
             /// </summary>
             RBDC_SaveCorrelationThresholdToRom = 0x0C,
             /// <summary>
             ///  Value RBDC_GetStatisticsCommunication :  Get the Communication statistics.
             /// </summary>
             RBDC_GetStatisticsPyxibus = 0x0D, 
             /// <summary>
             /// Value RBDC_GetStatisticsRfid : Get the RFID statistics.
             /// </summary>
             RBDC_GetStatisticsRfid = 0x0E, 
             /// <summary>
             /// Value  RBDC_SetBridgeState : Enable/Disable half bridge and set duty cycles.
             /// </summary>
             RBDC_SetBridgeState = 0x0F,
             /// <summary>
             /// RBDC_ClearTagListBeforeTagScan : Clear the tag list for the next tag scan.
             /// </summary>
             RBDC_ClearTagListBeforeTagScan = 0x10, 
             /// <summary>
             /// Value RBDC_TagPhaseTest : Do a tag-phase test.
             /// </summary>
             RBDC_TagPhaseTest = 0x11, 
             /// <summary>
             /// Value RBDC_GetTagResponseSignal : Return the tag response signal A/D values.
             /// </summary>
             RBDC_GetTagResponseSignal = 0x12, 
             /// <summary>
             /// Value RBDC_GetCarrierSignal : Return the carrier voltage and current A/D values.
             /// </summary>
             RBDC_GetCarrierSignal = 0x13, 
             /// <summary>
             /// Value RBDC_SetDeviceSerialNumber : Change the device serial number in ROM; must reset device to use new number.
             /// </summary>
             RBDC_SetDeviceSerialNumber = 0x14,
             /// <summary>
             /// Value RBDC_SetDeadBand : Change the dead-band for the PWM transitions.
             /// </summary>
             RBDC_SetDeadBand = 0x15, 
             /// <summary>
             /// Value RBDC_GetDeviceSerialNumber : Read the device serial number from ROM.
             /// </summary>
             RBDC_GetDeviceSerialNumber = 0x16, 
             /// <summary>
             /// Value RBDC_GetHalfBridgeDutyCycle_Retired : Retired: Get half bridge duty cycle; replaced with RBDC_GetBridgeState.
             /// </summary>
             RBDC_GetHalfBridgeDutyCycle_Retired = 0x17, 
             /// <summary>
             /// Value RBDC_SaveBridgeDutyCyclesToRom : Save the full- and half-bridge duty cycles to ROM.
             /// </summary>
             RBDC_SaveBridgeDutyCyclesToRom = 0x18, 
             /// <summary>
             /// Value RBDC_GetCorrelationCounts : Get correlation counts from a correlation sample session.
             /// </summary>
             RBDC_GetCorrelationCounts = 0x19, 
             /// <summary>
             /// Value  RBDC_SetAntennaPolarityAndPower : For Tag Tester; sets antenna polarity and selects 12V power supply.
             /// </summary>
             RBDC_SetAntennaPolarityAndPower = 0x1A, 
             /// <summary>
             /// Value RBDC_GetBridgeState :  Get half/full bridge state and duty cycles.
             /// </summary>
             RBDC_GetBridgeState = 0x1B, 
             /// <summary>
             /// Value RBDC_StartTagCharacterization : Collect phase shift information at specified duty cycle.
             /// </summary>
             RBDC_StartTagCharacterization = 0x1C,
             /// <summary>
             /// Value RBDC_GetTagCharacterizationResults  :  Get results of RBDC_StartTagCharacterization.
             /// </summary>
             RBDC_GetTagCharacterizationResults = 0x1D, 
             /// <summary>
             /// Value RBDC_SetAntennaOn : Set power on antenna
             /// </summary>
             RBDC_SetAntennaOn = 0x20,
             /// <summary>
             /// Value RBDC_SendSynchronization : Send synchronization command
             /// </summary>
             RBDC_SendSynchronization = 0x21, 
             /// <summary>
             /// Value RBDC_SendLowLevelOrder : Send  low level  basic command
             /// </summary>
             RBDC_SendLowLevelOrder = 0x22,
             /// <summary>
             /// Value RBDC_SetSynchronisationTime : send a synchronistation
             /// </summary>
             RBDC_SetSynchronisationTime = 0x23, // set synchro duration
             /// <summary>
             /// value RBDC_SetTagUidDigit : Load uid digit to confirm
             /// </summary>
             RBDC_SetTagUidDigit = 0x24, // set Tag digit
             /// <summary>
             /// Value  RBDC_SendUidConfirmation : Request confirmation of the loaded uid
             /// </summary>
             RBDC_SendUidConfirmation = 0x25, // Send uid confirmation order
             /// <summary>
             /// value  RBDC_SetModulationTime : request for change modulation time
             /// </summary>
             RBDC_SetModulationTime = 0x26, // set nb TC for modulation
             /// <summary>
             /// value RBDC_SetAckTime : request for change aknowledge time
             /// </summary>
             RBDC_SetAckTime = 0x27, // send nb TC ack
             /// <summary>
             /// Value RBDC_SendSwitchCommand : Send  command on RS485 for drive relay board
             /// </summary>
             RBDC_SendSwitchCommand = 0x28,  //command to control switch board through RS485
             /// <summary>
             /// value RBDC_GetSupply12V : recover value of the 12V
             /// </summary>
             RBDC_GetSupply12V = 0x29,  //command to recover 12v value
             /// <summary>
             /// Value RBDC_SampleCorrelationSimple to sample for wit or wothout response
             /// </summary>             
             RBDC_SampleCorrelationSimple = 0x2a,
             /// <summary>
             /// value RBDC_StartConfirmation : begin uid confirmation process
             /// </summary>
             RBDC_StartConfirmation = 0x30,
             /// <summary>
             /// value  RBDC_EndConfirmation : end uid confirmation process
             /// </summary>
             RBDC_EndConfirmation = 0x31,
             /// <summary>
             ///  value RBDC_ConfirmUID : Send confirmation of loaded digit
             /// </summary>
             RBDC_ConfirmUID = 0x32,
             /// <summary>
             /// 
             /// </summary>
             RBDC_GetCarrierFrequency = 0x34,
             /// <summary>
             /// 
             /// </summary>
             RBDC_IncreaseCarrierFrequency = 0x35,
             /// <summary>
             /// 
             /// </summary>
             RBDC_DecreaseCarrierFrequency = 0x36,
             /// <summary>
             /// 
             /// </summary>
             RBDC_FindGoodFrequency = 0x37,

             /// <summary>
             /// Value   RBDC_SetAlarmInfaOn : Switch On/Off infra red sensor
             /// </summary>
             RBDC_SetAlarmInfraOn = 0x38,

             RBDC_WriteBlock = 0x2B,
             RDBC_GetNumberMaxChannel = 0x2C,
             RBDC_ConfirmUIDFullMem = 0x2D,
         };
         
    /// <summary>
    /// Enumeration for low order level
    /// </summary>
        public enum LowlevelBasicOrder // For RBDC_SendLowLevelOrder
        {
            /// <summary>
            /// Send KO order
            /// </summary>
            K0 = 0x00,
            /// <summary>
            /// Send K1 order
            /// </summary>
            K1 = 0x01,
            /// <summary>
            /// Send K2 order
            /// </summary>
            K2 = 0x02,
            /// <summary>
            /// Send K3 order
            /// </summary>
            K3 = 0x03,
            /// <summary>
            /// Send K4 order
            /// </summary>
            K4 = 0x04,
            /// <summary>
            /// Send K5 order
            /// </summary>
            K5 = 0x05,
            /// <summary>
            /// Send K6 order
            /// </summary>
            K6 = 0x06,
            /// <summary>
            /// Send K7 order
            /// </summary>
            K7 = 0x07,
            /// <summary>
            /// Send unlock order
            /// </summary>
            KD = 0x0B,
            /// <summary>
            /// Send Test of presence
            /// </summary>
            KZ   = 0x0A,
            KZ_SPCE2 = 0x0D,
            /// <summary>
            /// Send Go down index order
            /// </summary>
            KR = 0x09,
            /// <summary>
            /// Send blocks command
            /// </summary>
            KB = 0x08,

            KL = 0x0C,
            
        };

    /// <summary>
    /// Enumeration for BackDoor Info request.
    /// </summary>
        public enum BackDoorEventType // For AsyncEventType.PBET_BackDoorInfo.
        {
            /// <summary>
            /// None value
            /// </summary>
            BDET_None = 0x00,
            /// <summary>
            /// Report a single correlation sample.
            /// </summary>
            BDET_CorrelationSample = 0x01,
            /// <summary>
            ///  Report a series of correlation samples.
            /// </summary>
            BDET_CorrelationSamplesComplete = 0x02,
            /// <summary>
            /// A tag characterization session has finished.
            /// </summary>
            BDET_TagCharacterizationComplete = 0x03,
            /// <summary>
            /// A change axis is notified
            /// </summary>
            BDET_AxisChange = 0x04,

            BDET_LowLevelOrder = 0x05,
            BDET_WriteBlock = 0x06, // return status while writing
        };

        /// <summary>
        /// Enumeration Door State
        /// </summary>
        public enum DoorStatusFlags // Used for doorStatus field for various response messages
        {
            /// <summary>
            /// 
            /// </summary>
            DSF_LedIsOn = 0x01,
            /// <summary>
            /// 
            /// </summary>
            DSF_LedIsFlashing = 0x02,
            /// <summary>
            /// 
            /// </summary>
            DSF_LedInLocalMode = 0x04, // Set if local mode, else remote mode.
            /// <summary>
            /// 
            /// </summary>
            DSF_Unused = 0x08, // This flag is not used.
            /// <summary>
            /// 
            /// </summary>
            DSF_LatchSolenoidEnergized = 0x10, // Set if solenoid energized (unlocked).
            /// <summary>
            /// 
            /// </summary>
            DSF_DoorLocksWhenOpened = 0x20,
            /// <summary>
            /// 
            /// </summary>
            DSF_DoorIsOpen = 0x40,
            /// <summary>
            /// 
            /// </summary>
            DSF_DoorIsUnlocked = 0x80,
        };
    #endregion

    #region class definition
        /// <summary>
        /// Class to retrieve KZ number need function of hardware
        /// </summary>
        public static class KZTabClass
        {
            /// <summary>
            /// Dictionnary of KZ value versus hardware - value = nbaxe + 1;
            /// </summary>
            public static Dictionary<DeviceTypeMajorType,int> KZValue = new Dictionary<DeviceTypeMajorType,int>()
            {
                {DeviceTypeMajorType.DEVICE_RFID_DIAMOND_SMART_BOX , 4},
                {DeviceTypeMajorType.DEVICE_RFID_JEWELLERY_CABINET , 10},
                {DeviceTypeMajorType.DEVICE_RFID_MEDICAL_CABINET , 5},
                {DeviceTypeMajorType.DEVICE_RFID_DIAMOND_SMART_BOX_V2 , 4},
                {DeviceTypeMajorType.DEVICE_RFID_MONO_AXE_READER , 2},
                {DeviceTypeMajorType.DEVICE_RFID_FLAT_3D_SHELVES , 5},
                {DeviceTypeMajorType.DEVICE_RFID_DIAMOND_SAS,4},
                {DeviceTypeMajorType.DEVICE_RFID_FRIDGE_1D,6},
                {DeviceTypeMajorType.DEVICE_RFID_MINI_SAS,4},
                {DeviceTypeMajorType.DEVICE_RFID_BLOOD_FRIDGE, 7},
                
            };
        }

    /// <summary>
    /// Struct to define TagIDtype for confirmation
    /// </summary>
        public unsafe struct TagIdType
        {
            /// <summary>
            /// basic representation of a 20 digits uid
            /// </summary>
            public fixed UInt16 word[4];
        }    

        /// <summary>
        /// Class ResponseType : High level Class herited from Serial MessageType for display ResponseCodeType
        /// overide ToString()
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class ResponseType : SerialMessageType
        {
            /// <summary>
            /// Overided method ToString
            /// </summary>
            /// <returns>The ResponseCodeType in a string format</returns>
            public override string ToString()
            {
                return string.Format("Serial.Response: {0}", (ResponseCodeType)serialMessage[0]);
            }
        }
        /// <summary>
        /// Class SerialMessageType : Low level message class that define the varibles of a serial message
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class SerialMessageType
        {
            /// <summary>
            /// Variable byte byteCount : Length of message. This will vary depending on length of message in Message field.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte byteCount; 
            /// <summary>
            /// Variable byte messageType : Message type from MessageIdType enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte messageType; 
            /// <summary>
            /// Variable uint messageId : Unique ID identifying this transaction; responses use the same messageID.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint messageId; 
            /// <summary>
            /// Variable uint deviceId : ID of device, if applicable.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public uint deviceId; 
            /// <summary>
            /// Variable ushort timeoutMSecs : Maximum allowable device response time, if applicable.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public ushort timeoutMSecs; 
            /// <summary>
            /// Variable byte errorCode : Return error code, if applicable.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte errorCode; 
            /// <summary>
            /// Variable  byte deviceTypeMajor : Device type major, if applicable.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte deviceTypeMajor; 
            /// <summary>
            /// Variable byte deviceTypeMinor : Device type minor, if applicable.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte deviceTypeMinor; 
            /// <summary>
            /// Variable byte spare1 : Extra data depending on message type.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte spare1; 
            /// <summary>
            /// Variable byte[] serialMessage : Message packet.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 48)]
            public byte[] serialMessage; // Message packet.
            /// <summary>
            /// Variable int LENGTH_SerialMessageHeader : // Not including High level Message.
            /// </summary>
            public const int LENGTH_SerialMessageHeader = 16; 
        }
       /* /// <summary>
        /// Class PBAE_RfidTagFullMem : Data class for exchange Uid when CommandType.PBCMD_AsyncEvent is readfull mem
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PBAE_RfidTagFullMem
        {
            /// <summary>
            /// Variable ushort command : Message command. Must be CommandType.PBCMD_AsyncEvent.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;
            /// <summary>
            /// Variable byte asyncEvent :  Should be PBET_TagAdded
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte asyncEvent;
            /// <summary>
            /// Variable byte[] serialNumber : 60-bit Tag serial number, stored as big-endian.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
            public byte[] TagId;
        };
    */

        /// <summary>
        /// Class PBAE_RfidTagAdded : Data class for exchange Uid when CommandType.PBCMD_AsyncEvent is TagAdded.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PBAE_RfidTagAdded
        {
            /// <summary>
            /// Variable ushort command : Message command. Must be CommandType.PBCMD_AsyncEvent.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// Variable byte asyncEvent :  Should be PBET_TagAdded
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte asyncEvent;
            /// <summary>
            /// Variable byte[] serialNumber : 60-bit Tag serial number, stored as big-endian.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] serialNumber; 
        };

        /// <summary>
        /// Class PBAE_PBAE_RfidTagRemoved : Data class for exchange Uid when CommandType.PBCMD_AsyncEvent is TagRemoved.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PBAE_RfidTagRemoved
        {
            /// <summary>
            /// Variable ushort command : Message command. Must be CommandType.PBCMD_AsyncEvent.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;
            /// <summary>
            /// Variable byte asyncEvent :  Should be PBET_TagRemoved
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte asyncEvent;
            /// <summary>
            /// Variable byte[] serialNumber : 60-bit Tag serial number, stored as big-endian.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] serialNumber; 
        };

        /// <summary>
        /// Class  AsyncEventMessage herited from SerialMessageType to define asynchronous operation.
        /// </summary>
        public class AsyncEventMessage : SerialMessageType
        {
            /// <summary>
            /// Property to Get the asyncEventType
            /// </summary>
            /// <value> return the asynchronous type of the message from enumeration AsyncEventType</value>
            public AsyncEventType asyncEventType
            {
                get
                {
                    MyDebug.Assert((((uint)serialMessage[1] << 8) | serialMessage[0]) == (ushort)CommandType.PBCMD_AsyncEvent, "Error in class AsyncEventMessage : SerialMessageType" );
                           return (AsyncEventType)serialMessage[2];
                   
                }
            }
            /// <summary>
            /// Override function ToString() to display asynchronous event depending of the asynchronous message type.
            /// </summary>
            /// <returns>return a formated string function of the asynchronous operation</returns>
            public override string ToString()
            {
                StringBuilder result = new StringBuilder(64);

                if (byteCount < 16)
                    result.AppendFormat(" *** Invalid Length: {0}", byteCount);
                else
                {
                    switch (asyncEventType)
                    {
                        case AsyncEventType.PBET_AlarmMove:
                            result.AppendFormat("Movement event");
                            break;

                        case AsyncEventType.PBET_AlarmInfra:
                            result.AppendFormat("Infra red event");
                            break;
                        case AsyncEventType.PBET_PowerOff:
                            result.AppendFormat("Power OFF");
                            break;
                        case AsyncEventType.PBET_PowerOn:
                             result.AppendFormat("Power ON");
                            break;
                        case AsyncEventType.PBET_DoorOpened:
                            result.AppendFormat("Door Opened");
                            break;
                        case AsyncEventType.PBET_DoorClosed:
                            result.AppendFormat("Door Closed");
                            break;
                        /*case AsyncEventType.PBET_TagFullMem:

                            PBAE_RfidTagFullMem tagFullMessage = (PBAE_RfidTagFullMem)Utilities.MarshalToStruct(
                                    serialMessage, typeof(PBAE_RfidTagFullMem));

                            result.AppendFormat("PBET_TagFullMem: '{0}'", tagFullMessage.TagId[0]);
                            break;*/
                        //case AsyncEventType.PBET_TagAdded:

                        case AsyncEventType.PBET_TagFullMem_R8:
                        case AsyncEventType.PBET_TagFullMem_RO:
                        case AsyncEventType.PBET_TagFullMem_RW:
                        case AsyncEventType.PBET_TagFullMem_DEC:

                            PBAE_RfidTagFullMem tagFullMessage = (PBAE_RfidTagFullMem)Utilities.MarshalToStruct(
                                    serialMessage, typeof(PBAE_RfidTagFullMem));

                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < 42; i++)
                                sb.AppendFormat("{0}", tagFullMessage.TagId[i]);

                            result.AppendFormat("{1}: '{0}'", sb.ToString(), asyncEventType.ToString());
                            break;
                        case AsyncEventType.PBET_TagAddedR8:
                            {
                                PBAE_RfidTagAdded tagAddedMessage = (PBAE_RfidTagAdded)Utilities.MarshalToStruct(
                                    serialMessage, typeof(PBAE_RfidTagAdded));
                                result.AppendFormat("PBET_TagAdded R8: '{0}'", SerialRFID.SerialNumberAsString(
                                    SerialRFID.SerialNumber(tagAddedMessage.serialNumber)));
                                break;
                            }
                        //case AsyncEventType.PBET_TagRemoved:
                        case AsyncEventType.PBET_TagAddedSPCE2_RO:
                            {
                                PBAE_RfidTagRemoved tagAddedMessage = (PBAE_RfidTagRemoved)Utilities.MarshalToStruct(
                                    serialMessage, typeof(PBAE_RfidTagRemoved));
                                //CR
                                result.AppendFormat("PBET_TagAdded SPCE2_RO: '{0}'", SerialRFID.SerialNumberAsAlphaString(
                                //result.AppendFormat("PBET_TagAdded SPCE2: '{0}'", SerialRFID.SerialNumberAsString(
                                    SerialRFID.SerialNumber(tagAddedMessage.serialNumber),TagType.TT_SPCE2_RO));
                                break;
                            }
                        case AsyncEventType.PBET_TagAddedSPCE2_RW:
                            {
                                PBAE_RfidTagRemoved tagAddedMessage = (PBAE_RfidTagRemoved)Utilities.MarshalToStruct(
                                    serialMessage, typeof(PBAE_RfidTagRemoved));
                                //CR
                                result.AppendFormat("PBET_TagAdded SPCE2_RW: '{0}'", SerialRFID.SerialNumberAsAlphaString(
                                    //result.AppendFormat("PBET_TagAdded SPCE2: '{0}'", SerialRFID.SerialNumberAsString(
                                    SerialRFID.SerialNumber(tagAddedMessage.serialNumber),TagType.TT_SPCE2_RW));
                                break;
                            }
                        case AsyncEventType.PBET_RfidScanStateChanged:
                            {
                                Debug.Assert(byteCount == 0x16, "Error in AsyncEventType.PBET_RfidScanStateChanged");
                               
                                PBAE_RfidScanStateChanged scanStateChangedMessage = (PBAE_RfidScanStateChanged)Utilities.MarshalToStruct(
                                    serialMessage, typeof(PBAE_RfidScanStateChanged));

                                switch ((ScanStatusType)scanStateChangedMessage.scanStatus)
                                {
                                    case ScanStatusType.SS_TagScanStarted: // Cabinet scan has begun.
                                        result.AppendFormat("RfidScanStateChanged: Scan started; async tag messages {0}.",
                                            (scanStateChangedMessage.info == 0) ? "disabled" : "enabled");
                                        break;
                                    case ScanStatusType.SS_TagScanCompleted: // Cabinet scan has completed
                                        result.AppendFormat("RfidScanStateChanged: Scan completed; tag changed {0}detected{1}.",
                                            ((scanStateChangedMessage.info & (byte)TagScanCompleteInfoFlagsType.TSCF_ChangesDetected) == 0) ? "not " : "",
                                            ((scanStateChangedMessage.info & (byte)TagScanCompleteInfoFlagsType.TSCF_TooManyTags) != 0) ? "; too many tags" : "");
                                        break;
                                    case ScanStatusType.SS_TagScanCanceledByDoorOpen: // Cabinet scan canceled due to door opening.
                                        result.AppendFormat("RfidScanStateChanged: Scan canceled due to door open.");
                                        break;
                                    case ScanStatusType.SS_TagScanCanceledByHost: // Cabinet scan canceled by host.
                                        result.AppendFormat("RfidScanStateChanged: Scan canceled by host.");
                                        break;
                                    case ScanStatusType.SS_TagScanFailedByUnrecoverableError: // Cabinet scan failed due to unrecoverable error.
                                        result.AppendFormat("RfidScanStateChanged: Scan failed due to unrecoverable error: " +
                                            ((UnrecoverableErrorType)scanStateChangedMessage.info).ToString().Substring(3));
                                        break;
                                    case ScanStatusType.SS_TagScanSendPourcent:
                                        result.AppendFormat("RfidScanStateChanged: Send % .");
                                        break;
                                    case ScanStatusType.SS_TagConfirmation:
                                        result.AppendFormat("RfidScanStateChanged: Send Confirmation");
                                        break;
                                    default:
                                        Debug.Fail("Unexpected case.");
                                        break;
                                }
                                break;
                            }
                        case AsyncEventType.PBET_BackDoorInfo:
                            {
                                PBAE_BackDoorInfo backDoorPacket = (PBAE_BackDoorInfo)Utilities.MarshalToStruct(
                             serialMessage, typeof(PBAE_BackDoorInfo));
                                if (backDoorPacket.backDoorEventType == (byte)BackDoorEventType.BDET_AxisChange)
                                {
                                    switch (backDoorPacket.value1)
                                    {
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                        case 6:
                                        case 7:
                                        case 8:
                                        case 9:

                                            if ( backDoorPacket.value2 > 50) //chgt axe
                                                 result.AppendFormat("Selected Axis: " + backDoorPacket.value1.ToString() + " DCU : " + backDoorPacket.value2.ToString());
                                            else
                                                result.AppendFormat("Notification : " + backDoorPacket.value1.ToString() + "," + backDoorPacket.value2.ToString());
                                           
                                            break;
                                        case 10:
                                            result.AppendFormat("KZ end channel OK: " + backDoorPacket.value2.ToString());
                                            break;
                                        case 13:
                                            result.AppendFormat("Nb Error Loop : " + backDoorPacket.value2.ToString());
                                            break;
                                        case 14 :
                                            result.AppendFormat("Digit Error Loop : " + backDoorPacket.value2.ToString());
                                            break;
                                        case 15:
                                            result.AppendFormat("First CRC : " + backDoorPacket.value2.ToString());
                                            break;
                                        case 16:
                                            result.AppendFormat("Second CRC : " + backDoorPacket.value2.ToString());
                                            break;
                                        case 20:
                                            result.AppendFormat("Tree Branch (6): " + backDoorPacket.value2.ToString());
                                            break;
                                        case 21:
                                            result.AppendFormat("Tree Branch (7): " + backDoorPacket.value2.ToString());
                                            break;
                                        case 30:
                                            result.AppendFormat("Antenna Carrier Period: " + backDoorPacket.value2.ToString());
                                            break;
                                        case 31:
                                            result.AppendFormat("Vmax: " + backDoorPacket.value2.ToString());
                                            break;
                                        case 65:
                                            switch (backDoorPacket.value2)
                                            {
                                                case 5: result.AppendFormat("Error 5v Failure"); break;
                                                case 12: result.AppendFormat("Error 12v Failure"); break;
                                                default: result.AppendFormat("Error Unknown"); break;
                                            }                                            
                                            break;
                                        case 66:
                                            switch (backDoorPacket.value2)
                                            {
                                                case 1: result.AppendFormat("Scan Stop on AbortReason"); break;
                                                case 2: result.AppendFormat("Scan Stop 1st digit < 3"); break;
                                                case 3: result.AppendFormat("Scan Stop 1st index < nbDigitToRead "); break;
                                                case 4: result.AppendFormat("Scan Stop error during blocking "); break;
                                                case 5: result.AppendFormat("Scan Stop erroron UpdateStatus "); break;

                                            }
                                            break;
                                        case 70:
                                        case 72:
                                            result.AppendFormat("Notification type : " + backDoorPacket.value2.ToString());
                                            break;
                                        case 71:
                                        case 73:
                                            result.AppendFormat("BChange Detected : " + backDoorPacket.value2.ToString());
                                            break;
                                        default :

                                            if (backDoorPacket.value1 > 314)
                                                result.AppendFormat("Carrier Frequency: " + backDoorPacket.value1.ToString() + " Vmax : " + backDoorPacket.value2.ToString());
                                            else
                                                result.AppendFormat("Notification : " + backDoorPacket.value1.ToString() + "," + backDoorPacket.value2.ToString());
                                            break;
                                       

                                    }
                                }
                                break;
                            }
                        default:                           
                            break;
                    }
                }
                return result.ToString();
            }
        }
        
        /// <summary>
        /// Class BaseConverter to pass from a data representation to another from base 2 to 36
        /// </summary>
        public class BaseConverter
        {
            
            /// <summary>
            /// Function Convert to Convert number in string representation from base:from to base:to
            /// </summary>
            /// <param name="from">source base of the number to convert (2 to 36)</param>
            /// <param name="to">Destination base for the number to convert (2 to 36)</param>
            /// <param name="s">Number to convert in a string format</param>
            /// <returns>A string of the converted number in base "to"</returns>
            public static String Convert(int from, int to, String s)
            {
                //Return error if input is empty
                if (String.IsNullOrEmpty(s))
                {
                    return ("Error: Nothing in Input String");
                }
                //only allow uppercase input characters in string
                s = s.ToUpper();

                //only do base 2 to base 36 (digit represented by charecaters 0-Z)"
                if (from < 2 || from > 36 || to < 2 || to > 36) { return ("Base requested outside range"); }

                //convert string to an array of integer digits representing number in base:from
                int il = s.Length;
                int[] fs = new int[il];
                int k = 0;
                for (int i = s.Length - 1; i >= 0; i--)
                {
                    if (s[i] >= '0' && s[i] <= '9') { fs[k++] = (int)(s[i] - '0'); }
                    else
                    {
                        if (s[i] >= 'A' && s[i] <= 'Z') { fs[k++] = 10 + (int)(s[i] - 'A'); }
                        else
                        { return ("Error: Input string must only contain any of 0-9 or A-Z"); } //only allow 0-9 A-Z characters
                    }
                }

                //check the input for digits that exceed the allowable for base:from
                foreach (int i in fs)
                {
                    if (i >= from) { return ("Error: Not a valid number for this input base"); }
                }

                //find how many digits the output needs
                int ol = il * (from / to + 1);
                int[] ts = new int[ol + 10]; //assign accumulation array
                int[] cums = new int[ol + 10]; //assign the result array
                ts[0] = 1; //initialise array with number 1 

                //evaluate the output
                for (int i = 0; i < il; i++) //for each input digit
                {
                    for (int j = 0; j < ol; j++) //add the input digit times (base:to from^i) to the output cumulator
                    {
                        cums[j] += ts[j] * fs[i];
                        int temp = cums[j];
                        int rem = 0;
                        int ip = j;
                        do // fix up any remainders in base:to
                        {
                            rem = temp / to;
                            cums[ip] = temp - rem * to; ip++;
                            cums[ip] += rem;
                            temp = cums[ip];
                        }
                        while (temp >= to);
                    }

                    //calculate the next power from^i) in base:to format
                    for (int j = 0; j < ol; j++)
                    {
                        ts[j] = ts[j] * from;
                    }
                    for (int j = 0; j < ol; j++) //check for any remainders
                    {
                        int temp = ts[j];
                        int rem = 0;
                        int ip = j;
                        do  //fix up any remainders
                        {
                            rem = temp / to;
                            ts[ip] = temp - rem * to; ip++;
                            ts[ip] += rem;
                            temp = ts[ip];
                        }
                        while (temp >= to);
                    }
                }

                //convert the output to string format (digits 0,to-1 converted to 0-Z characters) 
                String sout = String.Empty; //initialise output string
                bool first = false; //leading zero flag
                for (int i = ol; i >= 0; i--)
                {
                    if (cums[i] != 0) { first = true; }
                    if (!first) { continue; }
                    if (cums[i] < 10) { sout += (char)(cums[i] + '0'); }
                    else { sout += (char)(cums[i] + 'A' - 10); }
                }
                if (String.IsNullOrEmpty(sout)) { return "0"; } //input was zero, return 0
                //return the converted string          
                return sout;
            }
        }
        /// <summary>
        /// Class SerialRFID to manipulate and convert a tag serial number
        /// </summary>
        public class SerialRFID
        {
            const int nbBits = 134;
            /*public static readonly char[] alphaNumericTableRW = new char[] 
            { '0', '1', '2', '3', '4', '5', '6', '7',  // 00 - 07
              '8', '9', '[', '#', 'H', '=', '>', '?',  // 10 - 17
              '_', 'A', 'B', 'C', 'D', 'E', 'F', 'G',  // 20 - 27
              '@', 'I', '&', '.', ']', '(', ' ', '\\', // 30 - 37
              '^', 'J', 'K', 'L', 'M', 'N', 'O', 'P',  // 40 - 47
              'Q', 'R', '-', '$', '*', ')', ';', '+',  // 50 - 57
              '`', '/', 'S', 'T', 'U', 'V', 'W', 'X',  // 60 - 67
              'Y', 'Z', '<', ',', '%','\'', '"', '§' };// 70 - 77

            public static readonly char[] alphaNumericTableRO = new char[] 
            { '0', '1', '2', '3', '4', '5', '6', '7',  // 00 - 07
              '8', '9', '[', '#', 'H', '=', '>', '?',  // 10 - 17
              '_', 'A', 'B', 'C', 'D', 'E', 'F', 'G',  // 20 - 27
              '@', 'I', '&', '.', ']', '(', ' ', '\\', // 30 - 37
              '^', 'J', 'K', 'L', 'M', 'N', 'O', 'P',  // 40 - 47
              'Q', 'R', '-', '$', '*', ')', ';', '+',  // 50 - 57
              '`', '/', 'S', 'T', 'U', 'V', 'W', 'X',  // 60 - 67
              'Y', 'Z', '<', ',', '%','\'', '"', '§' };// 70 - 77*/

            public static readonly char[] alphaNumericTableRW = new char[] 
            { '0', '1', '2', '3', '4', '5', '6', '7',  // 00 - 07
              '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',  // 10 - 17
              'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',  // 20 - 27
              '@', '@', '@', '@', '@', '@', '@', '@',  // 30 - 37   // 3X reserved for RO UID from prod - If previoulsy Write need rewrite in hard.
              'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',  // 40 - 47
              'W', 'X', 'Y', 'Z', ' ', '-', '/', '.',  // 50 - 57
              '@', '@', '@', '@', '@', '@', '@', '@',  // 60 - 67
              '@', '@', '@', '@', '@', '@', '@', '§' }; // 70 - 77

            public static readonly char[] alphaNumericTableRO = new char[] 
            { '0', '1', '2', '3', '4', '5', '6', '7',  // 00 - 07
              '8', '9', 'A', 'B', 'C', 'D', 'E', 'F',  // 10 - 17
              'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N',  // 20 - 27
              'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V',  // 30 - 37
              'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd',  // 40 - 47
              'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l',  // 50 - 57
              'm', 'n', 'o', 'p', 'q', 'r', 's', 't',  // 60 - 67
              'u', 'v', 'w', 'x', 'y','z', '-', '/' }; // 70 - 77
    

            /// <summary>
            /// Function Serial number : Convert a serial number in byte array format to an int64.
            /// </summary>
            /// <param name="serialNumberBytes">Serial tag number in a  byte array </param>
            /// <returns>a Int64 of the tag serial number</returns>
            public static UInt64 SerialNumber(byte[] serialNumberBytes)
            {
                UInt64 serialNumber = 0;
                int bitShift = 64;
                uint byteIndex = 0;
                while (bitShift > 0)
                {
                    bitShift -= 8;
                    serialNumber += ((UInt64)serialNumberBytes[byteIndex++] << bitShift);
                }
                return serialNumber;
            }

            /// <summary>
            /// Function SerialNumberAsString : Convert the specified serial number into its octal value.
            /// </summary>
            /// <param name="serialNumber">The 60-bit serial number to display.</param>
            /// <returns>The octal string corresponding to the serial number.</returns>
            public static string SerialNumberAsString(UInt64 serialNumber)
            {
                int digitCount = 20;
                int shiftBits = digitCount * 3; // There are sixty bits in the serial number.
                StringBuilder builder = new StringBuilder(digitCount);
                while (shiftBits > 0)
                {
                    shiftBits -= 3; // Three bits per digit.
                    uint digit = (uint)(serialNumber >> shiftBits) & 0x07;
                    builder.Append((char)('0' + digit));
                }
                //return builder.ToString().Substring(0, 10);
                return builder.ToString();
            }

            public static string SerialNumberAsStringHex(UInt64 serialNumber)
            {
                int digitCount = 15;
                int shiftBits = digitCount * 4; // There are sixty bits in the serial number.
                StringBuilder builder = new StringBuilder(digitCount);
                while (shiftBits > 0)
                {
                    shiftBits -= 4; // Three bits per digit.
                    uint digit = (uint)(serialNumber >> shiftBits) & 0x0F;
                    if (digit < 10)
                        builder.Append((char)('0' + digit));
                    else
                    {
                        builder.Append((char)('A' + (digit-10)));
                    }
                }
                //return builder.ToString().Substring(0, 10);
                return builder.ToString();
            }


            /// <summary>
            /// Convert the specified serial number into its alphaValue value.
            /// </summary>
            /// <param name="serialNumber">The 60-bit serial number to display.</param>
            /// <returns>The alpha string corresponding to the serial number.</returns>
            public static string SerialNumberAsAlphaString(UInt64 serialNumber ,  TagType tagTypeUsed)
            {


                char[] codeOct = new char[20];
                int nIndex = 0;
                int digitCount = 20;
                int shiftBits = digitCount * 3; // There are sixty bits in the serial number.
                StringBuilder builder = new StringBuilder(10);
                while (shiftBits > 0)
                {
                    shiftBits -= 3; // Three bits per digit.
                    uint digit = (uint)(serialNumber >> shiftBits) & 0x07;
                    codeOct[nIndex++] = (char)('0' + digit);
                }

                for (int p = 0; p < 20; p = p + 2)
                {
                    string row = codeOct[p].ToString() + codeOct[p + 1].ToString();
                    int nRow = Convert.ToInt32(row, 8);
                    switch (tagTypeUsed)
                    {
                        case TagType.TT_SPCE2_RO: builder.Append(alphaNumericTableRO[nRow]);break;
                        case TagType.TT_SPCE2_RW: builder.Append(alphaNumericTableRW[nRow]); break;

                    }
                   

                }
                return builder.ToString();
            }

            /// <summary>
            /// Convert Tag UID with family number to string
            /// </summary>
            /// <param name="UidOctal"></param>
            /// <returns></returns>
            public static string CodeMemFullToString(string UidOctal)
            {
                
                // Check if family is 2 - Rewrite Tag 

                if (UidOctal[0] != '2') return null;

                // Remove CRC and family number
                string modifiedUid =
                    UidOctal.Remove(41, 1)
                        .Remove(35, 1)
                        .Remove(29, 1)
                        .Remove(23, 1)
                        .Remove(17, 1)
                        .Remove(11, 1)
                        .Remove(5, 1)
                        .Remove(0, 1);
                StringBuilder builder = new StringBuilder();
                int len = (modifiedUid.Length/2) * 2;
                for (int p = 0; p < len ; p = p + 2)
                {
                    string row = modifiedUid[p].ToString() + modifiedUid[p + 1].ToString();
                    int nRow = Convert.ToInt32(row, 8);
                    builder.Append(alphaNumericTableRW[nRow]);
                }

                return builder.ToString();
            }


            /// <summary>
            /// Convert Tag UID with family number to 12 decimal
            /// </summary>
            /// <param name="UidOctal"></param>
            /// <returns></returns>
            public static string CodeMemFullToDec(string UidOctal)
            {

                // Check if family is 2 - Rewrite Tag 

                if (UidOctal[0] != '1') return null;

                // Remove CRC and family number
                string modifiedUid =
                    UidOctal.Remove(41, 1)
                        .Remove(35, 1)
                        .Remove(29, 1)
                        .Remove(23, 1)
                        .Remove(17, 1)
                        .Remove(11, 1)
                        .Remove(5, 1)
                        .Remove(0, 1);
                modifiedUid = modifiedUid.Substring(0, 14);

                UInt64 valDec = Convert.ToUInt64(modifiedUid,8);
                return valDec.ToString().PadLeft(12, '0');
            }

            /// <summary>
            /// Function to convert a uid from a byte[] to a tagdigit structure
            /// </summary>
            /// <param name="pSerialNumber">tagdigit to store result</param>
            /// <param name="digitCount">Number of digit to convert</param>
            /// <param name="TagDigits">array of byte of the uid</param>
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
                        else if (digitIndex == 24)
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

            // Write SPCE2 function
            public static int GetAlphaIndexRO(char val)
            {
                return Array.IndexOf(alphaNumericTableRO, val);
            }

            public static int GetAlphaIndexRW(char val)
            {
                return Array.IndexOf(alphaNumericTableRW, val);
            }

            public static bool isStringValidToWrite(string strToTest)
            {
                bool ret = true;
                if (String.IsNullOrEmpty(strToTest)) return false;
                if (strToTest.Contains("@")) return false;
                if (strToTest.Length > 0)
                {
                    char[] testedChar = strToTest.ToCharArray();

                    for (int i = 0; i < testedChar.Length; i++)
                    {
                        int nCharIndex = SDK_SC_RfidReader.DeviceBase.SerialRFID.GetAlphaIndexRW(testedChar[i]);
                        if (nCharIndex == -1)
                        {
                            ret = false;
                            break;
                       }
                    }
                }
               

                return ret;
            }

            public static string ConvertAlphaNumToOct(string alphaNumValue , TagType tgType)
            {
                string octString = string.Empty;
                for (int i = 0; i < alphaNumValue.Length; i++)
                {
                    if (tgType == TagType.TT_SPCE2_RO )
                        octString += Convert.ToString(GetAlphaIndexRO((char)alphaNumValue[i]), 8).PadLeft(2, '0');
                    else
                        octString += Convert.ToString(GetAlphaIndexRW((char)alphaNumValue[i]), 8).PadLeft(2, '0');
                        
                    
                }
                return octString;
            }
            public static byte[] ConvertOctToUIDCodeWithCRC(string octString)
            {
                int chunkDigitIndex = 0;
                int crcIndex = 0;
                int digitIndex;
                byte[] CodeRead = new byte[45];
                int nbCRCCompute = 0;

                for (digitIndex = 0; digitIndex < CodeRead.Length; digitIndex++)
                {
                    if (chunkDigitIndex < 5)
                    {
                        if (digitIndex < octString.Length + nbCRCCompute)
                            CodeRead[digitIndex] = Convert.ToByte(octString[digitIndex - nbCRCCompute].ToString());
                        else
                            CodeRead[digitIndex] = 0;
                        chunkDigitIndex++;
                    }
                    else
                    {
                        chunkDigitIndex = 0;
                        int crc = ComputeCrc(CodeRead, crcIndex);
                        CodeRead[digitIndex] = (byte)crc;
                        crcIndex += 6;
                        nbCRCCompute++;
                    }
                }
                return CodeRead;
            }
            public static string[] ConvertCodeToHexa(byte[] CodeRead)
            {
                string HexaCode;
                byte[] Code135 = new byte[160];

                for (int bcl = 0; bcl < 135; bcl++) Code135[bcl] = 0x00;

                int row = 31;
                string tmpcode;
                int[] bitShift = { 0, 32, 64, 96, 128, 160 };
                int rowBitshift = 0;
                for (int loop = 0; loop < 45; loop++)
                {
                    tmpcode = Convert.ToString(CodeRead[loop], 2).PadLeft(3, '0');
                    for (int bcl = 2; bcl >= 0; bcl--)
                    {
                        if (tmpcode[bcl] == '1') Code135[row--] = 1;
                        else Code135[row--] = 0;

                        if (row == bitShift[rowBitshift] - 1)
                        {
                            rowBitshift++;
                            row = bitShift[rowBitshift + 1] - 1;
                        }

                    }
                }

                HexaCode = CodeToHexa(Code135, 33, CodeRead);

                string[] EElatchWord = new string[4];
                EElatchWord[0] = HexaCode.Substring(0, 8);
                EElatchWord[1] = HexaCode.Substring(9, 8);
                EElatchWord[2] = HexaCode.Substring(18, 8);
                EElatchWord[3] = HexaCode.Substring(27, 8);

                return EElatchWord;
            }
            public static string CodeToHexa(byte[] bytetoconvert, int nbdigit, byte[] CodeRead)
            {
                StringBuilder tmp = new StringBuilder();
                byte theshift = 0x00;
                byte HexValue = 0x00;

                for (int bcl1 = 1; bcl1 <= nbdigit; bcl1++)
                {
                    HexValue = 0;
                    for (int bcl2 = 0; bcl2 < 4; bcl2++)
                    {
                        theshift = RollLeft(bytetoconvert, 0);
                        HexValue <<= 1;
                        if (theshift == 0x01) HexValue += 1;

                    }
                    tmp.Append(HexValue.ToString("X1"));
                    if ((bcl1 % 8) == 0) tmp.Append("-");

                }

                HexValue = 0;
                tmp.Append("000000");
                if (CodeRead[42] > 3) HexValue += 1;
                HexValue += (byte)(2 * CodeRead[43]);
                HexValue += (byte)(4 * CodeRead[44]);
                tmp.Append(HexValue.ToString("X1"));

                return tmp.ToString();
            }
            public static byte StringOctToByte(string s)
            {
                byte ret = 0;
                for (int p = 0; p < 3; p++)
                {
                    ret <<= 1;
                    if (s[p] == '1') ret += 1;
                }
                return ret;
            }
            public static byte ComputeCrc(byte[] Tab, int rg)
            {
                return ((byte)(7 - ((Tab[rg] + Tab[rg + 1] + Tab[rg + 2] + Tab[rg + 3] + Tab[rg + 4]) & 0x07)));
            }
            public static byte RollRight(byte[] bytetoroll, byte inByte)
            {
                byte ret = bytetoroll[nbBits];
                for (int bcl = nbBits; bcl > 0; bcl--)
                {
                    bytetoroll[bcl] = bytetoroll[bcl - 1];
                }
                bytetoroll[0] = inByte;
                return ret;
            }
            public static byte RollLeft(byte[] bytetoroll, byte inByte)
            {
                byte ret = bytetoroll[0];
                for (int bcl = 0; bcl < nbBits; bcl++)
                {
                    bytetoroll[bcl] = bytetoroll[bcl + 1];
                }
                bytetoroll[nbBits] = inByte;
                return ret;


            }
        }

        /// <summary>
        /// Class SerialException herited from exception to define Exception process if occurs during communication
        /// </summary>
        public class SerialException : Exception
        {
            /// <summary>
            /// Variable  ResponseCodeType errorCode : Contain the error code of the exception
            /// </summary>
            private readonly ResponseCodeType errorCode;

            /// <summary>
            /// Constructor  of  SerialException class 
            /// </summary>
            /// <param name="errorCode">Contain the error code involved</param>
            /// <param name="exceptionDescription">Contain the description of the error</param>
            public SerialException(ResponseCodeType errorCode, string exceptionDescription)
                : base(exceptionDescription)
            {
                this.errorCode = errorCode;
            }
            /// <summary>
            /// Property  ErrorCode to retrieve the error code value.
            /// </summary>
            public ResponseCodeType ErrorCode { get { return errorCode; } }
        }

        /// <summary>
        /// Class Utilities that contain general purpose useful function
        /// </summary>
        public class Utilities
        {
            /// <summary>
            /// Function MarshalToStruct : Function that convert a byte array source to a spacifuc define type
            /// </summary>
            /// <param name="srcData"> A byte array of data to parse</param>
            /// <param name="resultType">The expected class in which this array should be convert</param>
            /// <returns> An object corresponding of the resultType containing the data parsed</returns>
            public static object MarshalToStruct(byte[] srcData, Type resultType)
            {
                GCHandle pinnedMessageContent = GCHandle.Alloc(srcData, GCHandleType.Pinned);
                object result = Marshal.PtrToStructure(pinnedMessageContent.AddrOfPinnedObject(), resultType);
                pinnedMessageContent.Free();
                return result;
            }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbMsgRfidCommand use when  Request for PBCMD_BackDoor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgRfidCommand 
        {
            /// <summary>
            /// Variable ushort command : Message command. CommandType.PBCMD_RfidCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;
            /// <summary>
            /// Variable byte rfidCommand : Should be one of RfidCommandType.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            byte rfidCommand; 
            /// <summary>
            /// Variable byte param1 : General purpose parameter depending on backDoorCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            byte param1;        
            /// <summary>
            /// Variable UInt16 param2 : General purpose parameter depending on backDoorCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            UInt16 param2;    
            /// <summary>
            /// Variables UInt16 param3 : General purpose parameter depending on backDoorCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            UInt16 param3;        
            /// <summary>
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>
            public const uint SIZE = 8;  

            /// <summary>
            /// Constructor of PbMsgRfidCommand
            /// </summary>
            /// <param name="rfidCommand">One of the enumeration RfidCommandType</param>
            /// <param name="param1">A byte containing data depend on RfidCommandType </param>
            /// <param name="param2">A 16 bits integer containing data depend on RfidCommandType </param>
            /// <param name="param3">A 16 bits integer containing data depend on RfidCommandType</param>
            public PbMsgRfidCommand(RfidCommandType rfidCommand, byte param1, ushort param2, ushort param3)
            {
                command = (UInt16)CommandType.PBCMD_RfidCommand;
                this.rfidCommand = (byte)rfidCommand;
                this.param1 = param1;
                this.param2 = param2;
                this.param3 = param3;
            }
            /// <summary>
            /// Override function ToString() to display PbMsgRfidCommand
            /// </summary>
            /// <returns>return a formated string </returns>
            public override string ToString()
            {
                return string.Format("PbMsgRfidBackDoor({0}, {1}, {2}, {3})",
                    (RfidCommandType)rfidCommand, param1, param2, param3);
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbRspRfidCommand which define the response to PBCMD_RfidCommand.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRfidCommand 
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
            /// <summary>
            /// Variable byte responseByte1 : Meaning varies based on rfidCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte responseByte1; 
            /// <summary>
            ///  Variable byte responseWord1 : Meaning varies based on rfidCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 responseWord1; 
            /// <summary>
            ///  Variable byte responseLon1 : Meaning varies based on rfidCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 responseLong1;
            /// <summary>
            /// Override function ToString() to display PbRspRfidCommand 
            /// </summary>
            /// <returns>return a formated string </returns>
            public override string ToString()
            {
                return string.Format("PbRspRfidCommand({0}, {1}, {2}, {3})",
                    (ResponseCodeType)completionStatus, responseByte1, responseWord1, responseLong1);
            }
        };
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Struct DeviceInfoType which contain the serial number and Description of the device
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceInfoType
        {
            /// <summary>
            /// Variable UInt32 deviceId : The 32-bit device serial number.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 deviceId;
            /// <summary>
            /// Variable  DeviceDescriptionType : Struct to basic definition of the device
            /// </summary>
            [MarshalAs(UnmanagedType.Struct)]
            public DeviceDescriptionType description;
        };
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Struct DeviceDescriptionType which contain the Device kind value and the sofware and hardware revision version
        /// </summary>        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct DeviceDescriptionType
        {
            /// <summary>
            /// Variable byte deviceTypeMajor : DeviceTypeMajorType enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte deviceTypeMajor; 
            /// <summary>
            /// Variable byte deviceTypeMinor :  DeviceTypeMinorType enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte deviceTypeMinor;
            /// <summary>
            /// Variable byte softwareVersionMajor : High number of software revision.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte softwareVersionMajor;
            /// <summary>
            /// Variable softwareVersionMinor :  Low number of software revision
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte softwareVersionMinor;
            /// <summary>
            /// Variable byte hardwareVersionMajor :  High number of hardware revision.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte hardwareVersionMajor;
            /// <summary>
            /// Variable byte hardwareVersionMinor :  Low number of hardware revision.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte hardwareVersionMinor;
        };
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbMsgGetHelloWorld : Request for PBCMD_HelloWorld.
        /// Variable class for discover new device function
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgGetHelloWorld 
        {
            /// <summary>
            /// Variable ushort command : Message command. Must be .CommandType.PBCMD_VersionNumber.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>
            public const uint SIZE = 2; 
            /// <summary>
            /// Constructor of class PbMsgGetHelloWorld
            /// </summary>
            public PbMsgGetHelloWorld()
            {
                command = (ushort)CommandType.PBCMD_HelloWorld;
            }
        };
        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbRspGetHelloWorld : Response to PBCMD_HelloWorld.
        /// Variable class for marshal response of PBCMD_HelloWorld.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspGetHelloWorld 
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
            /// <summary>
            /// Variable DeviceInfoType : Struct contains the software and hardware revision number
            /// </summary>
            [MarshalAs(UnmanagedType.Struct)]
            public DeviceInfoType DeviceInfo;
            /// <summary>
            /// Override function ToString() to display PbRspGetHelloWorld 
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return base.ToString() + string.Format(" <- Serial Number: {5:X8} -> Type: {0} HW V{1}.{2:D2} SW V{3}.{4:D2}",
                    DeviceInfo.description.deviceTypeMajor,
                    DeviceInfo.description.hardwareVersionMajor,
                    DeviceInfo.description.hardwareVersionMinor,
                    DeviceInfo.description.softwareVersionMajor,
                    DeviceInfo.description.softwareVersionMinor,
                    DeviceInfo.deviceId);
            }
        };

        /// <summary>        /// Class PBAE_RfidScanStateChanged : Class that contain result of the changed occurs during the last inventory
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PBAE_RfidScanStateChanged
        {
            /// <summary>
            /// Variable ushort command : Message command. Must be CommandType.PBCMD_AsyncEvent.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// Variable byte asyncEvent : Should be PBET_RfidScanStateChanged
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte asyncEvent; 
            /// <summary>
            /// Variable byte scanStatus : One of ScanStatusType.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte scanStatus; 
            /// <summary>
            /// Variable byte info : Information depending on scanStatus.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte info;
            /// <summary>
            /// Variable byte tagFailureCount : Number of tag failures detected. Retrieve with PBMCD_GetTagFailure.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte tagFailureCount; 
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbMsgRequestTagScan : Request for PBCMD_RequestTagCheck
        /// Standard variable class to request an inventory
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgRequestTagScan 
        {
            /// <summary>
            /// Variable ushort command : message command. Must be CommandType.PBCMD_RequestTagCheck.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            readonly ushort command;
            /// <summary>
            /// byte mode : Bits from TagScanModeFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            readonly byte mode; 
            /// <summary>
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>
            public const uint SIZE = 3; // Byte length of marshaled fields.

            /*  public PbMsgRequestTagScan(bool bClearTagListHasChangedFlag, bool bAsynchronousTagUpdates)
              {
                  command = (ushort)CommandType.PBCMD_RequestTagCheck;

                  mode = 0;
                  if (bClearTagListHasChangedFlag)
                      mode |= (byte)TagScanModeFlags.TSMF_ClearTagListHasChangedFlag;
                  if (bAsynchronousTagUpdates)
                      mode |= (byte)TagScanModeFlags.TSMF_SendAsyncMessages;
              }*/
            /// <summary>
            /// Method PbMsgRequestTagScan : Send command to start inventory to the board  with different mode.
            /// </summary>
            /// <param name="bClearTagListHasChangedFlag"> Set this bool for Clear stored list tag</param>
            /// <param name="bAsynchronousTagUpdates">Set this bool for received result inventory asynchronously during scan process</param>
            /// <param name="bUnlockTagAllAxis">Set this bool to unlock all tag on all axis prior to scan</param>
            /// <param name="bUseKR">Set This bool for perform inventory with KR during algo. </param>
            public PbMsgRequestTagScan(bool bClearTagListHasChangedFlag, bool bAsynchronousTagUpdates, bool bUnlockTagAllAxis, bool bUseKR)
            {
                command = (ushort)CommandType.PBCMD_RequestTagCheck;

                mode = 0;
                if (bClearTagListHasChangedFlag)
                    mode |= (byte)TagScanModeFlags.TSMF_ClearTagListHasChangedFlag;
                if (bAsynchronousTagUpdates)
                    mode |= (byte)TagScanModeFlags.TSMF_SendAsyncMessages;
                if (bUnlockTagAllAxis)
                    mode |= (byte)TagScanModeFlags.TSMF_UnlockTagAllAxis;
                if (bUseKR)
                    mode |= (byte)TagScanModeFlags.TSMF_UseKR;
            }
            /// <summary>
            /// Override function ToString() to display PBCMD_RequestTagCheck
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return base.ToString() + string.Format(" Mode: {0:X2}", mode);
            }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbRspRequestTagScan : Response to PBCMD_RequestTagCheck.
        /// Variable class for marshal response of PBCMD_RequestTagCheck.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRequestTagScan 
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
            /// <summary>
            /// Variable : byte systemStatus : Flags from SystemStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte systemStatus; 
            /// <summary>
            /// Variable byte doorStatus :  Flags from DoorStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte doorStatus; 
            /// <summary>
            /// Variable byte scanStatus :  Flags from ScanStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte scanStatus;
            /// <summary>
            /// Override function ToString() to display PbRspRequestTagScan 
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return string.Format("{0}: Sys({1:X2}) Door({2:X2}) Scan({3:X2})",
                    base.ToString(), base.ToString(), systemStatus, doorStatus, scanStatus);
            }
        }

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbMsgRfidGetNextTag :  Request for PBCMD_RequestNextTag.
        /// Class contain variable of retrieving the next stored tag in list
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgRfidGetNextTag 
        {
            /// <summary>
            ///  Variable ushort command : message command. Must be CommandType.PBCMD_RequestNextTag.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// Variable bGetFirst :  True to start from the beginning of the list; false to return next unreported tag.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public bool bGetFirst; //
            /// <summary>            
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>           
            public const uint SIZE = 3; 
            /// <summary>
            /// Constructor of PbMsgRfidGetNextTag Class
            /// </summary>
            public PbMsgRfidGetNextTag()
            {
                command = (UInt16)CommandType.PBCMD_RequestNextTag;
            }
            /// <summary>
            /// Override function ToString() to display PBCMD_RequestNextTag 
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return string.Format("{0}: bGetFirst = {1}", base.ToString(), bGetFirst);
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbRspRfidGetNextTag : Response to PBCMD_RequestNextTag.
        /// Variable class for marshal response of PBCMD_RequestNextTag.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRfidGetNextTag 
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus;
            /// <summary>
            /// Variable : byte systemStatus : Flags from SystemStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte systemStatus; 
            /// <summary>
            ///  Variable byte doorStatus :  Flags from DoorStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte doorStatus; 
            /// <summary>
            /// Variable byte scanStatus :  Flags from ScanStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte scanStatus; 
            /// <summary>
            /// Variable byte currentTagCount : The current count of detected tags.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte currentTagCount;
            /// <summary>
            /// Variable byte tagIndex : The index of the tag being returned.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte tagIndex;
            /// <summary>
            /// Variable byte tagInfo : Flags from TagInfoFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte tagInfo;
            /// <summary>
            /// Variable byte[] serialNumber : 60-bit serial number, stored as big-endian.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] serialNumber;
           
            /// <summary>
            /// Override function ToString() to display response of PBCMD_RequestNextTag 
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                if (completionStatus != (byte)ResponseCodeType.PBRC_SUCCESS)
                    return string.Format("{0}: Failed: {1}", base.ToString(), (ResponseCodeType)completionStatus);
                return string.Format("{0}:Current({1}) Index({2}) Info({3:X2}) Tag({4})",
                    base.ToString(), currentTagCount, tagIndex, tagInfo,
                    SerialRFID.SerialNumberAsString(SerialRFID.SerialNumber(serialNumber)));
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbMsgRfidGetTagFailure :  Request for PBMCD_GetTagFailure.
        /// Class contain variable of retrieving the failure of tag
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgRfidGetTagFailure 
        {
            /// <summary>
            /// Variable ushort command  : Message command. Must be CommandType.PBMCD_GetTagFailure.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// Variable byte failedTagIndex : Index of tag failure entry to retrieve
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte failedTagIndex;
            /// <summary>            
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>           
            public const uint SIZE = 3;
            /// <summary>
            /// Constructor of class PbMsgRfidGetTagFailure
            /// </summary>
            /// <param name="failedTagIndex">uint that contain the index of the tag to proceed</param>
            public PbMsgRfidGetTagFailure(uint failedTagIndex)
            {
                command = (UInt16)CommandType.PBMCD_GetTagFailure;
                this.failedTagIndex = (byte)failedTagIndex;
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbRspRfidGetTagFailure : Response to PBMCD_GetTagFailure.
        /// Variable class for marshal response of PBMCD_GetTagFailure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRfidGetTagFailure // Response for PBMCD_GetTagFailure.
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
            /// <summary>
            /// Variable byte failedTagCount : Total number of tag failures in failure list.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte failedTagCount; 
            /// <summary>
            /// Variable  byte failureType : Describes what failed for this tag failure from TagFailureEnumType enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte failureType; 
            /// <summary>
            /// Variable byte digitsRead : The number of serial number digits that were read before failure.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte digitsRead; 
            /// <summary>
            /// Variable checksumDigitsRead : The number of checksum digits that were read before failure.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte checksumDigitsRead; 
            /// <summary>
            /// Variable byte[] digitSequence : The successful digits of the serial number, stored as big-endian.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] digitSequence; 
            /// <summary>
            /// Variable byte[] checksumDigits :  The checksum digits that were successfully read.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] checksumDigits;

            /// <summary>
            /// Override function ToString() to display response of PBMCD_GetTagFailure 
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                if (completionStatus != (byte)ResponseCodeType.PBRC_SUCCESS)
                    return string.Format("{0}: Failed: {1}", base.ToString(), (ResponseCodeType)completionStatus);
                return string.Format("{0}: Count: {1}, Type: {2}, Digits: {3}, CS Digits: {4}",
                    base.ToString(), failedTagCount, (TagFailureEnumType)failureType, digitsRead, checksumDigitsRead);
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class  PbMsgGetStatus :  Request for  PBCMD_GetStatus.
        /// Class contain variable of retrieving the status of the board
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgGetStatus // Request for PBCMD_GetStatus.
        {
            /// <summary>
            ///  Variable ushort command  : Message command. Must be CommandType.PBCMD_GetStatus.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;
            /// <summary>            
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>          
            public const uint SIZE = 2;
            /// <summary>
            /// Constructor of class PbMsgGetStatus.
            /// </summary>
            public PbMsgGetStatus()
            {
                command = (ushort)CommandType.PBCMD_GetStatus;
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        /// Class PbRspGetStatus : Response to PBCMD_GetStatus.
        /// Variable class for marshal response of PBCMD_GetStatus.
        /// </summary>
        /// [StructLayout(LayoutKind.Sequential, Pack = 1)]
       
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspGetStatus  
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus;
            /// <summary>
            /// Variable : byte systemStatus : Flags from SystemStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte systemStatus;
            /// <summary>
            /// 
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte doorStatus; // Flags from DoorStatusFlags enumeration.
            /// <summary>
            /// Variable byte scanStatus :  Flags from ScanStatusFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte scanStatus;

            /// <summary>
            /// Override function ToString() to display response of PBCMD_GetStatus 
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                if ((ResponseCodeType)completionStatus != ResponseCodeType.PBRC_SUCCESS)
                    return ((ResponseCodeType)completionStatus).ToString();
                string systemStatusString = string.Format("12V:{0} Ant:{1} 5V:{2}",
                    (((systemStatus & (byte)SystemStatusFlags.SSF_12VoltsFailure) == 0) ? 'Y' : 'N'),
                    (((systemStatus & (byte)SystemStatusFlags.SSF_AntennaNotConnected) == 0) ? 'Y' : 'N'),
                    (((systemStatus & (byte)SystemStatusFlags.SSF_5VoltsFailure) == 0) ? 'Y' : 'N'));
                string doorStatusString = string.Format("{0}{1}{2}{3}{4}{5}{6}",
                    (((doorStatus & (byte)DoorStatusFlags.DSF_LedIsOn) != 0) ? " LED" : ""),
                    (((doorStatus & (byte)DoorStatusFlags.DSF_LedIsFlashing) != 0) ? " Flash" : ""),
                    (((doorStatus & (byte)DoorStatusFlags.DSF_LedInLocalMode) != 0) ? " Local" : ""),
                    (((doorStatus & (byte)DoorStatusFlags.DSF_LatchSolenoidEnergized) != 0) ? " Solenoid" : ""),
                    (((doorStatus & (byte)DoorStatusFlags.DSF_DoorLocksWhenOpened) != 0) ? " Auto-Lock" : ""),
                    (((doorStatus & (byte)DoorStatusFlags.DSF_DoorIsOpen) != 0) ? " Open" : ""),
                    (((doorStatus & (byte)DoorStatusFlags.DSF_DoorIsUnlocked) != 0) ? " Unlocked" : "")).Trim();
                string scanStatusString = string.Format("{0}{1}{2}{3}{4}",
                    (((scanStatus & (byte)ScanStatusFlags.SSF_ScanningTagsNow) != 0) ? " Scanning" : ""),
                    (((scanStatus & (byte)ScanStatusFlags.SSF_TagListHasChanged) != 0) ? " Changed" : ""),
                    (((scanStatus & (byte)ScanStatusFlags.SSF_TagScanWasCanceled) != 0) ? " Canceled" : ""),
                    (((scanStatus & (byte)ScanStatusFlags.SSF_TagScanRequired) != 0) ? " Required" : ""),
                    (((scanStatus & (byte)ScanStatusFlags.SSF_TooManyTagsDetected) != 0) ? " TooMany" : "")).Trim();

                return string.Format("System:{0:X2}({1}) Door:{2:X2}({3}) Scan:{4:X2}({5})",
                    systemStatus, systemStatusString, doorStatus, doorStatusString, scanStatus, scanStatusString);
            }
        };
        /// <summary>
        /// PbMsgRfidControl : request to PBCMD_Control.
        /// Variable class for marshal response of PBCMD_Control.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgRfidControl 
        {
            /// <summary>
            ///  Variable ushort command  : Message command. Must be CommandType.PBCMD_Control.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;
            /// <summary>
            /// Variable  byte modeControl; Flags from ControlModeFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            readonly byte modeControl;
            /// <summary>
            /// Variable  byte rfidControl :Flags from ControlControlFlags enumeration.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            readonly byte rfidControl;
            /// <summary>            
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>          
            public const uint SIZE = 4; // Byte length of marshaled fields.
            /// <summary>
            /// constructor of PbMsgRfidControl
            /// </summary>
            /// <param name="bEnableStatsAsyncEvent"></param>
            /// <param name="bStopScan">bool to request stop scan</param>
            public PbMsgRfidControl(bool bEnableStatsAsyncEvent, bool bStopScan)
            {
                command = (ushort)CommandType.PBCMD_Control;
                modeControl = 0;
                if (bEnableStatsAsyncEvent)
                    modeControl |= (byte)ControlModeFlags.CMF_EnableStatsAsyncEvent;
                rfidControl = 0;
                if (bStopScan)
                    rfidControl |= (byte)ControlControlFlags.CCF_StopScan;
            }
            /// <summary>
            /// Override function ToString() to display response of PBCMD_Control
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return string.Format("PbMsgRfidControl(AsyncEvents:{0}, StopScan:{1})",
                    ((modeControl & (byte)ControlModeFlags.CMF_EnableStatsAsyncEvent) != 0) ? "Y" : "N",
                    ((rfidControl & (byte)ControlControlFlags.CCF_StopScan) != 0) ? "Y" : "N"
                    );
            }
        };
    /// <summary>
        /// Class PbRspGetStatus : Response to PBCMD_Control..
        /// Variable class for marshal response of PBCMD_Control..
    /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRfidControl // Response to PBCMD_Control.
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
        };
       
    /// <summary>
        ///  PbMsgRfidBackDoor : request to PBCMD_BackDoor.
        /// Variable class for marshal response of PBCMD_BackDoor.
    /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgRfidBackDoor // Request for PBCMD_BackDoor.
        {
            /// <summary>
            /// Variable command : Message command. Must be CommandType.PBCMD_BackDoor.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// Variable byte backDoorCommand : Should be one of RfidBackDoorCommandType.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            byte backDoorCommand; 
            /// <summary>
            /// Variable  byte param1 : General purpose parameter depending on backDoorCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            byte param1;         
            /// <summary>
            /// Variable  UInt16 param2 : General purpose parameter depending on backDoorCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            UInt16 param2;      
            /// <summary>
            /// Variable  UInt16 param3 : General purpose parameter depending on backDoorCommand.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            UInt16 param3;
            /// <summary>            
            /// Variable const uint SIZE : Byte length of marshaled fields.
            /// </summary>          
            public const uint SIZE = 8; // Byte length of marshaled fields.
            /// <summary>
            /// Constructor of PbMsgRfidBackDoor class
            /// </summary>
            /// <param name="backDoorCommand">backDoorCommand : Should be one of RfidBackDoorCommandType.</param>
            /// <param name="param1">General purpose parameter depending on backDoorCommand.</param>
            /// <param name="param2">General purpose parameter depending on backDoorCommand.</param>
            /// <param name="param3">General purpose parameter depending on backDoorCommand.</param>
            public PbMsgRfidBackDoor(RfidCommandType backDoorCommand, byte param1, ushort param2, ushort param3)
            {
                command = (UInt16)CommandType.PBCMD_BackDoor;
                this.backDoorCommand = (byte)backDoorCommand;
                this.param1 = param1;
                this.param2 = param2;
                this.param3 = param3;
            }
            /// <summary>
            /// Override function ToString() to display message of PBCMD_BackDoor
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return string.Format("PbMsgRfidBackDoor({0}, {1}, {2}, {3})",
                    (RfidCommandType)backDoorCommand, param1, param2, param3);
            }
        };

        //-------------------------------------------------------------------------------------------------
        /// <summary>
        ///   /// Class PbRspGetStatus : Response to PBCMD_BackDoor.
        /// Variable class for marshal response of PBCMD_BackDoor
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRfidBackDoor // Response to PBCMD_BackDoor.
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary>         
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
            /// <summary>
            ///  byte responseByte1; :  Meaning varies based on backDoorCommand in request
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte responseByte1;
            /// <summary>
            /// UInt16 responseWord1 : Meaning varies based on backDoorCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 responseWord1;           
            /// <summary>
            ///  UInt32 responseLong1; : Meaning varies based on backDoorCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public UInt32 responseLong1;

            /// <summary>
            /// Override function ToString() to display response of PBCMD_BackDoor
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                return string.Format("PbRspRfidBackDoor({0}, {1}, {2}, {3})",
                    (ResponseCodeType)completionStatus, responseByte1, responseWord1, responseLong1);
            }
        };
        /// <summary>
        /// Class for retreive backdoor info command
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PBAE_BackDoorInfo
        {
            /// <summary>
            /// Variable  command: Message command. Must be CommandType.PBCMD_AsyncEvent.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;
            /// <summary>
            ///  Variable byte asyncEvent : Should be PBET_BackDoorInfo
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte asyncEvent; 
            /// <summary>
            /// Varaible byte backDoorEventType :  Indicates which back door information is provided.
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte backDoorEventType;
            /// <summary>
            /// Variable UInt16 value1 : Meaning varies based on backDoorCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 value1;
            /// <summary>
            /// Variable UInt16 value2 : Meaning varies based on backDoorCommand in request.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 value2;
        };

    /// <summary>
    /// Class for retrieve correlation sample series
    /// Response for PBCMD_BackDoor when backDoorCommand is RBDC_GetCorrelationCounts.
    /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbRspRfidBackdoorGetCorrelationCounts 
        {
            /// <summary>
            /// Variable byte completionStatus : ResponseCodeType.PBRC_SUCCESS if successful.
            /// </summary> 
            [MarshalAs(UnmanagedType.U1)]
            public byte completionStatus; 
            /// <summary>
            ///  byte correlationOffset :  The correlation corresponding to correlationCounts[0].
            /// </summary>
            [MarshalAs(UnmanagedType.U1)]
            public byte correlationOffset; 
            /// <summary>
            /// UInt16[] correlationCounts :  16 counts of correlation values.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public UInt16[] correlationCounts;
            /// <summary>
            /// Override function ToString() to display response of  RBDC_GetCorrelationCounts
            /// </summary>
            /// <returns>return a formated string of the results</returns>
            public override string ToString()
            {
                if ((ResponseCodeType)completionStatus != ResponseCodeType.PBRC_SUCCESS)
                    return ((ResponseCodeType)completionStatus).ToString();
                StringBuilder builder = new StringBuilder(80);
                builder.AppendFormat("CorrelationCounts[{0}]: ", correlationOffset);
                for (uint countIndex = 0; countIndex < correlationCounts.Length; countIndex++)
                    builder.AppendFormat("{0}{1}", (countIndex > 0) ? "," : "", correlationCounts[countIndex]);
                return builder.ToString();
            }
        };


        //-------------------------------------------------------------------------------------------------
    /// <summary>
    /// Class for Request an unlock door/drawer Unlock
    /// Request for PBCMD_DrawerUnlock.
    /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgDrawerUnlock 
        {
            /// <summary>
            ///  message command. Must be CommandType.PBCMD_DrawerUnlock.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command;  
            /// <summary>
            /// Size of field to marshal
            /// </summary>
            public const uint SIZE = 4; // Byte length of marshaled fields.
            /// <summary>
            /// Ushort to select door to unlock
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public ushort doorChoice;

            /// <summary>
            /// Constructor of class PbMsgDrawerUnlock
            /// </summary>
            public PbMsgDrawerUnlock(ushort doorChoice)
            {
                this.doorChoice = doorChoice;
                command = (ushort)CommandType.PBCMD_DrawerUnlock;
            }
        };
        /// <summary>
        /// Class for Request a lock door/drawer Unlock
        /// Request for PBCMD_Unlock.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class PbMsgDrawerLock // Request for PBCMD_DrawerLock.
        {
            /// <summary>
            /// message command. Must be CommandType.PBCMD_DrawerUnlock. 
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            ushort command; 
            /// <summary>
            /// total Size i=of the field byte to marshal
            /// </summary>
            public const uint SIZE = 4; // Byte length of marshaled fields.

            /// <summary>
            /// Ushort to select door to lock
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public ushort doorChoice;
            /// <summary>
            /// Constructor of the class PbMsgDrawerLock
            /// </summary>
            public PbMsgDrawerLock(ushort doorChoice)
            {
                this.doorChoice = doorChoice;
                command = (ushort)CommandType.PBCMD_DrawerLock;
            }
        };

        /// <summary>
        /// Class for set the power of the light
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public class pbMsgSetLightDuty
        {
            /// <summary>
            ///  Message command. Must be CommandType.PBCMD_DrawerSetLight.
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]            
            ushort command;
            /// <summary>
            /// Variable Duty cycle for power - from 0 to 300
            /// </summary>
            [MarshalAs(UnmanagedType.U2)]
            public ushort duty; // 32-bit device serial number.
            /// <summary>
            /// Total size of the field to marshal
            /// </summary>
            public const uint SIZE = 4; // Byte length of marshaled fields.
            /// <summary>
            /// Constructor of the class
            /// </summary>
            /// <param name="duty">Power to set</param>
            public pbMsgSetLightDuty(ushort duty)
            {
                this.duty = duty;
                command = (ushort)CommandType.PBCMD_DrawerSetLight;
            }
        };

    /// <summary>
    /// 
    /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PbMsgRfidBackDoorTagCharacterization // Request for RBDC_TagCharacterization.
{
	[MarshalAs(UnmanagedType.U2)]
	ushort command; // Pyxibus message command. Must be Pyxibus.CommandType.PBCMD_BackDoor.
	[MarshalAs(UnmanagedType.U1)]
	byte backDoorCommand; // Must be RBDC_TagCharacterization.
	[MarshalAs(UnmanagedType.U2)]
	UInt16 dutyCycle; // Full-bridge duty cycle at which to run the test.
	[MarshalAs(UnmanagedType.U2)]
	UInt16 maxSampleCount; // Maximum number of samples to acquire.
	[MarshalAs(UnmanagedType.I2)]
	Int16 minPhaseLimit; // Lower limit of in-phase phase shift in degrees.
	[MarshalAs(UnmanagedType.I2)]
	Int16 maxPhaseLimit; // Upper limit of in-phase phase shift in degrees.
	[MarshalAs(UnmanagedType.U1)]
	byte bStopOnBadPhase; // True to stop when an out-of-phase sample is detected.
    /// <summary>
    /// 
    /// </summary>
	public const uint SIZE = 12; // Byte length of marshaled fields.
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dutyCycle"></param>
    /// <param name="maxSampleCount"></param>
    /// <param name="minPhaseLimit"></param>
    /// <param name="maxPhaseLimit"></param>
    /// <param name="bStopOnBadPhase"></param>
	public PbMsgRfidBackDoorTagCharacterization(uint dutyCycle, uint maxSampleCount,
		int minPhaseLimit, int maxPhaseLimit, bool bStopOnBadPhase)
	{
		command = (UInt16)CommandType.PBCMD_BackDoor;
		this.backDoorCommand = (byte)RfidCommandType.RBDC_StartTagCharacterization;
		this.dutyCycle = (ushort)dutyCycle;
		this.maxSampleCount = (ushort)maxSampleCount;
		this.minPhaseLimit = (short)minPhaseLimit;
		this.maxPhaseLimit = (short)maxPhaseLimit;
		this.bStopOnBadPhase = (byte)(bStopOnBadPhase ? 1 : 0);
	}
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
	public override string ToString()
	{
		return string.Format("PbMsgRfidBackDoorTagCharacterization({0}, {1}, {2}, {3}, {4})",
			dutyCycle, maxSampleCount, minPhaseLimit, maxPhaseLimit, bStopOnBadPhase);
	}
};

//-------------------------------------------------------------------------------------------------
    /// <summary>
    /// 
    /// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PbRspRfidBackDoorGetTagCharacterizationResults // Response for PBCMD_BackDoor when backDoorCommand is RBDC_TagCharacterization.
{
    /// <summary>
    /// 
    /// </summary>
	[MarshalAs(UnmanagedType.U1)]
	public byte completionStatus; // ResponseCodeType.PBRC_SUCCESS if successful.
    /// <summary>
    /// 
    /// </summary>
	[MarshalAs(UnmanagedType.U2)]
	public UInt16 carrierVoltage;  // Peak-to-peak A/D measurement of carrier voltage.
	/// <summary>
	/// 
	/// </summary>
    [MarshalAs(UnmanagedType.U2)]
	public UInt16 correlationOkCount; // Number of samples with good correlation counts.
	/// <summary>
	/// 
	/// </summary>
    [MarshalAs(UnmanagedType.U2)]
	public UInt16 inPhaseCount;    // Number of samples that were in phase.
	/// <summary>
	/// 
	/// </summary>
    [MarshalAs(UnmanagedType.I2)]
	public Int16 inPhaseAverage;   // Average of samples that were in phase.
    /// <summary>
    /// 
    /// </summary>
	[MarshalAs(UnmanagedType.U2)]
    public UInt16 outPhaseCount;   // Number of samples that were out of phase.
	/// <summary>
	/// 
	/// </summary>
    [MarshalAs(UnmanagedType.I2)]
	public Int16 outPhaseAverage;  // Average of samples that were out of phase.

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
	public override string ToString()
	{
		if((ResponseCodeType)completionStatus != ResponseCodeType.PBRC_SUCCESS)
			return ((ResponseCodeType)completionStatus).ToString();
		return string.Format("PbRspRfidBackDoorGetTagCharacterizationResults({0}, {1}, {2}, {3}, {4})",
			carrierVoltage, inPhaseCount, inPhaseAverage, outPhaseCount, outPhaseAverage);
	}
};
/// <summary>
/// 
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PbRspRfidBackdoorGetCarrierSignal // Response for PBCMD_BackDoor when backDoorCommand is RBDC_GetCarrierSignal.
{
    /// <summary>
    /// 
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public byte completionStatus; // ResponseCodeType.PBRC_SUCCESS if successful.
    /// <summary>
    /// 
    /// </summary>
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 __retired__; // No longer used.
    /// <summary>
    /// 
    /// </summary>
    [MarshalAs(UnmanagedType.U2)]
    public UInt16 dutyCycle; // Current duty cycle.
    /// <summary>
    /// 
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public sbyte[] signalValues; // 32 bytes of tag response signal data.
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        if ((ResponseCodeType)completionStatus != ResponseCodeType.PBRC_SUCCESS)
            return ((ResponseCodeType)completionStatus).ToString();
        StringBuilder builder = new StringBuilder(80);
        builder.AppendFormat("PbRspRfidBackdoorGetCarrierSignal(dutyCycle={0}, ",
            dutyCycle);
        builder.AppendFormat("{0}", signalValues[0]);
        for (uint sampleIndex = 1; sampleIndex < signalValues.Length; sampleIndex++)
            builder.AppendFormat("{0},", signalValues[sampleIndex]);
        return builder.ToString();
    }
};


/// <summary>
/// Class PBAE_RfidTagFullMem : Data class for exchange Uid when CommandType.PBCMD_AsyncEvent is readfull mem
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PBAE_RfidTagFullMem
{
    /// <summary>
    /// Variable ushort command : Message command. Must be CommandType.PBCMD_AsyncEvent.
    /// </summary>
    [MarshalAs(UnmanagedType.U2)]
    ushort command;
    /// <summary>
    /// Variable byte asyncEvent :  Should be PBET_TagAdded
    /// </summary>
    [MarshalAs(UnmanagedType.U1)]
    public byte asyncEvent;
    /// <summary>
    /// Variable byte[] serialNumber : 60-bit Tag serial number, stored as big-endian.
    /// </summary>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 42)]
    public byte[] TagId;
};

//-------------------------------------------------------------------------------------------------
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PbMsgRfidBackDoorConfirmationFullMem // Request for RBDC_TConfirmationFullMem.
{
    [MarshalAs(UnmanagedType.U2)]
    ushort command; // Pyxibus message command. Must be Pyxibus.CommandType.PBCMD_BackDoor.
    [MarshalAs(UnmanagedType.U1)]
    byte backDoorCommand; // Must be RBDC_ConfirmationFullMem.
    [MarshalAs(UnmanagedType.U1)]
    byte nbDigitToConfirm; // Nb Digit To confirm
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    byte[] TagDigitToConfirm; // 42 tag digit to confirm

    public const uint SIZE = 20; // Byte length of marshaled fields.

    public PbMsgRfidBackDoorConfirmationFullMem(byte nbDigitToConfirm, byte[] TagDigitToConfirm)
    {
        command = (UInt16)CommandType.PBCMD_BackDoor;
        this.backDoorCommand = (byte)RfidCommandType.RBDC_ConfirmUIDFullMem;
        this.nbDigitToConfirm = nbDigitToConfirm;
        this.TagDigitToConfirm = TagDigitToConfirm;
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 16; i++)
            sb.AppendFormat("{0}", TagDigitToConfirm[i]);

        return string.Format(" PbMsgRfidBackDoorConfirmationFullMem({0}, {1})",
            nbDigitToConfirm, sb.ToString());
    }
};

    #endregion




}
