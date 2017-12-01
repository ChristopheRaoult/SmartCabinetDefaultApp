using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonProdutToSelect
    {
        [DataMember]
        public string[] listOfTagId { get; set; }
    }
}
