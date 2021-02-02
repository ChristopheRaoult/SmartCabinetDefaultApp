using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class LedTriggerInfo
    {
        public LedTriggerData[] data { get; set; }

        public static LedTriggerInfo DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<LedTriggerInfo>(responseJson);
        }
    }

    public class LedTriggerData
    {
        public string serialNumber { get; set; }
        public string tag { get; set; }
        public string comments { get; set; }
        public int drawer { get; set; }
    }
}
