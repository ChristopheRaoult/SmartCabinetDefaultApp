using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Class RS232channel from base class RS232 with IDeviceChannel interface
    /// This class provide variable and high level method for communicate through an serial port
    /// </summary>
    public class RS232DeviceChannel : RS232Port, IDeviceChannel
    {

        object lockObj = new object();
        /// <summary>
        /// Variable uint theDeviceId  : the serial number of the board
        /// </summary>
        public uint theDeviceId = 0x00000000;
        /// <summary>
        /// const DeviceTypeMajorType : Major type of the board
        /// </summary>
        public const DeviceTypeMajorType deviceTypeMajor = DeviceTypeMajorType.DEVICE_UNKNOWN;
        /// <summary>
        /// const DeviceTypeMinorType : Minor type of the board
        /// </summary>
        private const DeviceTypeMinorType deviceTypeMinor = DeviceTypeMinorType.DT_RfidDeviceTypeMinor;

        /// <summary>
        ///  AutoResetEvent completionEvent : event occurs when a completion message is received
        /// </summary>
        private readonly AutoResetEvent completionEvent = new AutoResetEvent(false);
        /// <summary>
        /// Queue byte[]  asyncEventQueue : contain the list of valid asynchronous message to process
        /// </summary>
        private readonly Queue<byte[]> asyncEventQueue = new Queue<byte[]>();
        /// <summary>
        /// AutoResetEvent asyncEventEvent  : event occurs when an asynchronous message is received
        /// </summary>
        private readonly AutoResetEvent asyncEventEvent = new AutoResetEvent(false);

        /// <summary>
        /// Queue inboundMessageQueue : Contain list of message during discover process
        /// </summary>
        public readonly Queue<string> inboundMessageQueue = new Queue<string>();
        /// <summary>
        /// AutoResetEvent inboundMessageEvent : event when message arrive during discover process
        /// </summary>
        public readonly AutoResetEvent inboundMessageEvent = new AutoResetEvent(false);

        /// <summary>
        /// byte[] completionPacket : contain message receive during a completion response
        /// </summary>
        private byte[] completionPacket = null;
        /// <summary>
        /// bool cancelOps : if true launch event with message null to stop thread event loop
        /// </summary>
        private bool cancelOps = false;
               
       

        /// <summary>
        /// Contructor of RS232DeviceChannel
        /// </summary>
        /// <param name="IdToGive">Dummy variable for discover process</param>
        public RS232DeviceChannel(uint IdToGive)
        {
            theDeviceId = IdToGive;
        }
        /// <summary>
        /// Property to retrieve or set the value of the serial number
        /// </summary>
        public uint TheDeviceId { get { return theDeviceId; } set { theDeviceId = value; } }

        private ErrorBoard errBoard = new ErrorBoard();
        /// <summary>
        /// Property to get and set last error board
        /// </summary>
        public ErrorBoard ErrBoard { get { return errBoard; } set { errBoard = value; } }
        
        /// <summary>
        /// Overide method RS232Port.OnMessageReceived(). This is called by the base class when a complete
        /// inbound message has been received.
        /// </summary>
        /// <param name="message">full message receive to sort and process</param>
        public override void OnMessageReceived(string message)
        {
            const string completionTag = "Tunnel|";
            const string asyncEventTag = "AsyncEvent|";
            if (message.StartsWith(completionTag))
            {
               // MyDebug.Assert(completionPacket == null,"Error in OnMessageReceived 1");
                
                int dstBytes = (message.Length - completionTag.Length) / 2;
                byte[] newCompletionPacket = new byte[dstBytes];
                int srcIndex = completionTag.Length;
                uint dstIndex = 0;
                while (srcIndex < message.Length)
                {
                    newCompletionPacket[dstIndex++] = (byte)Convert.ToUInt32(message.Substring(srcIndex, 2), 16);
                    srcIndex += 2;
                }
                MyDebug.Assert(dstIndex == dstBytes, "Error in OnMessageReceived 2");
                
                completionPacket = newCompletionPacket;
                completionEvent.Set();
            }
            else if (message.StartsWith(asyncEventTag))
            {
                int dstBytes = 2 + (message.Length - asyncEventTag.Length) / 2;
                byte[] newAsyncEventPacket = new byte[dstBytes];
                int srcIndex = asyncEventTag.Length;
                ushort command = (ushort)CommandType.PBCMD_AsyncEvent;
                newAsyncEventPacket[0] = (byte)(command & 0xFF);
                newAsyncEventPacket[1] = (byte)((command >> 8) & 0xFF);
                uint dstIndex = 2;
                while (srcIndex < message.Length)
                {
                    newAsyncEventPacket[dstIndex++] = (byte)Convert.ToUInt32(message.Substring(srcIndex, 2), 16);
                    srcIndex += 2;
                }
                MyDebug.Assert(dstIndex == dstBytes, "Error in OnMessageReceived 3");

                lock (lockObj)
                {
                asyncEventQueue.Enqueue(newAsyncEventPacket);
                }

                asyncEventEvent.Set();
            }
            else if (message.StartsWith("PowerOFF"))
            {
                int dstBytes = 10;
                byte[] newAsyncEventPacket = new byte[dstBytes];
                int srcIndex = asyncEventTag.Length;
                ushort command = (ushort)CommandType.PBCMD_AsyncEvent;
                newAsyncEventPacket[0] = (byte)(command & 0xFF);
                newAsyncEventPacket[1] = (byte)((command >> 8) & 0xFF);
                uint dstIndex = 2;
                newAsyncEventPacket[dstIndex++] = (byte)Convert.ToUInt32("FF",16);
                srcIndex += 2;
                newAsyncEventPacket[dstIndex++] = (byte)Convert.ToUInt32("FF",16);
                srcIndex += 2;

                lock (lockObj)
                {
                asyncEventQueue.Enqueue(newAsyncEventPacket);
                }
                asyncEventEvent.Set();
            }
            else if (message.StartsWith("PowerON"))
            {
                int dstBytes = 10;
                byte[] newAsyncEventPacket = new byte[dstBytes];
                int srcIndex = asyncEventTag.Length;
                ushort command = (ushort)CommandType.PBCMD_AsyncEvent;
                newAsyncEventPacket[0] = (byte)(command & 0xFF);
                newAsyncEventPacket[1] = (byte)((command >> 8) & 0xFF);
                uint dstIndex = 2;
                newAsyncEventPacket[dstIndex++] = (byte)Convert.ToUInt32("EE", 16);
                srcIndex += 2;
                newAsyncEventPacket[dstIndex++] = (byte)Convert.ToUInt32("EE", 16);
                srcIndex += 2;
                lock (lockObj)
                {
                asyncEventQueue.Enqueue(newAsyncEventPacket);
                }
                asyncEventEvent.Set();
            }
            else
            {
                if (inboundMessageQueue != null)
                {
                    inboundMessageQueue.Enqueue(message);
                    inboundMessageEvent.Set();
                    
                }
            }
        }
        /// <summary>
        /// Method to send a comman to the board and receive a completion
        /// </summary>
        /// <param name="targetDevice">serial number of the device</param>
        /// <param name="messageContent">Message to send</param>
        /// <returns>the class object in regards of the request expected response</returns>
        public ResponseType sendMessageWaitResponse(uint targetDevice, byte[] messageContent)
        {
            const string command = "TUNNEL|";


            // Build the embedded message for tunneling.
            StringBuilder message = new StringBuilder(command);
            foreach (byte messageByte in messageContent)
                message.AppendFormat("{0:X2}", messageByte);

            while (true)
            {
                // Send the message.
                base.sendMessage(message.ToString());

                // Wait for the response.
                if (completionEvent.WaitOne(2000, false))
                    break;

                // Timed out waiting for response.
               /* if (MessageBox.Show("Device not responding. Try again?", "Device not responding.",
                    MessageBoxButtons.RetryCancel) == DialogResult.Retry)
                {
                    continue;
                }*/

                // User clicked Cancel.
                //throw new Exception("Device not responding.");
               
                errBoard.dt = DateTime.Now;
                errBoard.message = message.ToString();

                return null;
            }

            // Completion message is ready or operation was canceled.
            if (completionPacket == null)
                return null;

            // Completion message has been received. Convert it into the expected format. Leave the
            // message header fields unset since they aren't used. KB000: Return just the packet?
            ResponseType response = new ResponseType();
            response.serialMessage = completionPacket; // Pyxibus packet.
            completionPacket = null;
            return response;
        }

        /// <summary>
        /// Send Message and check and expected response
        /// </summary>
        /// <param name="message">Message to send</param>
        /// <param name="expectedResponse">Response to test</param>
        public void sendMessageExpectResponse(string message, string expectedResponse)
        {
            string response = sendMessageWaitResponse(message);
            if (response != expectedResponse)
            {
                if (response == null)
                    response = "--no response--";
                throw new Exception(string.Format("Sent {0}; expected {1}; received {2}.",
                    message, expectedResponse, response));
            }
        }
        /// <summary>
        /// Function to return last frame in serial buffer reception queue
        /// </summary>
        /// <param name="message">message to send</param>
        /// <returns>Last frame receive</returns>
        public string sendMessageWaitResponse(string message)
        {
            inboundMessageEvent.Reset();
            inboundMessageQueue.Clear();
            base.sendMessage(message);
            while (true)
            {
                if (!inboundMessageEvent.WaitOne(2000, true))
                    return null;
                if (inboundMessageQueue.Count > 0)
                    return inboundMessageQueue.Dequeue();
            }
        }

        /// <summary>
        /// Method for receive asynchronous message
        /// This method blocks until an inbound asynchronous event is received or cancelPendingOperations()
        ///is called.
        /// </summary>
        /// <returns>a message in a serialMessage type class</returns>
        public SerialMessageType waitNextAsyncEvent()
        {
            try
            {
            while (asyncEventQueue.Count == 0)
            {
                // Wait for an asynchronous event.
                    asyncEventEvent.WaitOne(Timeout.Infinite, false);

                // Asynchronous event is ready or operation was canceled.
                if (cancelOps)
                    return null;
            }

            // Asynchronous event has been received. Convert it into the expected format.
            byte[] messagePacket = null;

            lock (lockObj)
            {
                messagePacket = asyncEventQueue.Dequeue();
            }

            if (messagePacket == null) return null;
            AsyncEventMessage asyncEventMessage = new AsyncEventMessage();
            asyncEventMessage.byteCount = (byte)(SerialMessageType.LENGTH_SerialMessageHeader + messagePacket.Length); // Length of message.
            asyncEventMessage.messageType = (byte)PbMessageType.ASYNC_MSG_FROM_PB;
            asyncEventMessage.messageId = 0; // Unique ID identifying this transaction; responses use the same messageID.
            //  asyncEventMessage.deviceId = deviceId; // ID of  device, if applicable.
            asyncEventMessage.deviceId = TheDeviceId; // ID of  device, if applicable.
            asyncEventMessage.timeoutMSecs = 0; // Maximum allowable device response time, if applicable.
            asyncEventMessage.errorCode = 0; // Return error code, if applicable.
            asyncEventMessage.deviceTypeMajor = (byte)deviceTypeMajor; // Device type major, if applicable.
            asyncEventMessage.deviceTypeMinor = (byte)deviceTypeMinor; // Device type minor, if applicable.
            asyncEventMessage.spare1 = 0; // Extra data depending on message type.
            asyncEventMessage.serialMessage = messagePacket;

            return asyncEventMessage;
        }
            catch (Exception exp)
            {
                MessageBox.Show("exception async Message : " + exp.Message);
                return null;
            }

        }
        /// <summary>
        /// Method to stop the asynchronous process
        /// </summary>
        public void cancelPendingOperations()
        {
            cancelOps = true;
            completionEvent.Set();
            asyncEventEvent.Set();
        }
    }

    
}
