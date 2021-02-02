using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReturnRawActivity
    {
        public bool status { get; set; }
        public string message { get; set; }
        public ReactEventInfo data { get; set; }

        public static ReturnRawActivity DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReturnRawActivity>(responseJson);
        }
    }
}
