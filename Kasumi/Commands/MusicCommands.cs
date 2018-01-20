using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;

namespace Kasumi.Commands
{
    public class MusicCommands
    {
        [Command("join")]
        [Description("Joins the voice channel you are currently in")]
        public async Task Join(CommandContext ctx)
        {
            var vnext = ctx.Client.GetVoiceNext();
            if(vnext == null)
            {
                await ctx.RespondAsync("VoiceNext is not enabled or configured.");
                return;
            }
            var vnc = vnext.GetConnection(ctx.Guild);
            if(vnc != null)
            {
                await ctx.RespondAsync("I'm already connected to a voice channel in this guild.");
                return;
            }
            var vstat = ctx.Member?.VoiceState;
            if(vstat?.Channel == null)
            {
                await ctx.RespondAsync("You're not in a voice channel.");
                return;
            }
            vnc = await vnext.ConnectAsync(vstat.Channel);
            await ctx.RespondAsync($"Connected to `{vstat.Channel.Name}`");
        }
        [Command("leave")]
        [Description("Leaves the current voice channel.")]
        public async Task Leave(CommandContext ctx)
        {
            var vnc = ctx.Client.GetVoiceNext().GetConnection(ctx.Guild);
            if(vnc == null)
            {
                await ctx.RespondAsync("I'm not in a voice channel.");
                return;
            }
            vnc.Disconnect();
            await ctx.RespondAsync("Disconnected.");
        }
    }
}
