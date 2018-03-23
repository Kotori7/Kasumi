using System;
using System.Diagnostics;
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
            if (((DateTime.Now - Globals.StartTime).Days) != 0)
            {
                sb.Append($"{(DateTime.Now - Globals.StartTime).Days} days ");
            }
            if (((DateTime.Now - Globals.StartTime).Hours) != 0)
            {
                sb.Append($"{(DateTime.Now - Globals.StartTime).Hours} hours ");
            }
            if (((DateTime.Now - Globals.StartTime).Minutes) != 0)
            {
                sb.Append($"{(DateTime.Now - Globals.StartTime).Minutes} minutes ");
            }
            sb.Append($"{(DateTime.Now - Globals.StartTime).Seconds} seconds.");
            await ctx.RespondAsync(sb.ToString());
        }
        [Command("shutdown")]
        [Hidden]
        [RequireOwner]
        public async Task Shutdown(CommandContext ctx)
        {
            await ctx.RespondAsync("Farewell!");
            await ctx.Client.DisconnectAsync();
            Environment.Exit(0);
        }
        [Command("info")]
        [Description("Shows information about the bot.")]
        public async Task Info(CommandContext ctx)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.AddField("System Version", Environment.OSVersion.VersionString);
            embedBuilder.AddField("Architecture", Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE"), true);
            embedBuilder.AddField("RAM Usage", (Process.GetCurrentProcess().PrivateMemorySize64 / 1024 / 1024).ToString() + "MB", true);
            embedBuilder.AddField("Bot Version", System.IO.File.ReadAllText("version"));
            await ctx.RespondAsync(embed: embedBuilder.Build());
        }
        [Command("status")]
        [Description("Sets the bot's now playing status")]
        [RequireOwner]
        public async Task Status(CommandContext ctx, [RemainingText] string status)
        {
            await ctx.Client.UpdateStatusAsync(new DiscordActivity(status));
        }
    }
}
