using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Kasumi.Database;

namespace Kasumi.Commands
{
    [Group("role")]
    [Description("Commands for self-assignable roles")]
    public class RoleCommands : BaseCommandModule
    {
        [Command("get")]
        [Description("Gives you a self-assignable role")]
        [RequireBotPermissions(false, DiscordPermission.ManageRoles)]
        public async Task GetCommand(CommandContext ctx, [RemainingText] string name)
        {
            using (var db = new RolesContext())
            {
                var role = db.AssignableRoles
                    .Where(r => r.ServerId == ctx.Guild.Id.ToString())
                    .Single(r => r.Name == name);
                
                if (role == null)
                {
                    await ctx.RespondAsync("I couldn't find a role in this server by that name.");
                    return;
                }

                try
                {
                    DiscordRole dRole = ctx.Guild.Roles.Single(r => r.Value.Id.ToString() == role.RoleId).Value;
                    
                    await ctx.Member.GrantRoleAsync(dRole, "[Kasumi] Giving user self-assignable role.");
                    
                    await ctx.RespondAsync($"Gave you the `{dRole.Name}` role.");
                }
                
                catch
                {
                    await ctx.RespondAsync(
                        "An error occurred when trying to give you a role. The role you want has probably been deleted.");
                }
                
            }
        }

        [Command("remove")]
        [Description("Removes a self-assignable role from you.")]
        [RequireBotPermissions(false, DiscordPermission.ManageRoles)]
        public async Task RemoveCommand(CommandContext ctx, [RemainingText] string name)
        {
            using (var db = new RolesContext())
            {
                var role = db.AssignableRoles
                    .Where(r => r.ServerId == ctx.Guild.Id.ToString())
                    .Single(r => r.Name == name);
                
                if (role == null)
                {
                    await ctx.RespondAsync("I couldn't find a role in this server by that name.");
                    return;
                }
                
                DiscordRole dRole = ctx.Guild.Roles.Single(r => r.Value.Id.ToString() == role.RoleId).Value;
                
                if (!ctx.Member.Roles.Contains(dRole))
                {
                    await ctx.RespondAsync("You don't seem to have that role.");
                    
                    return;
                }
                
                await ctx.Member.RevokeRoleAsync(dRole, "[Kasumi] Removing self-assignable role.");
                
                await ctx.RespondAsync($"You should no longer have the `{dRole.Name}` role.");
            }
        }

        [Command("add")]
        [Description("Adds a self-assignable role that members can add to themselves.")]
        [RequirePermissions(false, DiscordPermission.ManageRoles)]
        public async Task AddCommand(CommandContext ctx, DiscordRole role, [RemainingText] string name)
        {
            using (var db = new RolesContext())
            {
                var dbRole = new AssignableRole
                {
                    Name = name,
                    RoleId = role.Id.ToString(),
                    ServerId = ctx.Guild.Id.ToString()
                };
                
                db.AssignableRoles.Add(dbRole);
                await db.SaveChangesAsync();
                
                await ctx.RespondAsync("Role added successfully.");
            }
        }

        [Command("delete")]
        [Description("Deletes a self-assignable role. This will not automatically remove it from users who have it.")]
        [RequirePermissions(false, DiscordPermission.ManageRoles)]
        public async Task DeleteCommand(CommandContext ctx, [RemainingText] string name)
        {
            using (var db = new RolesContext())
            {
                var role = db.AssignableRoles
                    .Where(r => r.ServerId == ctx.Guild.Id.ToString())
                    .Single(r => r.Name == name);
                
                if (role == null)
                {
                    await ctx.RespondAsync("I couldn't find a role in this server by that name.");
                    return;
                }
                
                db.AssignableRoles.Remove(role);
                await db.SaveChangesAsync();
                
                await ctx.RespondAsync("Removed that role successfully. It has not been taken from any users that have it, though.");
            }
        }

        [Command("list")]
        [Description("Lists all available roles in this guild")]
        public async Task ListCommand(CommandContext ctx)
        {
            using (var db = new RolesContext())
            {
                var roles = db.AssignableRoles
                    .Where(r => r.ServerId == ctx.Guild.Id.ToString());
                
                if (!roles.Any())
                {
                    await ctx.RespondAsync("There are no roles available in this server.");
                    
                    return;
                }

                string output = "";
                foreach (var role in roles)
                {
                    output += $"`{role.Name}` ";
                }

                await ctx.RespondAsync($"Available roles: {output}");
            }
        }
    }
}
