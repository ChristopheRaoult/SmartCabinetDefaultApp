using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.WcfServer
{
    public class JsonInventory
    {
        public string serial_num { get; set; }
        public List<string> added_tags { get; set; }
        public List<string> removed_tags { get; set; }
        public List<string> present_tags { get; set; }
        public List<string> user_login { get; set; }
        public string drawer { get; set; }
        public DateTime created_at { get; set; }
    }
}
