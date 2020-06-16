using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class RemoteUserInfo
    {
        public string user_id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string salt { get; set; }
        public string name { get; set; }
        public string local_user_id { get; set; }
        public string company_id { get; set; }
        public string badge { get; set; }
        public string fingerprints { get; set; }
        public string created_date { get; set; }
        public int deleted { get; set; }

        public static RemoteUserInfo[] DeserializedJsonList(string responseJson)
        {
            string str = responseJson;
            if (responseJson.IndexOf('[') != 0)
            {
                int start = responseJson.IndexOf('[');
                int end = responseJson.IndexOf(']') + 1;
                str = responseJson.Substring(start, end - start);
            }
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<RemoteUserInfo[]>(str);
        }
    }
}
