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
        /// <summary>
        /// Allow to check if a user already exists or not.
        /// </summary>
        /// <param name="grantedUsers">Self-instance (extension method).</param>
        /// <param name="login">Given login to search in the database.</param>
        /// <returns>True if a user with the given login has been found. False otherwise.</returns>
        public static bool IsUser(this DbSet<GrantedUser> grantedUsers, string login)
        {
            return grantedUsers.SingleOrDefault(gu => gu.Login == login) != null;
        }

        /// <summary>
        /// Add a new user to the database. Doesn't check if a user already exists with the same login.
        /// </summary>
        /// <param name="grantedUsers">Self-instance (extension method).</param>
        /// <param name="login">New user's login</param>
        /// <param name="password">New user's password</param>
        /// <param name="badgeNumber">New user's badge number</param>
        /// <param name="userRank">New user's rank</param>
        /// <returns>Just created/inserted GrantedUser instance.</returns>
        public static GrantedUser AddNewUser(this DbSet<GrantedUser> grantedUsers, string login, string password, string badgeNumber, UserRank userRank , string firstname , string lastname)
        {
            var newUser = new GrantedUser
            {
                Login = login,
                Password = PasswordHashing.Sha256Of(password),
                BadgeNumber = badgeNumber,
                UserRank = userRank,
                FirstName = firstname,
                LastName = lastname
            };

            return grantedUsers.Add(newUser);
        }
    }
}
