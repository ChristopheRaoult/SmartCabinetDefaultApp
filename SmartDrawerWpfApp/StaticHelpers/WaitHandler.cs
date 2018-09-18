using SmartDrawerWpfApp.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.StaticHelpers
{
    public class WaitHandler
    {
        private Thread StatusThread = null;
        private PleaseWaitWindow Popup = null;
        public string Msg;
        public void Start()
        {
            //create the thread with its ThreadStart method
            this.StatusThread = new Thread(() =>
            {
                try
                {
                    this.Popup = new PleaseWaitWindow();
                    this.Popup.setText(Msg);
                    this.Popup.Show();
                    this.Popup.Closed += (lsender, le) =>
                    {
                        //when the window closes, close the thread invoking the shutdown of the dispatcher
                        this.Popup.Dispatcher.InvokeShutdown();
                        this.Popup = null;
                        this.StatusThread = null;
                    };

                    //this call is needed so the thread remains open until the dispatcher is closed
                    System.Windows.Threading.Dispatcher.Run();
                }
                catch (Exception ex)
                {
                    ExceptionMessageBox exp = new ExceptionMessageBox(ex, "Error in Waiting Dialog");
                }
            });

            //run the thread in STA mode to make it work correctly
            this.StatusThread.SetApartmentState(ApartmentState.STA);
            this.StatusThread.Priority = ThreadPriority.Normal;
            this.StatusThread.Start();
        }

        public void Stop()
        {
            if (this.Popup != null)
            {
                //need to use the dispatcher to call the Close method, because the window is created in another thread, and this method is called by the main thread
                this.Popup.Dispatcher.BeginInvoke(new Action(() =>
                {
                    this.Popup.Close();
                }));
            }
        }
    }
}
