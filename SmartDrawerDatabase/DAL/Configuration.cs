using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartDrawerDatabase.DAL
{
    [Table("Configuration")]
    public class Configuration
    {
        public int ConfigurationId { get; set; }
        [Required]
        [StringLength(1024)]
        public string Parameter { get; set; }
        [Required]
        [StringLength(1024)]
        public string Value { get; set; }
    }
}
