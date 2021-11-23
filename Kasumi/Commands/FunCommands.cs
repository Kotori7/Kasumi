using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Kasumi.Commands
{
    [Description("fun commands")]
    
    public class FunCommands : BaseCommandModule
    {
        private string[] EightBallResponses = { "It is certain.", "Ask again later.", "Most likely.", "Very doubtful.", 
            "Reply hazy, try again.", "Outlook good.", "You may rely on it.", "My reply is no.", 
            "Cannot predict now.", "My sources say no.", "Yes.", "As I see it, yes.", 
            "Without a doubt.", "Better not tell you now.", "Don't count on it.",
            "Concentrate and ask again.", "Outlook not so good.", "Yes, definitely."};
        
        private Random rng = new Random();
        private HttpClient client = new HttpClient();
        
        [Command("dice")]
        [Description("Rolls a dice.")]
        [Aliases("roll")]
        public async Task Dice(CommandContext ctx, params string[] args)
        {
            string ok = args[0];
            
            if (!Regex.IsMatch(ok, "\\dd\\d"))
            {
                await ctx.RespondAsync("Argument must be in the format of XdX, where X is a number.");
                
                return;
            }
            
            int dice = Int32.Parse(ok.Split('d')[0]);
            int max = Int32.Parse(ok.Split('d')[1]);
            
            if (dice > 2048 || max > 2048)
            {
                await ctx.RespondAsync("Either number of dice or maximum number cannot be more than 2048.");
                
                return;
            }
            
            if (dice < 0 || max < 0)
            {
                await ctx.RespondAsync("Either number of dice or maximum number cannot be negative.");
                
                return;
            }
            
            string result = $"{ctx.User.Username} rolled {ok} and got ";
            int total = 0;
            
            for (int i = 0; i < dice; i++)
            {
                int roll = rng.Next(1, max);
                total += roll;
                if (i == dice - 1) result += $"{roll}";
                else result += $"{roll}, ";
            }
            
            await ctx.RespondAsync($"{result} [{total}].");
        }

        [Command("8ball")]
        [Description("Asks the 8 ball a question.")]
        public async Task EightBall(CommandContext ctx, [Description("Question to ask.")] params string[] query)
        {
            await ctx.RespondAsync($":8ball: | {EightBallResponses[rng.Next(EightBallResponses.Length)]}");
        }

        [Command("mac")]
        [Description("Looks up a MAC address using the MACVendors API.")]
        public async Task Mac(CommandContext ctx, string address)
        {
            if(!Regex.IsMatch(address, @"^([0-9a-fA-F][0-9a-fA-F]:){5}([0-9a-fA-F][0-9a-fA-F])$"))
            {
                await ctx.RespondAsync("That's not a valid MAC address");
                
                return;
            }
            
            string vendor = await client.GetStringAsync("http://api.macvendors.com/" + address.Replace(':', '-'));
            
            await ctx.RespondAsync(vendor);
        }
    }
}
