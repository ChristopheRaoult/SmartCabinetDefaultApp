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

        public static Inventory GetLastInventoryforDrawerBeforeDate(this DbSet<Inventory> inventories, int drawerNb, DateTime dt)
        {
            return inventories.Where(i => i.DrawerNumber == drawerNb && i.InventoryDate < dt).Include(o => o.InventoryProducts).OrderByDescending(i => i.InventoryId).FirstOrDefault();
        }


        public static List<Inventory> GetNotNotifiedInventory(this DbSet<Inventory> inventories, int drawerNumber)
        {
            if (drawerNumber == 0) //AllDrawer
                return inventories.Where(i => i.IsNotify == 0).Include(o => o.InventoryProducts).Include(u => u.GrantedUser).ToList();
            else
                return inventories.Where(i => i.IsNotify == 0 && i.DrawerNumber == drawerNumber).Include(o => o.InventoryProducts).Include(u => u.GrantedUser).ToList();
        }

        public static bool UpdateInventoryForNotification(this DbSet<Inventory> inventories, Inventory newInventory)
        {
            var inv = inventories.FirstOrDefault(iv => iv.InventoryId == newInventory.InventoryId);
            if (inv != null)
            {
                inv.IsNotify = 1;               
                return true;
            }
            return false;
        }
    }
}
