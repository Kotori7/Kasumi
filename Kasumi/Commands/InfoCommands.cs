using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

namespace Kasumi.Commands
{
    [Description("info commands")]
    public class InfoCommands
    {
        [Command("guildinfo")]
        [Description("Shows information about the current guild.")]
        [Aliases(new[] {  "guild", "serverinfo", "server"})]
        public async Task GuildInfo(CommandContext ctx)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Kasumi",
                IconUrl = ctx.Client.CurrentUser.AvatarUrl
            };
            embedBuilder.Color = DiscordColor.Purple;
            embedBuilder.AddField("Name", ctx.Guild.Name);
            embedBuilder.AddField("ID", ctx.Guild.Id.ToString());
            embedBuilder.AddField("Owner", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator);
            embedBuilder.AddField("Region", ctx.Guild.VoiceRegion.Name);
            embedBuilder.AddField("Members", ctx.Guild.MemberCount.ToString());
            embedBuilder.AddField("Creation Date", ctx.Guild.CreationTimestamp.Date.ToLongDateString());
            DiscordEmbed embed = embedBuilder.Build();
            await ctx.RespondAsync(embed: embed);
        }
        [Command("userinfo")]
        [Description("Shows information about you, or a specified user")]
        [Aliases("user")]
        public async Task UserInfo(CommandContext ctx, [Description("Optionally specify a user.")] params string[] args)
        {
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Kasumi",
                IconUrl = ctx.Client.CurrentUser.AvatarUrl
            };
            embedBuilder.Color = DiscordColor.Purple;
            DiscordUser user = null;
            if (!args.Any())
            {
                user = ctx.User;
            }
            else if (ulong.TryParse(args[0], out ulong id))
            {
                try
                {
                    user = await ctx.Guild?.GetMemberAsync(id) ?? await ctx.Client.GetUserAsync(id);
                }
                catch (Exception e)
                {
                    await ctx.RespondAsync("Unable to find that user.");
                    return;
                }
            }
            else if (ctx.Message.MentionedUsers.Any())
            {
                user = ctx.Message.MentionedUsers.First();
            }
            if (user == null)
            {
                await ctx.RespondAsync("user is null ok");
                return;
            }
            embedBuilder.AddField("Name", user.Username + "#" + user.Discriminator);
            embedBuilder.AddField("ID", user.Id.ToString());
            embedBuilder.AddField("Discord Join Date", user.CreationTimestamp.UtcDateTime.ToLongDateString());
            if (user.Presence.Activity.Name != null)
            {
                embedBuilder.AddField("Currently Playing", user.Presence.Activity.Name);
            }
            DiscordEmbed em = embedBuilder.Build();
            await ctx.RespondAsync(embed: em);
        }
    }
}
