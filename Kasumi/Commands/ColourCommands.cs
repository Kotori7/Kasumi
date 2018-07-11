using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;

namespace Kasumi.Commands
{
    [Description("colour commands")]
    public class ColourCommands
    {
        [Command("colour")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageRoles)]
        [Aliases("color")] // fuckin americans
        public async Task Colour(CommandContext ctx, [Description("Hex code of a colour.")] string colour)
        {
            if (!colour.StartsWith('#')) colour = "#" + colour;
            if (!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            {
                await ctx.RespondAsync("Invalid hex code.");
                return;
            }
            foreach (DiscordRole r in ctx.Member.Roles.ToArray())
            {
                //if(r.Name == "kasumi" + colour)
                //{
                //    await RemoveKasumiRoles(ctx.Member);
                //    await ctx.Member.GrantRoleAsync(r, "[Kasumi] Colour role for " + ctx.User.Username + "#" + ctx.User.Discriminator);
                //    await ctx.RespondAsync("Updated your roles!");
                //    return;
                //}

                if (r.Name.StartsWith("kasumi"))
                {
                    await ctx.Member.RevokeRoleAsync(r);
                }
            }
            foreach (DiscordRole r in ctx.Guild.Roles.ToArray())
            {
                if (r.Name == "kasumi" + colour)
                {
                    await ctx.Member.GrantRoleAsync(r);
                    return;
                }
            }
            DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, DSharpPlus.Permissions.None, new DiscordColor(colour), false, false, "[Kasumi] Colour role for " + ctx.User.Username + "#" + ctx.User.Discriminator);
            await ctx.Member.GrantRoleAsync(rr, "[Kasumi] Colour role for " + ctx.User.Username + "#" + ctx.User.Discriminator);
            await ctx.RespondAsync("Updated your roles!");
        }
        [Command("setcolour")]
        [Description("Sets someone elses colour.")]
        [RequirePermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task SetColour(CommandContext ctx, DiscordMember member, string colour)
        {
            if (!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            {
                await ctx.RespondAsync("Invalid hex code.");
                return;
            }
            foreach (DiscordRole r in member.Roles.ToArray())
            {
                if (r.Name.StartsWith("kasumi"))
                {
                    await member.RevokeRoleAsync(r);
                }
            }
            foreach (DiscordRole r in ctx.Guild.Roles.ToArray())
            {
                if (r.Name == "kasumi" + colour)
                {
                    await member.GrantRoleAsync(r);
                    return;
                }
            }
            DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, DSharpPlus.Permissions.None, new DiscordColor(colour), false, false, "[Kasumi] Colour role override for " + member.Username + "#" + member.Discriminator);
            await member.GrantRoleAsync(rr, "[Kasumi] Colour role override for " + member.Username + "#" + member.Discriminator);
            await ctx.RespondAsync("Updated their roles!");
        }
        [Command("cleanup")]
        [Description("Cleans up unused colour roles.")]
        [RequirePermissions(DSharpPlus.Permissions.ManageRoles)]
        public async Task Cleanup(CommandContext ctx)
        {
            int count = 0;
            await ctx.RespondAsync("Starting role cleanup, may take a while...");
            foreach(DiscordRole r in ctx.Guild.Roles.ToArray())
            {
                if (r.Name.StartsWith("kasumi#"))
                {
                    if (ctx.Guild.Members.Count(m => m.Roles.Any(p => p == r)) == 0)
                    {
                        await r.DeleteAsync();
                        count++;
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }
            await ctx.RespondAsync($"Cleaned up {count} roles.");
        }
    }
}
