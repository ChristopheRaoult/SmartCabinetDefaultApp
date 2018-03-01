using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.WcfServer
{  
    public class JsonUserList
    {
        public List<string> ftemplate { get; set; }
        public List<string> finger_index { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public string _id { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public string user_rankid { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string badge_num { get; set; }
        public int user_id { get; set; }
        public int __v { get; set; }

        public static JsonUserList[] DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonUserList[]>(responseJson);
        }

        public static JsonUserList DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonUserList>(responseJson);
        }
        public static string SerializedJsonAlone(JsonUserList jsl)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(jsl);
        }
    }

   

  
}
