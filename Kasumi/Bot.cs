﻿using System;
using System.Collections.Generic;
using Kasumi.Commands;
using System.Threading.Tasks;
using DSharpPlus;
using Kasumi.Economy;
using DSharpPlus.Exceptions;
using DSharpPlus.CommandsNext;
using DSharpPlus.VoiceNext;

namespace Kasumi
{
    public class Bot
    {
        public static DiscordClient Client { get; set; }
        public static CommandsNextExtension Commands { get; set; }
        public static VoiceNextExtension Voice { get; set; }
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
            Commands = Client.UseCommandsNext(cfg2);
            Commands.RegisterCommands<BasicCommands>();
            Commands.RegisterCommands<InfoCommands>();
            Commands.RegisterCommands<FunCommands>();
            //Commands.RegisterCommands<ImageCommands>();
            Commands.RegisterCommands<NsfwCommands>();
            //Commands.RegisterCommands<ImageAnalysisCommand>();
            Commands.RegisterCommands<HashingCommands>();
            Commands.RegisterCommands<ModerationCommands>();
            Commands.RegisterCommands<BankCommands>();
            Commands.CommandErrored += Commands_CommandErrored; // this needs to be here otherwise it fucking nullrefs
            Globals.StartTime = DateTime.Now;
            await Client.ConnectAsync();
            await Task.Delay(-1);
        }

        private static async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            await e.Context.Channel.SendMessageAsync("Error: \n ```" + e.Exception + "```");
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
