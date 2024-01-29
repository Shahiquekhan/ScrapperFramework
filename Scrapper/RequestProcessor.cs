using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace RPE
{    
    internal class RequestProcessor
    {
        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();        
        //static ConcurrentQueue<Dictionary<string, List<string>>> objRequestsQueueDict = new ConcurrentQueue<Dictionary<string, List<string>>>();        
        static ConcurrentDictionary<string, ConcurrentQueue<string>> concurrentRequestsDict = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        //static object queueLock = new object();
        //string strRequest = string.Empty;
        //string botFilePath = string.Empty;
        //public ProcessRequest(string _trRequest, string _botFilePath)
        //{
        //    strRequest = _trRequest;
        //    botFilePath = _botFilePath; 
        //}

        public async Task ProcessRequests(ConcurrentDictionary<string, ConcurrentQueue<string>> objConcurrentDict, string botFilePath)
        {            
            if (objConcurrentDict.Count > 0)
            {
                LaunchBot objLaunchBot = new LaunchBot();
                foreach (var requestsDict in objConcurrentDict)
                {
                    string strBotName = requestsDict.Key;
                    foreach (var _request in requestsDict.Value)
                    {
                        try
                        {
                            await objLaunchBot.sendRequestToBot(strBotName, _request, botFilePath);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception or log it for debugging
                            Console.WriteLine($"Exception in loop iteration: {ex.Message}");
                        }
                    }
                    concurrentRequestsDict.TryRemove(strBotName, out ConcurrentQueue<string> removedValue);
                }
            }
        }

        //public async Task ProcessRequests(string strRequest, string botFilePath)
        //{
        //    GetRequests(strRequest, botFilePath);
        //    if (concurrentRequestsDict.Count > 0)
        //    {
        //        LaunchBot objLaunchBot = new LaunchBot();
        //        foreach (var requestsDict in concurrentRequestsDict)
        //        {
        //            string strBotName = requestsDict.Key;
        //            foreach (var _request in requestsDict.Value)
        //            {
        //                try
        //                {
        //                    await objLaunchBot.sendRequestToBot(strBotName, _request, botFilePath);
        //                }
        //                catch (Exception ex)
        //                {
        //                    // Handle the exception or log it for debugging
        //                    Console.WriteLine($"Exception in loop iteration: {ex.Message}");
        //                }
        //            }
        //            concurrentRequestsDict.TryRemove(strBotName, out ConcurrentQueue<string> removedValue);
        //        }
        //    }            
        //}

        public void GetRequests(string strRequest, string botFilePath) 
        {
            string strRequestProcessorID = configuration["AppSettings:RequestProcessorID"];
            if (strRequest != string.Empty)
            {
                try
                {
                    var request_json = JsonConvert.DeserializeObject<Root[]>(strRequest);
                    if (request_json != null)
                    {
                        ConcurrentQueue<string> objRequestsQueue = new ConcurrentQueue<string>();
                        string strBotName = string.Empty;
                        foreach (var item in request_json)
                        {
                            strBotName = item.scan_package_name;
                            string _item = JsonConvert.SerializeObject(item);                            
                            objRequestsQueue.Enqueue(_item);
                        }
                        if (concurrentRequestsDict.Count > 0 && concurrentRequestsDict.TryGetValue(strBotName, out ConcurrentQueue<string> _result))
                        {
                            while (objRequestsQueue.TryDequeue(out string _item))
                            {
                                _result.Enqueue(_item);
                            }                            
                        }
                        else
                        {
                            concurrentRequestsDict.TryAdd(strBotName, objRequestsQueue);
                        }
                        ThrottlerController.CheckAndSetThrottler(strBotName, botFilePath);
                    }
                }
                catch (Exception ex) 
                { 
                }
            }           
        }        

        //public void CheckAndAddRequestInQueue(string BotName, List<string> objRequestsList,
        //    Dictionary<string, List<string>> objRequestsDict)
        //{
        //    lock (queueLock)
        //    {
        //        if (objRequestsQueueDict.Count > 0)
        //        {                    
        //            bool IsMatched = false;
        //            if (objRequestsQueueDict.TryPeek(out Dictionary<string, List<string>> _result) && _result.ContainsKey(BotName))
        //            {
        //                _result[BotName] = objRequestsList.Concat(_result[BotName]).ToList();
        //                IsMatched = true;
        //            }
        //            if (!IsMatched)
        //            {
        //                objRequestsQueueDict.Enqueue(objRequestsDict);
        //            }
        //            //ConcurrentQueue<Dictionary<string, List<string>>> tempRequestsQueueDict = new ConcurrentQueue<Dictionary<string, List<string>>>();
        //            //bool IsMatched = false;
        //            //while (objRequestsQueueDict.TryDequeue(out Dictionary<string, List<string>> _result))
        //            //{
        //            //    if (_result.ContainsKey(BotName))
        //            //    {
        //            //        objRequestsList = objRequestsList.Concat(_result[BotName]).ToList();
        //            //        Dictionary<string, List<string>> _dict = new Dictionary<string, List<string>>();
        //            //        _dict.Add(BotName, objRequestsList);
        //            //        tempRequestsQueueDict.Enqueue(_dict);
        //            //        IsMatched = true;
        //            //    }
        //            //    else
        //            //    {
        //            //        tempRequestsQueueDict.Enqueue(_result);
        //            //    }

        //            //}
        //            //objRequestsQueueDict = tempRequestsQueueDict;
        //            //if (!IsMatched)
        //            //{
        //            //    objRequestsQueueDict.Enqueue(objRequestsDict);
        //            //}

        //        }
        //        else
        //        {
        //            objRequestsQueueDict.Enqueue(objRequestsDict);
        //        }
        //    }
        //}

    }
}
