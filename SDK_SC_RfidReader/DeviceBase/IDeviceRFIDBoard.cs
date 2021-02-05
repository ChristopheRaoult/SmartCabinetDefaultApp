using System;
using System.Collections.Generic;
using System.Text;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Interface of Class deviceRfidBoardClass
    /// </summary>
    public interface IDeviceRfidBoard
    {   
        /// <summary>
        /// Property to retrieve serial number of the device.
        /// </summary>
        uint DeviceId { get; set; }
        /// <summary>
        /// Method to request the connection of the board to the serial port.
        /// </summary>
        /// <returns>true if succeed to connect</returns>
        bool ConnectBoard();
        /// <summary>
        /// Method to request the disconnection of the board to the serial port.
        /// </summary>
        /// <returns>true if succeed to disconnect</returns>
        bool DisconnectBoard();
        /// <summary>
        /// Property to retrieve the connection status.
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// property to get software version
        /// </summary>
        string FirmwareVersion { get; }
       

    }
}
