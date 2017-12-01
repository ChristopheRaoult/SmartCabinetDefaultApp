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
                        Product p =  ctx.Products.GetByTagUid(jsonProducts.listOfProducts[loop].tagUID);
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
                            ctx.SaveChanges();
                            nbSuccess++;
                        }
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
                        if (mainview0.myDatagrid.View.Records.Count() > 0)
                        {
                            var watch = System.Diagnostics.Stopwatch.StartNew();
                            foreach (string uid in mylist)
                            {                              
                                mainview0.myDatagrid.SearchHelper.ClearSearch();
                                mainview0.myDatagrid.SearchHelper.Search(uid);
                                var list = mainview0.myDatagrid.SearchHelper.GetSearchRecords();
                                if (list != null && list.Count > 0)
                                {
                                   int recordIndex =  mainview0.myDatagrid.ResolveToRecordIndex(mainview0.myDatagrid.ResolveToRowIndex(list[0].Record));
                                    var datarow = mainview0.myDatagrid.GetRowGenerator().Items[recordIndex];
                                    mainview0.myDatagrid.SelectedItems.Add(datarow);
                                }
                            }
                            watch.Stop();
                            msg = string.Format(" - {0} Product(s) Selected from {1} in {2} ms", mainview0.myDatagrid.SelectedItems.Count, mylist.Count, watch.ElapsedMilliseconds);
                        }
                        else
                            return "Failed : datagrid is empty !";                    }   
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
    }


}
