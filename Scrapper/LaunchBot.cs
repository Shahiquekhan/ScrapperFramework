using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Diagnostics;


namespace RPE
{
    internal class LaunchBot
    {
        //string Request_item = string.Empty;
        //string BotName = string.Empty;
        //public LaunchBot(string _BotName, string _Request_item) 
        //{
        //    BotName = _BotName;
        //    Request_item = _Request_item;  
        //}
        public async Task sendRequestToBot(string strBotName, string Request_item, string botFilePath)
        {
            var _path = $"{botFilePath}\\{strBotName}\\{strBotName}.exe";
            await StartExternalProcess(_path, Request_item, strBotName);
            await Task.Delay(100);
        }
        
        protected async Task StartExternalProcess(string executablePath, string request, string strBotName)
        {
            try
            {                
                request = $"\"{request.Replace("\"", "\\\"")}\"";
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = request,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };                
                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    await process.WaitForExitAsync();
                    string output = process.StandardOutput.ReadToEnd();                    
                    if (output != null && output.Contains("Response saved"))
                    {
                        if (output.Contains("Response saved"))
                            Console.WriteLine($"Success: {output}");
                        else
                            Console.WriteLine($"Request failed: {output}");

                    }                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occured for BotName {strBotName}: {ex.Message}");
            }
        }       
    }
    static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process process)
        {
            var tcs = new TaskCompletionSource<object>();

            process.EnableRaisingEvents = true;
            process.Exited += (sender, args) => tcs.TrySetResult(null);

            return tcs.Task;
        }
    }
}
