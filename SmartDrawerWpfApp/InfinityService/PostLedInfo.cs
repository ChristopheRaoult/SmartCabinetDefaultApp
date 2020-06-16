using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class PostLedInfo
    {
        public string ref_id { get; set; }
        public string[] success { get; set; }
        public string[] fail { get; set; }

        public static string SerializedJsonAlone(PostLedInfo pli)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(pli);
        }
    }
}
