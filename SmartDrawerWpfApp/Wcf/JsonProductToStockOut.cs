using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonProductToStockOut
    {
        [DataMember]
        public string[] listOfTagId { get; set; }
    }
}
