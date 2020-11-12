using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReturnLogin
    {
        public bool status { get; set; }
        public string message { get; set; }
        public LoginData data { get; set; }

        public static ReturnLogin DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReturnLogin>(responseJson);
        }
    }
}
