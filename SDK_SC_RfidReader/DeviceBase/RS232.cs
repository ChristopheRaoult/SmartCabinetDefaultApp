using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SDK_SC_RfidReader.DeviceBase
{
   
    /// <summary>
    /// Base class for handling a communication through a RS232 port.
    /// </summary>
    public abstract class RS232Port:IDisposable
    {
        /// <summary>
        /// Constant variable which define start char of the frame.
        /// </summary>
        const char CHAR_StartOfFrame = (char)0x0A;
        /// <summary>
        /// Constant variable which define end char of the frame.
        /// </summary>
        const char CHAR_EndOfFrame = (char)0x0D;

        /// <summary>
        /// Variable of the serialport class
        /// </summary>
        public SerialPort serialPort;
        /// <summary>
        /// Variable that accumulate data from reception buffer.
        /// </summary>
        private string inboundBuffer = "";
        /// <summary>
        /// Bool for process message from a helloworld Request
        /// </summary>
        private bool bHelloWorldProcess = true;
        /// <summary>
        /// String message to send to the board for requesting a helloworld
        /// </summary>
        const string HelloWorld = "TUNNEL|4857";
        /// <summary>
        /// String for storing the serial number of the board
        /// </summary>
        private string strSerialNumber;

       
        /// <summary>
        /// Variable AutoResetEvent for notify message when helloworld request
        /// </summary>
        private readonly AutoResetEvent ReceiveEvent = new AutoResetEvent(false);

        /// <summary>
        /// Method definition declare in RS232DeviceChannel Class
        /// Function to launch when a full message is available.
        /// </summary>
        /// <param name="message">String of the full message to process</param>
        public abstract void OnMessageReceived(string message);

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
                    if (serialPort != null) serialPort.Dispose();
                    if (ReceiveEvent != null) ReceiveEvent.Close();
                }
            }
        }

       /// <summary>
       /// Method for open the serial port
       /// </summary>
       /// <param name="PortCom">String of the serial port to open "COMi"</param>
       /// <param name="serialnumber">Uint to store the serial number of the board</param>
       /// <returns>true if succeed, false otherwise</returns>     
        public bool OpenSerialPort(string PortCom, out uint serialnumber)
        {
            serialnumber = 0x0000000;
            bool bRet = true;
            try
            {
                serialPort = null;
                serialPort = new SerialPort(PortCom, 115200, Parity.None, 8, StopBits.One);
                serialPort.Open();

                GC.SuppressFinalize(serialPort.BaseStream);

                serialPort.RtsEnable = false;
                serialPort.DataReceived += new SerialDataReceivedEventHandler(OnDataReceived);
                serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(OnErrorReceived);
                serialPort.PinChanged += new SerialPinChangedEventHandler(OnSerialPinChanged);
                sendMessage(HelloWorld);
                if (ReceiveEvent.WaitOne(2000, false))
                {
                    bHelloWorldProcess = false;
                    bRet = true;
                    serialnumber = UInt32.Parse(strSerialNumber, System.Globalization.NumberStyles.HexNumber);
                }
                else
                {
                    bRet = false;
                }
                return bRet;
            }
            catch 
            {

                return false;
            }
           
        }
        /// <summary>
        /// Method to close the serial port
        /// </summary>
        /// <returns>true if succeed, false otherwise</returns>    
        public bool CloseSerialPort()
        {
            try
            {
                serialPort.DataReceived -= new SerialDataReceivedEventHandler(OnDataReceived);
                serialPort.ErrorReceived -= new SerialErrorReceivedEventHandler(OnErrorReceived);
                serialPort.PinChanged -= new SerialPinChangedEventHandler(OnSerialPinChanged);

                if (serialPort.IsOpen)
                {
                    GC.ReRegisterForFinalize(serialPort.BaseStream);
                    serialPort.Close();
                }

                serialPort = null;
                bHelloWorldProcess = true;
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            catch
            {
                return false;
                // throw new Exception();
            }
        }
        /// <summary>
        /// Calculate and verify CRC of incoming message
        /// </summary>
        /// <param name="message">Message to process</param>   
        /// <returns>1 if the message is valid 0 otherwise</returns>
        private bool HasValidCRC(string message)
        {
            if (message.Length <= 4)
                return false;
            int crcLocation = message.Length - 4;
            string messageSansCRC = message.Substring(0, crcLocation);
            UInt16 messageCRC = Convert.ToUInt16(message.Substring(crcLocation, 4), 16);
            return (messageCRC == (UInt16)~CRC.calculatePbCRC(messageSansCRC));
        }

        /// <summary>
        /// Send a message on the communication channel
        /// </summary>
        /// <param name="message">Message to send</param>          
        protected virtual void sendMessage(string message)
        {
            try
            {
                UInt16 crc = (UInt16)~CRC.calculatePbCRC(message);
                serialPort.Write(string.Format("\n{0}{1:X4}\r", message, crc));
            }
            catch
            {
                //throw new Exception();
            }
        }
        /// <summary>
        /// Function to receive and parse incoming  message
        /// </summary>       
        public void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            while (true)
            {
                inboundBuffer += serialPort.ReadExisting();
                int length = inboundBuffer.Length;
                int frameStart = inboundBuffer.IndexOf(CHAR_StartOfFrame);
                if (frameStart != 0)
                {
                    if (frameStart < 0)
                    {
                        inboundBuffer = "";
                        break;
                    }
                    inboundBuffer = inboundBuffer.Substring(frameStart);
                    frameStart = 0;
                }

                int frameEnd = inboundBuffer.IndexOf(CHAR_EndOfFrame, 0);
                if (frameEnd < 0)
                    break;

                // A full message is available.
                string message = inboundBuffer.Substring(1, frameEnd - 1);
                inboundBuffer = inboundBuffer.Substring(frameEnd + 1);
                if (bHelloWorldProcess)
                {
                    if (message.Length > 12)
                        strSerialNumber = message.Substring(15, 2) + message.Substring(13, 2) + message.Substring(11, 2) + message.Substring(9, 2);

                    ReceiveEvent.Set();
                }
                else if (HasValidCRC(message))
                {
                    message = message.Substring(0, message.Length - 4); // Remove the CRC.
                    OnMessageReceived(message);
                } 
                //Else CRC non valid , remove la frame
            }
        }
        /// <summary>
        /// Function to recovery the error
        /// </summary>       
        public void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialError.Frame:
                    break;
                case SerialError.Overrun:
                    break;
                case SerialError.RXOver:
                    break;
                case SerialError.RXParity:
                    break;
                case SerialError.TXFull:
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Method to notify event on the serial port.
        /// </summary>       
        public void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
           // MessageBox.Show("An event occur on power detection", Properties.ResStrings.strInfo);
            switch (e.EventType)
            {
               
                case SerialPinChange.Break:
                    
                    break;
                case SerialPinChange.CDChanged:
                    break;
                case SerialPinChange.CtsChanged:
                    try
                    {
                        if (!(sender as SerialPort).CtsHolding) OnMessageReceived("PowerOFF");
                        else OnMessageReceived("PowerON");
                    }
                    catch(Exception exp)
                    {
                        string mes = "Error reading CTS :" + exp.Message;
                        //MyDebug.Assert(false,"Error in OnSerialPinChanged : " + mes );
                        //throw new Exception();
                    }
                    break;
                case SerialPinChange.DsrChanged:
                    break;
                case SerialPinChange.Ring:
                    
                    break;
                default:
                    break;
            }
        }
    }
}
