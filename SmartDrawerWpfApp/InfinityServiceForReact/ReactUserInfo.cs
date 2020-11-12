using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReactUserInfo
    {
        public bool status { get; set; }
        public string message { get; set; }
        public Page page { get; set; }
        public userData[] data { get; set; }

        public static ReactUserInfo DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReactUserInfo>(responseJson);
        }
    }
}
