using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.WcfServer
{
    public class JsonInventory
    {
        public string serial_num { get; set; }
        public List<string> added_tags { get; set; }
        public List<string> removed_tags { get; set; }
        public List<string> present_tags { get; set; }
        public List<string> user_login { get; set; }
        public string drawer { get; set; }
        public DateTime created_at { get; set; }

        public static JsonInventory[] DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            jsSerializer.MaxJsonLength = int.MaxValue;
            return jsSerializer.Deserialize<JsonInventory[]>(responseJson);
        }

        public static JsonInventory DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonInventory>(responseJson);
        }
        public static string SerializedJsonAlone(JsonSelectionList jsl)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(jsl);
        }
    }
}
