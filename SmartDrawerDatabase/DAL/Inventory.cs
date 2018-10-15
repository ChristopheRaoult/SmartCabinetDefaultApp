using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("Inventory")]
    public class Inventory
    {
        public Inventory()
        {
            this.InventoryProducts = new HashSet<InventoryProduct>();            
        }
        [Key]
        public int InventoryId { get; set; }
        [Required]
        public int DeviceId { get; set; }
        [Required] // 0 if no drawer or drawer index
        public int DrawerNumber { get; set; }       
        public Nullable<int> GrantedUserId { get; set; }
        [Required]
        public int AccessTypeId { get; set; }
        [Required]
        public int TotalAdded { get; set; }
        [Required]
        public int TotalPresent { get; set; }
        [Required]
        public int TotalRemoved { get; set; }
        [Required]
        public DateTime InventoryDate { get; set; }        
        [MaxLength]
        public string InventoryStream { get; set; }

        public virtual Device Device { get; set; }
        public virtual GrantedUser GrantedUser { get; set; }
        public virtual AccessType AccessType { get; set; }
        public virtual ICollection<InventoryProduct> InventoryProducts { get; set; }
    }
}
