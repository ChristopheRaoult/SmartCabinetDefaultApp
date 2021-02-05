using System;
using System.Collections.Generic;
using System.Text;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Interface for Rs232Channel class
    /// </summary>
    public interface IDeviceChannel
    {
        /// <summary>
        /// Method for open the the serial port
        /// </summary>
        /// <param name="PortCom">serial port com number in string format "COM1"</param>
        /// <param name="serialnumber">serial number of the board</param>
        /// <returns>true if succeed</returns>
        bool OpenSerialPort(string PortCom, out uint serialnumber);
        /// <summary>
        /// Method to close the serial port
        /// </summary>
        /// <returns>true if succeed</returns>
        bool CloseSerialPort();
        /// <summary>
        /// Property to get and set serial number 
        /// </summary>
        uint TheDeviceId { get; set; }
        /// <summary>
        /// Method to send a specific message to a device.
        /// </summary>
        /// <param name="targetDevice">serial board number to which message will be send</param>
        /// <param name="messageContent">Message value in a byte array</param>
        /// <returns>the struct or class in relation of the request with filled data</returns>
        ResponseType sendMessageWaitResponse(uint targetDevice, byte[] messageContent);
        /// <summary>
        /// Send message for firmware mode
        /// </summary>
        /// <param name="message">message to send</param>
        /// <returns>string response</returns>
        string sendMessageWaitResponse(string message);
        /// <summary>
        /// Method to send message and test specific response
        /// </summary>
        /// <param name="message">message to send</param>
        /// <param name="expectedResponse">Response to test</param>
        void sendMessageExpectResponse(string message, string expectedResponse);
        /// <summary>
        /// delegate message for handle asynchronous message.
        /// </summary>
        /// <returns>the asynchronous message received</returns>
        SerialMessageType waitNextAsyncEvent();
        /// <summary>
        /// Method to set event for terminate event thread loop.
        /// </summary>
        void cancelPendingOperations();
        /// <summary>
        /// Property of the last erreor board
        /// </summary>
        ErrorBoard ErrBoard { get; set; }
     
    }
}
