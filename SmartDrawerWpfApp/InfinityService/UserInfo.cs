using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class UserInfo
    {
        public string username { get; set; }
        public string password { get; set; }
        public string name { get; set; }
        public int local_user_id { get; set; }
        public string extra { get; set; }

        public static string SerializedJsonAlone(UserInfo ui)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(ui);
        }
    }
}
