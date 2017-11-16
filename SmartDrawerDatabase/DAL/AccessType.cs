using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("AccessType")]
    public class AccessType
    {
        public AccessType()
        {
            this.Inventories = new HashSet<Inventory>();
        }
        [Key]
        public int AccessTypeId { get; set; }
        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }
    }
}
