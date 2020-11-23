using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public class SmartDrawerDatabaseContext : DbContext
    {
        //public SmartDrawerDatabaseContext() : base("SmartDrawerDB")
        public SmartDrawerDatabaseContext(string connectionStringOrName) : base(connectionStringOrName)       
        {
            //Database.SetInitializer<SmartDrawerDatabaseContext>(null);
            Database.SetInitializer<SmartDrawerDatabaseContext>(new SmartDrawerDatabaseInitializer());
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RfidTag>()
                .HasIndex(p => new { p.TagUid })
                .IsUnique(true);

            modelBuilder.Entity<GrantedUser>()
                .HasIndex(p => new { p.Login })
                .IsUnique(true);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine(@"Entity of type ""{0}"" in state ""{1}"" 
                   has the following validation errors:",
                        eve.Entry.Entity.GetType().Name,
                        eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine(@"- Property: ""{0}"", Error: ""{1}""",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (DbUpdateException e)
            {
                Console.WriteLine(e.Source);
                Console.WriteLine(e.Message);
                //Add your code to inspect the inner exception and/or
                //e.Entries here.
                //Or just use the debugger.
                //Added this catch (after the comments below) to make it more obvious 
                //how this code might help this specific problem
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            return -1;
        }


        public virtual DbSet<AccessType> AccessTypes { get; set; }
        public virtual DbSet<Authentication> Authentications { get; set; }
        public virtual DbSet<Column> Columns { get; set; }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<DeviceType> DeviceTypes { get; set; }
        public virtual DbSet<Fingerprint> Fingerprints { get; set; }
        public virtual DbSet<GrantedAccess> GrantedAccesses { get; set; }       
        public virtual DbSet<GrantedUser> GrantedUsers { get; set; }
        public virtual DbSet<GrantType> GrantTypes { get; set; }
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<InventoryProduct> InventoryProducts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RfidTag> RfidTags { get; set; }
        public virtual DbSet<UserRank> UserRanks { get; set; }
        public virtual DbSet<PullItem> PullItems { get; set; }
        public virtual DbSet<PullItemDetail> PullItemsDetails { get; set; }
        public virtual DbSet<EventDrawerDetail> EventDrawerDetails { get; set; }
        public virtual DbSet<Configuration> Configurations { get; set; }

    }
}
