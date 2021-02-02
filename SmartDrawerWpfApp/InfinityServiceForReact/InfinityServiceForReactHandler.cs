using RestSharp;
using SmartDrawerWpfApp.StaticHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SmartDrawerWpfApp.InfinityServiceForReact
{
    public static class InfinityServiceForReactHandler
    {

        private static int Timeout = 10;

        /// <summary>
        /// Property to save user token
        /// </summary>
        private static ReturnLogin _LastReturnLogin;
        public static ReturnLogin LastReturnLogin
        {
            get { return _LastReturnLogin; }
            set { _LastReturnLogin = value; }
        }

        /// <summary>
        /// Property to save last Device registered
        /// </summary>
        private static ReturnRegisterDevice _LastReturnRegisterDevice;
        public static ReturnRegisterDevice LastReturnRegisterDevice
        {
            get { return _LastReturnRegisterDevice; }
            set { _LastReturnRegisterDevice = value; }
        }

        private static SkuInfo _LastSkuInfo;
        public static SkuInfo LastSkuInfo
        {
            get { return _LastSkuInfo; }
            set { _LastSkuInfo = value; }
        }

        private static ReturnUpdateUser _LastReturnUpdateUser;
        public static ReturnUpdateUser LastReturnUpdateUser
        {
            get { return _LastReturnUpdateUser; }
            set { _LastReturnUpdateUser = value; }
        }

        private static ReactUserInfo _LastUserInfo;
        public static ReactUserInfo LastUserInfo
        {
            get { return _LastUserInfo; }
            set { _LastUserInfo = value; }
        }

        /*private static DiamondMatchSelectionInfo _LastDiamondMatchSelectionInfo;
        public static DiamondMatchSelectionInfo LastDiamondMatchSelectionInfo
        {
            get { return _LastDiamondMatchSelectionInfo; }
            set { _LastDiamondMatchSelectionInfo = value; }
        }*/

        private static ReturnRawActivity _LastReturnRawActivity;
        public static ReturnRawActivity LastReturnRawActivity
        {
            get { return _LastReturnRawActivity; }
            set { _LastReturnRawActivity = value; }
        }

        private static ReturnUpdateDiamondMatch _LastReturnUpdateDiamondMatch;
        public static ReturnUpdateDiamondMatch LastReturnUpdateDiamondMatch
        {
            get { return _LastReturnUpdateDiamondMatch; }
            set { _LastReturnUpdateDiamondMatch = value; }
        }

        private static LedTriggerInfo _LastLedSelection;
        public static LedTriggerInfo LastLedSelection
        {
            get { return _LastLedSelection; }
            set { _LastLedSelection = value; }
        }

        /// <summary>
        /// Function to Get Token to use API REACT - 
        /// </summary>
        /// <param name="url">base url of the server</param>
        /// <param name="email">email of user</param>
        /// <param name="password">password of user</param>
        /// <returns></returns>
        public static async Task<bool> Login(string url, string email, string password)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Login --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/auth/login", Method.POST);

                LoginInfo li = new LoginInfo() { email = email, password = password };
                string body = LoginInfo.SerializedJsonAlone(li);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                LogToFile.LogMessageToFile("Body :" + body);

                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);
                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = ReturnLogin.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnLogin = tmp;
                    else
                        _LastReturnLogin = null;
                    LogToFile.LogMessageToFile("------- End Login --------");
                    return true;
                }
                else
                    LogToFile.LogMessageToFile("------- End Login --------");
                return false;


            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End Login --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End Login --------");
                return false;
            }
        }
        public static async Task<bool> RegisterDevice(string url, string token, string deviceSerial)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Register --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/device/register-device", Method.POST);
                request.AddHeader("Authorization", token);

                RegisterDeviceInfo rdi = new RegisterDeviceInfo() { serialNumber = deviceSerial };
                string body = RegisterDeviceInfo.SerializedJsonAlone(rdi);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                LogToFile.LogMessageToFile("Body :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = ReturnRegisterDevice.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnRegisterDevice = tmp;
                    else
                        _LastReturnRegisterDevice = null;
                    LogToFile.LogMessageToFile("------- End  Register --------");
                    return true;
                }
                else
                    LogToFile.LogMessageToFile("------- End  Register --------");
                return false;


            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End  Register --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End  Register --------");
                return false;
            }
        }
        public static async Task<bool> GetSku(string url, string token, List<string> TagIds)
        {
            if (string.IsNullOrEmpty(url)) return false;


            if (TagIds.Count == 1)
            {
                if (LastSkuInfo != null)
                {
                    SkuData sto = (from si in InfinityServiceForReactHandler.LastSkuInfo.data
                                   where si.rfId.rfid == TagIds[0]
                                   select si).SingleOrDefault();
                    if (sto != null)
                        return true;
                }
            }


            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start GetSku --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/sku/get-by-tag", Method.GET);
                request.AddHeader("Authorization", token);

                string tags = "[\"" + string.Join("\",\"", TagIds) + "\"]";
                request.AddParameter("tagNo", string.Join(",", tags));

                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = SkuInfo.DeserializedJsonAlone(response.Content);
                    if (tmp != null)
                        _LastSkuInfo = tmp;
                    else
                        _LastSkuInfo = null;
                    LogToFile.LogMessageToFile("------- End  GetSku --------");
                    return true;
                }
                else
                {
                    _LastSkuInfo = null;
                    LogToFile.LogMessageToFile("------- End  GetSku --------");
                }
                return false;


            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End  GetSku --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End  GetSku --------");
                return false;
            }
        }
        public static async Task<bool> GetAllUsers(string url, string token)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start GetAllUsers --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/user/index", Method.GET);
                request.AddHeader("Authorization", token);

                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = ReactUserInfo.DeserializedJsonAlone(response.Content);
                    if (tmp != null)
                        _LastUserInfo = tmp;
                    else
                        _LastUserInfo = null;
                    LogToFile.LogMessageToFile("------- End  GetAllUsers --------");
                    return true;
                }
                else
                {
                    _LastUserInfo = null;
                    LogToFile.LogMessageToFile("------- End  GetAllUsers --------");
                }
                return false;


            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End  GetAllUsers --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End  GetAllUsers --------");
                return false;
            }
        }

        /*public static async Task<bool> GetDiamondMatchSelection(string url, string token)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start GetDiamondMatchSelection --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/diamond-match/index/", Method.GET);
                request.AddHeader("Authorization", token);

                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = DiamondMatchSelectionInfo.DeserializedJsonAlone(response.Content);
                    if (tmp != null)
                        _LastDiamondMatchSelectionInfo = tmp;
                    else
                        _LastDiamondMatchSelectionInfo = null;
                    LogToFile.LogMessageToFile("------- End  GetDiamondMatchSelection  --------");
                    return true;
                }
                else
                {
                    _LastDiamondMatchSelectionInfo = null;
                    LogToFile.LogMessageToFile("------- End  GetDiamondMatchSelection  --------");
                }
                return false;


            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End  GetDiamondMatchSelection  --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End  GetDiamondMatchSelection  --------");
                return false;
            }
        }*/

        public static async Task<bool> UpdateUser(string url, string token, ReactUpdateUserBadgeAndFp rufp)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Update user --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/user/update-fingerprint", Method.PUT);
                request.AddHeader("Authorization", token);

                string body = ReactUpdateUserBadgeAndFp.SerializedJsonAlone(rufp);
                request.AddParameter("application/json", body, ParameterType.RequestBody);

                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                LogToFile.LogMessageToFile("Body :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = ReturnUpdateUser.DeserializedJsonAlone(response.Content);
                    if (tmp != null)
                        _LastReturnUpdateUser = tmp;
                    else
                        _LastReturnUpdateUser = null;
                    LogToFile.LogMessageToFile("------- End  Update User --------");
                    return true;
                }
                else
                {
                    _LastReturnUpdateUser = null;
                    LogToFile.LogMessageToFile("------- End  Update User --------");
                }
                return false;

            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End  Register --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End  Register --------");
                return false;
            }
        }

        public static async Task<bool> SendRawActivity(string url, string token, ReactEventInfo rei)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send Raw Activity --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/raw-activity", Method.POST);
                request.AddHeader("Authorization", token);

                string body = ReactEventInfo.SerializedJsonAlone(rei);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                LogToFile.LogMessageToFile("Body :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = ReturnRawActivity.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnRawActivity = tmp;
                    else
                        _LastReturnRawActivity = null;
                    LogToFile.LogMessageToFile("------- End Send Raw Activity --------");
                    return true;
                }
                else
                    LogToFile.LogMessageToFile("------- End Send Raw Activity --------");
                return false;
            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End Send Raw Activity --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End Send Raw Activity --------");
                return false;
            }
        }

        public static async Task<bool> UpdateDiamondMatch(string url, string token, ReactDiamondMatchInfo rdmi)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send Update DiamondMatch --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/device-integration/diamond-match", Method.POST);
                request.AddHeader("Authorization", token);

                string body = ReactDiamondMatchInfo.SerializedJsonAlone(rdmi);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                LogToFile.LogMessageToFile("Body :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = ReturnUpdateDiamondMatch.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnUpdateDiamondMatch = tmp;
                    else
                        _LastReturnUpdateDiamondMatch = null;
                    LogToFile.LogMessageToFile("------- End Send Update DiamondMatch --------");
                    return true;
                }
                else
                    LogToFile.LogMessageToFile("------- End Send Update DiamondMatch --------");
                return false;
            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End Send Update DiamondMatch --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End Send Update DiamondMatch --------");
                return false;
            }
        }


        public static async Task<bool> GetLedSelection(string url, string token, string deviceSerial)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Get Led --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/device-integration/led-selection", Method.GET);
                request.AddHeader("Authorization", token);


                request.AddParameter("serialNumber", deviceSerial);
                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    var tmp = LedTriggerInfo.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastLedSelection = tmp;
                    else
                        _LastLedSelection = null;
                    LogToFile.LogMessageToFile("------- End  Get Led Selection --------");
                    return true;
                }
                else
                    LogToFile.LogMessageToFile("------- End  Get Led Selection --------");
                return false;


            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End Get Led Selection --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End Get Led Selection --------");
                return false;
            }
        }

        public static async Task<bool> UpdateLedSelection(string url, string token, ReactLedSelectionInfo rlsi)
        {
            if (string.IsNullOrEmpty(url)) return false;
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send Update LED Selection --------");
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = Timeout * 1000;
                client.ReadWriteTimeout = Timeout * 1000;

                var request = new RestRequest("api/v1/device-integration/triggered-led", Method.POST);
                request.AddHeader("Authorization", token);

                string body = ReactLedSelectionInfo.SerializedJsonAlone(rlsi);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, Timeout));
                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                LogToFile.LogMessageToFile("Body :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                LogToFile.LogMessageToFile("Received : " + response.Content);

                if (response.Content.Contains("\"status\":"))
                {
                    //TODO
                    var tmp = ReturnUpdateDiamondMatch.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnUpdateDiamondMatch = tmp;
                    else
                        _LastReturnUpdateDiamondMatch = null;
                    LogToFile.LogMessageToFile("------- End Send Update LED Selection --------");
                    return true;
                }
                else
                    LogToFile.LogMessageToFile("------- End Send Update LED Selection --------");
                return false;
            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                LogToFile.LogMessageToFile("------- End Send Update LED Selection --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- End Send Update LED Selection --------");
                return false;
            }
        }

        /* public static async Task<bool> GetSku(string url, string token, List<string> TagIds)
         {
             if (string.IsNullOrEmpty(url)) return false;
             var cts = new CancellationTokenSource();
             try
             {
             }
             catch (OperationCanceledException)
             {
                 LogToFile.LogMessageToFile("Received OperationCanceledException from timeout " + Timeout);
                 LogToFile.LogMessageToFile("------- End  Register --------");
                 return false;
             }
             catch (Exception e)
             {
                 LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                 LogToFile.LogMessageToFile("------- End  Register --------");
                 return false;
             }
         }*/
    }
}
