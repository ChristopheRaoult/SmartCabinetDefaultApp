using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("DeviceType")]
    public class DeviceType
    {
        public DeviceType()
        {
            this.Devices = new HashSet<Device>();
        }

        [Key]
        public int DeviceTypeId { get; set; }
        [Required]
        public int HardwareIndex { get; set; }
        [Required][StringLength(20)]
        public string Type { get; set; }

        public virtual ICollection<Device> Devices { get; set; }
    }

}
