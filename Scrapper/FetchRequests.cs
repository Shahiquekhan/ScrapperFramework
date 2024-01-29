using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace RPE
{
    public class FetchRequests
    {        
        static ConcurrentDictionary<string, ConcurrentQueue<string>> concurrentRequestsDict = new ConcurrentDictionary<string, ConcurrentQueue<string>>();
        public async Task CheckAndReadRequest(string botFilePath, string rootFolder, List<string> RequestProcessorIDs)
        {
            if (Directory.Exists(rootFolder))
            {
                DirectoryInfo dir = new DirectoryInfo(rootFolder);
                if (dir.Exists && dir.GetFiles().Count() > 0)
                {
                    RequestProcessor objRequestProcessor = new RequestProcessor();
                    foreach (FileInfo flInfo in dir.GetFiles())
                    {
                        using (StreamReader sr = new StreamReader(flInfo.FullName))
                        {
                            UpdateRequestsInQueue(sr.ReadToEnd(), botFilePath);                            
                        }
                    }
                    if (concurrentRequestsDict.Count > 0)
                    {
                        foreach (var RequestProcessorIds in RequestProcessorIDs)
                        {
                            ThrottlerController.UpdateRequestsQueueForRequestProcessor(concurrentRequestsDict, RequestProcessorIds);
                            ConcurrentDictionary<string, ConcurrentQueue<string>> objConcurrentDict = ThrottlerController.GetRequestsForRequestProcessor(RequestProcessorIds);
                            await objRequestProcessor.ProcessRequests(objConcurrentDict, botFilePath);
                        }
                    }
                }
            }            
        }
        public void UpdateRequestsInQueue(string strRequest, string botFilePath)
        {               
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
    }   
    
}
    
