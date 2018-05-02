using RestSharp;
using RestSharp.Authenticators;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.WcfServer
{
    public static class ProcessSelectionFromServer
    {

        private static string publicApiLogin = "userp";
        private static string publicApiMdp = "F00lpro0f";

        private static string privateApiLogin = "userc";
        private static string privateApiMdp = "Spacecode4SmarrtDrawer";
        #region Selection
        public static async Task<bool> GetAndStoreSelectionAsync()
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("selections", Method.GET);
                var response = await client.ExecuteTaskAsync(request);

                if (response.IsSuccessful)
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    ctx.PullItems.Clear();
                    await ctx.SaveChangesAsync();

                    var lstSelection = JsonSelectionList.DeserializedJsonList(response.Content);
                    if ((lstSelection != null) && (lstSelection.Length > 0))
                    {
                        foreach (JsonSelectionList jsl in lstSelection)
                        {
                            if (jsl.state == "closed") continue;
                            if (jsl.listOfTagToPull == null) continue;

                            /********************/
                            GrantedUser user = null;
                            if (jsl.user_id.HasValue)
                            {
                                user = ctx.GrantedUsers.GetByServerId(jsl.user_id.Value);
                            }
                            var pullItemToAdd = new SmartDrawerDatabase.DAL.PullItem
                            {
                                ServerPullItemId = jsl.selection_id,
                                PullItemDate = jsl.created_at,
                                Description = string.IsNullOrEmpty(jsl.description) ? " " : jsl.description,
                                GrantedUser = user,
                                TotalToPull = jsl.listOfTagToPull.Count,

                            };
                            ctx.PullItems.Add(pullItemToAdd);
                            foreach (string uid in jsl.listOfTagToPull)
                            {
                                if (string.IsNullOrEmpty(uid)) continue;
                                RfidTag tag = ctx.RfidTags.AddIfNotExisting(uid);
                                ctx.PullItemsDetails.Add(new PullItemDetail
                                {
                                    PullItem = pullItemToAdd,
                                    RfidTag = tag,
                                });
                            }
                            await ctx.SaveChangesAsync();
                        }

                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                        return true;
                    }
                    return true;
                }
                return false;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                return false;
                
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error getting selection");
                exp.ShowDialog();
                return false;
            }
        }
        public static async Task<bool> UpdateSelectionAsync(int IdSel, List<string> TagToRemove)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("selections/" + IdSel, Method.GET);
                var response = await client.ExecuteTaskAsync(request);

                if (response.IsSuccessful)
                {
                    var Selection = JsonSelectionList.DeserializedJsonAlone(response.Content);
                    if (Selection != null)
                    {

                        if (Selection.state == "closed") return false;

                        var request2 = new RestRequest("/selections/" + IdSel, Method.PUT);

                        foreach (string uid in TagToRemove)
                            request2.AddParameter("listOfTagPulled", uid);

                        var client2 = new RestClient(urlServer);
                        client2.Authenticator = new HttpBasicAuthenticator(privateApiLogin, privateApiMdp);
                        var response2 = await client2.ExecuteTaskAsync(request2);
                        return response2.IsSuccessful;

                    }
                    return false;
                }
                return false;
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error update selection");
                exp.ShowDialog();
                return false;
            }
        }
        public static async Task<bool> DeleteSelectionAsync(int IdSel)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("selections/" + IdSel, Method.DELETE);
                var response = await client.ExecuteTaskAsync(request);
                return response.IsSuccessful;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error Delete Selection");
                exp.ShowDialog();
                return false;
            }
        }

        #endregion
        #region User
        public static async Task<bool> GetAndStoreUserAsync()
        {
            try
            {

                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;
                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("/users", Method.GET);
                var response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {

                    // remove  granted standard users
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    ctx.GrantedAccesses.Clear();
                    await ctx.SaveChangesAsync();

                    //get device
                    Device mydev = ctx.Devices.GetBySerialNumber(Properties.Settings.Default.WallSerial);

                    if (mydev == null) return false;

                    var lstUser = JsonUserList.DeserializedJsonList(response.Content);
                    if ((lstUser != null) && (lstUser.Length > 0))
                    {
                        foreach (JsonUserList jsl in lstUser)
                        {

                            var original = ctx.GrantedUsers.GetByServerId(jsl.user_id);
                            var original2 = ctx.GrantedUsers.GetByLogin(jsl.login);
                            if ((original != null) && (original.Login != "Admin"))
                            {
                                TimeSpan ts = jsl.updated_at - original.UpdateAt;
                                if (Math.Abs(ts.TotalSeconds) > 1)  // Not the latest but avoid ms 
                                {
                                    original.ServerGrantedUserId = jsl.user_id;
                                    original.Login = jsl.login;
                                    original.Password = jsl.password;
                                    original.FirstName = jsl.fname;
                                    original.LastName = jsl.lname;
                                    original.BadgeNumber = jsl.badge_num;
                                    original.UserRankId = 3;
                                    original.UpdateAt = jsl.updated_at;
                                    ctx.Entry(original).State = EntityState.Modified;
                                    await ctx.SaveChangesAsync();

                                    //deletefingerprint for this user if exists

                                    var fpUser = ctx.Fingerprints.Where(gu => gu.GrantedUserId == original.GrantedUserId).ToList();
                                    if (fpUser != null)
                                    {
                                        foreach (SmartDrawerDatabase.DAL.Fingerprint fp in fpUser)
                                            ctx.Fingerprints.Remove(fp);
                                        await ctx.SaveChangesAsync();
                                    }

                                    if ((jsl.ftemplate != null) & (jsl.ftemplate.Count > 0))
                                    {
                                        for (int loop = 0; loop < jsl.ftemplate.Count; loop++)
                                        {
                                            ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                            {
                                                GrantedUserId = original.GrantedUserId,
                                                Index = Convert.ToInt32(jsl.finger_index[loop]),
                                                Template = jsl.ftemplate[loop],
                                            });
                                        }
                                        await ctx.SaveChangesAsync();
                                    }
                                }
                                ctx.GrantedAccesses.AddOrUpdateAccess(original, mydev, ctx.GrantTypes.All());
                                await ctx.SaveChangesAsync();

                            }
                            else if (original2 != null)
                            {
                                TimeSpan ts = jsl.updated_at - original.UpdateAt;
                                if (Math.Abs(ts.TotalSeconds) > 1)  // Not the latest but avoid ms 
                                {
                                    original2.ServerGrantedUserId = jsl.user_id;
                                    original2.Login = jsl.login;
                                    original2.Password = jsl.password;
                                    original2.FirstName = jsl.fname;
                                    original2.LastName = jsl.lname;
                                    original2.BadgeNumber = jsl.badge_num;
                                    original2.UserRankId = 3;
                                    original2.UpdateAt = jsl.updated_at;
                                    ctx.Entry(original2).State = EntityState.Modified;
                                    await ctx.SaveChangesAsync();

                                    //deletefingerprint for this user if exists

                                    var fpUser = ctx.Fingerprints.Where(gu => gu.GrantedUserId == original.GrantedUserId).ToList();
                                    if (fpUser != null)
                                    {
                                        foreach (SmartDrawerDatabase.DAL.Fingerprint fp in fpUser)
                                            ctx.Fingerprints.Remove(fp);
                                        await ctx.SaveChangesAsync();
                                    }

                                    if ((jsl.ftemplate != null) & (jsl.ftemplate.Count > 0))
                                    {
                                        for (int loop = 0; loop < jsl.ftemplate.Count; loop++)
                                        {
                                            ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                            {
                                                GrantedUserId = original2.GrantedUserId,
                                                Index = Convert.ToInt32(jsl.finger_index[loop]),
                                                Template = jsl.ftemplate[loop],
                                            });
                                        }
                                        await ctx.SaveChangesAsync();
                                    }
                                }
                                ctx.GrantedAccesses.AddOrUpdateAccess(original2, mydev, ctx.GrantTypes.All());
                                await ctx.SaveChangesAsync();
                            }
                            else if ((original == null) && (original2 == null))
                            {
                                GrantedUser newUser = new GrantedUser()
                                {
                                    ServerGrantedUserId = jsl.user_id,
                                    Login = jsl.login,
                                    Password = jsl.password,
                                    FirstName = jsl.fname,
                                    LastName = jsl.lname,
                                    BadgeNumber = jsl.badge_num,
                                    UpdateAt = jsl.updated_at,
                                    UserRankId = 3,
                                };
                                ctx.GrantedUsers.Add(newUser);
                                await ctx.SaveChangesAsync();

                                if ((jsl.ftemplate != null) & (jsl.ftemplate.Count > 0))
                                {
                                    for (int loop = 0; loop < jsl.ftemplate.Count; loop++)
                                    {
                                        ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                                        {
                                            GrantedUserId = newUser.GrantedUserId,
                                            Index = Convert.ToInt32(jsl.finger_index[loop]),
                                            Template = jsl.ftemplate[loop],
                                        });
                                    }
                                    await ctx.SaveChangesAsync();
                                }
                                ctx.GrantedAccesses.AddOrUpdateAccess(newUser, mydev, ctx.GrantTypes.All());
                                await ctx.SaveChangesAsync();
                            }

                        }

                        ctx.Database.Connection.Close();
                        ctx.Dispose();

                        return true;
                    }

                    return true;
                }
                return false;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error getting user");
                exp.ShowDialog();
                return false;
            }
        }
        public static async Task<bool> UpdateUserAsync(string login)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;
                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("users/" + login, Method.GET);
                var response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    var User = JsonUserList.DeserializedJsonAlone(response.Content);
                    if (User != null)
                    {
                        var request2 = new RestRequest("users/" + login, Method.PUT);
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                        var dbUser = ctx.GrantedUsers.GetByServerId(User.user_id);
                        if (dbUser != null)
                        {
                            request2.AddParameter("login", dbUser.Login);
                            request2.AddParameter("password", dbUser.Password);
                            request2.AddParameter("lname", dbUser.LastName);
                            request2.AddParameter("fname", dbUser.FirstName);
                            request2.AddParameter("badge_num", dbUser.BadgeNumber);
                            var dbFingers = ctx.Fingerprints.Where(fp => fp.GrantedUserId == dbUser.GrantedUserId).ToList();

                            if (dbFingers != null)
                            {

                                foreach (SmartDrawerDatabase.DAL.Fingerprint fp in dbFingers)
                                {
                                    request2.AddParameter("finger_index", fp.Index.ToString());
                                    request2.AddParameter("ftemplate", fp.Template);
                                }
                            }
                            var response2 = await client.ExecuteTaskAsync(request2);
                            return response2.IsSuccessful;
                        }
                    }
                    return false;
                }
            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error update user");
                exp.ShowDialog();
            }
            return false;
        }


        #endregion
        #region Inventory
        public static async Task<bool> PostInventoryForDrawer(Device device, int drawerId, Inventory inventory)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;
                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(privateApiLogin, privateApiMdp);

                var request = new RestRequest("stockhistories", Method.POST);

                request.AddParameter("serial_num", device.DeviceSerial);
                request.AddParameter("drawer", drawerId.ToString());
                request.AddParameter("created_at", inventory.InventoryDate.ToUniversalTime().ToString("u"));

                foreach (InventoryProduct ip in inventory.InventoryProducts)
                {
                    switch (ip.MovementType)
                    {
                        case -1:
                            request.AddParameter("removed_tags", ip.RfidTag.TagUid);
                            break;
                        case 0:
                            request.AddParameter("present_tags", ip.RfidTag.TagUid);
                            break;
                        case 1:
                            request.AddParameter("added_tags", ip.RfidTag.TagUid);
                            break;
                    }
                }

                var ctx = await RemoteDatabase.GetDbContextAsync();
                var invUser = ctx.EventDrawerDetails.GetEventForDrawerByInventoryID(device, drawerId, inventory.InventoryId);
                if (invUser != null)
                    foreach (EventDrawerDetail edd in invUser)
                    {
                        if ((edd.GrantedUser !=  null) && (!string.IsNullOrEmpty(edd.GrantedUser.Login)))
                        request.AddParameter("user_login", edd.GrantedUser.Login);
                    }
                var response = await client.ExecuteTaskAsync(request);
                LogToFile.LogMessageToFile(response.ResponseStatus.ToString());
                LogToFile.LogMessageToFile(response.Content.ToString());              

                return response.IsSuccessful;

            }
            catch (Exception error)
            {
                LogToFile.LogMessageToFile(error.InnerException.ToString());
                LogToFile.LogMessageToFile(error.Message);
                LogToFile.LogMessageToFile(error.StackTrace);

            }
            return false;
        }
        #endregion
        #region device

        public static async Task<List<Device>> GetCabinets()
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("cabinets", Method.GET);
                var response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    List<Device> lstDevice = new List<Device>();
                    var devices = JsonDevice.DeserializedJsonList(response.Content);
                    if ((devices != null) && (devices.Count() > 0))
                    {
                        foreach (JsonDevice jd in devices)
                        {
                            Device newDev = new Device
                            {
                                DeviceName = jd.name,
                                DeviceSerial = jd.serial_num,
                                DeviceLocation = jd.Location,
                                IpAddress = jd.IP_addr,
                            };
                            lstDevice.Add(newDev);
                        }
                    }
                    return lstDevice;

                }
                else
                    return null;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error Delete Selection");
                exp.ShowDialog();
                return null;
            }
        }
        public static async Task<Device> GetCabinet(string serial)
        {
            try
            {
                Properties.Settings.Default.Reload();
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("cabinets/" + serial, Method.GET);
                var response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {
                    
                    var device = JsonDevice.DeserializedJsonAlone(response.Content);
                    if (device != null) 
                    {                       
                        Device newDev = new Device
                        {
                            DeviceName = device.name,
                            DeviceSerial = device.serial_num,
                            DeviceLocation = device.Location,
                            IpAddress = device.IP_addr,
                        };
                        return newDev;
                      
                    }
                    return null;
                }
                else
                    return null;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error Delete Selection");
                exp.ShowDialog();
                return null;
            }

           /* try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("cabinets", Method.GET);
                var response = await client.ExecuteTaskAsync(request);
                if (response.IsSuccessful)
                {                  
                    var devices = JsonDevice.DeserializedJsonList(response.Content);
                    if ((devices != null) && (devices.Count() > 0))
                    {
                        foreach (JsonDevice jd in devices)
                        {
                            if (jd.serial_num == serial)
                            {
                                Device newDev = new Device
                                {
                                    DeviceName = jd.name,
                                    DeviceSerial = jd.serial_num,
                                    DeviceLocation = jd.Location,
                                    IpAddress = jd.IP_addr,
                                };
                                return newDev;
                            }
                        }
                    }
                    return null;
                }
                else
                    return null;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error Delete Selection");
                exp.ShowDialog();
                return null;
            }*/

        }
        public static async Task<bool> CreateCabinet(Device dev)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("cabinets/", Method.POST);

                request.AddParameter("serial_num", dev.DeviceSerial);
                request.AddParameter("name", dev.DeviceName);
                request.AddParameter("location", dev.DeviceLocation);

                // If default parameter is nul get found IP
                if (string.IsNullOrEmpty(Properties.Settings.Default.NotificationIp))
                    request.AddParameter("IP_addr", dev.IpAddress);
                else
                    request.AddParameter("IP_addr", Properties.Settings.Default.NotificationIp);
                request.AddParameter("port", Properties.Settings.Default.NotificationPort);

                var response = await client.ExecuteTaskAsync(request);
                return response.IsSuccessful;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error Delete Selection");
                exp.ShowDialog();
                return false;
            }
        }
        public static async Task<bool> UpdateCabinet(Device dev)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.ServerPort;

                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(publicApiLogin, publicApiMdp);
                var request = new RestRequest("cabinets/" + dev.DeviceSerial, Method.PUT);

                request.AddParameter("name", dev.DeviceName);
                request.AddParameter("location", dev.DeviceLocation);
                // If default parameter is nul get found IP
                if (string.IsNullOrEmpty(Properties.Settings.Default.NotificationIp))
                    request.AddParameter("IP_addr", dev.IpAddress);
                else
                    request.AddParameter("IP_addr", Properties.Settings.Default.NotificationIp);
                request.AddParameter("port", Properties.Settings.Default.NotificationPort);

                var response = await client.ExecuteTaskAsync(request);
                return response.IsSuccessful;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error Delete Selection");
                exp.ShowDialog();
                return false;
            }
        }
    }

        #endregion
        
}
