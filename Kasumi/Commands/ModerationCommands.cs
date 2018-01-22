using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace Kasumi.Commands
{
    public class ModerationCommands
    {
        [Command("ban")]
        [Description("Bans a user.")]
        [RequirePermissions(DSharpPlus.Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordMember mem, [RemainingText] string reason)
        {
            await ctx.Guild.BanMemberAsync(mem, 0, $"[Ban by {ctx.User.Username}#{ctx.User.Discriminator}] {reason}");
            await ctx.RespondAsync($"{mem.Username}#{mem.Discriminator} got bent");
        }
    }
}
