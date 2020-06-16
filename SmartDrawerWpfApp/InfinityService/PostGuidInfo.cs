using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class PostGuidInfo
    {
        public User user { get; set; }
        public string tag_id { get; set; }
        public Guid guid { get; set; }
        public string action { get; set; }

        public static string SerializedJsonAlone(PostGuidInfo pgi)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(pgi);
        }

        public class ActionTypes
        {
            public const string FINGERPRINT = "fingerprint";
            public const string MATCH = "match";
        }
    }
}
