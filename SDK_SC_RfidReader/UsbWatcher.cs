using System;
using System.Collections.Generic;
using System.Text;
using System.Management;
using System.Windows.Forms;

namespace SDK_SC_RfidReader
{
    /// <summary>
    /// Class to notify event on USB for unplug cable action
    /// </summary>
    public class USBWatcher:IDisposable
    {
        private ManagementEventWatcher m_watcher;

        private DateTime lastEventUsb;
        private DateTime currentEventUsb;

    /// <summary>
    /// 
    /// </summary>
        public event EventHandler DeviceEvent;
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
            if (disposing)
            {
                // Free other state (managed objects).
                Stop();              
              
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
        }       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pollingInterval"></param>
     public void Start(int pollingInterval)
    {
        lastEventUsb = DateTime.Now;
         try
            {
                string queryString =
                "SELECT * " +
                " FROM __InstanceOperationEvent" +
                " WITHIN " + pollingInterval +
                    //" WHERE TargetInstance ISA 'Win32_DiskDrive'";
                " WHERE TargetInstance ISA 'Win32_PnPEntity'";
                

            EventQuery processQuery = new EventQuery(queryString);

            m_watcher = new ManagementEventWatcher(processQuery);
            m_watcher.EventArrived += new System.Management.EventArrivedEventHandler(EventArrived);
            m_watcher.Start();
        }

        catch 
        {           
            //throw new Exception();       
        }
    }
     private void Stop()
     {
         if (m_watcher != null)
         {
             try
             {
                 m_watcher.Stop();
                 m_watcher.EventArrived -= new System.Management.EventArrivedEventHandler(EventArrived);
                 m_watcher = null;
             }
             catch
             {
                 //throw new Exception();
             }
         }

     }

      private void EventArrived(object sender, System.Management.EventArrivedEventArgs e)
      {
          foreach (PropertyData pd in e.NewEvent.Properties)
          {
              ManagementBaseObject mbo = null;
              if ((mbo = pd.Value as ManagementBaseObject) != null)
              {
                  foreach (PropertyData prop in mbo.Properties)
                  {
                      string info = string.Format("{0} - {1}", prop.Name, prop.Value);

                      if (info.Contains("USB Serial Port"))
                      {
                          currentEventUsb = DateTime.Now;
                          TimeSpan ts = currentEventUsb - lastEventUsb;
                          double TimeInSecond = ts.TotalSeconds;
                          lastEventUsb = currentEventUsb;
                          if (TimeInSecond > 5.0)
                          {
                              if (DeviceEvent != null)
                              {
                                  DeviceEvent(this, EventArgs.Empty);
                              }
                          }                         
                      }
                  }
              }
          }        
           
      }

    }

    
}
