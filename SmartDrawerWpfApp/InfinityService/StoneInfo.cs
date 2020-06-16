using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class StoneInfo
    {
        //public int stone_id { get; set; }
        public string report_number { get; set; }
        public string report_date { get; set; }
        public string shape { get; set; }
        public string measurements { get; set; }
        public string color { get; set; }
        public string clarity { get; set; }
        public string cut { get; set; }
        public string carat_weight { get; set; }
        public string polish { get; set; }
        public string symmetry { get; set; }
        public string fluorescence { get; set; }
        public string tag_id { get; set; }
        public Guid? guid { get; set; }
        public string pdf { get; set; }
        //public DateTime inventory_date { get; set; }
        // public int company_id { get; set; }
        //public int smartbox_id { get; set; }
        //public int user_id { get; set; }
        //public double value_carat { get; set; }
        //public object drv { get; set; }
        //public object iav { get; set; }
        //public object ppwv { get; set; }
        //public string status { get; set; }
        //public string company_name { get; set; }
        //public string email { get; set; }
        //public string first_name { get; set; }
        //public string last_name { get; set; }
        //public string password { get; set; }
        //public object salt { get; set; }
        //public bool is_active { get; set; }
        //public bool is_deleted { get; set; }


        public static StoneInfo[] DeserializedJsonList(string responseJson)
        {
            string str = responseJson;
            if (responseJson.IndexOf('[') != 0)
            {
                int start = responseJson.IndexOf('[');
                int end = responseJson.IndexOf(']') + 1;
                str = responseJson.Substring(start, end - start);
            }
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<StoneInfo[]>(str);
        }
        public static StoneInfo DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<StoneInfo>(responseJson);
        }
        public static string SerializedJsonAlone(StoneInfo si)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(si);
        }
    }
}
