using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("EventDrawerDetail")]
    public class EventDrawerDetail
    {
        [Key]
        public int EventDrawerDetailId { get; set; }
        public int DeviceId { get; set; }
        public int DrawerNumber { get; set; }    // 0 if no drawer or drawer index
        public Nullable<int> GrantedUserId { get; set; }
        public Nullable<int> InventoryId { get; set; }  // Inventory to null until scan is competed and then updated
        public DateTime EventDrawerDate { get; set; }

        public virtual Device Device { get; set; }
        public virtual GrantedUser GrantedUser { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
