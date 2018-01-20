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
        public async Task Ban(CommandContext ctx, DiscordMember mem)
        {
            if (ctx.Channel.IsPrivate) return;
            if(ctx.Message.MentionedUsers.Count == 1)
            {
                DiscordMember m = await ctx.Guild.GetMemberAsync(ctx.Message.MentionedUsers[0].Id);
                await ctx.Guild.BanMemberAsync(m, 0, $"Ban command issued by {ctx.User.Username}#{ctx.User.Discriminator}");
                await ctx.RespondAsync($"{m.Username}#{m.Discriminator} got bent");
                return;
            }
            else if(ctx.Message.MentionedUsers.Count > 1)
            {
                foreach(DiscordUser u in ctx.Message.MentionedUsers)
                {
                    DiscordMember m = await ctx.Guild.GetMemberAsync(u.Id);
                    await ctx.Guild.BanMemberAsync(m, 0, $"Ban command issued by {ctx.User.Username}#{ctx.User.Discriminator}");
                    await ctx.RespondAsync($"{m.Username}#{m.Discriminator} got bent");
                }
                return;
            }
            else
            {
                await ctx.Guild.BanMemberAsync(mem, 0, $"Ban command issued by {ctx.Member.Username}#{ctx.User.Discriminator}");
                await ctx.RespondAsync($"{mem.Username}#{mem.Discriminator} got bent");
            }
        }
    }
}
