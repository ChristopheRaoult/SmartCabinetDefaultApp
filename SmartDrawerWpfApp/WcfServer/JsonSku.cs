using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.WcfServer
{

    public class JsonSku
    {
        public bool status { get; set; }
        public Errors errors { get; set; }
        public Data data { get; set; }
        public Data_Array[] data_array { get; set; }

        public static JsonSku[] DeserializedJsonList(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonSku[]>(responseJson);
        }

        public static JsonSku DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<JsonSku>(responseJson);
        }
        public static string SerializedJsonAlone(JsonSku jsl)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(jsl);
        }

    }
    public class Errors
    {
        public string msg { get; set; }
        public string code { get; set; }
    }

    public class Data
    {
        public string _id { get; set; }
        public string refNumber { get; set; }
        public string status { get; set; }
        public bool isAssociated { get; set; }
        public string rfidNumber { get; set; }
        public string drawer { get; set; }
        public int __v { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }

    public class Data_Array
    {
        public string _id { get; set; }
        public string refNumber { get; set; }
        public string woNumber { get; set; }
        public string itemNumber { get; set; }
        public string status { get; set; }
        public bool isAssociated { get; set; }
        public string rfidNumber { get; set; }
        public int __v { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public string drawer { get; set; }
        public DateTime lastScan { get; set; }
        public string serial_num { get; set; }

    }
}
