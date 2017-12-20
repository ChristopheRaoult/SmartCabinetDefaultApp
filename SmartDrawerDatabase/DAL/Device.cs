using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("Device")]
    public class Device
    {        
        public Device()
        {
            this.GrantedAccesses = new HashSet<GrantedAccess>();
            this.Inventories = new HashSet<Inventory>();
            this.Authentications = new HashSet<Authentication>();
        }
        [Key]
        public int DeviceId { get; set; }
        [Required]
        public int DeviceTypeId { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        [StringLength(20)]
        public string SerialNumber { get; set; }        
        [StringLength(20)]
        public string RfidSerial { get; set; }
        [StringLength(20)]
        public string IpAddress { get; set; }

        public virtual DeviceType DeviceType { get; set; }
        public virtual ICollection<GrantedAccess> GrantedAccesses { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<Authentication> Authentications { get; set; }
    }
}
