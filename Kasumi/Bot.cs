using System;
using System.Collections.Generic;
using Kasumi.Commands;
using System.Threading.Tasks;
using DSharpPlus;
using Kasumi.Economy;
using DSharpPlus.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.ApplicationInsights;

namespace Kasumi
{
    public class Bot
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        public static VoiceNextExtension Voice { get; set; }
        public static TelemetryClient TelemetryClient = new TelemetryClient(); // """""datamining"""""
        private static Random rng = new Random();
        public static async Task BotMain()
        {
            // Discord Client Configuration
            var cfg = new DiscordConfiguration
            {
                Token = Globals.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                //LogLevel = LogLevel.Debug,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };
            Client = new DiscordClient(cfg);
            // Events
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientErrored += Client_ClientErrored;
            Client.MessageCreated += Client_MessageCreated;
            
            // CommandsNext Configuration
            var cfg2 = new CommandsNextConfiguration
            {
                EnableMentionPrefix = true,
                EnableDms = true,
                StringPrefixes = new List<string>{ Globals.Prefix }
            };
            var cfg3 = new VoiceNextConfiguration
            {
                VoiceApplication = DSharpPlus.VoiceNext.Codec.VoiceApplication.Music
            };
            Voice = Client.UseVoiceNext(cfg3);
            TelemetryClient.InstrumentationKey = Globals.AIKey;
            Commands = Client.UseCommandsNext(cfg2);
            Commands.RegisterCommands<BasicCommands>();
            Commands.RegisterCommands<InfoCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<NsfwCommands>();
            Commands.RegisterCommands<RoleCommands>();
            Commands.RegisterCommands<HashingCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<BankCommands>();
            Commands.RegisterCommands<ColourCommands>();
            Commands.RegisterCommands<AnimeCommands>();
            Commands.CommandErrored += Commands_CommandErrored;
            Commands.CommandExecuted += Commands_CommandExecuted;
            Globals.StartTime = DateTime.Now;
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static Task Commands_CommandExecuted(CommandExecutionEventArgs e)
        {
            TelemetryClient.TrackEvent("Command Run: " + e.Command.Name);
            return Task.CompletedTask;
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
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

        private static Task Client_MessageCreated(DSharpPlus.EventArgs.MessageCreateEventArgs e)
        {
            if (!Bank.CheckExistance(e.Author.Id.ToString()))
            {
                Bank.CreateAccount(e.Author.Id.ToString());
                return Task.CompletedTask;
            }
            decimal f = rng.Next(5);
            Bank.AddMessagePayout(e.Author.Id, f);
            return Task.CompletedTask;
        }

        private static Task Client_ClientErrored(DSharpPlus.EventArgs.ClientErrorEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "Kasumi", $"Client error: {e.Exception.Message}", DateTime.Now);
            TelemetryClient.TrackException(e.Exception);
            return Task.CompletedTask;
        }

        private static Task Client_GuildAvailable(DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Kasumi", $"Guild available: {e.Guild.Name}", DateTime.Now);
            return Task.CompletedTask;
        }

        private static Task Client_Ready(DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "Kasumi", "Client ready!", DateTime.Now);
            return Task.CompletedTask;
        }
    }
}
