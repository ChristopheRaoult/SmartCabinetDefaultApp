﻿using RestSharp;
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
                int serverPort = Properties.Settings.Default.serverPort;

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
                throw;
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
                int serverPort = Properties.Settings.Default.serverPort;

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
                int serverPort = Properties.Settings.Default.serverPort;

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
                int serverPort = Properties.Settings.Default.serverPort;
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
                    Device mydev = ctx.Devices.GetByRfidSerialNumber(Properties.Settings.Default.RfidSerial);

                    var lstUser = JsonUserList.DeserializedJsonList(response.Content);
                    if ((lstUser != null) && (lstUser.Length > 0))
                    {
                        foreach (JsonUserList jsl in lstUser)
                        {

                            var original = ctx.GrantedUsers.GetByServerId(jsl.user_id);
                            if (original != null)
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
                            else if (original == null)
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
                                ctx.GrantedAccesses.AddOrUpdateAccess(original, mydev, ctx.GrantTypes.All());
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
                int serverPort = Properties.Settings.Default.serverPort;
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
        public static async Task<bool> PostInventoryForDrawer(Device device, int drawerId , Inventory inventory)
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.serverPort;
                string urlServer = "http://" + serverIP + ":" + serverPort;
                var client = new RestClient(urlServer);
                client.Authenticator = new HttpBasicAuthenticator(privateApiLogin, privateApiMdp);

                var request = new RestRequest("stockhistories", Method.POST);

                request.AddParameter("serial_num", device.RfidSerial);
                request.AddParameter("drawer", drawerId.ToString());
                request.AddParameter("created_at", inventory.InventoryDate);

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
                var invUser = ctx.EventDrawerDetails.GetEventForDrawerByInventoryID(device, drawerId ,inventory.InventoryId);
                if (invUser != null)
                foreach (EventDrawerDetail edd in invUser)
                        request.AddParameter("user_login", edd.GrantedUser.Login);
                var response = await client.ExecuteTaskAsync(request);
                return response.IsSuccessful;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error post inventory");
                exp.ShowDialog();
            }
            return false;
        }
        #endregion
    }
}