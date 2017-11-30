using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model.DeviceModel
{
    public static class DrawerStatusList
    {
        public static string Disable { get { return "Disable"; } }
        public static string Open { get { return "Open"; } }
        public static string Ready { get { return "Ready"; } }
        public static string NotReady { get { return "Not Ready"; } }
        public static string InScan { get { return "Scanning"; } }
        public static string InLight { get { return "Lighting"; } }
        public static string InConnection { get { return "In Connection"; } }
        public static string Unknown { get { return "Unknown"; } }
    }
}
