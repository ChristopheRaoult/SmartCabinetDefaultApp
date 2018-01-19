using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerDatabase.DAL
{
    public static class EventDrawerDetailExtension
    {
        public static List<EventDrawerDetail> GetcurrentEventForDrawer(this DbSet<EventDrawerDetail> EventDrawerDetails, Device device , int drawerNb )
        {
            return EventDrawerDetails.Where(edd => edd.DeviceId == device.DeviceId && edd.DrawerNumber == drawerNb && edd.InventoryId == null).ToList();
        }

        public static void UpdateInventoryForEventDrawer(this DbSet<EventDrawerDetail> EventDrawerDetails, Device device, int drawerNb , Inventory newInventory)
        {
            List<EventDrawerDetail> theEventToUpdate = EventDrawerDetails.GetcurrentEventForDrawer(device, drawerNb);

            if (theEventToUpdate != null)
            foreach (EventDrawerDetail item in theEventToUpdate)
            {
                item.Inventory = newInventory;
                item.InventoryId = newInventory.InventoryId;
            }
        }
    }
}
