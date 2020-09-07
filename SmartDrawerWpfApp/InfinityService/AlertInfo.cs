using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    /// <summary>
    /// Object to send Alert 
    /// Use to send unreferenced stones to server
    /// </summary>
    public class AlertInfo
    {
        public User user { get; set; }
        public Int32 timestamp { get; set; }
        public int type { get; set; }
        public string comment { get; set; }

        public static string SerializedJsonAlone(AlertInfo ai)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(ai);
        }
    }

    // 1 - Door opened for too long
    // 2 - Tag non referencé - tagID with comma

}
