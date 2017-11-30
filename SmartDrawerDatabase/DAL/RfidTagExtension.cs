using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class RfidTagExtension
    {
        /// <summary>
        /// Adds a new RfidTag and returns it. If the RfidTag already exists (an rfidtag with the same UID already exists), simply returns it.
        /// </summary>
        /// <param name="rfidTags">Self-instance (extension method).</param>
        /// <param name="tagUid">RfidTag UID (string) to be added.</param>
        /// <returns>Instance of RfidTag already existing or just created.</returns>
        public static RfidTag AddIfNotExisting(this DbSet<RfidTag> rfidTags, string tagUid)
        {
            RfidTag rfidTag = rfidTags.SingleOrDefault(rt => rt.TagUid == tagUid);

            // not existing: add it to the DbSet.
            if (rfidTag == null)
            {
                rfidTag = new RfidTag
                {
                    TagUid = tagUid
                };

                rfidTags.Add(rfidTag);
            }

            return rfidTag;
        }
    }
}
