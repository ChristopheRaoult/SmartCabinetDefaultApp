using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.WcfServer
{
    public class JsonSelectionList
    {
        public string state { get; set; }
        public List<string> tags { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string _id { get; set; }
        public int selection_id { get; set; }
        public int __v { get; set; }
        public int? user_id { get; set; }
        public string desc { get; set; }
        public List<string> listOfTagToPull { get; set; }
        public List<string> listOfTagPulled { get; set; }
        public string description { get; set; }

        public static JsonSelectionList[] DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonSelectionList[]>(responseJson);
        }

        public static JsonSelectionList DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonSelectionList>(responseJson);
        }
        public static string SerializedJsonAlone(JsonSelectionList jsl)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(jsl);
        }
    }
}
