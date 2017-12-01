using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class JsonProductAddOrUpdate
    {
        [DataMember]
        public ProductClassTemplate[] listOfProducts { get; set; }
    }

    [Serializable]
    [DataContract]
    public class ProductClassTemplate
    {
        [DataMember]
        public string tagUID { get; set; }
        [DataMember]
        public string[] productInfo { get; set; }
        public ProductClassTemplate()
        {
            productInfo = new string[101];
        }
    }
}
