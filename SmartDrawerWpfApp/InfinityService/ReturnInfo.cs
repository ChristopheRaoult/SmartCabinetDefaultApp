using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{

    public class ReturnInfo
    {
        public bool success { get; set; }
        public int eventsRegistered { get; set; }
        public int lightLed { get; set; }
        public int diamondMatch { get; set; }
        public int inTransit { get; set; }
        public int adminRights { get; set; }
        public int clearDbase { get; set; }
        public int setOperational { get; set; }
        public int log { get; set; }
        public int addUsers { get; set; }
        public int userList { get; set; }
        public int hostUpdated { get; set; }
        public string deviceStatus { get; set; }


        public static ReturnInfo DeserializedJsonList(string responseJson)
        {          

            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReturnInfo>(responseJson);
        }
    }

}
