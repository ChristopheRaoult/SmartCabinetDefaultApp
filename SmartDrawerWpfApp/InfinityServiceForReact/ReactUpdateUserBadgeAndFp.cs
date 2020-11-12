using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReactUpdateUserBadgeAndFp
    {
        public string _id { get; set; }
        public Fingerprint[] fingerPrint { get; set; }
        public string badgeId { get; set; }

        public static ReactUpdateUserBadgeAndFp DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReactUpdateUserBadgeAndFp>(responseJson);
        }
        public static string SerializedJsonAlone(ReactUpdateUserBadgeAndFp ruubf)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(ruubf);
        }
    }
}
