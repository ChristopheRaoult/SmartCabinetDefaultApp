using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("PullItem")]
    public class PullItem
    {
        public PullItem()
        {
            this.PullItems = new HashSet<PullItemDetail>();
        }

        [Key]
        public int PullItemId { get; set; }
        [Required]
        public DateTime PullItemDate { get; set; }
        [StringLength(256)]
        public string Description{ get; set; }
        public Nullable<int> GrantedUserId { get; set; }
        [Required]
        public int TotalToPull { get; set; }      

        public virtual GrantedUser GrantedUser { get; set; }
        public virtual ICollection<PullItemDetail> PullItems { get; set; }

    }
}
