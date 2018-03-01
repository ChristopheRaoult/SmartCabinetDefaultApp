using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class GrantedAccessExtension    
    {
        public static bool HasAccess(this DbSet<GrantedAccess> accesses, GrantedUser user, Device device)
        {
            return accesses.SingleOrDefault(ga => ga.GrantedUserId == user.GrantedUserId && ga.DeviceId == device.DeviceId) != null;
        }
        public static List<GrantedAccess> GetByUser(this DbSet<GrantedAccess> accesses, GrantedUser user)
        {
            return accesses.Where(ga => ga.GrantedUserId == user.GrantedUserId).ToList();
        }
        public static List<GrantedAccess> GetByDevice(this DbSet<GrantedAccess> accesses, Device device)
        {
            return accesses.Where(ga => ga.DeviceId == device.DeviceId).ToList();
        }
        public static GrantedAccess AddOrUpdateAccess(this DbSet<GrantedAccess> accesses, GrantedUser user,
           Device device, GrantType type)
        {
            var access = accesses.FirstOrDefault(ga => ga.GrantedUserId == user.GrantedUserId && ga.DeviceId == device.DeviceId);

            if (access == null)
            {
                access = new GrantedAccess
                {
                    DeviceId = device.DeviceId,
                    GrantedUserId = user.GrantedUserId,
                    GrantTypeId = type.GrantTypeId
                };

                return accesses.Add(access);
            }

            access.DeviceId = device.DeviceId;
            access.GrantedUserId = user.GrantedUserId;
            access.GrantTypeId = type.GrantTypeId;   

            return access;
        }
        public static void RemoveAccess(this DbSet<GrantedAccess> accesses, GrantedUser user, Device device)
        {
            var access = accesses.FirstOrDefault(ga => ga.GrantedUserId == user.GrantedUserId && ga.DeviceId == device.DeviceId);

            if (access != null)
            {
                accesses.Remove(access);
            }
        }

    }
}
