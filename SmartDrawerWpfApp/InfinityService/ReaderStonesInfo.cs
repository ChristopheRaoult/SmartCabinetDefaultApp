using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class ReaderStonesInfo
    {
        public string status { get; set; }
        public string inventory_date { get; set; }
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
        public string deviceStatus { get; set; }

        public static ReaderStonesInfo[] DeserializedJsonList(string responseJson)
        {
            string str = responseJson;
            if (responseJson.IndexOf('[') != 0)
            {
                int start = responseJson.IndexOf('[');
                int end = responseJson.IndexOf(']') + 1;
                str = responseJson.Substring(start, end - start);
            }
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<ReaderStonesInfo[]>(str);
        }
    }
}
