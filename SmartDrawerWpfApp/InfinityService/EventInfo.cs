using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class EventInfo
    {
        public Reader reader { get; set; }
        public User user { get; set; }
        public Int32 timestamp { get; set; }
        public Event[] events { get; set; }

        public static string SerializedJsonAlone(EventInfo ei)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(ei);
        }
    }

    public class Reader
    {
        public string serial { get; set; }
        public int? drawer { get; set; }
    }

    public class User
    {
        public string name { get; set; }
        public int id { get; set; }
    }

    public class Event
    {
        public string EventType { get; set; }
        public string[] stones { get; set; }
    }

    public static class EventType
    {
        public const string ARRIVAL = "ARRIVAL";
        public const string IN = "IN";
        public const string OUT = "OUT";
        public const string INVENTORY = "INVENTORY";
        public const string CLOSEBIZ = "CLOSEBIZ";
        public const string OPENBIZ = "OPENBIZ";
        public const string SOLD = "SOLD";
        public const string MISSING = "MISSING";
        public const string VAULT = "VAULT";
        public const string RESTART = "RESTART";
        public const string PING = "PING";
        public const string OPERATIONAL = "OPERATIONAL";
        public const string STANDBY = "STANDBY";

    }

    public static class UnixTimeStamp
    {
        public static Int32 ConvertToUnixTimestamp(DateTime dt)
        {
            return (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
