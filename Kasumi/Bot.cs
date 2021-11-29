using System;
using System.Collections.Generic;
using Kasumi.Commands;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;
using Microsoft.Extensions.Logging;

#pragma warning disable CS0618

namespace Kasumi
{
    public static class Bot
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }

        public static async Task BotMain()
        {
            // Discord Client Configuration
            var cfg = new DiscordConfiguration
            {
                Token = Globals.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                Intents = DiscordIntents.All
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

        private static async Task Commands_CommandExecuted(CommandsNextExtension cne, CommandExecutionEventArgs e)
        {
            var payload = new Dictionary<string, object>()
            {
                { "eventType", "CommandExecuted" },
                { "command", e.Command.QualifiedName },
                { "timestamp", e.Context.Message.Timestamp.ToString() }
            };

            await Globals.TelemetryClient.SendEvent(payload);
        }

        private static async Task Commands_CommandErrored(CommandsNextExtension cne, CommandErrorEventArgs e)
        {
            if (e.Exception.GetType().Name == "ChecksFailedException")
            {
                await e.Context.Channel.SendMessageAsync("You can't run that command here.");
                return;
            }
            
            if (e.Exception.GetType().Name == "CommandNotFoundException")
                return;
            
            await e.Context.Channel.SendMessageAsync(
                $"There was a problem running that command, and a {e.Exception.GetType().Name} occured.\n" +
                $"More info: ```{e.Exception.Message}```");

            var payload = new Dictionary<string, object>()
            {
                { "eventType", "CommandException" },
                { "exception", e.Exception.GetType().Name },
                { "message", e.Exception.Message },
                { "command", e.Command.QualifiedName },
                { "timestamp", e.Context.Message.Timestamp.ToString() }
            };
            await Globals.TelemetryClient.SendEvent(payload);
            // cne.Client.Logger.Log(LogLevel.Error, new EventId(704, "CommandError"),
            //     $"Exception {e.Exception.GetType().Name} occurred while running command {e.Command.Name}. \nMessage: {e.Exception.Message}\nStacktrace: {e.Exception.StackTrace}");
        }

        private static Task Client_ClientErrored(DiscordClient client, DSharpPlus.EventArgs.ClientErrorEventArgs e)
        {
            client.Logger.Log(LogLevel.Error, new EventId(703, "ClientError"),
                $"Client error: {e.Exception.GetType().Name}. \n" +
                $"Message: {e.Exception.Message} \n" +
                $"Stacktrace: {e.Exception.StackTrace}");

            return Task.CompletedTask;
        }

        private static Task Client_GuildAvailable(DiscordClient client, DSharpPlus.EventArgs.GuildCreateEventArgs e)
        {
            client.Logger.Log(LogLevel.Information, new EventId(701, "GuildAvailable"),
                $"Guild available: {e.Guild.Name}");
            
            return Task.CompletedTask;
        }

        private static Task Client_Ready(DiscordClient client, DSharpPlus.EventArgs.ReadyEventArgs e)
        {
            client.Logger.Log(LogLevel.Information, new EventId(700, "ClientReady"), 
                "Client ready!");
            
            client.Logger.Log(LogLevel.Information, new EventId(700, "ClientReady"), 
                $"Logged in as {client.CurrentUser.Username}#{client.CurrentUser.Discriminator} ({client.CurrentUser.Id})");
            
            return Task.CompletedTask;
        }
    }
}
