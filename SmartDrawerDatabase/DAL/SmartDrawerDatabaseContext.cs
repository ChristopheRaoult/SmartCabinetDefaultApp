using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public class SmartDrawerDatabaseContext : DbContext
    {
        public SmartDrawerDatabaseContext() : base("SmartDrawerDB")
        {
            Database.SetInitializer<SmartDrawerDatabaseContext>(new SmartDrawerDatabaseInitializer());
        }
        public DbSet<RfidTag> RfidTags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Column> Columns { get; set; }
    }
}
