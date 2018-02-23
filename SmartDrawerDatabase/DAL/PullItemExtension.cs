using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static  class PullItemExtension
    {
        /// <summary>
        /// Looks for an user matching the given serverID  and returns a GrantedUser instance.
        /// </summary>
        /// <param name="grantedUsers">Self-instance (extension method).</param>
        /// <param name="serverId">Given serverId.</param>
        /// <returns>A GrantedUser instance if a user matches, Null otherwise.</returns>
        public static PullItem GetByServerId(this DbSet<PullItem> pullItems, int serverId)
        {
            return pullItems.SingleOrDefault(gu => gu.ServerPullItemId == serverId);
        }

        public static void Clear<T>(this DbSet<T> dbSet) where T : class
        {
            dbSet.RemoveRange(dbSet);
        }

    }
}
