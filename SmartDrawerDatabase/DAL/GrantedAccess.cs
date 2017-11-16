using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("GrantedAccess")]
    public class GrantedAccess
    {
        [Key]
        public int GrantedAccessId { get; set; }
        [Required]
        public int GrantTypeId { get; set; }
        [Required]
        public int GrantedUserId { get; set; }
        [Required]
        public int DeviceId { get; set; }

        public virtual GrantType GrantType { get; set; }
        public virtual GrantedUser GrantedUser { get; set; }
        public virtual Device Device { get; set; }


    }
}
