using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("Fingerprint")]
    public class Fingerprint
    {
        [Key]
        public int FingerprintId { get; set; }
        [Required]
        public int Index { get; set; }
        [Required]
        public string Template { get; set; }
        [Required]
        public int GrantedUserId { get; set; }       

        public virtual GrantedUser GrantedUser { get; set; }
    }
}
