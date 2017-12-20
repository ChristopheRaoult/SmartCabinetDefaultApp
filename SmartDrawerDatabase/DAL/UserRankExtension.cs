using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class UserRankExtension
    {
        /// <summary>
        /// Extension method. Return the unique value (as an entity) corresponding to the Administrator user rank.
        /// </summary>
        /// <param name="userRanks">Extension method parameter.</param>
        /// <returns>An UserRank instance of the Administrator value.</returns>
        public static UserRank Administrator(this DbSet<UserRank> userRanks)
        {
            UserRank urAdministrator = userRanks.SingleOrDefault(ur => ur.Rank == UserRank.Administrator);

            if (urAdministrator == null)
            {
                throw new Exception();
            }

            return urAdministrator;
        }

        /// <summary>
        /// Extension method. Return the unique value (as an entity) corresponding to the Super User user rank.
        /// </summary>
        /// <param name="userRanks">Extension method parameter.</param>
        /// <returns>An UserRank instance of the Super User value.</returns>
        public static UserRank SuperUser(this DbSet<UserRank> userRanks)
        {
            UserRank urSuperUser = userRanks.SingleOrDefault(ur => ur.Rank == UserRank.SuperUser);

            if (urSuperUser == null)
            {
                throw new Exception();
            }

            return urSuperUser;
        }

        /// <summary>
        /// Extension method. Return the unique value (as an entity) corresponding to the User user rank.
        /// </summary>
        /// <param name="userRanks">Extension method parameter.</param>
        /// <returns>An UserRank instance of the User value.</returns>
        public static UserRank User(this DbSet<UserRank> userRanks)
        {
            UserRank urUser = userRanks.SingleOrDefault(ur => ur.Rank == UserRank.User);

            if (urUser == null)
            {
                throw new Exception();
            }

            return urUser;
        }
    }
}
