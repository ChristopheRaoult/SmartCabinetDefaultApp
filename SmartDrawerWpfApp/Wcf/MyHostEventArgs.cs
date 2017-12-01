using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    public delegate void MyHostEventHandler(object sender, MyHostEventArgs e);
    public class MyHostEventArgs
    {
        public string NotificationName { get; set; }
        public string Message { get; set; }
        public MyHostEventArgs(string NotificationName, string Message)
        {
            this.NotificationName = NotificationName;
            this.Message = Message;
        }
    }
}
