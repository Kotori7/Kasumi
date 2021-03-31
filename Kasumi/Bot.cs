using System;
using System.Collections.Generic;
using Kasumi.Commands;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Logging;

#pragma warning disable CS0618

namespace Kasumi
{
    public class Bot
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        public static VoiceNextExtension Voice { get; set; }
        public static readonly TelemetryClient TelemetryClient = new(); // """""datamining"""""
        public static async Task BotMain()
        {
            // Discord Client Configuration
            var cfg = new DiscordConfiguration
            {
                Token = Globals.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };
            Client = new DiscordClient(cfg);
            // Events
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientErrored;

            // CommandsNext Configuration
            var cfg2 = new CommandsNextConfiguration
            {
                EnableMentionPrefix = true,
                EnableDms = true,
                StringPrefixes = new List<string>{ Globals.Prefix }
            };
            //var cfg3 = new VoiceNextConfiguration
            //{
            //    VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.Music
            //};
            //Voice = Client.UseVoiceNext(cfg3);
            TelemetryClient.InstrumentationKey = Globals.AIKey;
            Commands = Client.UseCommandsNext(cfg2);
            Commands.RegisterCommands<BasicCommands>();
            Commands.RegisterCommands<InfoCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<NsfwCommands>();
            Commands.RegisterCommands<RoleCommands>();
            Commands.RegisterCommands<HashingCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<ColourCommands>();
            Commands.RegisterCommands<AnimeCommands>();
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.CommandExecuted += Commands_CommandExecuted;
            Globals.StartTime = DateTime.Now;
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Commands_CommandExecuted(CommandsNextExtension cne, CommandExecutionEventArgs e)
        {
            TelemetryClient.TrackEvent("Command Run: " + e.Command.Name);
            return Task.CompletedTask;
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension cne, CommandErrorEventArgs e)
        {
            if (e.Exception.GetType().Name == "ChecksFailedException")
            {
                await e.Context.Channel.SendMessageAsync("You can't run that command here.");
                return;
            }
            if (e.Exception.GetType().Name == "CommandNotFoundException") return;
            await e.Context.Channel.SendMessageAsync($"There was a problem running that command, and a {e.Exception.GetType().Name} occured.\n More info: ```{e.Exception.Message}```");
            TelemetryClient.TrackException(e.Exception);
        }

        private static Task Client_ClientErrored(DiscordClient client, DSharpPlus.EventArgs.ClientErrorEventArgs e)
        {
            Console.WriteLine($"Client error: {e.Exception.Message}");
            TelemetryClient.TrackException(e.Exception);
            return Task.CompletedTask;
        }

        private static Task Client_GuildAvailable(DiscordClient client, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            Console.WriteLine($"Guild available: {e.Guild.Name}");
            return Task.CompletedTask;
        }

        private static Task Client_Ready(DiscordClient client, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            Console.WriteLine("Client ready!");
            return Task.CompletedTask;
        }
    }
}
