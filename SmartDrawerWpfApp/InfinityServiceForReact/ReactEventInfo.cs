using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{ 
    public class ReactEventInfo
    {
        //public Body body { get; set; }

        public string token { get; set; }

        public long inventory_id { get; set; }

        public ReactReader reader { get; set; }
        //public ReactUser user { get; set; }
        public string user { get; set; }
        public Int32 timestamp { get; set; }
        public ReactEvent[] events { get; set; }


        public static string SerializedJsonAlone(ReactEventInfo rei)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(rei);
        }
    }
}
