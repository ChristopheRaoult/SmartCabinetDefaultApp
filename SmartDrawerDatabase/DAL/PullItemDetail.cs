using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("PullItemDetail")]
    public class PullItemDetail
    {
        [Key]
        public int PullItemDetailId { get; set; }
        [Required]
        public int PullItemId { get; set; }
        [Required]
        public int RfidTagId { get; set; }

        public virtual PullItem PullItem { get; set; }
        public virtual RfidTag RfidTag { get; set; }
    }
}
