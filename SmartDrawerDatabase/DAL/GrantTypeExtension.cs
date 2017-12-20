using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;

namespace SmartDrawerDatabase.DAL
{
    public static class GrantTypeExtension
    {
        /// <summary>
        /// Extension method. Return the unique value (as an entity) corresponding to the "ALL" grant type.
        /// </summary>
        /// <param name="grantTypes">Extension method parameter.</param>
        /// <returns>A GrantType instance of the Administrator value.</returns>
        public static GrantType All(this DbSet<GrantType> grantTypes)
        {
            GrantType gtAll = grantTypes.SingleOrDefault(gt => gt.Type == "ALL");

            if (gtAll == null)
            {
                return null;
            }

            return gtAll;
        }

        /// <summary>
        /// Extension method. Return the unique value (as an entity) corresponding to the "MASTER" grant type.
        /// </summary>
        /// <param name="grantTypes">Extension method parameter.</param>
        /// <returns>A GrantType instance of the Master value.</returns>
        public static GrantType Master(this DbSet<GrantType> grantTypes)
        {
            GrantType gtMaster = grantTypes.SingleOrDefault(gt => gt.Type == "MASTER");

            if (gtMaster == null)
            {
                return null;
            }

            return gtMaster;
        }

        /// <summary>
        /// Extension method. Return the unique value (as an entity) corresponding to the "SLAVE" grant type.
        /// </summary>
        /// <param name="grantTypes">Extension method parameter.</param>
        /// <returns>A GrantType instance of the Slave value.</returns>
        public static GrantType Slave(this DbSet<GrantType> grantTypes)
        {
            GrantType gtSlave = grantTypes.SingleOrDefault(gt => gt.Type == "SLAVE");

            if (gtSlave == null)
            {
                return null;
            }

            return gtSlave;
        }
    }
}