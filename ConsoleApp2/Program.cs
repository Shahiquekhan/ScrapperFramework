using RPE;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace ConsoleApp
{
    public class Program
    {
        static async Task Main(string[] args)
        {            
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            string botFilePath = configuration["AppSettings:botFilePath"];
            string rootFolder = configuration["AppSettings:rootFolder"];
            var RequestProcessorIDs = JsonConvert.DeserializeObject<List<string>>(configuration["AppSettings:RequestProcessorIDs"]);
            FetchRequests obj = new FetchRequests();
            await obj.CheckAndReadRequest(botFilePath, rootFolder, RequestProcessorIDs);
        }
    }
}
