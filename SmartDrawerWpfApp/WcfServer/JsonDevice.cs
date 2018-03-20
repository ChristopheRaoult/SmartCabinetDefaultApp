using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.WcfServer
{  

    public class JsonDevice
    {
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string _id { get; set; }
        public string name { get; set; }
        public string serial_num { get; set; }
        public string Location { get; set; }
        public string IP_addr { get; set; }
        public int __v { get; set; }

        public static JsonDevice[] DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonDevice[]>(responseJson);
        }

        public static JsonDevice DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonDevice>(responseJson);
        }
        public static string SerializedJsonAlone(JsonDevice jsl)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(jsl);
        }
    }

}
