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
    
    public class FunCommands : BaseCommandModule
    {
        private string[] PetResponses = new[] { "thanks", "ok", @"tyvm \<3", @"ありがと！" };
        private string[] EightBallResponses = new[] { "It is certain.", "Ask again later.", "Most likely.", "Very doubtful.", "Reply hazy try again.", "Outlook good.", "You may rely on it.",
            "My reply is no.", "Cannot predict now.", "My sources say no.", "Yes.", "As I see it, yes.", "Without a doubt.", "Better not tell you now.", "Don't count on it.",
            "Concentrate and ask again.", "Outlook not so good.", "Yes, definitely."};
        private Random rng = new Random();
        private HttpClient client = new HttpClient();
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
            int total = 0;
            for (int i = 0; i < dice; i++)
            {
                int roll = rng.Next(1, max);
                total += roll;
                if (i == dice - 1) result += $"{roll}";
                else result += $"{roll}, ";
            }
            await ctx.RespondAsync($"{result} [{total}].");
        }
        [Command("pet")]
        [Description("Pets Kasumi.")]
        public async Task Pet(CommandContext ctx)
        {
            await ctx.RespondAsync(PetResponses[rng.Next(PetResponses.Length)]);
        }
        
        [Command("8ball")]
        [Description("Asks the 8 ball a question.")]
        public async Task EightBall(CommandContext ctx, [Description("Question to ask.")] params string[] query)
        {
            await ctx.RespondAsync(":8ball: | " + EightBallResponses[rng.Next(EightBallResponses.Length)]);
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
        
        [Command("mac")]
        [Description("Looks up a MAC address using the MACVendors API.")]
        public async Task Mac(CommandContext ctx, string address)
        {
            if(!Regex.IsMatch(address, @"^([0-9a-fA-F][0-9a-fA-F]:){5}([0-9a-fA-F][0-9a-fA-F])$"))
            {
                await ctx.RespondAsync("That's not a valid MAC address");
                return;
            }
            string vendor = await client.GetStringAsync("http://api.macvendors.com/" + address.Replace(':', '-'));
            await ctx.RespondAsync(vendor);
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
