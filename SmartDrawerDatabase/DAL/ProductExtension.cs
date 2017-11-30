using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class ProductExtension
    {
        public static Product GetByTagUid(this DbSet<Product> products, string tagUid)
        {
            return products.SingleOrDefault(p => p.RfidTag.TagUid == tagUid);
        }
    }
}
