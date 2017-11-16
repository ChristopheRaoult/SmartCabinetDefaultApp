using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("RfidTag")]
    public class RfidTag
    {       
        public RfidTag()
        {
            this.InventoryProduct = new HashSet<InventoryProduct>();
            this.Product = new HashSet<Product>();
        }

        [Key]
        public int RfidTagId { get; set; }
        [Required]
        [StringLength(20)]
        public string TagUid { get; set; }

        public virtual ICollection<InventoryProduct> InventoryProduct { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
