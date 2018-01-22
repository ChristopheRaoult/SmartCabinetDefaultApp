using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class InventoryExtension
    {

        public static Inventory GetLastInventoryforDrawer (this DbSet<Inventory> inventories ,int drawerNb)
        {
            return inventories.Where(i => i.DrawerNumber == drawerNb).OrderByDescending(i => i.InventoryId).FirstOrDefault();
        }
    }
}
