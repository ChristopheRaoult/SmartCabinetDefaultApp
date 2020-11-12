using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public class SkuInfo
    {
        public bool status { get; set; }
        public string message { get; set; }
        public SkuData[] data { get; set; }

        public static SkuInfo DeserializedJsonAlone(string responseJson)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Deserialize<SkuInfo>(responseJson);
        }
    }


    public class SkuData
    {
        public string _id { get; set; }
        public Labsid labsId { get; set; }
        public string colorSubCategory { get; set; }
        public string cut { get; set; }
        public string measurement { get; set; }
        public string polish { get; set; }
        public string tagId { get; set; }
        public string symmetry { get; set; }
        public string dmGuid { get; set; }
        public string movementStatus { get; set; }
        public string status { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string companyId { get; set; }
        public string clientRefId { get; set; }
        public string infinityShape { get; set; }
        public string clientShape { get; set; }
        public string labShape { get; set; }
        public string shape { get; set; }
        public float weight { get; set; }
        public string colorCategory { get; set; }
        public string gradeReportColor { get; set; }
        public string colorRapnet { get; set; }
        public string clarity { get; set; }
        public string colorType { get; set; }
        public string pwvImport { get; set; }
        public string infinityRefId { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public object[] comments { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
        public string iavId { get; set; }
        public Rfid rfId { get; set; }
        public string deviceId { get; set; }
        public ReactReader reader { get; set; }
    }
}
