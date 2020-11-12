using SmartDrawerWpfApp.InfinityServiceForReact;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class RetDMSocket
    {
        public string body { get; set; }
        public string status { get; set; }

        public static RetDMSocket DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<RetDMSocket>(responseJson);
        }
    }

    public class RetDMSocket2
    {
        public RegisterDeviceData body { get; set; }
        public string status { get; set; }
        public static RetDMSocket2 DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<RetDMSocket2>(responseJson);
        }
    }
}
