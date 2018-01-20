using System;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Kasumi.Commands
{
    [Description("Basic commands.")]
    public class BasicCommands
    {
        [Command("echo")]
        [Description("Echo command. Self-explanitory.")]
        public async Task Echo(CommandContext ctx, params string[] args)
        {
            await ctx.RespondAsync(String.Join(" ", args));
        }
        [Command("ping")]
        [Description("Checks the bot's ping to Discord.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            await ctx.RespondAsync($"Pong, {ctx.Client.Ping}ms");
        }
        [Command("uptime")]
        [Description("Respons with the bot's uptime.")]
        public async Task Uptime(CommandContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("I've been up and running for ");
            if(((DateTime.Now - Globals.StartTime).Days) != 0)
            {
                sb.Append($"{(DateTime.Now - Globals.StartTime).Days} days ");
            }
            if(((DateTime.Now - Globals.StartTime).Hours) != 0)
            {
                sb.Append($"{(DateTime.Now - Globals.StartTime).Hours} hours ");
            }
            if(((DateTime.Now - Globals.StartTime).Minutes) != 0)
            {
                sb.Append($"{(DateTime.Now - Globals.StartTime).Minutes} minutes ");
            }
            sb.Append($"{(DateTime.Now - Globals.StartTime).Seconds} seconds.");
            await ctx.RespondAsync(sb.ToString());
        }
        [Command("shutdown")]
        [Hidden]
        public async Task Shutdown(CommandContext ctx)
        {
            if(ctx.User.Id == 235019900618407937)
            {
                await ctx.RespondAsync("Farewell!");
                await ctx.Client.DisconnectAsync();
                Environment.Exit(0);
            }
            else
            {
                await ctx.RespondAsync("No chance.");
            }
        }
    }
}
