using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Kasumi.Economy;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace Kasumi.Commands
{
    [Description("fun commands")]
    public class FunCommands
    {
        private string[] PetResponses = new[] { "Thanks!", "Meh.", "Okay then.", "Appreciated." };
        private string[] EightBallResponses = new[] { "It is certain.", "Ask again later.", "Most likely.", "Very doubtful.", "Reply hazy try again.", "Outlook good.", "You may rely on it.",
            "My reply is no.", "Cannot predict now.", "My sources say no.", "Yes.", "As I see it, yes.", "Without a doubt.", "Better not tell you now.", "Don't count on it.",
            "Concentrate and ask again.", "Outlook not so good.", "Yes, definitely."};
        private Random rng = new Random();
        [Command("dice")]
        [Description("Rolls a dice.")]
        [Aliases("roll")]
        public async Task Dice(CommandContext ctx, params string[] args)
        {
            string ok = args[0];
            if (!Regex.IsMatch(ok, "\\dd\\d"))
            {
                await ctx.RespondAsync("Argument must be in the format of XdX, where X is a number.");
                return;
            }
            int dice = Int32.Parse(ok.Split('d')[0]);
            int max = Int32.Parse(ok.Split('d')[1]);
            if (dice > 2048 || max > 2048)
            {
                await ctx.RespondAsync("Either number of dice or maximum number cannot be more than 2048.");
                return;
            }
            if (dice < 0 || max < 0)
            {
                await ctx.RespondAsync("Either number of dice or maximum number cannot be negative.");
                return;
            }
            string result = $"{ctx.User.Username} rolled {ok} and got ";
            for (int i = 0; i < dice; i++)
            {
                result += rng.Next(1, max) + " ";
            }
            await ctx.RespondAsync(result + ".");
        }
        [Command("pet")]
        [Description("Pets Kasumi.")]
        public async Task Pet(CommandContext ctx)
        {
            Happiness.IncrementHappiness(ctx.User.Id, rng.Next(0, 5));
            switch (Happiness.GetHappinessLevel(ctx.User.Id))
            {
                case Entities.HappinessLevel.Dispise:
                    await ctx.RespondAsync("Fuck off!");
                    return;
                case Entities.HappinessLevel.Hate:
                    await ctx.RespondAsync("Ehh.");
                    return;
                case Entities.HappinessLevel.Like:
                    await ctx.RespondAsync("Thanks!");
                    return;
                case Entities.HappinessLevel.Love:
                    await ctx.RespondAsync("<3");
                    return;
                case Entities.HappinessLevel.Neutral:
                    await ctx.RespondAsync("Thank you.");
                    return;
            }
        }
        
        [Command("8ball")]
        [Description("Asks the 8 ball a question.")]
        public async Task EightBall(CommandContext ctx, [Description("Question to ask.")] params string[] query)
        {
            await ctx.RespondAsync(":8ball: | " + EightBallResponses[rng.Next(20)]);
        }
        [Command("osu")]
        [Description("Retrieves a user's osu! stats.")]
        public async Task Osu(CommandContext ctx, string user, [Description("0 = osu!, 1 = Taiko, 2 = CtB, 3 = Mania")] int mode)
        {
            if (mode >= 4 || mode <= -1)
            {
                await ctx.RespondAsync("Invalid mode specified.");
                return;
            }
            HttpClient client = new HttpClient();
            string url = $"https://osu.ppy.sh/api/get_user?k={Globals.OsuKey}&u={user}&m={mode}";
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, url);
            HttpResponseMessage resp = await client.SendAsync(req);
            string response = await resp.Content.ReadAsStringAsync();
            JArray a = JArray.Parse(response);
            JObject o = JObject.Parse(a[0].ToString());
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            {
                Name = "Kasumi",
                IconUrl = ctx.Client.CurrentUser.AvatarUrl
            };
            embedBuilder.AddField("User ID", o["user_id"].ToString(), true);
            embedBuilder.AddField("Username", o["username"].ToString(), true);
            embedBuilder.AddField("Country", o["country"].ToString(), true);
            embedBuilder.AddField("Profile URL", $"https://osu.ppy.sh/user/{o["user_id"].ToString()}", true);
            embedBuilder.AddField("Play Count", o["playcount"].ToString(), true);
            embedBuilder.AddField("Ranked Score", o["ranked_score"].ToString(), true);
            embedBuilder.AddField("Total Score", o["total_score"].ToString(), true);
            embedBuilder.AddField("Accuracy", o["accuracy"].ToString().Remove(5), true);
            embedBuilder.AddField("SS Count", o["count_rank_ss"].ToString(), true);
            embedBuilder.AddField("S Count", o["count_rank_s"].ToString(), true);
            await ctx.RespondAsync(embed: embedBuilder.Build());
        }
        [Command("colour")]
        [RequireBotPermissions(DSharpPlus.Permissions.ManageRoles)]
        [Aliases("color")] // fuckin americans
        public async Task Colour(CommandContext ctx, [Description("Hex code of a colour.")] string colour)
        {
            if(!Regex.IsMatch(colour, @"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$"))
            {
                await ctx.RespondAsync("Invalid hex code.");
                return;
            }
           foreach(DiscordRole r in ctx.Member.Roles)
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
            foreach(DiscordRole r in ctx.Guild.Roles)
            {
                if(r.Name == "kasumi" + colour)
                {
                    await ctx.Member.GrantRoleAsync(r);
                    return;
                }
            }
            DiscordRole rr = await ctx.Guild.CreateRoleAsync("kasumi" + colour, DSharpPlus.Permissions.None, new DiscordColor(colour), false, false, "[Kasumi] Colour role for " + ctx.User.Username + "#" + ctx.User.Discriminator);
            await ctx.Member.GrantRoleAsync(rr, "[Kasumi] Colour role for " + ctx.User.Username + "#" + ctx.User.Discriminator);
            await ctx.RespondAsync("Updated your roles!");
        }
        private async Task RemoveKasumiRoles(DiscordMember m)
        {
            foreach(DiscordRole r in m.Roles)
            {
                if (r.Name.StartsWith("kasumi"))
                {
                    await m.RevokeRoleAsync(r, "[Kasumi] Colour role cleanup.");
                }
            }
        }
    }
}
