using Newtonsoft.Json;
using SmartDrawerWpfApp.StaticHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using SmartDrawerDatabase.DAL;
using SmartDrawerWpfApp.Model.DeviceModel;
using Syncfusion.Data.Extensions;
using System.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.Data;
using System.Data.Entity;
using SmartDrawerDatabase;

namespace SmartDrawerWpfApp.Wcf
{

    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]
    public class SslWallNotificationService
    {
        public event MyHostEventHandler MyHostEvent = delegate { };
        public WallDevice myWall = new WallDevice();
        public MainWindow mainview0;

        [WebInvoke(Method = "GET",
       UriTemplate = "/GetWallInfoByGet",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       RequestFormat = WebMessageFormat.Json,
       ResponseFormat = WebMessageFormat.Json)]
        [PrincipalPermission(SecurityAction.Demand, Role = "WcfUser")]
        public WallDevice GetWallInfoByGet()
        {
            myWall.DeviceType = "SMARTWALL";
            if (MyHostEvent != null)
                MyHostEvent(this, new MyHostEventArgs("GetWallInfoByGet", myWall.DeviceType + ":" + myWall.DeviceSerial));
            return myWall;
        }

        [WebInvoke(Method = "POST",
           UriTemplate = "/GetWallInfo",
           BodyStyle = WebMessageBodyStyle.WrappedRequest,
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json)]
        [PrincipalPermission(SecurityAction.Demand, Role = "WcfUser")]
        public WallDevice GetWallInfo()
        {
            myWall.DeviceType = "SMARTWALL";
            if (MyHostEvent != null)
                MyHostEvent(this, new MyHostEventArgs("GetWallInfo", myWall.DeviceType + ":" + myWall.DeviceSerial));
            return myWall;
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
         UriTemplate = "/AddOrUpdateProduct",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
        public string AddOrUpdateProduct(Stream streamdata)
        {
            try
            {
                StreamReader reader = new StreamReader(streamdata);
                string res = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                JsonProductAddOrUpdate jsonProducts = JsonConvert.DeserializeObject<JsonProductAddOrUpdate>(res);
                if (jsonProducts != null)
                {
                    var ctx = RemoteDatabase.GetDbContext();
                    int nbSuccess = 0;
                    for (int loop = 0; loop < jsonProducts.listOfProducts.Length; loop++)
                    {
                        int LastCol = 100;
                        while (LastCol > 0 && string.IsNullOrEmpty(jsonProducts.listOfProducts[loop].productInfo[LastCol]))
                            LastCol--;

                        for (int bcl = 0; bcl < LastCol; bcl++)
                            if (string.IsNullOrEmpty(jsonProducts.listOfProducts[loop].productInfo[bcl]))
                                jsonProducts.listOfProducts[loop].productInfo[bcl] = " ";

                        RfidTag tag = ctx.RfidTags.AddIfNotExisting(jsonProducts.listOfProducts[loop].tagUID);
                        Product p = ctx.Products.GetByTagUid(jsonProducts.listOfProducts[loop].tagUID);
                        if (p != null)
                        {
                            ctx.Products.Attach(p);
                            p.ProductInfo0 = jsonProducts.listOfProducts[loop].productInfo[0];
                            p.ProductInfo1 = jsonProducts.listOfProducts[loop].productInfo[1];
                            p.ProductInfo2 = jsonProducts.listOfProducts[loop].productInfo[2];
                            p.ProductInfo3 = jsonProducts.listOfProducts[loop].productInfo[3];
                            p.ProductInfo4 = jsonProducts.listOfProducts[loop].productInfo[4];
                            p.ProductInfo5 = jsonProducts.listOfProducts[loop].productInfo[5];
                            p.ProductInfo6 = jsonProducts.listOfProducts[loop].productInfo[6];
                            p.ProductInfo7 = jsonProducts.listOfProducts[loop].productInfo[5];
                            p.ProductInfo8 = jsonProducts.listOfProducts[loop].productInfo[8];
                            p.ProductInfo9 = jsonProducts.listOfProducts[loop].productInfo[9];
                            p.ProductInfo10 = jsonProducts.listOfProducts[loop].productInfo[10];
                            p.ProductInfo11 = jsonProducts.listOfProducts[loop].productInfo[11];
                            p.ProductInfo12 = jsonProducts.listOfProducts[loop].productInfo[12];
                            p.ProductInfo13 = jsonProducts.listOfProducts[loop].productInfo[13];
                            p.ProductInfo14 = jsonProducts.listOfProducts[loop].productInfo[14];
                            p.ProductInfo15 = jsonProducts.listOfProducts[loop].productInfo[15];
                            p.ProductInfo16 = jsonProducts.listOfProducts[loop].productInfo[16];
                            p.ProductInfo17 = jsonProducts.listOfProducts[loop].productInfo[17];
                            p.ProductInfo18 = jsonProducts.listOfProducts[loop].productInfo[18];
                            p.ProductInfo19 = jsonProducts.listOfProducts[loop].productInfo[19];
                            ctx.Entry(p).State = System.Data.Entity.EntityState.Modified;
                            nbSuccess++;
                        }
                        else
                        {
                            var newProduct = new Product
                            {
                                RfidTag = tag,
                                ProductInfo0 = jsonProducts.listOfProducts[loop].productInfo[0],
                                ProductInfo1 = jsonProducts.listOfProducts[loop].productInfo[1],
                                ProductInfo2 = jsonProducts.listOfProducts[loop].productInfo[2],
                                ProductInfo3 = jsonProducts.listOfProducts[loop].productInfo[3],
                                ProductInfo4 = jsonProducts.listOfProducts[loop].productInfo[4],
                                ProductInfo5 = jsonProducts.listOfProducts[loop].productInfo[5],
                                ProductInfo6 = jsonProducts.listOfProducts[loop].productInfo[6],
                                ProductInfo7 = jsonProducts.listOfProducts[loop].productInfo[5],
                                ProductInfo8 = jsonProducts.listOfProducts[loop].productInfo[8],
                                ProductInfo9 = jsonProducts.listOfProducts[loop].productInfo[9],
                                ProductInfo10 = jsonProducts.listOfProducts[loop].productInfo[10],
                                ProductInfo11 = jsonProducts.listOfProducts[loop].productInfo[11],
                                ProductInfo12 = jsonProducts.listOfProducts[loop].productInfo[12],
                                ProductInfo13 = jsonProducts.listOfProducts[loop].productInfo[13],
                                ProductInfo14 = jsonProducts.listOfProducts[loop].productInfo[14],
                                ProductInfo15 = jsonProducts.listOfProducts[loop].productInfo[15],
                                ProductInfo16 = jsonProducts.listOfProducts[loop].productInfo[16],
                                ProductInfo17 = jsonProducts.listOfProducts[loop].productInfo[17],
                                ProductInfo18 = jsonProducts.listOfProducts[loop].productInfo[18],
                                ProductInfo19 = jsonProducts.listOfProducts[loop].productInfo[19],
                            };
                            ctx.Products.Add(newProduct);
                            nbSuccess++;
                        }
                        ctx.SaveChanges();
                    }
                    ctx.Database.Connection.Close();
                    ctx.Dispose();

                    if (MyHostEvent != null)
                        MyHostEvent(this, new MyHostEventArgs("AddOrUpdateProduct", nbSuccess + " Product(s) added"));
                    return "Success";
                }
                else
                    return "Failed : List of product is null or empty";
            }
            catch (Exception exp)
            {
                return "failed : " + exp.InnerException;
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
          UriTemplate = "/StockOutProduct",
          BodyStyle = WebMessageBodyStyle.WrappedRequest,
          ResponseFormat = WebMessageFormat.Json)]
        public string StockOutProduct(Stream streamdata)
        {
            try
            {
                StreamReader reader = new StreamReader(streamdata);
                string res = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                JsonProductToStockOut ListOfTags = JsonConvert.DeserializeObject<JsonProductToStockOut>(res);
                if (ListOfTags != null)
                {

                    var ctx = RemoteDatabase.GetDbContext();

                    int nbSuccess = 0;
                    for (int loop = 0; loop < ListOfTags.listOfTagId.Length; loop++)
                    {
                        Product p = ctx.Products.GetByTagUid(ListOfTags.listOfTagId[loop]);
                        if (p != null)
                            ctx.Products.Remove(p);
                        nbSuccess++;
                    }
                    ctx.SaveChanges();
                    ctx.Database.Connection.Close();
                    ctx.Dispose();

                    if (MyHostEvent != null)
                        MyHostEvent(this, new MyHostEventArgs("StockOutProduct", nbSuccess + " Product(s) Stocked out"));

                    return "Success";
                }
                else
                    return "Failed : List of tags is null or empty";


            }
            catch (Exception exp)
            {
                return "failed : " + exp.InnerException;
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
         UriTemplate = "/SelectProduct",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
        public string SelectProduct(Stream streamdata)
        {

            try
            {
                StreamReader reader = new StreamReader(streamdata);
                string res = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                if (IsOneDrawerOpen())
                    return "Failed : SmartWall has a drawer open !";

                if (mainview0 == null)
                    return "Failed : No datagrid access !";

                if (IsWallInScan())
                {
                    if (MyHostEvent != null)
                        MyHostEvent(this, new MyHostEventArgs("StopWallScan", null));

                    System.Threading.Thread.Sleep(1000);
                }

                string msg = string.Empty;
                JsonProdutToSelect ListOfTags = JsonConvert.DeserializeObject<JsonProdutToSelect>(res);
                if (ListOfTags != null)
                {
                    List<string> mylist = ListOfTags.listOfTagId.ToList();
                    //mainview0.Dispatcher.BeginInvoke(new System.Action(() =>
                    //{
                    if ((mylist != null) && (mylist.Count > 0))
                    {
                        mainview0.myDatagrid.SelectedItems.Clear();
                        if (mainview0.Data != null && mainview0.Data.Count > 0)
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            foreach (string uid in mylist)
                            {
                                foreach (RecordEntry re in mainview0.myDatagrid.View.Records)
                                {
                                    DataRowView drv = re.Data as DataRowView;
                                    if (drv.Row[0].Equals(uid))
                                        mainview0.myDatagrid.SelectedItems.Add(drv);
                                }
                            }
                            mainview0.myDatagrid.View.Refresh();
                            watch.Stop();
                            msg = string.Format(" - {0} Product(s) Selected from {1} in {2} ms", mainview0.myDatagrid.SelectedItems.Count, mylist.Count, watch.ElapsedMilliseconds);
                        }
                        else
                            return "Failed : datagrid is empty !";
                    }
                }
                else
                    msg = " - Tag list is empty";

                if (MyHostEvent != null)
                    MyHostEvent(this, new MyHostEventArgs("SelectProduct", msg));

                return "Success" + msg;

            }
            catch (Exception exp)
            {
                return "Exception : " + exp.InnerException + "-" + exp.Message;
            }
        }

        private bool IsWallInScan()
        {
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.InScan)
                    return true;
            }
            return false;
        }
        private bool IsOneDrawerOpen()
        {
            bool bRet = false;
            for (int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
            {
                if (DevicesHandler.DrawerStatus[loop] == DrawerStatusList.Open)
                    bRet = true;
            }
            return bRet;
        }


         [OperationContract]
         [WebInvoke(Method = "POST",
         UriTemplate = "/PullItemsRequest",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
         public async Task<string> PullItem(Stream streamdata)
            {
                try
                {
                    StreamReader reader = new StreamReader(streamdata);
                    string res = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();

                    JsonItemToPull jitp = JsonConvert.DeserializeObject<JsonItemToPull>(res);
                    if (jitp != null)
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                        var user = ctx.GrantedUsers.Find(jitp.userId);
                        var pullItemToAdd = new SmartDrawerDatabase.DAL.PullItem
                        {
                            PullItemDate = jitp.pullItemDate,
                            Description = jitp.description,
                            GrantedUser = user,
                            TotalToPull = jitp.listOfTagToPull.Length,

                        };
                        ctx.PullItems.Add(pullItemToAdd);
                        foreach (string uid in jitp.listOfTagToPull)
                        {
                            RfidTag tag = ctx.RfidTags.AddIfNotExisting(uid);
                            ctx.PullItemsDetails.Add(new PullItemDetail
                            {
                                PullItem = pullItemToAdd,
                                RfidTag = tag,
                            });
                        }
                        await ctx.SaveChangesAsync();
                       
                        ctx.Database.Connection.Close();
                        ctx.Dispose();

                        if (MyHostEvent != null)
                            MyHostEvent(this, new MyHostEventArgs("PullItemsRequest", null));
                        return "Success : " + pullItemToAdd.ServerPullItemId;
                    }
                    return "Error : Bad Parameters";

                }
                catch (Exception exp)
                {
                    return "Exception : " + exp.InnerException + "-" + exp.Message;
                }
            }

         [OperationContract]
         [WebInvoke(Method = "POST",
         UriTemplate = "/RemovePullItemsRequest",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
         public async Task<string> RemovePullItem(Stream streamdata)
            {
                try
                {
                    StreamReader reader = new StreamReader(streamdata);
                    string res = reader.ReadToEnd();
                    int IdToRemove;
                    reader.Close();
                    reader.Dispose();
                    if (int.TryParse(res, out IdToRemove))
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                    var pullitem = ctx.PullItems.GetByServerId(IdToRemove);
                        if (pullitem != null)
                        {
                            ctx.PullItems.Remove(pullitem);
                            await ctx.SaveChangesAsync();
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            if (MyHostEvent != null)
                                MyHostEvent(this, new MyHostEventArgs("PullItemsRequest", null));
                            return "Success : " + IdToRemove;
                        }
                        else
                        {
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            return "Failed : " + IdToRemove;
                        }
                    }
                    return "Error : Bad Parameters";
                }
                catch (Exception exp)
                {
                    return "Exception : " + exp.InnerException + "-" + exp.Message;
                }
            }

         [OperationContract]
         [WebInvoke(Method = "POST",
         UriTemplate = "/AddOrUpdateUser",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
         public async Task<string> AddOrUpdateUser(Stream streamdata)
            {
                try
                {
                    StreamReader reader = new StreamReader(streamdata);
                    string res = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();

                    JsonUser ju = JsonConvert.DeserializeObject<JsonUser>(res);
                    if (ju != null)
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                    var user = ctx.GrantedUsers.GetByServerId(ju.ServerUserId);
                        if (user != null) //update
                        {
                            if (ju.Password != null)
                                user.Password = PasswordHashing.Sha256Of(ju.Password);
                            user.FirstName = ju.FirstName;
                            user.LastName = ju.LastName;
                            user.BadgeNumber = ju.BadgeNumber;
                            ctx.Entry(user).State = EntityState.Modified;
                            await ctx.SaveChangesAsync();
                            foreach (var dev in ctx.Devices)
                                ctx.GrantedAccesses.AddOrUpdateAccess(user, dev, ctx.GrantTypes.All());
                            await ctx.SaveChangesAsync();
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            return "Success : " + user.ServerGrantedUserId;
                        }
                        else //create
                        {                        
                            GrantedUser gu = new GrantedUser()
                            {
                                Login = ju.Login,
                                ServerGrantedUserId = ju.ServerUserId,
                                Password = ju.Password != null ? PasswordHashing.Sha256Of(ju.Password) : PasswordHashing.Sha256Of("123456"),
                                FirstName = ju.FirstName,
                                LastName = ju.LastName,
                                BadgeNumber = ju.BadgeNumber,
                                UserRank = ctx.UserRanks.User(),                              
                            };
                            ctx.GrantedUsers.Add(gu);
                            await ctx.SaveChangesAsync();
                            foreach (var dev in ctx.Devices)
                                ctx.GrantedAccesses.AddOrUpdateAccess(gu, dev, ctx.GrantTypes.All());
                            await ctx.SaveChangesAsync();                           
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            if (MyHostEvent != null)
                                MyHostEvent(this, new MyHostEventArgs("UpdateUserInfoList", null));
                            return "Success : " + gu.ServerGrantedUserId;
                        }
                    }
                    return "Error : Bad Parameters";
                }
                catch (Exception exp)
                {
                    return "Exception : " + exp.InnerException + "-" + exp.Message;
                }
            }

         [OperationContract]
         [WebInvoke(Method = "POST",
         UriTemplate = "/RemoveUser",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
         public async Task<string> RemoveUser(Stream streamdata)
            {
                try
                {
                    StreamReader reader = new StreamReader(streamdata);
                    string res = reader.ReadToEnd();
                    int IdToRemove;
                    reader.Close();
                    reader.Dispose();
                    if (int.TryParse(res, out IdToRemove))
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                        var user = ctx.GrantedUsers.GetByServerId(IdToRemove);
                    if (user != null)
                        {
                            ctx.GrantedUsers.Remove(user);
                            await ctx.SaveChangesAsync();
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            if (MyHostEvent != null)
                                MyHostEvent(this, new MyHostEventArgs("UpdateUserInfoList", null));
                            return "Success : " + IdToRemove;
                        }
                        else
                        {
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            return "Failed : " + IdToRemove;
                        }
                    }
                    return "Error : Bad Parameters";
                }
                catch (Exception exp)
                {
                    return "Exception : " + exp.InnerException + "-" + exp.Message;
                }
            }


        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/RemoveUserByLogin",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json)]
        public async Task<string> RemoveUserByLogin(Stream streamdata)
        {
            try
            {
                StreamReader reader = new StreamReader(streamdata);
                string Login = reader.ReadToEnd();               
                reader.Close();
                reader.Dispose();
               
                var ctx = await RemoteDatabase.GetDbContextAsync();
                var user = ctx.GrantedUsers.GetByLogin(Login);
                if (user != null)
                {
                    ctx.GrantedUsers.Remove(user);
                    await ctx.SaveChangesAsync();
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                    if (MyHostEvent != null)
                        MyHostEvent(this, new MyHostEventArgs("UpdateUserInfoList", null));
                    return "Success : " + Login;
                }
                else
                {
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                    return "Failed : " + Login;
                }            
               
            }
            catch (Exception exp)
            {
                return "Exception : " + exp.InnerException + "-" + exp.Message;
            }
        }

        [OperationContract]
         [WebInvoke(Method = "POST",
         UriTemplate = "/AddOrUpdateUserFingerprint",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json)]
         public async Task<string> AddOrUpdateUserFingerprint(Stream streamdata)
            {
                try
                {
                    StreamReader reader = new StreamReader(streamdata);
                    string res = reader.ReadToEnd();
                    reader.Close();
                    reader.Dispose();

                    JsonUserFingerprint juf = JsonConvert.DeserializeObject<JsonUserFingerprint>(res);
                    if (juf != null)
                    {
                        var ctx = await RemoteDatabase.GetDbContextAsync();
                        var user = ctx.GrantedUsers.Find(juf.GrantedUserId);
                        if (user != null) //user Exist
                        {
                            var fingerprint =  ctx.Fingerprints.FirstOrDefault(fp => fp.Index == juf.Index && fp.GrantedUserId == user.GrantedUserId);
                            if (fingerprint != null)
                                ctx.Fingerprints.Remove(fingerprint);

                            ctx.Fingerprints.Add(new SmartDrawerDatabase.DAL.Fingerprint
                            {
                                Index = juf.Index,
                                GrantedUserId = user.GrantedUserId,
                                Template = juf.Template
                            });

                            await ctx.SaveChangesAsync();
                            ctx.Database.Connection.Close();
                            ctx.Dispose();
                            MyHostEvent(this, new MyHostEventArgs("UpdateUserInfoList", null));
                            return "Success : " + user.GrantedUserId;
                        }
                        else //create
                        {                        
                            return "Failed : " + juf.GrantedUserId;
                        }
                    }
                    return "Error : Bad Parameters";
                }
                catch (Exception exp)
                {
                    return "Exception : " + exp.InnerException + "-" + exp.Message;
                }
            }

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/RemoveUserFingerprint",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json)]
        public async Task<string> RemoveUserFingerprint(Stream streamdata)
        {
            try
            {
                StreamReader reader = new StreamReader(streamdata);
                string res = reader.ReadToEnd();
                reader.Close();
                reader.Dispose();

                JsonUserFingerprint juf = JsonConvert.DeserializeObject<JsonUserFingerprint>(res);
                if (juf != null)
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    var user = ctx.GrantedUsers.Find(juf.GrantedUserId);
                    if (user != null) //user Exist
                    {
                        var fingerprint = ctx.Fingerprints.FirstOrDefault(fp => fp.Index == juf.Index && fp.GrantedUserId == user.GrantedUserId);
                        if (fingerprint != null)
                            ctx.Fingerprints.Remove(fingerprint);
                        MyHostEvent(this, new MyHostEventArgs("UpdateUserInfoList", null));
                        return "Success : " + user.GrantedUserId;
                    }
                    else //create
                    {
                        return "Failed : " + juf.GrantedUserId;
                    }
                    ctx.Database.Connection.Close();
                    ctx.Dispose();
                }
                return "Error : Bad Parameters";
            }
            catch (Exception exp)
            {
                return "Exception : " + exp.InnerException + "-" + exp.Message;
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/getCurrentInventory",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json)]
        public JsonInventory getCurrentInventory()
        {
            try
            {
                List<TagInfo> lstTag = new List<TagInfo>();                             
                for(int loop = 1; loop <= DevicesHandler.NbDrawer; loop++)
                {
                    List<string> TmpListCtrlPerDrawer = new List<string>(DevicesHandler.GetTagFromDictionnary(loop, DevicesHandler.ListTagPerDrawer));
                    List<string> TmpListCtrlPerDrawerAdded = new List<string>(DevicesHandler.GetTagFromDictionnary(loop, DevicesHandler.ListTagAddedPerDrawer));
                    List<string> TmpListCtrlPerDrawerRemoved = new List<string>(DevicesHandler.GetTagFromDictionnary(loop, DevicesHandler.ListTagRemovedPerDrawer));

                    foreach (string uid in TmpListCtrlPerDrawerAdded)
                    {
                        if (TmpListCtrlPerDrawer.Contains(uid))
                            TmpListCtrlPerDrawer.Remove(uid);
                        lstTag.Add(new TagInfo()
                        {
                            tagUID = uid,
                            DrawerId = loop,
                            Movement = 1,
                        });
                    }

                    foreach (string uid in TmpListCtrlPerDrawer)
                    {                      
                        lstTag.Add(new TagInfo()
                        {
                            tagUID = uid,
                            DrawerId = loop,
                            Movement = 0,
                        });
                    }
                    foreach (string uid in TmpListCtrlPerDrawerRemoved)
                    {
                        lstTag.Add(new TagInfo()
                        {
                            tagUID = uid,
                            DrawerId = loop,
                            Movement = -1,
                        });
                    }
                }
                JsonInventory ji = new JsonInventory();
                ji.listOfTags = lstTag.ToArray();
                ji.Status = "Success";
                return ji;
               
            }
            catch (Exception exp)
            {
                JsonInventory ret = new JsonInventory();
                ret.Status = "Failed";
                ret.Reason = "Exception : " + exp.InnerException + " - " + exp.Message;
                ret.listOfTags = null;
                return ret;
            }
        }

        [OperationContract]
        [WebInvoke(Method = "POST",
        UriTemplate = "/getLastDrawerInventory",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json)]
        public async Task<JsonDrawerInventory> getLastDrawerInventory(Stream streamdata)
        {
            try
            {
                StreamReader reader = new StreamReader(streamdata);
                string res = reader.ReadToEnd();
                int drawerNb;
                reader.Close();
                reader.Dispose();
                if (int.TryParse(res, out drawerNb))
                {
                    var ctx = await RemoteDatabase.GetDbContextAsync();
                    var LastDrawerInv = ctx.Inventories.GetLastInventoryforDrawer(drawerNb);
                    if (LastDrawerInv != null)
                    {
                        JsonDrawerInventory ret = new JsonDrawerInventory();
                        ret.Status = "Success";
                        ret.ServerDeviceId = LastDrawerInv.Device.ServerDeviceID;
                        ret.DrawerNumber = LastDrawerInv.DrawerNumber;
                        ret.InventoryDate = LastDrawerInv.InventoryDate;
                        ret.TotalAdded = LastDrawerInv.TotalAdded;
                        ret.TotalPresent = LastDrawerInv.TotalPresent;
                        ret.TotalRemoved = LastDrawerInv.TotalRemoved;

                        var invProducts = LastDrawerInv.InventoryProducts;
                        if (invProducts != null)
                        {
                            List<TagInfo> lstTags = new List<TagInfo>();
                            int nIndex = 0;                            foreach (var tag in LastDrawerInv.InventoryProducts)
                            {
                                TagInfo ti = new TagInfo() { DrawerId = tag.Shelve, tagUID = tag.RfidTag.TagUid , Movement = tag.MovementType };
                                lstTags.Add(ti);
                            }
                            ret.listOfTags = lstTags.ToArray();
                        }

                        var ListEvent = ctx.EventDrawerDetails.GetEventForDrawerByInventoryID(LastDrawerInv.Device, drawerNb,LastDrawerInv.InventoryId);
                        if ((ListEvent != null) )
                        {
                            List<UserEventInfo> lstUserEventInfo = new List<UserEventInfo>();
                          
                            foreach (var userEvent in ListEvent)
                            {
                                UserEventInfo uei = new UserEventInfo() { ServerGrantedUserId = userEvent.GrantedUser.ServerGrantedUserId, EventDrawerDate = userEvent.EventDrawerDate };
                                lstUserEventInfo.Add(uei);
                            }
                            ret.listOfUserEvent = lstUserEventInfo.ToArray();
                        }
                      
                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                        return ret;
                    }
                    else
                    {
                        JsonDrawerInventory ret = new JsonDrawerInventory();
                        ret.Status = "Failed";
                        ret.Reason = "No Inventory found";
                        ret.listOfTags = null;
                        ret.listOfUserEvent = null;
                        ctx.Database.Connection.Close();
                        ctx.Dispose();
                        return ret;
                    }
                  
                }
                else
                {
                    JsonDrawerInventory ret = new JsonDrawerInventory();
                    ret.Status = "Failed";
                    ret.Reason = "Error : Bad Parameters"; 
                    ret.listOfTags = null;
                    ret.listOfUserEvent = null;
                    return ret;
                }                  
            }
            catch (Exception exp)
            {
                JsonDrawerInventory ret = new JsonDrawerInventory();
                ret.Status = "Failed";
                ret.Reason = "Exception : " + exp.InnerException + " - " + exp.Message;
                ret.listOfTags = null;
                ret.listOfUserEvent = null;
                return ret;
            }
        }
    }    
}
