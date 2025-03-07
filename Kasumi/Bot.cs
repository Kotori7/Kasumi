﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using Kasumi.Commands;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using DSharpPlus.VoiceNext;
using Kasumi.SlashCommands;
using Kasumi.Telemetry;
using Microsoft.Extensions.Logging;

#pragma warning disable CS0618

namespace Kasumi
{
    public class Bot
    {
        public static DiscordClient Client { get; private set; }
        public static CommandsNextExtension Commands { get; private set; }
        public static DateTime StartTime { get; private set; }
        public static bool IsDevelopment { get; private set; }
        public static TelemetryClient TelemetryClient { get; private set; }
        public static string Version { get; private set; }

        public Bot(Entities.ConfigJson config)
        {
            // Discord Client Configuration
            DiscordClientBuilder clientBuilder = DiscordClientBuilder.CreateDefault(
                config.Token,
                DiscordIntents.All);
            
            // Events
            clientBuilder.ConfigureEventHandlers(
                b => b.HandleSessionCreated((s, e) => Client_SessionCreated(s, e))
                    .HandleGuildAvailable((s, e) => Client_GuildAvailable(s, e))
            );

            // CommandsNext Configuration
            var cfg2 = new CommandsNextConfiguration
            {
                EnableMentionPrefix = true,
                EnableDms = true,
                StringPrefixes = new List<string> { config.Prefix }
            };
            
            IsDevelopment = config.Dev;
            
            // Register CommandsNext
            clientBuilder.UseCommandsNext(cne =>
            {
                cne.RegisterCommands<BasicCommands>();
                cne.RegisterCommands<InfoCommands>();
                cne.RegisterCommands<FunCommands>();
                cne.RegisterCommands<NsfwCommands>();
                cne.RegisterCommands<RoleCommands>();
                cne.RegisterCommands<HashingCommands>();
                cne.RegisterCommands<ModerationCommands>();
                cne.RegisterCommands<ColourCommands>();
                cne.RegisterCommands<AnimeCommands>();
                cne.CommandErrored += Commands_CommandErrored;
                cne.CommandExecuted += Commands_CommandExecuted;
            }, cfg2);
            clientBuilder.UseSlashCommands(slash =>
            {
                if (IsDevelopment)
                    slash.RegisterCommands<SlashCommands.SlashCommands>(ulong.Parse(config.DevServerId));
                else
                    slash.RegisterCommands<SlashCommands.SlashCommands>();
            });
            
            Client = clientBuilder.Build();
            
            StartTime = DateTime.Now;
            
            TelemetryClient = new(config.NewRelicID, config.NewRelicKey);

            Version = "1.2.0";
        }

        public async Task Start()
        {
            Client.Logger.Log(LogLevel.Information, new EventId(100, "Startup"), 
                $"Starting Kasumi version {Version} ({(IsDevelopment ? "development" : "production")})");
            
            await Client.ConnectAsync();

            Thread metricsThread = new Thread(RunMetrics);
            metricsThread.Start();
            
            await Task.Delay(-1);
        }

        private static async void RunMetrics()
        {
            int interval;
            interval = IsDevelopment ? 10000 : 60000;
            
            while (true)
            {
                await Task.Delay(10000);
                
                MetricPayload pingPayload = new()
                {
                    Name = "kasumi.ping",
                    Type = "gauge",
                    Value = Client.GetConnectionLatency(Client.Guilds.First().Value.Id).Milliseconds
                };
                MetricPayload serverPayload = new()
                {
                    Name = "kasumi.server.count",
                    Type = "count",
                    Value = Client.Guilds.Count
                };

                if (!IsDevelopment) await TelemetryClient.SendMetrics(new [] {pingPayload, serverPayload}, interval);
                
                await Task.Delay(interval - 10000);
            }

        }

        private static async Task Commands_CommandExecuted(CommandsNextExtension cne, CommandExecutionEventArgs e)
        {
            var payload = new Dictionary<string, object>()
            {
                { "eventType", "CommandExecuted" },
                { "command", e.Command.QualifiedName },
                { "timestamp", e.Context.Message.Timestamp.ToString() }
            };

            if (!IsDevelopment) await TelemetryClient.SendEvent(payload);
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
            if (!IsDevelopment) await TelemetryClient.SendEvent(payload);
            cne.Client.Logger.Log(LogLevel.Error, new EventId(704, "CommandError"),
                $"Exception {e.Exception.GetType().Name} occurred while running command {e.Command.Name}. \nMessage: {e.Exception.Message}\nStacktrace: {e.Exception.StackTrace}");
        }

        private static Task Client_ClientErrored(DiscordClient client, DSharpPlus.EventArgs.ClientErrorEventArgs e)
        {
            client.Logger.Log(LogLevel.Error, new EventId(703, "ClientError"),
                $"Client error: {e.Exception.GetType().Name}. \n" +
                $"Message: {e.Exception.Message} \n" +
                $"Stacktrace: {e.Exception.StackTrace}");

            return Task.CompletedTask;
        }

        private static Task Client_GuildAvailable(DiscordClient client, DSharpPlus.EventArgs.GuildAvailableEventArgs e)
        {
            client.Logger.Log(LogLevel.Information, new EventId(701, "GuildAvailable"),
                $"Guild available: {e.Guild.Name}");
            
            return Task.CompletedTask;
        }

        private static Task Client_SessionCreated(DiscordClient client, DSharpPlus.EventArgs.SessionCreatedEventArgs e)
        {
            client.Logger.Log(LogLevel.Information, new EventId(700, "ClientReady"), 
                "Client ready!");
            
            client.Logger.Log(LogLevel.Information, new EventId(700, "ClientReady"), 
                $"Logged in as {client.CurrentUser.Username}#{client.CurrentUser.Discriminator} ({client.CurrentUser.Id})");
            
            return Task.CompletedTask;
        }
    }
}
