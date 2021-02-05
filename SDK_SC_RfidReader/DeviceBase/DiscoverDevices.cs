using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO.Ports;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Class to search and find device
    /// </summary>
    public class DiscoverDevices:IDisposable
    {
        const char CHAR_StartOfFrame = (char)0x0A;
        const char CHAR_EndOfFrame = (char)0x0D;
        private SerialPort serialPort;
        private string inboundBuffer = "";
        const string HelloWorld = "TUNNEL|4857";
        private string serialNumber;
        private readonly AutoResetEvent ReceiveEvent = new AutoResetEvent(false);

        private int nbStartFrame;

        private bool _disposed = false;
        /// <summary>
        /// Dispose method
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
        /// Method to test a device on a particular com
        /// </summary>
        /// <param name="PortCom">Serial port to tesr</param>
        /// <param name="serial"> return the serial number found</param>
        /// <returns>true if a device is present , false otherwise</returns>
        public bool TestSerialPort(string PortCom , out string serial)
        {
            bool bRet = true;
            serial = null;
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

                
                nbStartFrame = 0;
                if (ReceiveEvent.WaitOne(2000, false))
                {
                    serial = serialNumber;
                    bRet = true;
                }
                else
                {
                    bRet = false;
                }
                serialPort.Close();
                return bRet;
            }
            catch 
            {
                return false;
            }
            
        }
        /// <summary>
        /// Method to send message on port
        /// </summary>
        /// <param name="message">message to send</param>
        protected virtual void sendMessage(string message)
        {
            UInt16 crc = (UInt16)~CRC.calculatePbCRC(message);
            serialPort.Write(string.Format("\n{0}{1:X4}\r", message, crc));
        }

        private void OnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            const string completionTag = "Tunnel|";
            while (true)
            {
                try
                {
                    //if ((serialPort.BytesToRead > 5) && (serialPort.BytesToRead < 100))
                    inboundBuffer += serialPort.ReadExisting();
                    //else
                    //   break;
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                catch
                {
                    break;
                    //throw new Exception();
                }
                int length = inboundBuffer.Length;

                if (length > 100) break;
                if (nbStartFrame > 5) break;
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
                    nbStartFrame++;
                }

                int frameEnd = inboundBuffer.IndexOf(CHAR_EndOfFrame, 0);
                if (frameEnd < 0)
                    break;

                // A full message is available.
                string message = inboundBuffer.Substring(1, frameEnd - 1);
                inboundBuffer = inboundBuffer.Substring(frameEnd + 1);

                if (!message.StartsWith(completionTag)) break;

                if (message.Length > 12)
                {
                    serialNumber = message.Substring(15, 2) + message.Substring(13, 2) + message.Substring(11, 2) + message.Substring(9, 2);
                    
                }
                else
                    break;
                ReceiveEvent.Set();

            }
        }

       private void OnErrorReceived(object sender, SerialErrorReceivedEventArgs e)
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
        private void OnSerialPinChanged(object sender, SerialPinChangedEventArgs e)
        {
            switch (e.EventType)
            {
                case SerialPinChange.Break:
                    break;
                case SerialPinChange.CDChanged:
                    break;
                case SerialPinChange.CtsChanged:
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
