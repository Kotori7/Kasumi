﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;

namespace Kasumi.Commands
{

    [Description("colour commands")]
    public class ColourCommands : BaseCommandModule
    {
        
        [Command("colour")]
        [RequireBotPermissions(false, DiscordPermission.ManageRoles)]
        [Aliases("color")] // fuckin americans
        public async Task Colour(CommandContext ctx, [Description("Hex code or name of a colour.")] string colour)
        {
            if (colours.TryGetValue(colour, out var colour1))
                colour = colour1;

            if (!colour.StartsWith('#'))
                colour = $"#{colour}";
            
            if (!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            {
                await ctx.RespondAsync("Invalid hex code or colour name.");
                
                return;
            }
            
            foreach (DiscordRole r in ctx.Member.Roles.ToArray())
            {
                if (r.Name.StartsWith("kasumi"))
                    await ctx.Member.RevokeRoleAsync(r);
                
            }
            foreach (DiscordRole r in ctx.Guild.Roles.Values)
            {
                if (r.Name == "kasumi" + colour)
                {
                    await ctx.Member.GrantRoleAsync(r);
                    
                    await ctx.RespondAsync("Updated your roles!");
                    
                    return;
                }
                if (r.Name.StartsWith("kasumi#"))
                    if (ctx.Guild.Members.Count(m => m.Value.Roles.Any(p => p == r)) == 0)
                        await r.DeleteAsync();
                
            }
            
            DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, DiscordPermissions.None,
                new DiscordColor(colour), false, false,
                $"[Kasumi] Colour role for {Helpers.GetUsername(ctx.User)}");
            
            await ctx.Member.GrantRoleAsync(rr,
                $"[Kasumi] Colour role for {Helpers.GetUsername(ctx.User)}");
            
            await ctx.RespondAsync("Updated your roles!");
        }
        
        [Command("setcolour")]
        [Description("Sets someone else's colour.")]
        [Aliases("setcolor")]
        [RequirePermissions(false, DiscordPermission.ManageRoles)]
        public async Task SetColour(CommandContext ctx, DiscordMember member, string colour)
        {
            if (colours.ContainsKey(colour))
                colour = colours[colour];

            if (!colour.StartsWith('#'))
                colour = $"#{colour}";
            
            if (!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            {
                await ctx.RespondAsync("Invalid hex code or colour name.");
                
                return;
            }
            
            foreach (DiscordRole r in member.Roles.ToArray())
            {
                if (r.Name.StartsWith("kasumi"))
                    await member.RevokeRoleAsync(r);
            }
            
            foreach (DiscordRole r in ctx.Guild.Roles.Values)
            {
                if (r.Name == "kasumi" + colour)
                {
                    await member.GrantRoleAsync(r);
                    
                    await ctx.RespondAsync("Updated their roles!");
                    
                    return;
                }
                
                if (r.Name.StartsWith("kasumi#"))
                    if (ctx.Guild.Members.Count(m => m.Value.Roles.Any(p => p == r)) == 0)
                        await r.DeleteAsync();
            }
            DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, DiscordPermissions.None, 
                new DiscordColor(colour), false, false,
                $"[Kasumi] Colour role override for {Helpers.GetUsername(ctx.User)}");
            
            await member.GrantRoleAsync(rr,
                $"[Kasumi] Colour role override for {Helpers.GetUsername(ctx.User)}");
            
            await ctx.RespondAsync("Updated their roles!");
        }
        
        [Command("cleanup")]
        [Description("Cleans up unused colour roles.")]
        [RequirePermissions(false, DiscordPermission.ManageRoles)]
        public async Task Cleanup(CommandContext ctx)
        {
            int count = 0;
            
            DiscordMessage msg = await ctx.RespondAsync("Starting role cleanup, may take a while...");
            
            foreach (DiscordRole r in ctx.Guild.Roles.Values)
            {
                if (r.Name.StartsWith("kasumi#"))
                {
                    if (ctx.Guild.Members.Count(m => m.Value.Roles.Any(p => p == r)) == 0)
                    {
                        await r.DeleteAsync();
                        
                        count++;
                    }
                }
            }
            
            await msg.ModifyAsync($"Cleaned up {count} roles.");
        }

        
        #region Predefined Colours

        private Dictionary<string, string> colours = new()
        {
            {"black", "#000000"},
            {"grey", "#808080"},
            {"white", "#ffffff"},
            {"maroon", "#800000"},
            {"red", "#ff0000"},
            {"purple", "#800080"},
            {"pink", "#ff00ff"},
            {"green", "#008000"},
            {"lime", "#00ff00"},
            {"olive", "#808000"},
            {"yellow", "#ffff00"},
            {"navy", "#000080"},
            {"blue", "#0000ff"},
            {"teal", "#008080"},
            {"aqua", "#00ffff"}
        };

        #endregion
    }
}
