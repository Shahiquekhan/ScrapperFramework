using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPE
{
    static internal class ThrottlerController
    {
        static ConcurrentDictionary<string, int> concurrentThrottleDict = new ConcurrentDictionary<string, int>();
        static public ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentQueue<string>>> concurrentThrottleWiseRequestsQueue = new ConcurrentDictionary<string, ConcurrentDictionary<string, ConcurrentQueue<string>>>();
        static public void CheckAndSetThrottler(string strBotName, string strBotPath)
        {
            try
            {
                bool IsBotMax_BW = false;
                var _path = $"{strBotPath}\\{strBotName}\\appsettings.json";
                if (File.Exists(_path))
                {
                    string fileContent = File.ReadAllText(_path);
                    if (fileContent != null)
                    {
                        var config_json = JsonConvert.DeserializeObject<BotConfigCls>(fileContent);
                        if (config_json != null)
                        {
                            int Max_BandWidth = config_json.Max_BandWidth;
                            if (Max_BandWidth > 0)
                            {
                                concurrentThrottleDict.AddOrUpdate(strBotName, Max_BandWidth, (key, oldValue) => Max_BandWidth);
                                IsBotMax_BW = true;
                            }

                        }
                    }
                }
                if (!IsBotMax_BW)
                {
                    concurrentThrottleDict.TryAdd(strBotName, 30);
                }
            }
            catch (Exception ex)
            {
                // Default Throttle
                concurrentThrottleDict.TryAdd(strBotName, 30);
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static public void UpdateRequestsQueueForRequestProcessor(ConcurrentDictionary<string, ConcurrentQueue<string>> concurrentRequestsDict, string strRequestProcessorID)
        {            
            if (concurrentRequestsDict.Count > 0)
            {
                foreach (var requestsDict in concurrentRequestsDict)
                {
                    string strBotName = requestsDict.Key;
                    int intMaxBandWidth = GetMaxBandWidth(strBotName);
                    ConcurrentQueue<string> objCurrentRequestInReqProcessor = GetAllRequestsByProcessorID(strBotName, strRequestProcessorID);
                    if (intMaxBandWidth > objCurrentRequestInReqProcessor.Count)
                    {
                        if (concurrentRequestsDict.TryGetValue(strBotName, out ConcurrentQueue<string> _resultQueue))
                        {
                            while(_resultQueue.Count > 0)
                            {
                                if (objCurrentRequestInReqProcessor.Count > intMaxBandWidth)
                                    break;
                                _resultQueue.TryDequeue(out string _value);
                                objCurrentRequestInReqProcessor.Enqueue(_value);
                            }

                        }                         
                        ConcurrentDictionary<string, ConcurrentQueue<string>> objConcurrentRequestprocessorDict = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
                        objConcurrentRequestprocessorDict.AddOrUpdate(strRequestProcessorID, objCurrentRequestInReqProcessor, (key, oldValue) => objCurrentRequestInReqProcessor);
                        concurrentThrottleWiseRequestsQueue.AddOrUpdate(strBotName, objConcurrentRequestprocessorDict, (key, oldValue) => objConcurrentRequestprocessorDict);
                        if (requestsDict.Value != null && requestsDict.Value.Count == 0)
                            concurrentRequestsDict.TryRemove(strBotName, out _);
                    }
                }
            }
        }

        static public ConcurrentDictionary<string, ConcurrentQueue<string>> GetRequestsForRequestProcessor(string strRequestProcessorID)
        {
            ConcurrentDictionary<string, ConcurrentQueue<string>> objConcurrentDict = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
            if (concurrentThrottleWiseRequestsQueue.Count > 0)
            {                
                foreach (var ThrottleRequestsDict in concurrentThrottleWiseRequestsQueue)
                {
                    string strBotName = ThrottleRequestsDict.Key;
                    ConcurrentQueue<string> objCurrentRequestInReqProcessor = GetAllRequestsByProcessorID(strBotName, strRequestProcessorID);
                    if (objCurrentRequestInReqProcessor.Count > 0)
                        objConcurrentDict.AddOrUpdate(strBotName, objCurrentRequestInReqProcessor, (key, oldValue) => objCurrentRequestInReqProcessor);
                }                
            }
            return objConcurrentDict;
        }

        static ConcurrentQueue<string> GetAllRequestsByProcessorID(string strBotName, string strRequestProcessorID)
        {
            if (!string.IsNullOrEmpty(strBotName) && concurrentThrottleWiseRequestsQueue.Count() > 0)
            {
                var _result = concurrentThrottleWiseRequestsQueue.FirstOrDefault(x => x.Key == strBotName);
                if (!Equals(_result, default(KeyValuePair<string, ConcurrentDictionary<string, ConcurrentQueue<string>>>)))
                {
                    var _requestsInrequestProcessor = _result.Value.FirstOrDefault(x => x.Key == strRequestProcessorID);
                    if (_requestsInrequestProcessor.Key != null)
                    {
                        if (!Equals(_requestsInrequestProcessor, default(KeyValuePair<int, string>)))
                        {
                            concurrentThrottleWiseRequestsQueue.TryRemove(strBotName, out _);
                            return _requestsInrequestProcessor.Value;
                        }
                    }
                }
            }
            return new ConcurrentQueue<string>();
        }
        static int GetMaxBandWidth(string strBotName)
        {
            if (!string.IsNullOrEmpty(strBotName))
            {
                var _result = concurrentThrottleDict.FirstOrDefault(x => x.Key == strBotName);
                if (!Equals(_result, default(KeyValuePair<int, string>)))
                {
                    return _result.Value;
                }
            }
            return 30;
        }
    }
}
