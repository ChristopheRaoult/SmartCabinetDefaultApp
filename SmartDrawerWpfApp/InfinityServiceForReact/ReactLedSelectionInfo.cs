using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReactLedSelectionInfo
    {
        public int event_id { get; set; }
        public string token { get; set; }
        public Info[] info { get; set; }

        public static string SerializedJsonAlone(ReactLedSelectionInfo rlsi)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(rlsi);
        }
    }

    public class Info
    {
        public string tagNumber { get; set; }
        public bool status { get; set; }
    }
}
