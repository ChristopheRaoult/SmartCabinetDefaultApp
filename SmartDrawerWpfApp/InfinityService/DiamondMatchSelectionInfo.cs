using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class DiamondMatchSelectionInfo
    {
        public string ref_id { get; set; }
        public DateTime request_date { get; set; }
        public string comment { get; set; }
        public string mandatory { get; set; }
        public string tag_id { get; set; }
        public string guid { get; set; }
        public string pdf { get; set; }

        public static DiamondMatchSelectionInfo[] DeserializedJsonList(string responseJson)
        {
            string str = responseJson;
            if (responseJson.IndexOf('[') != 0)
            {
                int start = responseJson.IndexOf('[');
                int end = responseJson.IndexOf(']') + 1;
                str = responseJson.Substring(start, end - start);
            }
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<DiamondMatchSelectionInfo[]>(str);
        }
    }

}
