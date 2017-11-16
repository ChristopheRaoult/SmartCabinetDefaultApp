using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("Column")]
    public class Column
    {
        public int ColumnId { get; set; }
        [Required]
        public int ColumnIndex { get; set; }
        [Required]
        [StringLength(20)]
        public string ColumnName { get; set; }
    }
}
