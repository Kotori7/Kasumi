using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace Kasumi.SlashCommands;

public class SlashCommands : ApplicationCommandModule
{
    [SlashCommand("test", "A slash command made to test the DSharpPlusSlashCommands library!")]
    public async Task TestCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Success!"));
    }

    [SlashCommand("colour", "Sets your colour within the current server")]
    [SlashRequireGuild]
    [SlashRequireBotPermissions(Permissions.ManageRoles)]
    public async Task Colour(InteractionContext ctx, 
        [Option("colour", "Hex code or name of colour to set")] string colour)
    {
        if (_colours.TryGetValue(colour, out var colour1))
            colour = colour1;

        if (!colour.StartsWith('#'))
            colour = $"#{colour}";
            
        if (!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("You've specified an invalid colour."));
                
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
                await ctx.Member.GrantRoleAsync(r,
                    $"[Kasumi] Colour role for {ctx.User.Username}");
                    
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Updated successfully!"));
                    
                return;
            }
            if (r.Name.StartsWith("kasumi#"))
                if (ctx.Guild.Members.Count(m => m.Value.Roles.Any(p => p == r)) == 0)
                    await r.DeleteAsync();
                
        }
            
        DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, Permissions.None,
            new DiscordColor(colour), false, false,
            $"[Kasumi] Colour role for {ctx.Member.Username}");
            
        await ctx.Member.GrantRoleAsync(rr,
            $"[Kasumi] Colour role for {ctx.User.Username}");
            
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Updated successfully!"));
    }

    [SlashCommand("setcolour", "Sets another user's colour. (Requires you have Manage Roles permission)")]
    [SlashRequireGuild]
    [SlashRequirePermissions(Permissions.ManageRoles)]
    public async Task SetColour(InteractionContext ctx,
        [Option("target", "Target user to set colour of")]
        DiscordUser targetUser,
        [Option("colour", "Colour to set")] string colour)
    {
        if (_colours.TryGetValue(colour, out var colour1))
            colour = colour1;

        if (!colour.StartsWith('#'))
            colour = $"#{colour}";
            
        if (!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("You've specified an invalid colour."));
                
            return;
        }

        if (!ctx.Guild.Members.TryGetValue(targetUser.Id, out var target))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Invalid target user."));
            
            return;
        }
            
        foreach (DiscordRole r in target.Roles.ToArray())
        {
            if (r.Name.StartsWith("kasumi"))
                await target.RevokeRoleAsync(r);
                
        }
        foreach (DiscordRole r in ctx.Guild.Roles.Values)
        {
            if (r.Name == "kasumi" + colour)
            {
                await target.GrantRoleAsync(r,
                    $"[Kasumi] Colour role for {target.Username} set by {ctx.User.Username}");
                    
                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                    new DiscordInteractionResponseBuilder().WithContent("Updated successfully!"));
                    
                return;
            }
            if (r.Name.StartsWith("kasumi#"))
                if (ctx.Guild.Members.Count(m => m.Value.Roles.Any(p => p == r)) == 0)
                    await r.DeleteAsync();
                
        }
            
        DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, Permissions.None,
            new DiscordColor(colour), false, false,
            $"[Kasumi] Colour role for {target.Username} set by {ctx.User.Username}");
            
        await target.GrantRoleAsync(rr,
            $"[Kasumi] Colour role for {target.Username} set by {ctx.User.Username}");
            
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent("Updated successfully!"));
    }
    
    [SlashCommand("setnick", "Sets a nickname for another user")]
    [SlashRequireGuild]
    [SlashRequirePermissions(DSharpPlus.Permissions.ManageNicknames)]
    public async Task Nickname(InteractionContext ctx, 
        [Option("target", "Target user to change nickname of")] DiscordUser targetUser,
        [Option("nick", "New nickname to set")] string nickname)
    {
        if (!ctx.Guild.Members.TryGetValue(targetUser.Id, out var target))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Invalid target user."));
            
            return;
        }
        
        await target.ModifyAsync(delegate(DSharpPlus.Net.Models.MemberEditModel model)
        {
            model.AuditLogReason = $"Nickname updated by {ctx.User.Username}";
            model.Nickname = nickname;
        });

        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(":white_check_mark:"));
    }
    
    [SlashCommand("unnick", "Removes a nickname from a user")]
    [SlashRequirePermissions(DSharpPlus.Permissions.ManageNicknames)]
    public async Task Unnick(InteractionContext ctx, 
        [Option("target", "User to remove nickname from")] DiscordUser targetUser)
    {
        if (!ctx.Guild.Members.TryGetValue(targetUser.Id, out var target))
        {
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().WithContent("Invalid target user."));
            
            return;
        }
        
        await target.ModifyAsync(delegate(DSharpPlus.Net.Models.MemberEditModel model)
        {
            model.AuditLogReason = $"Nickname removed by {ctx.User.Username}";
            model.Nickname = "";
        });
            
        await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder().WithContent(":white_check_mark:"));
    }
    
    #region Predefined Colours

    private readonly Dictionary<string, string> _colours = new()
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