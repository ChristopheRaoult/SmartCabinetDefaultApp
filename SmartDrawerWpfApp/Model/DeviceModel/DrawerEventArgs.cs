using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model.DeviceModel
{
    public class DrawerEventArgs : EventArgs
    {
        private readonly string _serial;
        private readonly int _drawerId;

        public DrawerEventArgs(string serial, int drawerId)
        {
            _serial = serial;
            _drawerId = drawerId;
        }

        public string Serial { get { return _serial; } }
        public int DrawerId { get { return _drawerId; } }
    }
}
