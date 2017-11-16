using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("Authentication")]
    public class Authentication
    {
        [Key]
        public int AuthenticationId { get; set; }
        [Required]
        public int DeviceId { get; set; }
        [Required]
        public int GrantedUserId { get; set; }
        [Required]
        public DateTime AuthentificationDate { get; set; }
       

        public virtual Device Device { get; set; }
        public virtual GrantedUser GrantedUser { get; set; }
    }
}
