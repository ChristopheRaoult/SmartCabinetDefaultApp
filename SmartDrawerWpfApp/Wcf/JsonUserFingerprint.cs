using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonUserFingerprint
    {
        [DataMember]
        public int GrantedUserId { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public string Template { get; set; }
       
    }
}
