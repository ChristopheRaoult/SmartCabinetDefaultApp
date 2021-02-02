using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{ 
    public class RegisterDeviceData
    {
        public string token { get; set; }
        public object[] userIds { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string _id { get; set; }
        public string name { get; set; }
        public string serialNumber { get; set; }
        public string companyId { get; set; }
        public string deviceTypeId { get; set; }
        public string description { get; set; }
        public string updatedBy { get; set; }
        public string createdBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }
    public class LoginData
    {
        public string token { get; set; }
        public LoginUser user { get; set; }
    }
    public class LoginUser
    {
        public string email { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
    }
    public class Header
    {
        public int totalStone { get; set; }
        public int notMatched { get; set; }
        public int matched { get; set; }
        public int minimumThreshold { get; set; }
    }

    public class Page
    {
        public bool hasNextPage { get; set; }
        public int totalCount { get; set; }
        public int currentPage { get; set; }
        public int filterCount { get; set; }
        public int totalPage { get; set; }
    }

    public class userData
    {
        public string _id { get; set; }
        public string salt { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public Roleid roleId { get; set; }
        public string altEmail { get; set; }
        public long phone { get; set; }
        public Companyid companyId { get; set; }
        public string updatedBy { get; set; }
        // public string createdBy { get; set; }
        public Createdby createdBy { get; set; }

        public Addressid addressId { get; set; }
        public Fingerprint[] fingerPrint { get; set; }
        public string badgeId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }

    public class Roleid
    {
        public string _id { get; set; }
        public int __v { get; set; }
        public int code { get; set; }
        public string companyTypeId { get; set; }
        public DateTime createdAt { get; set; }
        public string createdBy { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string longDescription { get; set; }
        public string shortDescription { get; set; }
        public DateTime updatedAt { get; set; }
        public string updatedBy { get; set; }
    }

    public class Companyid
    {
        public string _id { get; set; }
        public string parentId { get; set; }
        public bool isDeleted { get; set; }
        public bool isActive { get; set; }
        public string name { get; set; }
        public string addressId { get; set; }
        public Contact[] contacts { get; set; }
        public string logoUrl { get; set; }
        public string companyTypeId { get; set; }
        public string companySubTypeId { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }

    public class Contact
    {
        public string _id { get; set; }
        public long number { get; set; }
        public long altNumber { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string jobDescription { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
    }
    public class Createdby
    {
        public string _id { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string altEmail { get; set; }
        public string password { get; set; }
        public long phone { get; set; }
        public string addressId { get; set; }
        public string salt { get; set; }
        public string roleId { get; set; }
        public string companyId { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
        public string badgeId { get; set; }
        public Fingerprint[] fingerPrint { get; set; }
    }

    public class Addressid
    {
        public string _id { get; set; }
        public bool isDeleted { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string state { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string zipCode { get; set; }
        public string updatedBy { get; set; }
        public string createdBy { get; set; }
        public object[] attributes { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }

    public class Fingerprint
    {
        public int index { get; set; }
        public string data { get; set; }
    }




    public class Rfid
    {
        public string _id { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string skuId { get; set; }
        public string rfid { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }
    public class Labsid
    {
        public string _id { get; set; }
        public string lab { get; set; }
        public string labReportId { get; set; }
        public string labReportPath { get; set; }
        public DateTime labReportDate { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
    }
    public class Skuid
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
        public Iavid iavId { get; set; }
        public Rfid rfId { get; set; }
        public string deviceId { get; set; }
        public ReactReader reader { get; set; }
    }

    public class Iavid
    {
        public string _id { get; set; }
        public float iav { get; set; }
        public float iavAverage { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public string createdBy { get; set; }
        public string updatedBy { get; set; }
        public int drv { get; set; }
        public float pwv { get; set; }
        public string skuId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int __v { get; set; }
        public string rapPriceId { get; set; }
    }

    public class ReactReader
    {
        public string serial { get; set; }
        public int drawer { get; set; }
    }

    public class ReactUser
    {
        public string name { get; set; }
        public int id { get; set; }
        public string action { get; set; }
    }

    public class ReactEvent
    {
        public string EventType { get; set; }
        public string[] stones { get; set; }
    }

    public static class ReactEventType
    {
        public const string IN = "IN";
        public const string OUT = "OUT";
        public const string INVENTORY = "INVENTORY";
    }

    public static class ReactUnixTimeStamp
    {
        public static Int32 ConvertToUnixTimestamp(DateTime dt)
        {
            return (Int32)(dt.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        }
    }
}
