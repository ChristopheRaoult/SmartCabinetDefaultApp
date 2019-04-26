using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.Model.DeviceModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.StaticHelpers.Security
{
    public static class GrantedUsersCache    
    {
        public static List<GrantedUser> Cache { get; private set; }
        private static Device _deviceEntity;
        /// <summary>
        /// Last user authenticated by its badge or its fingerprint
        /// </summary>
        public static GrantedUser LastAuthenticatedUser { get;  set; }
        static GrantedUsersCache()
        {
            Cache = new List<GrantedUser>();
        }

        public async static void Reload()
        {
            if (_deviceEntity == null)
            {
                _deviceEntity = DevicesHandler.GetDeviceEntity();

                if (_deviceEntity == null)
                {
                    return;
                }
            }

            try
            {
                var ctx = await RemoteDatabase.GetDbContextAsync();
                // Get all users who have accesses on this device, and Administrators. Include Fingerprints to the request, as they're needed by FPReaderHandle.
                //Cache = ctx.GrantedUsers.Where(gu => gu.GrantedAccesses.Any(ga => ga.DeviceId == _deviceEntity.DeviceId) || gu.UserRank.Rank == UserRank.Administrator).Include(gu => gu.Fingerprints).ToList();
                Cache = ctx.GrantedUsers.Include(gu => gu.Fingerprints).ToList();
                ctx.Database.Connection.Close();
                ctx.Dispose();

            }           
            catch (Exception error)
            {              
                 ExceptionMessageBox exp = new ExceptionMessageBox(error, "Unable to get data from local DB [Users Cache]");
                 exp.ShowDialog();                
            }
        }
    }
}

