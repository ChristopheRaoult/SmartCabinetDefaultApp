using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class LoginInfo
    {
        public string email { get; set; }
        public string password { get; set; }
        public static string SerializedJsonAlone(LoginInfo li)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(li);
        }
    }
}
