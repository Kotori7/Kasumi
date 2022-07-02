using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;

namespace Kasumi.Commands
{
    
    public class BasicCommands : BaseCommandModule
    {
        
        [Command("echo")]
        [Description("Echo command. Self-explanatory.")]
        public async Task Echo(CommandContext ctx, params string[] args)
        {
            await ctx.RespondAsync(String.Join(" ", args));
        }
        
        [Command("ping")]
        [Description("Checks the bot's ping to Discord.")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.RespondAsync($"Pong, {ctx.Client.Ping}ms");
        }
        
        [Command("uptime")]
        [Description("Responds with the bot's uptime.")]
        public async Task Uptime(CommandContext ctx)
        {
            StringBuilder sb = new StringBuilder();
            
            sb.Append("I've been up and running for ");
            
            if ((DateTime.Now - Bot.StartTime).Days != 0)
            {
                sb.Append($"{(DateTime.Now - Bot.StartTime).Days} days ");
            }
            
            if ((DateTime.Now - Bot.StartTime).Hours != 0)
            {
                sb.Append($"{(DateTime.Now - Bot.StartTime).Hours} hours ");
            }
            
            if ((DateTime.Now - Bot.StartTime).Minutes != 0)
            {
                sb.Append($"{(DateTime.Now - Bot.StartTime).Minutes} minutes ");
            }
            
            sb.Append($"{(DateTime.Now - Bot.StartTime).Seconds} seconds.");
            
            await ctx.RespondAsync(sb.ToString());
        }
        
        [Command("shutdown")]
        [Hidden]
        [RequireOwner]
        public async Task Shutdown(CommandContext ctx)
        {
            await ctx.RespondAsync("Farewell!");
            
            ctx.Client.Logger.Log(LogLevel.Information, new EventId(705, "ClientShutdown"), 
                $"Shutdown initiated by {ctx.User.Username}#{ctx.User.Discriminator} ({ctx.User.Id})");
            
            await ctx.Client.DisconnectAsync();

            Environment.Exit(0);
        }
        
        [Command("info")]
        [Description("Shows information about the bot.")]
        public async Task Info(CommandContext ctx)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            
            embedBuilder.AddField("System Version", Environment.OSVersion.VersionString);
            embedBuilder.AddField("Architecture", System.Runtime.InteropServices.RuntimeInformation.OSArchitecture.ToString(), true);
            embedBuilder.AddField("RAM Usage", $"{Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024} MB", true);
            embedBuilder.AddField("Bot Version", Bot.Version, true);
            embedBuilder.AddField(".NET Version", System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription.ToString(), true);
            
            await ctx.RespondAsync(embedBuilder.Build());
        }
        
        [Command("status")]
        [Description("Sets the bot's now playing status")]
        [RequireOwner]
        public async Task Status(CommandContext ctx, string type, [RemainingText] string status)
        {
            switch (type)
            {
                case "p":
                    await ctx.Client.UpdateStatusAsync(new DiscordActivity(status, ActivityType.Playing));
                    break;
                
                case "l":
                    await ctx.Client.UpdateStatusAsync(new DiscordActivity(status, ActivityType.ListeningTo));
                    break;
                
                case "w":
                    await ctx.Client.UpdateStatusAsync(new DiscordActivity(status, ActivityType.Watching));
                    break;
                
                case "s":
                    await ctx.Client.UpdateStatusAsync(new DiscordActivity(status, ActivityType.Streaming));
                    break;
            }
        }
    }
}
