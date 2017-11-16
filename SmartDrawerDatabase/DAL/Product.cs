using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    [Table("Product")]
    public class Product
    {
        public Product()
        {
        }
        [Key]
        public int ProductId { get; set; }
        [Required]
        public int RfidTagId { get; set; }
        [StringLength(20)]
        public string ProductInfo0 { get; set; }
        [StringLength(20)]
        public string ProductInfo1 { get; set; }
        [StringLength(20)]
        public string ProductInfo2 { get; set; }
        [StringLength(20)]
        public string ProductInfo3 { get; set; }
        [StringLength(20)]
        public string ProductInfo4 { get; set; }
        [StringLength(20)]
        public string ProductInfo5 { get; set; }
        [StringLength(20)]
        public string ProductInfo6 { get; set; }
        [StringLength(20)]
        public string ProductInfo7 { get; set; }
        [StringLength(20)]
        public string ProductInfo8 { get; set; }
        [StringLength(20)]
        public string ProductInfo9 { get; set; }
        [StringLength(20)]
        public string ProductInfo10 { get; set; }
        [StringLength(20)]
        public string ProductInfo11 { get; set; }
        [StringLength(20)]
        public string ProductInfo12 { get; set; }
        [StringLength(20)]
        public string ProductInfo13 { get; set; }
        [StringLength(20)]
        public string ProductInfo14 { get; set; }
        [StringLength(20)]
        public string ProductInfo15 { get; set; }
        [StringLength(20)]
        public string ProductInfo16 { get; set; }
        [StringLength(20)]
        public string ProductInfo17 { get; set; }
        [StringLength(20)]
        public string ProductInfo18 { get; set; }
        [StringLength(20)]
        public string ProductInfo19 { get; set; }

        public virtual RfidTag RfidTag { get; set; }
    }
}
