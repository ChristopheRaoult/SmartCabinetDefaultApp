using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("GrantedType")]
    public class GrantType
    {
        public GrantType()
        {
            this.GrantedAccesses = new HashSet<GrantedAccess>();
        }
        [Key]
        public int GrantTypeId { get; set; }
        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        public virtual ICollection<GrantedAccess> GrantedAccesses { get; set; }

    }
}
