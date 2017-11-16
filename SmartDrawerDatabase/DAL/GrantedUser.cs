using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("GrantedUser")]
    public class GrantedUser
    {
        public GrantedUser()
        {
            this.GrantedAccesses = new HashSet<GrantedAccess>();
            this.Inventories = new HashSet<Inventory>();          
            this.Fingerprints = new HashSet<Fingerprint>();
            this.Authentications = new HashSet<Authentication>();
        }
        [Key]
        public int GrantedUserId { get; set; }
        [Required]
        [StringLength(20)]
        public string Login { get; set; }
        [Required]
        [StringLength(20)]
        public string Password { get; set; }
        public string BadgeNumber { get; set; }
        [Required]
        public int UserRankId { get; set; }

        public virtual UserRank UserRank { get; set; }
        public virtual ICollection<GrantedAccess> GrantedAccesses { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<Fingerprint> Fingerprints { get; set; }
        public virtual ICollection<Authentication> Authentications { get; set; }
    }
}
