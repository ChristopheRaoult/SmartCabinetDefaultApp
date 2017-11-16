using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("InventoryProduct")]
    public class InventoryProduct
    {
        [Key]
        public int InventoryProductId { get; set; }
        [Required]
        public int InventoryId { get; set; }
        [Required]
        public int RfidTagId { get; set; }
        [Required]
        public int MovementType { get; set; }
        [Required]
        public int Shelve { get; set; }

        public virtual Inventory Inventory { get; set; }
        public virtual RfidTag RfidTag { get; set; }
    }
}
