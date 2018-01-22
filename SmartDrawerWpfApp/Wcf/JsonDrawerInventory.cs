using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonDrawerInventory
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Reason { get; set; }
        [DataMember]
        public int ServerDeviceId { get; set; }
        [DataMember]
        public int DrawerNumber { get; set; }
        [DataMember]
        public int TotalAdded { get; set; }
        [DataMember]
        public int TotalPresent { get; set; }
        [DataMember]
        public int TotalRemoved { get; set; }
        [DataMember]
        public DateTime InventoryDate { get; set; }
        [DataMember]
        public TagInfo[] listOfTags { get; set; }
        [DataMember]
        public UserEventInfo[] listOfUserEvent { get; set; }
    }


    [Serializable]
    [DataContract]
    public class UserEventInfo
    {
        [DataMember]
        public int ServerGrantedUserId { get; set; }
        [DataMember]
        public DateTime EventDrawerDate { get; set; }       
    }
}
