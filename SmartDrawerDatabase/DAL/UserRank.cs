using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("UserRank")]
    public class UserRank
    {
        public const string Administrator = "ADMINISTRATOR";
        public const string SuperUser = "SUPER_USER";
        public const string User = "USER";
        public UserRank()
        {
            this.GrantedUsers = new HashSet<GrantedUser>();
        }
        [Key]
        public int UserRankId { get; set; }
        [Required]
        public string Rank { get; set; }

        public virtual ICollection<GrantedUser> GrantedUsers { get; set; }
    }
}
