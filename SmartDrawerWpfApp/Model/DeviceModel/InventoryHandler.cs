using SDK_SC_RfidReader;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.StaticHelpers;
using SmartDrawerWpfApp.StaticHelpers.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.Model.DeviceModel
{
    public static class InventoryHandler
    {
        public static event InventoryEventHandler InventoryCompleted;
        public static event InventoryEventHandler InventoryAborted;
        public delegate void InventoryEventHandler(InventoryEventArgs args);

        private static SmartDrawerDatabase.DAL.Device _deviceEntity;
        public static void HandleNewScanCompleted(int drawerId)
        {
            var ctx = RemoteDatabase.GetDbContext();
            Inventory newInventory = null;
            try
            {
                if (_deviceEntity == null)
                {
                    _deviceEntity = DevicesHandler.GetDeviceEntity();
                    if (_deviceEntity == null)
                    {
                        return;
                    }
                }


                //remove previous entry Older than 30 days
                DateTime dtToKeep = DateTime.Now.AddDays(-30.0);
                var itemBinding = ctx.Inventories.Where(i => i.DrawerNumber == drawerId && i.DeviceId == _deviceEntity.DeviceId && i.InventoryDate < dtToKeep);
                if (itemBinding != null)
                foreach (var ib in itemBinding)
                {                  
                    ctx.Inventories.Remove(ib);                   
                }
                ctx.SaveChanges();


                AccessType newInventoryAccessType = null;
                int? newInventoryGrantedUserId = null;

                switch (DevicesHandler.LastScanAccessTypeName)
                {
                    case AccessType.Badge:
                        newInventoryAccessType = ctx.AccessTypes.Badge();
                        newInventoryGrantedUserId = GrantedUsersCache.LastAuthenticatedUser.GrantedUserId;
                        break;

                    case AccessType.Fingerprint:
                        newInventoryAccessType = ctx.AccessTypes.Fingerprint();
                        newInventoryGrantedUserId = GrantedUsersCache.LastAuthenticatedUser.GrantedUserId;
                        break;
                    default:
                        newInventoryAccessType = ctx.AccessTypes.Manual();
                        newInventoryGrantedUserId = null;
                        break;
                }


                newInventory = new Inventory
                {
                    DeviceId = _deviceEntity.DeviceId,
                    DrawerNumber = drawerId,
                    GrantedUserId = newInventoryGrantedUserId,
                    AccessTypeId = newInventoryAccessType.AccessTypeId,
                    TotalAdded = DevicesHandler.ListTagAddedPerDrawer.Where(kvp => kvp.Value == drawerId).ToList().Count(),
                    TotalPresent = DevicesHandler.ListTagPerDrawer.Where(kvp => kvp.Value == drawerId).ToList().Count(),
                    TotalRemoved = DevicesHandler.ListTagRemovedPerDrawer.Where(kvp => kvp.Value == drawerId).ToList().Count(),
                    InventoryDate = DateTime.Now,
                    InventoryStream = SerializeHelper.SeralizeObjectToXML(DevicesHandler.Device.ReaderData),
                };

                ctx.Inventories.Add(newInventory);
                ctx.SaveChanges(); // now the inventory is saved, we can use its ID

                var addedTags = DevicesHandler.GetTagFromDictionnary(drawerId,DevicesHandler.ListTagAddedPerDrawer);
                var presentTags = DevicesHandler.GetTagFromDictionnary(drawerId, DevicesHandler.ListTagPerDrawer);
                var removedTags = DevicesHandler.GetTagFromDictionnary(drawerId, DevicesHandler.ListTagRemovedPerDrawer);

                newInventory.TotalPresent = presentTags.Count;

                AddMovementToInventory(ctx, newInventory, addedTags, MovementType.Added, drawerId);
                ctx.SaveChanges();
                AddMovementToInventory(ctx, newInventory, presentTags, MovementType.Present, drawerId);
                ctx.SaveChanges();
                AddMovementToInventory(ctx, newInventory, removedTags, MovementType.Removed, drawerId);
                ctx.SaveChanges();
                var handler = InventoryCompleted;
                if (handler != null)
                {
                    handler(new InventoryEventArgs(newInventory, null));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().FullName);
                Console.WriteLine(ex.Message);
                var handler = InventoryAborted;
                if (handler != null)
                {
                    handler(null);
                }
                if (newInventory != null)
                {
                    ctx.Inventories.Remove(newInventory);
                    ctx.SaveChanges();
                }
            }
            finally
            {
                ctx.Database.Connection.Close();
                ctx.Dispose();
            }
        }

        private static void AddMovementToInventory(SmartDrawerDatabaseContext ctx, Inventory newInventory, IEnumerable tags, MovementType movement , int drawerId)
        {
            foreach (string uid in tags)
            {
                var rfidTag = ctx.RfidTags.AddIfNotExisting(uid);
                int shelveNumber = drawerId;
                newInventory.InventoryProducts.Add(
                        ctx.InventoryProducts.Add(new InventoryProduct
                        {
                            InventoryId = newInventory.InventoryId,
                            RfidTag = rfidTag,
                            MovementType = (int)movement,
                            Shelve = shelveNumber
                        })
                    );
            }           
        }
    }

    public enum MovementType
    {
        Added = 1,
        Present = 0,
        Removed = -1
    }
}
