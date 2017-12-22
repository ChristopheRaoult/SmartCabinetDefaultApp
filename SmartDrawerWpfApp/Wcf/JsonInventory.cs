using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonInventory
    {
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string Reason { get; set; }
        [DataMember]
        public TagInfo[] listOfTags { get; set; }
    }

    [Serializable]
    [DataContract]
    public class TagInfo
    {
        [DataMember]
        public string tagUID { get; set; }
        [DataMember]
        public int  DrawerId { get; set; }
        [DataMember]
        public int  Movement { get; set; }
    }
}
