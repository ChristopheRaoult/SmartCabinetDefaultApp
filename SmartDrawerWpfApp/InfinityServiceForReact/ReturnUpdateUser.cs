using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReturnUpdateUser
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int data { get; set; }

        public static ReturnUpdateUser DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReturnUpdateUser>(responseJson);
        }
    }
}
