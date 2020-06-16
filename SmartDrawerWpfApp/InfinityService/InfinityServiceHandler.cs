using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartDrawerWpfApp.StaticHelpers;
using System.IO;
using System.Threading;

namespace SmartDrawerWpfApp.InfinityService
{
    public static class InfinityServiceHandler
    {
        public static object somePublicStaticObject = new Object();
        private static StoneInfo[] _LastStonesListNotScan;
        public static StoneInfo[] LastStonesListNotScan
        {
            get { return _LastStonesListNotScan; }
            set { _LastStonesListNotScan = value; }
        }

        private static SelectionInfo[] _LastSelectionList;
        public static SelectionInfo[] LastSelectionList
        {
            get { return _LastSelectionList; }
            set { _LastSelectionList = value; }
        }

        private static ReturnInfo _LastReturnInfo;
        public static ReturnInfo LastReturnInfo
        {
            get { return _LastReturnInfo; }
            set { _LastReturnInfo = value; }
        }

        private static RemoteUserInfo[] _LastRemoteUserInfo;
        public static RemoteUserInfo[] LastRemoteUserInfo
        {
            get { return _LastRemoteUserInfo; }
            set { _LastRemoteUserInfo = value; }
        }

        private static ReaderStonesInfo[] _LastReaderStonesInfoIn;
        public static ReaderStonesInfo[] LastReaderStonesInfoIn
        {
            get { return _LastReaderStonesInfoIn; }
            set { _LastReaderStonesInfoIn = value; }
        }

        private static ReaderStonesInfo[] _LastReaderStonesInfoOut;
        public static ReaderStonesInfo[] LastReaderStonesInfoOut
        {
            get { return _LastReaderStonesInfoOut; }
            set { _LastReaderStonesInfoOut = value; }
        }

        private static ReaderStonesInfo[] _LastReaderStonesInfoVault;
        public static ReaderStonesInfo[] LastReaderStonesInfoVault
        {
            get { return _LastReaderStonesInfoVault; }
            set { _LastReaderStonesInfoVault = value; }
        }

        private static ReaderStonesInfo[] _LastReaderStonesInfoMissing;
        public static ReaderStonesInfo[] LastReaderStonesInfoMissing
        {
            get { return _LastReaderStonesInfoMissing; }
            set { _LastReaderStonesInfoMissing = value; }
        }

        private static ReaderStonesInfo[] _LastReaderStonesInfoSold;
        public static ReaderStonesInfo[] LastReaderStonesInfoSold
        {
            get { return _LastReaderStonesInfoSold; }
            set { _LastReaderStonesInfoSold = value; }
        }

        private static DiamondMatchSelectionInfo[] _LastDiamondMatchSelectionInfo;
        public static DiamondMatchSelectionInfo[] LastDiamondMatchSelectionInfo
        {
            get { return _LastDiamondMatchSelectionInfo; }
            set { _LastDiamondMatchSelectionInfo = value; }            
        }

        private static InTransitInfo[] _LastInTransitInfo;
        public static InTransitInfo[] LastInTransitInfo
        {
            get { return _LastInTransitInfo; }
            set { _LastInTransitInfo = value; }
        }

        private static string _LastEndPoint;
        public static string LastEndPoint
        {
            get { return _LastEndPoint; }
            set { _LastEndPoint = value; }
        }

        public static string LastError { get; set; }

