using System;
using System.IO;
using Newtonsoft.Json;
using Kasumi.Entities;
using System.Text;
using System.Threading.Tasks;
using Kasumi.Telemetry;

namespace Kasumi
{
    internal static class Program
    {
        // Bootstrap for checking the runtime environment and initializing the main bot procedure.
        private static async Task Main()
        {
            // Check that config.json exists, we need it.
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("Please create and populate config.json before running!");
                
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(0);
            }
            
            // Parse the configuration
            string json;
            await using (var fs = File.OpenRead("config.json")) 
                // ReSharper disable line HeapView.ObjectAllocation.Evident
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEndAsync().GetAwaiter().GetResult();
            var config = JsonConvert.DeserializeObject<ConfigJson>(json);
            
            Globals.Token = config.Token;
            Globals.Prefix = config.Prefix;
            Globals.Dev = config.Dev;
            Globals.TelemetryClient = new TelemetryClient(config.NewRelicID, config.NewRelicKey);
            
            // Run the actual bot.
            await Bot.BotMain();
        }
    }
}
