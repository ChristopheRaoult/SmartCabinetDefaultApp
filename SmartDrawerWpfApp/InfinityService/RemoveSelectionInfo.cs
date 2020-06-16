using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace SmartDrawerWpfApp.InfinityService
{
    public class RemoveSelectionInfo
    {
        public string[] ref_ids { get; set; }

        public static string SerializedJsonAlone(RemoveSelectionInfo rsi)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            return jsSerializer.Serialize(rsi);
        }
    }
}
