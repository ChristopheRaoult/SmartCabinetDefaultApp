using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
   public class SmartDrawerDatabaseInitializer : DropCreateDatabaseIfModelChanges<SmartDrawerDatabaseContext>
    //public class SmartDrawerDatabaseInitializer : DropCreateDatabaseAlways<SmartDrawerDatabaseContext>
    {

        public override void InitializeDatabase(SmartDrawerDatabaseContext context)
        {
            /*if (context.Database.Exists())
            {
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction
                    , string.Format("ALTER DATABASE {0} SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.Database.Connection.Database));

                // drop the database
                context.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction,
                    "USE master DROP DATABASE [" + context.Database.Connection.Database + "]");
            }*/

            base.InitializeDatabase(context);
        }

        protected override void Seed (SmartDrawerDatabaseContext context)
        {

            //Seed Access Type
            IList<AccessType> defaultAccessTypes = new List<AccessType>();
            defaultAccessTypes.Add(new AccessType() { AccessTypeId = 1, Type = "MANUAL" });
            defaultAccessTypes.Add(new AccessType() { AccessTypeId = 2, Type = "BADGE" });
            defaultAccessTypes.Add(new AccessType() { AccessTypeId = 3, Type = "FINGERPRINT" });
            foreach (AccessType at in defaultAccessTypes)
                context.AccessTypes.Add(at);

            //Seed Column  for not null at Start
            IList<Column> DefaultColumns = new List<Column>();
            DefaultColumns.Add(new Column() { ColumnId = 1, ColumnIndex = 0, ColumnName = "Tag UID" });
            //DefaultColumns.Add(new Column() { ColumnId = 2, ColumnIndex = 1, ColumnName = "LOT ID" });
            foreach (Column c in DefaultColumns)
                context.Columns.Add(c);

            //Seed Device Type
            IList<DeviceType> defaultDeviceTypes = new List<DeviceType>();
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 1, HardwareIndex = 0, Type = "DT_UNKNOWN" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 2, HardwareIndex = 1, Type = "DT_STR" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 3, HardwareIndex = 2, Type = "DT_JSC" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 4, HardwareIndex = 3, Type = "DT_SMC" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 5, HardwareIndex = 4, Type = "DT_SBX" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 6, HardwareIndex = 5, Type = "DT_STR" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 7, HardwareIndex = 6, Type = "DT_SBR" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 8, HardwareIndex = 7, Type = "DT_SAS" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 9, HardwareIndex = 8, Type = "DT_SFR" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 10, HardwareIndex = 9, Type = "DT_MSR" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 11, HardwareIndex = 10, Type = "DT_SBF" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 12, HardwareIndex = 11, Type = "DT_PAD" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 13, HardwareIndex = 12, Type = "DT_CAT" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 14, HardwareIndex = 13, Type = "DT_MCS" });
            defaultDeviceTypes.Add(new DeviceType() { DeviceTypeId = 15, HardwareIndex = 14, Type = "DT_WAL" });
            foreach (DeviceType dt in defaultDeviceTypes)
                context.DeviceTypes.Add(dt);

            //Seed GrantType
            IList<GrantType> defaultGrantTypes = new List<GrantType>();
            defaultGrantTypes.Add(new GrantType() { GrantTypeId = 1, Type = "NONE" });
            defaultGrantTypes.Add(new GrantType() { GrantTypeId = 2, Type = "SLAVE" });
            defaultGrantTypes.Add(new GrantType() { GrantTypeId = 3, Type = "MASTER" });
            defaultGrantTypes.Add(new GrantType() { GrantTypeId = 4, Type = "ALL" });
            foreach (GrantType gt in defaultGrantTypes)
                context.GrantTypes.Add(gt);

            // Seed User Rank
            IList<UserRank> defaultUserRanks = new List<UserRank>();
            defaultUserRanks.Add(new UserRank() { UserRankId = 1, Rank = "ADMINISTRATOR" });
            defaultUserRanks.Add(new UserRank() { UserRankId = 2, Rank = "SUPER_USER" });
            defaultUserRanks.Add(new UserRank() { UserRankId = 3, Rank = "USER" });
            foreach (UserRank ur in defaultUserRanks)
                context.UserRanks.Add(ur);

            //seed Admin User
            GrantedUser adminUser = new GrantedUser() { GrantedUserId = 1, Login = "Admin", Password = PasswordHashing.Sha256Of("Rfid123456") , UserRankId = 1 , UpdateAt = DateTime.Now };
            context.GrantedUsers.Add(adminUser);

            //seed device
            //Device newDev = new Device() { DeviceTypeId = 15, DeviceName = "Wall Test Lab Paris", DeviceSerial = "Wall-V2-0013", RfidSerial = "14100258", UpdateAt = DateTime.Now };
            //context.Devices.Add(newDev);

            base.Seed(context);
        }
    }
}
