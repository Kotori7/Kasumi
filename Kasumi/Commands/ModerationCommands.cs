using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

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
    }
}
