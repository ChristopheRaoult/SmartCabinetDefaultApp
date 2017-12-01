using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Runtime.Serialization;

namespace SmartDrawerWpfApp.Wcf
{
    [DataContract]
    public class WallDevice
    {
        string _DeviceType;
        string _DeviceSerial;

        [DataMember]
        public string DeviceType
        {
            get { return _DeviceType; }
            set { _DeviceType = value; }
        }

        [DataMember]
        public string DeviceSerial
        {
            get { return _DeviceSerial; }
            set { _DeviceSerial = value; }
        }
    }
}
