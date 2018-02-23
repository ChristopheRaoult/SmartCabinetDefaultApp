using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.WcfServer
{
    public static class ProcessSelectionFromServer
    {
        #region Selection
        public static async Task<bool> GetAndStoreSelectionAsync()
        {
            try
            {
                string serverIP = Properties.Settings.Default.ServerIp;
                int serverPort = Properties.Settings.Default.serverPort;

                string urlSelection = "http://" + serverIP + ":" + serverPort + "/selections";
                HttpClient client = new HttpClient();

                var ctx = await RemoteDatabase.GetDbContextAsync();
                ctx.PullItems.Clear();
                await ctx.SaveChangesAsync();

                var responseString = await client.GetStringAsync(urlSelection);
                var lstSelection = JsonSelectionList.DeserializedJsonList(responseString);
                if ((lstSelection != null) && (lstSelection.Length > 0 ))
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
                        foreach (string uid in  jsl.listOfTagToPull)
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
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error selection");
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

                string urlSelection = "http://" + serverIP + ":" + serverPort + "/selections/" + IdSel;
                HttpClient client = new HttpClient();

                var responseString = await client.GetStringAsync(urlSelection);
                client.Dispose();
                var Selection = JsonSelectionList.DeserializedJsonAlone(responseString);

                if (Selection != null)
                {
                    if (Selection.state == "closed") return false;
                    if (Selection.listOfTagPulled == null) return false;

                    foreach (string uid in TagToRemove)
                    {
                        if (!Selection.listOfTagPulled.Contains(uid))
                        {                           
                            Selection.listOfTagPulled.Add(uid);
                        }
                    }                   

                    HttpClient clientRequest = new HttpClient();
                    var JsonContent = new StringContent(JsonSelectionList.SerializedJsonAlone(Selection));

                    var task =  await clientRequest.PutAsync(urlSelection, JsonContent);
                    return task.IsSuccessStatusCode;
                }
                return false;

            }
            catch (Exception error)
            {
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error selection");
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

                string urlSelection = "http://" + serverIP + ":" + serverPort + "/selections/" + IdSel;              
                HttpClient clientRequest = new HttpClient();
                   
                 var task = await clientRequest.DeleteAsync(urlSelection);
                 return task.IsSuccessStatusCode; 
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

                string urlUser = "http://" + serverIP + ":" + serverPort + "/users";
                HttpClient client = new HttpClient();

                var ctx = await RemoteDatabase.GetDbContextAsync();
                ctx.GrantedUsers.Clear();
                await ctx.SaveChangesAsync();

                var responseString = await client.GetStringAsync(urlUser);
                var lstUser = JsonUserList.DeserializedJsonList(responseString);
                if ((lstUser != null) && (lstUser.Length > 0))
                {
                    foreach (JsonUserList jsl in lstUser)
                    {    
                        
                        GrantedUser newUser = new GrantedUser()
                        {
                            ServerGrantedUserId = jsl.user_id,
                            Login = jsl.login,
                            Password = jsl.password,
                            FirstName = jsl.fname,
                            LastName = jsl.lname,
                            BadgeNumber = jsl.badge_num,
                            UserRankId = 3,
                        };

                        ctx.GrantedUsers.Add(newUser);
                        await ctx.SaveChangesAsync();

                        if ( (jsl.ftemplate != null) & (jsl.ftemplate.Count > 0 ))
                        {
                            for (int loop = 0; loop < jsl.ftemplate.Count; loop ++)
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
                    }
                  
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                    return true;
                }

                return true;
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
                ExceptionMessageBox exp = new ExceptionMessageBox(error, "Error selection");
                exp.ShowDialog();
                return false;
            }
        }
        #endregion
    }
}
