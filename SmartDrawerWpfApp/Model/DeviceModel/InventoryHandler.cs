using SDK_SC_RfidReader;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.StaticHelpers;
using SmartDrawerWpfApp.StaticHelpers.Security;
using SmartDrawerWpfApp.WcfServer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
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

        static object lockMethodInventory = new object();

        public static void HandleNewScanCompleted(int drawerId)
        {

            lock (lockMethodInventory)
            {

                LogToFile.LogMessageToFile("______________________________________________________________ ");
                LogToFile.LogMessageToFile("Process Inventory for drawer " + drawerId);

                var ctx = RemoteDatabase.GetDbContext();
                ctx.Configuration.AutoDetectChangesEnabled = false;
                //ctx.Configuration.ValidateOnSaveEnabled = false;
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


                    //remove previous entry Older than 7 days
                    DateTime dtToKeep = DateTime.Now.AddDays(-7.0);

                    using (new PerfTimerLogger("remove previous entry Older than 7 days"))
                    {

                        //remove event drawer
                        var itemBinding2 = ctx.EventDrawerDetails.Where(i => i.DrawerNumber == drawerId && i.DeviceId == _deviceEntity.DeviceId && i.EventDrawerDate < dtToKeep);
                        if (itemBinding2 != null)
                            foreach (var ib in itemBinding2)
                            {
                                ctx.EventDrawerDetails.Remove(ib);
                            }

                        //removed inventory
                        var itemBinding = ctx.Inventories.Where(i => i.DrawerNumber == drawerId && i.DeviceId == _deviceEntity.DeviceId && i.InventoryDate < dtToKeep);
                        if (itemBinding != null)
                            foreach (var ib in itemBinding)
                            {
                                ctx.Inventories.Remove(ib);
                            }
                        ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges();
                    }

                    using (new PerfTimerLogger("Store inventory"))
                    {
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
                        ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges(); // now the inventory is saved, we can use its ID
                    }


                    using (new PerfTimerLogger("Store movement added"))
                    {
                        var addedTags = DevicesHandler.GetTagFromDictionnary(drawerId, DevicesHandler.ListTagAddedPerDrawer);
                        AddMovementToInventory(ctx, newInventory, addedTags, MovementType.Added, drawerId);
                        //ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges();
                    }
                    using (new PerfTimerLogger("Store movement present"))
                    {
                        var presentTags = DevicesHandler.GetTagFromDictionnary(drawerId, DevicesHandler.ListTagPerDrawer);
                        AddMovementToInventory(ctx, newInventory, presentTags, MovementType.Present, drawerId);
                        ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges();
                    }
                    using (new PerfTimerLogger("Store movement removed"))
                    {
                        var removedTags = DevicesHandler.GetTagFromDictionnary(drawerId, DevicesHandler.ListTagRemovedPerDrawer);
                        AddMovementToInventory(ctx, newInventory, removedTags, MovementType.Removed, drawerId);
                        ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges();
                    }

                    //Update event drawer
                    using (new PerfTimerLogger("Update Inventory"))
                    {
                        ctx.EventDrawerDetails.UpdateInventoryForEventDrawer(_deviceEntity, drawerId, newInventory);
                        ctx.ChangeTracker.DetectChanges();
                        ctx.SaveChanges();
                    }


                    using (new PerfTimerLogger("Send to  Indian server"))
                    {
                        ProcessSelectionFromServer.PostInventoryForDrawer(_deviceEntity, drawerId, newInventory);
                    }

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
        }

        private static void AddMovementToInventory(SmartDrawerDatabaseContext ctx, Inventory newInventory, IEnumerable tags, MovementType movement , int drawerId)
        {
            /* foreach (string uid in tags)
             {
                 var rfidTag = ctx.RfidTags.AddIfNotExisting(uid);
                 int shelveNumber = drawerId;

                 newInventory.InventoryProducts.Add(ctx.InventoryProducts.Add(new InventoryProduct
                 {
                     InventoryId = newInventory.InventoryId,
                     RfidTag = rfidTag,
                     MovementType = (int)movement,
                     Shelve = shelveNumber
                 }));              
             }  
             */
           List<InventoryProduct> ListProdToAdd = new List<InventoryProduct>();
            foreach (string uid in tags)
            {
                var rfidTag = ctx.RfidTags.AddIfNotExisting(uid);
                int shelveNumber = drawerId;
                ListProdToAdd.Add(new InventoryProduct
                {
                    InventoryId = newInventory.InventoryId,
                    RfidTag = rfidTag,
                    MovementType = (int)movement,
                    Shelve = shelveNumber
                });
            }
            IEnumerable<InventoryProduct> LstProducts =  ctx.InventoryProducts.AddRange(ListProdToAdd);
            foreach (InventoryProduct ip in LstProducts)
                newInventory.InventoryProducts.Add(ip);
        }
    }

    public enum MovementType
    {
        Added = 1,
        Present = 0,
        Removed = -1
    }
}
