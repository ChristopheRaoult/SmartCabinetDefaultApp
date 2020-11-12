using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class RegisterDeviceInfo
    {
        public string serialNumber { get; set; }
        public static string SerializedJsonAlone(RegisterDeviceInfo rdi)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(rdi);
        }
        public static RegisterDeviceInfo DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<RegisterDeviceInfo>(responseJson);
        }
    }
}
