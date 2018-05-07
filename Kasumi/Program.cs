using System;
using System.IO;
using Newtonsoft.Json;
using Kasumi.Entities;
using System.Text;
using System.Threading.Tasks;

namespace Kasumi
{
    class Program
    {
        // Bootstrap for checking the runtime environment and initializing the main bot procedure.
        static async Task Main(string[] args)
        {
            // Check that config.json exists, we need it.
            if (!File.Exists("config.json"))
            {
                Console.WriteLine("Please create and populate config.json before running!");
                System.Threading.Thread.Sleep(2000);
                Environment.Exit(0);
            }
            // Parse the configuration
            var json = "";
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = sr.ReadToEndAsync().GetAwaiter().GetResult();
            var config = JsonConvert.DeserializeObject<ConfigJson>(json);
            Globals.Token = config.Token;
            Globals.Prefix = config.Prefix;
            Globals.OsuKey = config.OsuKey;
            // Run the actual bot.
            await Bot.BotMain();
            // Exit with the bot's exit code
            Environment.Exit(Globals.ExitCode);
        }
    }
}
