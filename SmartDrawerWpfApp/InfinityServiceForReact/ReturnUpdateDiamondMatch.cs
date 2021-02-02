using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class ReturnUpdateDiamondMatch
    {
        public bool status { get; set; }
        public string message { get; set; }
        public ReactDiamondMatchInfo data { get; set; }

        public static ReturnUpdateDiamondMatch DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReturnUpdateDiamondMatch>(responseJson);
        }
    }

    public class ReactDiamondMatchInfo
    {
        public enum status_type
        {
            success,
            failure
        }

        public enum action_Type
        {
            REGISTRATION,
            ALTER_REGISTRATION,
            MATCH,
            MATCH_PER_GUID
        }
        public int event_id { get; set; }
        public string token { get; set; }
        public string user { get; set; }
        public string actionType { get; set; }
        public string foundDmGuid { get; set; }
        public Error error { get; set; }
        public string tagNo { get; set; }
        public string dmGuId { get; set; }
        public string status { get; set; }

        public static string SerializedJsonAlone(ReactDiamondMatchInfo rdm)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(rdm);
        }

        public static ReactDiamondMatchInfo DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReactDiamondMatchInfo>(responseJson);
        }
    }
    public class Error
    {
        public int code { get; set; }
        public string description { get; set; }
    }
}
