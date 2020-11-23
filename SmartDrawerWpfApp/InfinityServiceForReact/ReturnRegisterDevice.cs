using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReturnRegisterDevice
    {
        public string status { get; set; }
        public string message { get; set; }
        public RegisterDeviceData data { get; set; }

        public static ReturnRegisterDevice DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReturnRegisterDevice>(responseJson);
        }
    }


}