        public static async Task<bool> GetStonesNotReadAtAll(string url, string token, string DeviceSerial)
        {
            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                var request = new RestRequest("api/stones", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);

                 request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                if (response.IsSuccessful)
                {
                    lock (somePublicStaticObject)
                    {

                        if (response.Content.StartsWith("{\"success\":false"))
                        {
                            LastError = response.Content;
                            return false;
                        }
                        else
                        {

                            var tmp = StoneInfo.DeserializedJsonList(response.Content);
                            if (tmp != null)
                                _LastStonesListNotScan = tmp;
                            else
                                _LastStonesListNotScan = null;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }


        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> fullBatch, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "chunkSize",
                    chunkSize,
                    "Chunk size cannot be less than or equal to zero.");
            }

            if (fullBatch == null)
            {
                throw new ArgumentNullException("fullBatch", "Input to be split cannot be null.");
            }

            var cellCounter = 0;
            var chunk = new List<T>(chunkSize);

            foreach (var element in fullBatch)
            {
                if (cellCounter++ == chunkSize)
                {
                    yield return chunk;
                    chunk = new List<T>(chunkSize);
                    cellCounter = 1;
                }

                chunk.Add(element);
            }

            yield return chunk;
        }

        
        public static async Task<bool> GetStonesByList(string url, string token, string DeviceSerial, List<string> TagList)
        {
            int ChunkSize = 50;
            if (TagList.Count < ChunkSize + 1)
            {

                var cts = new CancellationTokenSource();
                try
                {
                    LogToFile.LogMessageToFile("------- Start Get Stone By List--------");
                    string urlServer = url;
                    var client = new RestClient(urlServer);
                    client.Timeout = 20000;
                    client.ReadWriteTimeout = 20000;

                    var request = new RestRequest("api/stones", Method.GET);
                    request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                    //request.AddHeader("s", DeviceSerial);
                    request.AddParameter("filter", string.Join(",", TagList));
                    LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                     request.Timeout = 20000;
                    request.ReadWriteTimeout = 20000;
                    cts.CancelAfter(new TimeSpan(0, 0, 20));
                    var response = await client.ExecuteTaskAsync(request, cts.Token);
                    LogToFile.LogMessageToFile("Received : " + response.Content);
                    LogToFile.LogMessageToFile("------- Stop Get Stone By List --------");
                    if (response.IsSuccessful)
                    {
                        lock (somePublicStaticObject)
                        {

                            if (response.Content.StartsWith("{\"success\":false"))
                            {
                                LastError = response.Content;
                                return false;
                            }
                            else
                            {

                                var tmp = StoneInfo.DeserializedJsonList(response.Content);
                                if (tmp != null)
                                    _LastStonesListNotScan = tmp;
                                else
                                    _LastStonesListNotScan = null;
                                return true;
                            }
                        }
                    }
                    return false;
                }
                catch (OperationCanceledException)
                {
                    LogToFile.LogMessageToFile("Received OperationCanceledException from timeout 10s");
                    LogToFile.LogMessageToFile("------- Stop Get Stone By List --------");
                    return false;

                }
                catch (Exception e)
                {
                    LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                    LogToFile.LogMessageToFile("------- Stop Get Stone By List --------");
                    return false;
                }
            }
            else
            {
                try
                {
                    List<StoneInfo> tmpList = new List<StoneInfo>();

                    LogToFile.LogMessageToFile("------- Start Get Stone By List Splitted in 100 --------");
                    var SplittedList = TagList.Split<string>(100);
                    if ((SplittedList != null) && (SplittedList.Count() > 0))
                    {
                        foreach (var listItem in SplittedList)
                        {
                            var cts = new CancellationTokenSource();
                            try
                            {
                                LogToFile.LogMessageToFile("------- Start Get Stone By List--------");
                                string urlServer = url;
                                var client = new RestClient(urlServer);
                                client.Timeout = 20000;
                                client.ReadWriteTimeout = 20000;

                                var request = new RestRequest("api/stones", Method.GET);
                                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                                //request.AddHeader("s", DeviceSerial);
                                request.AddParameter("filter", string.Join(",", listItem));
                                LogToFile.LogMessageToFile("Send :" + client.BuildUri(request));
                                 request.Timeout = 20000;
                                request.ReadWriteTimeout = 20000;
                                cts.CancelAfter(new TimeSpan(0, 0, 20));
                                var response = await client.ExecuteTaskAsync(request, cts.Token);
                                LogToFile.LogMessageToFile("Received : " + response.Content);
                                LogToFile.LogMessageToFile("------- Stop Get Stone By List --------");
                                if (response.IsSuccessful)
                                {
                                    lock (somePublicStaticObject)
                                    {
                                        if (response.Content.StartsWith("{\"success\":false"))
                                        {
                                            LastError = response.Content;
                                            return false;
                                        }
                                        else
                                        {

                                            var tmp = StoneInfo.DeserializedJsonList(response.Content);
                                            if (tmp != null)
                                            {
                                                foreach (var item in tmp)
                                                    tmpList.Add(item);
                                            }

                                        }
                                    }
                                }
                                else
                                {
                                    LogToFile.LogMessageToFile("Received Not succesful operation");
                                    break;
                                }
                               
                            }
                            catch (OperationCanceledException)
                            {
                                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout 10s");
                                LogToFile.LogMessageToFile("------- Stop Get Stone By List --------");
                                return false;

                            }
                        }
                        if (tmpList.Count > 0)
                        {
                            _LastStonesListNotScan = tmpList.ToArray();
                            LogToFile.LogMessageToFile("Added Stone info : " + tmpList.Count);
                        }
                        else
                            LogToFile.LogMessageToFile("NO Added Stone info ");
                        LogToFile.LogMessageToFile("------- Stop Get Stone By List Splitted in 50 --------");
                        return true;

                    }
                    else
                    {                      
                        LogToFile.LogMessageToFile("Spitted list null or having no element");
                        LogToFile.LogMessageToFile("------- Stop Get Stone By List Splitted in 50 --------");
                        return false;
                    }
                }
                catch (Exception e)
                {
                    LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                    LogToFile.LogMessageToFile("------- Stop Get Stone By List Splitted in 50 --------");
                    return false;
                }
            }
        }
        public static async Task<bool> GetSelection(string url, string token, string DeviceSerial)
        {
            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                var request = new RestRequest("api/lightLed", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);

                 request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                if (response.IsSuccessful)
                {
                    lock (somePublicStaticObject)
                    {

                        if (response.Content.StartsWith("{\"success\":false"))
                        {
                            LastError = response.Content;
                            return false;
                        }
                        else
                        {

                            var tmp = SelectionInfo.DeserializedJsonList(response.Content);
                            if (tmp != null)
                                _LastSelectionList = tmp;
                            else
                                _LastSelectionList = null;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> GetRemoteUsers(string url, string token, string DeviceSerial)
        {
            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                var request = new RestRequest("api/user", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);

                 request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                if (response.IsSuccessful)
                {
                    lock (somePublicStaticObject)
                    {

                        if (response.Content.StartsWith("{\"success\":false"))
                        {
                            LastError = response.Content;
                            return false;
                        }
                        else
                        {
                            //string change = response.Content.Replace("ref", "refId");
                            var tmp = RemoteUserInfo.DeserializedJsonList(response.Content);
                            if (tmp != null)
                                _LastRemoteUserInfo = tmp;
                            else
                                _LastRemoteUserInfo = null;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> GetDiamondMatchSelection(string url, string token, string DeviceSerial)
        {
            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                var request = new RestRequest("api/diamondMatch", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);


                 request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                if (response.IsSuccessful)
                {
                    lock (somePublicStaticObject)
                    {

                        if (response.Content.StartsWith("{\"success\":false"))
                        {
                            LastError = response.Content;
                            return false;
                        }
                        else
                        {

                            var tmp = DiamondMatchSelectionInfo.DeserializedJsonList(response.Content);
                            if (tmp != null)
                                _LastDiamondMatchSelectionInfo = tmp;
                            else
                                _LastDiamondMatchSelectionInfo = null;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout 10s");
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> GetDiamondInTransitSelection(string url, string token, string DeviceSerial)
        {
            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                var request = new RestRequest("api/inTransit", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);


                request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                if (response.IsSuccessful)
                {
                    lock (somePublicStaticObject)
                    {

                        if (response.Content.StartsWith("{\"success\":false"))
                        {
                            LastError = response.Content;
                            return false;
                        }
                        else
                        {

                            var tmp = InTransitInfo.DeserializedJsonList(response.Content);
                            if (tmp != null)
                                _LastInTransitInfo = tmp;
                            else
                                _LastInTransitInfo = null;
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout 10s");
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public class StoneStatus
        {
            public const int OUT = 0;
            public const int IN = 1;
            public const int VAULT = 2;
            public const int SOLD = 3;
            public const int MISSING = 4;
        }
        public static async Task<bool> GetReaderStonesInfo(string url, string token, string DeviceSerial, int InOut)
        {

            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                var request = new RestRequest("api/stones/reader", Method.GET);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("status", InOut);
             

                 request.Timeout = 20000;
                request.ReadWriteTimeout = 20000;
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                if (response.IsSuccessful)
                {
                    lock (somePublicStaticObject)
                    {

                        if (response.Content.StartsWith("{\"success\":false"))
                        {
                            LastError = response.Content;
                            return false;
                        }
                        else
                        {
                            //string change = response.Content.Replace("ref", "refId");
                            var tmp = ReaderStonesInfo.DeserializedJsonList(response.Content);
                            if (InOut == StoneStatus.OUT)
                            {
                                if (tmp != null)
                                    _LastReaderStonesInfoOut = tmp;
                                else
                                    _LastReaderStonesInfoOut = null;
                                return true;
                            }
                            else if (InOut == StoneStatus.IN)
                            {
                                if (tmp != null)
                                    _LastReaderStonesInfoIn = tmp;
                                else
                                    _LastReaderStonesInfoIn = null;
                                return true;
                            }
                            else if (InOut == StoneStatus.VAULT)
                            {
                                if (tmp != null)
                                    _LastReaderStonesInfoVault = tmp;
                                else
                                    _LastReaderStonesInfoVault = null;
                                return true;
                            }
                            else if (InOut == StoneStatus.MISSING)
                            {
                                if (tmp != null)
                                    _LastReaderStonesInfoMissing = tmp;
                                else
                                    _LastReaderStonesInfoMissing = null;
                                return true;
                            }
                            else if (InOut == StoneStatus.SOLD)
                            {
                                if (tmp != null)
                                    _LastReaderStonesInfoSold = tmp;
                                else
                                    _LastReaderStonesInfoSold = null;
                                return true;
                            }
                        }
                    }
                }
                return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> RemoveSelection(string url, string token, string DeviceSerial, string refId)
        {
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Remove Selection --------");

                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                RemoveSelectionInfo rsi = new RemoveSelectionInfo();
                rsi.ref_ids = new string[1];
                rsi.ref_ids[0] = refId;
                string body = RemoveSelectionInfo.SerializedJsonAlone(rsi);
                //string body = "[\"" + refId + "\"]";      

                var request = new RestRequest("api/offLed ", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                LogToFile.LogMessageToFile("Send :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);

                string ret = response.Content;               
                LogToFile.LogMessageToFile("Received : " + response.Content);
                LogToFile.LogMessageToFile("------- Stop Remove Selection --------");

                if (ret.Equals("{\"off\":1}"))
                    return true;
                else
                    return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> RemoveSelections(string url, string token, string DeviceSerial, List<string> refIds)
        {
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Remove Selections --------");

                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                RemoveSelectionInfo rsi = new RemoveSelectionInfo();
                rsi.ref_ids = refIds.ToArray();
                // rsi.ref_ids[0] = refId;
                string body = RemoveSelectionInfo.SerializedJsonAlone(rsi);
                //string body = "[\"" + refId + "\"]";      

                var request = new RestRequest("api/offLed ", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                LogToFile.LogMessageToFile("Send :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);

                string ret = response.Content;              
                LogToFile.LogMessageToFile("Received : " + response.Content);
                LogToFile.LogMessageToFile("------- Stop Remove Selections --------");

                if (ret.Equals("{\"off\":" + refIds.Count + "}"))
                    return true;
                else
                    return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> SendLog(string url, string token, string DeviceSerial, string filePath)
        {
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send Logs --------");

                if (File.Exists(filePath))
                {
                    string urlServer = url;
                    var client = new RestClient(urlServer);
                    client.Timeout = 20000;
                    client.ReadWriteTimeout = 20000;

                    var request = new RestRequest("api/device/log", Method.POST);
                    request.AddHeader("Authorization", "Bearer " + token);
                    request.AddHeader("s", DeviceSerial);
                    request.AddFile("Log", filePath);
                    cts.CancelAfter(new TimeSpan(0, 0, 20));
                    var response = await client.ExecuteTaskAsync(request, cts.Token);

                    string ret = response.Content;
                    LogToFile.LogMessageToFile("Send Log:" + filePath);
                    LogToFile.LogMessageToFile("Received : " + response.Content);
                    LogToFile.LogMessageToFile("------- Stop Send Logs --------");

                    if (ret.StartsWith("{\"success\":true"))
                    {
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }

        }
        public static async Task<bool> SendEvents(string url, string token, string DeviceSerial, EventInfo newEvent)
        {
            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send Events --------");

                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                string body = EventInfo.SerializedJsonAlone(newEvent);

                var request = new RestRequest("api/stones/activity", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                LogToFile.LogMessageToFile("Send :" + body);
                var response = await client.ExecuteTaskAsync(request,cts.Token);
                string ret = response.Content;              
                LogToFile.LogMessageToFile("Received : " + response.Content);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");

                if (ret.StartsWith("{\"success\":true"))
                {
                    var tmp = ReturnInfo.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnInfo = tmp;
                    else
                        _LastReturnInfo = null;

                    return true;
                }
                else
                    return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }

        }
        public static async Task<bool> SendAlert(string url, string token, string DeviceSerial, AlertInfo alert)
        {
            var cts = new CancellationTokenSource();
            try
            {

            LogToFile.LogMessageToFile("------- Start Send Alert --------");

            string urlServer = url;
            var client = new RestClient(urlServer);
            client.Timeout = 20000;
            client.ReadWriteTimeout = 20000;

            string body = AlertInfo.SerializedJsonAlone(alert);

            var request = new RestRequest("api/alert", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                LogToFile.LogMessageToFile("Send :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);
                string ret = response.Content;
           
            LogToFile.LogMessageToFile("Received : " + response.Content);
            LogToFile.LogMessageToFile("------- Stop Send alert--------");

            if (ret.StartsWith("{\"success\":true"))
                return true;
            else
                return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<bool> SendLightedLed(string url, string token, string DeviceSerial, PostLedInfo pli)
        {

            var cts = new CancellationTokenSource();
            try
            {

            LogToFile.LogMessageToFile("------- Start Send Light Led --------");

            string urlServer = url;
            var client = new RestClient(urlServer);
            client.Timeout = 20000;
            client.ReadWriteTimeout = 20000;

            string body = PostLedInfo.SerializedJsonAlone(pli);

            var request = new RestRequest("api/ledAlert", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                LogToFile.LogMessageToFile("Send :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);

                string ret = response.Content;
          
            LogToFile.LogMessageToFile("Received : " + response.Content);
            LogToFile.LogMessageToFile("------- Stop Send Light Led--------");

            if (ret.StartsWith("{\"success\":true"))
                return true;
            else
                return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }

        }
        public static async Task<bool> SendUser(string url, string token, string DeviceSerial, UserInfo ui)
        {

            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send user --------");

                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                string body = UserInfo.SerializedJsonAlone(ui);

                var request = new RestRequest("api/user", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);
                //request.AddHeader("s", DeviceSerial);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                LogToFile.LogMessageToFile("Send :" + body);
                var response = await client.ExecuteTaskAsync(request, cts.Token);

                string ret = response.Content;               
                LogToFile.LogMessageToFile("Received : " + response.Content);
                LogToFile.LogMessageToFile("------- Stop Send user--------");

                if (ret.StartsWith("{\"success\":true"))
                {                   
                    return true;
                }
                else
                    return false;
            }
            catch (OperationCanceledException oce)
            {
                LogToFile.LogMessageToFile("OperationCanceledException : " + oce.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception exp)
            {
                LogToFile.LogMessageToFile("Exception : " + exp.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }

        }
        public static async Task<bool> GetEndpoint(string token)
        {
            var cts = new CancellationTokenSource();
            try
            {
                string urlServer = "http://spacecode.com";
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                string path = "endpoint/" + token + ".txt";

                var request = new RestRequest(path, Method.GET);

                cts.CancelAfter(new TimeSpan(0, 0, 20));

                var response = await client.ExecuteTaskAsync(request, cts.Token);

                if (response.IsSuccessful)
                {
                    LastEndPoint = response.Content;
                    return true;
                }
                else
                    return false;

            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout 10s");
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return false;
            }
        }
        public static async Task<int> SendGuid(string url, string token, string DeviceSerial, PostGuidInfo pgi)
        {

            var cts = new CancellationTokenSource();
            try
            {
                LogToFile.LogMessageToFile("------- Start Send Guid--------");

                string urlServer = url;
                var client = new RestClient(urlServer);
                client.Timeout = 20000;
                client.ReadWriteTimeout = 20000;

                string body = PostGuidInfo.SerializedJsonAlone(pgi);

                var request = new RestRequest("api/stones/guid", Method.POST);
                request.AddHeader("Authorization", "Bearer " + token + "." + DeviceSerial);

                request.AddParameter("application/json", body, ParameterType.RequestBody);
                cts.CancelAfter(new TimeSpan(0, 0, 20));
                var response = await client.ExecuteTaskAsync(request, cts.Token);

                string ret = response.Content;
                LogToFile.LogMessageToFile("Send :" + body);
                LogToFile.LogMessageToFile("Received : " + response.Content);
                LogToFile.LogMessageToFile("------- Stop Send Guid --------");

                if (ret.StartsWith("{\"success\":false"))
                {
                    var tmp = ReturnInfo.DeserializedJsonList(response.Content);
                    if (tmp != null)
                        _LastReturnInfo = tmp;
                    else
                        _LastReturnInfo = null;
                    return 0;
                }
                else
                    return 1;

            }
            catch (OperationCanceledException)
            {
                LogToFile.LogMessageToFile("Received OperationCanceledException from timeout 10s");
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return -1;
            }
            catch (Exception e)
            {
                LogToFile.LogMessageToFile("Received Exception : " + e.Message);
                LogToFile.LogMessageToFile("------- Stop Send Events --------");
                return -1;
            }
        }

    }
}
