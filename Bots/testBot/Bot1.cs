using Newtonsoft.Json;
using RPE;
using System;
using static System.Net.Mime.MediaTypeNames;
namespace Bot
{
    public class Bot1
    {
        static void Main(string[] args)
        {
            if (args.Length >= 0)
            {
                //string request = string.Join(" ", args);
                string request = args[0];
                //Console.WriteLine(request);
                try
                {
                    var request_json = JsonConvert.DeserializeObject<Root>(request);
                    if (request_json != null)
                    {
                        string unique_id = GenerateUniqueId();
                        string bot_name_with_scan_id = $"{request_json.scan_package_name}_{request_json.scan_id}_{unique_id}";
                        string filePath = $"D:\\Projects\\ScrapperFramework\\Response\\{bot_name_with_scan_id}.json";
                        var out_put = JsonConvert.SerializeObject(request_json);
                        File.WriteAllText(filePath, out_put);
                        Console.WriteLine($"Response saved for Bot. {request_json.scan_package_name}");
                    }
                    else
                    {
                        Console.WriteLine("Request not Deserialized.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception found in json Deserialized." + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("No records.");
            }
            
        }
        static string GenerateUniqueId()
        {
            // Combine current date and time with a GUID
            string uniqueId = $"{DateTime.Now:yyyyMMddHHmmssfff}-{Guid.NewGuid()}";
            return uniqueId;
        }
    }
}
