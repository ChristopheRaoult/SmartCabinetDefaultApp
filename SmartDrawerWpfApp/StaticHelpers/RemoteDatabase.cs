using SmartDrawerDatabase.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.StaticHelpers
{
    public class RemoteDatabase
    {
        public static string Host
        {
            get { return Properties.Settings.Default.DbHost.Replace(':', ','); }
            set { Properties.Settings.Default.DbHost = value; }
        }

        public static string Name
        {
            get { return Properties.Settings.Default.DbName; }
            set { Properties.Settings.Default.DbName = value; }
        }

        public static string Login
        {
            get { return Properties.Settings.Default.DbLogin; }
            set { Properties.Settings.Default.DbLogin = value; }
        }

        public static string Password
        {
            get
            {
                using (var secureString = Properties.Settings.Default.DbPassword.DecryptString())
                {
                    return secureString.ToInsecureString();
                }
            }

            set
            {
                using (var secureString = value.ToSecureString())
                {
                    Properties.Settings.Default.DbPassword = secureString.EncryptString();
                }
            }
        }

        public static void SaveSettings()
        {
            Properties.Settings.Default.Save();
            //Properties.Settings.Default.Upgrade();          
            Properties.Settings.Default.Reload();
        }

        public static bool IsConfigured
        {
            get
            {
                return
                    !String.IsNullOrEmpty(Host) &&
                    !String.IsNullOrEmpty(Name) &&
                    !String.IsNullOrEmpty(Login) &&
                    !String.IsNullOrEmpty(Password);
            }
        }

        internal static string GetConnectionString()
        {
            var sqlBuilder = new SqlConnectionStringBuilder
            {
                DataSource = Host,
                InitialCatalog = Name,
                UserID = Login,
                Password = Password,
                IntegratedSecurity = false,
                PersistSecurityInfo = true,
                MultipleActiveResultSets = true,
                ApplicationName = "EntityFramework",
                Encrypt = true,
                TrustServerCertificate = true
            };            
            return sqlBuilder.ToString();
        }

        /// <summary>
        /// Provide an LTWModelContext instance connected to the remote database.
        /// </summary>
        /// <returns>LTWModelContext instance if succeeded, null otherwise.</returns>
        public static async Task<SmartDrawerDatabaseContext> GetDbContextAsync()
        {
            try
            {
                var dbContext = new SmartDrawerDatabaseContext(GetConnectionString());
                await dbContext.Database.Connection.OpenAsync();
                return dbContext;
            }

            catch (SqlException sqle)
            {
                string sqlerror = sqle.Message;
                // connection to remote database failed (OpenAsync() raised an SqlException)
                return null;
            }
        }
        public static  SmartDrawerDatabaseContext GetDbContext()
        {
            try
            {
                var dbContext = new SmartDrawerDatabaseContext(GetConnectionString());
                dbContext.Database.Connection.Open();
                return dbContext;
            }

            catch (SqlException)
            {
                // connection to remote database failed (OpenAsync() raised an SqlException)
                return null;
            }
        }

    }
}

