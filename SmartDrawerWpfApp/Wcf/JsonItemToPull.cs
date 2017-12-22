using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonItemToPull
    {
        [DataMember]
        public DateTime pullItemDate { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public int userId { get; set; }
        [DataMember]
        public string[] listOfTagToPull { get; set; }
    }
}
