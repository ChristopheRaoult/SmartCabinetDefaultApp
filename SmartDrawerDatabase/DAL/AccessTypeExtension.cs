using SmartDrawerDatabase.DAL.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class AccessTypeExtension
    {
        public static AccessType Manual(this DbSet<AccessType> accessTypes)
        {
            AccessType accessType = accessTypes.SingleOrDefault(at => at.Type == AccessType.Manual);

            if (accessType == null)
            {
                throw new NonexistentConstantException("AT:Manual");
            }

            return accessType;
        }

        public static AccessType Badge(this DbSet<AccessType> accessTypes)
        {
            AccessType accessType = accessTypes.SingleOrDefault(at => at.Type == AccessType.Badge);

            if (accessType == null)
            {
                throw new NonexistentConstantException("AT:Badge");
            }

            return accessType;
        }

        public static AccessType Fingerprint(this DbSet<AccessType> accessTypes)
        {
            AccessType accessType = accessTypes.SingleOrDefault(at => at.Type == AccessType.Fingerprint);

            if (accessType == null)
            {
                throw new NonexistentConstantException("AT:Fingerprint");
            }

            return accessType;
        }
    }
}
