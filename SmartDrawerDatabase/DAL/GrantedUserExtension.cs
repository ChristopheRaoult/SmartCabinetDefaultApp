using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class GrantedUserExtension
    {
        /// <summary>
        /// Looks for an user matching the given login and returns a GrantedUser instance.
        /// </summary>
        /// <param name="grantedUsers">Self-instance (extension method).</param>
        /// <param name="login">Given login.</param>
        /// <returns>A GrantedUser instance if a user matches, Null otherwise.</returns>
        public static GrantedUser GetByLogin(this DbSet<GrantedUser> grantedUsers, string login)
        {
            return grantedUsers.SingleOrDefault(gu => gu.Login == login);
        }

        /// <summary>
        /// Tries to find a GrantedUser matching both given login & password.
        /// </summary>
        /// <param name="grantedUsers">Self-instance (extension method).</param>
        /// <param name="login">Given login (most of the time provided by User).</param>
        /// <param name="password">Given password (most of the time provided by User).</param>
        /// <returns>True if a user has been found, false otherwise.</returns>
        public static bool IsPasswordValid(this DbSet<GrantedUser> grantedUsers, string login, string password)
        {
            GrantedUser test = grantedUsers.SingleOrDefault(gu => gu.Login == login && gu.Password == password);
            return test != null;
        }
    }
}
