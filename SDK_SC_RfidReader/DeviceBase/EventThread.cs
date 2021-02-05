using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace SDK_SC_RfidReader.DeviceBase
{
    /// <summary>
    /// Class for delegaye asynchronous receive message.
    /// </summary>
    public class EventThread
    {
        /// <summary>
        /// Delegate of the event consumer 
        /// </summary>
        /// <param name="asyncEventMessage">variable asyncEventMessage to process</param>
        public delegate void ConsumeEventDelegate(AsyncEventMessage asyncEventMessage);
        /// <summary>
        /// Variable of the deviceChannel that receive the asynchronous Event
        /// </summary>
        private readonly IDeviceChannel deviceChannel;
        /// <summary>
        /// Variable of the consumer event
        /// </summary>
        private readonly ConsumeEventDelegate eventDelegate;
        /// <summary>
        /// Thread Variable 
        /// </summary>
        private readonly Thread eventThread;
        /// <summary>
        /// bool for request end of the thread
        /// </summary>
        private bool stop = false;
        /// <summary>
        /// Constructo of the class
        /// </summary>
        /// <param name="deviceChannel">involved deviceChannel for the event thread</param>
        /// <param name="eventDelegate">Delegate of the methof send when message received</param>
        public EventThread(IDeviceChannel deviceChannel,          
                           ConsumeEventDelegate eventDelegate)
        {
            this.deviceChannel = deviceChannel;          
            this.eventDelegate = eventDelegate;
            eventThread = new Thread(eventThreadProc);
            eventThread.Name = "Serial Async Events";
            eventThread.IsBackground = true;
            eventThread.Start();
        }
        /// <summary>
        /// Method of the thread to process asynchronous Message
        /// </summary>
        private void eventThreadProc()
        {
            try
            {
                while (!stop)
                {
                    SerialMessageType serialMessage = deviceChannel.waitNextAsyncEvent();
                    if (serialMessage != null)
                    {
                        switch ((PbMessageType)serialMessage.messageType)
                        {
                            case PbMessageType.ASYNC_MSG_FROM_PB:
                            {
                                AsyncEventMessage asyncEventMessage = (AsyncEventMessage)serialMessage;                                
                                eventDelegate(asyncEventMessage);
                                break;
                            }
                            default:
                            {
                                Debug.Assert(false, "Error in  eventThreadProc() : " +
                                    ((PbMessageType)serialMessage.messageType).ToString());
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
               // MessageBox.Show(DateTime.Now.ToString() + " : " + exp.Source + " : " + exp.TargetSite + " : " + exp.Message);

                string LogPath;
                string mes = DateTime.Now.ToString() + " : ERROR : " + exp.Source + " : " + exp.TargetSite + " : " + exp.Message;
                DateTime TCurrent = DateTime.Now;
                DateTime TRef = new DateTime(TCurrent.Year,
                                                      TCurrent.Month,
                                                      TCurrent.Day,
                                                      12, 0, 0);

                        int res = DateTime.Compare(TCurrent, TRef);
                        if (res < 0)
                            LogPath = @"c:\temp\RfidTracking\log\ReaderLog" + string.Format("[{0}]_{1}_AM.txt", deviceChannel.TheDeviceId, DateTime.Now.Date.ToString("dd_MM_yyyy"));

                        else
                            LogPath = @"c:\temp\RfidTracking\log\ReaderLog" + string.Format("[{0}]_{1}_PM.txt", deviceChannel.TheDeviceId, DateTime.Now.Date.ToString("dd_MM_yyyy"));
                        WriteToLog(LogPath, mes, TCurrent);
                //throw new Exception();
                
            }
        }        

        /// <summary>
        /// Method to request end of the thread
        /// </summary>
        public void stopThread()
        {
            stop = true;
            
          
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
                log.Write(dt.ToString("s") + ":" + currentMS.ToString("000") + message);
                // Close the stream:
                log.Close();
            }
            catch 
            {
                //throw new Exception();
            }

        }
    }
}
