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
        public virtual DbSet<AccessType> AccessTypes { get; set; }
        public virtual DbSet<Authentication> Authentications { get; set; }
        public DbSet<Column> Columns { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Fingerprint> Fingerprints { get; set; }
        public virtual DbSet<GrantedAccess> GrantedAccesses { get; set; }       
        public virtual DbSet<GrantedUser> GrantedUsers { get; set; }
        public virtual DbSet<GrantType> GrantTypes { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryProduct> InventoryProducts { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<RfidTag> RfidTags { get; set; }
        public virtual DbSet<UserRank> UserRanks { get; set; }

    }
}
