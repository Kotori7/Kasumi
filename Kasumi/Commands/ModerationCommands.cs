using System;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using DSharpPlus.Net.Models;

namespace Kasumi.Commands
{
    public class ModerationCommands : BaseCommandModule
    {
        [Command("ban")]
        [Description("Bans a user.")]
        [RequirePermissions(DSharpPlus.Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordMember mem, [RemainingText] string reason)
        {
            await ctx.Guild.BanMemberAsync(mem, 0, 
                $"[Ban by {ctx.User.Username}#{ctx.User.Discriminator}] {reason}");
            
            await ctx.RespondAsync($"{mem.Username}#{mem.Discriminator} got bent");
        }
        
        [Command("nuke")]
        [Description("Nukes a specified amount of messages from the channel.")]
        [Aliases("purge", "massdel")]
        [RequirePermissions(DSharpPlus.Permissions.ManageMessages)]
        public async Task Nuke(CommandContext ctx, [Description("The amount of messages to delete")] int amount)
        {
            await ctx.Message.DeleteAsync($"Message nuke called by {ctx.User.Username}#{ctx.User.Discriminator}");
            
            System.Collections.Generic.IReadOnlyList<DiscordMessage> messages = await ctx.Channel.GetMessagesAsync(amount);
            
            foreach(DiscordMessage m in messages)
                await m.DeleteAsync($"Message nuke called by {ctx.User.Username}#{ctx.User.Discriminator}");
            
        }

        [Command("nickname")]
        [Description("Sets a nickname for another user")]
        [Aliases("nick")]
        [RequirePermissions(DSharpPlus.Permissions.ManageNicknames)]
        public async Task Nickname(CommandContext ctx, DiscordMember target, [RemainingText] string nickname)
        {
            await target.ModifyAsync(delegate(MemberEditModel model)
            {
                model.AuditLogReason = $"Nickname updated by {ctx.User.Username}#{ctx.User.Discriminator}";
                model.Nickname = nickname;
            });

            await ctx.Message.CreateReactionAsync(
                DiscordEmoji.FromName(ctx.Client, ":white_check_mark:", false));
        }

        [Command("unnick")]
        [Description("Removes a nickname from a user")]
        [RequirePermissions(DSharpPlus.Permissions.ManageNicknames)]
        public async Task Unnick(CommandContext ctx, DiscordMember target)
        {
            await target.ModifyAsync(delegate(MemberEditModel model)
            {
                model.AuditLogReason = $"Nickname removed by {ctx.User.Username}#{ctx.User.Discriminator}";
                model.Nickname = "";
            });
            
            await ctx.Message.CreateReactionAsync(
                DiscordEmoji.FromName(ctx.Client, ":white_check_mark:", false));
        }
    }
}
