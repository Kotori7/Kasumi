using System;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;

namespace Kasumi.Commands
{
    [Description("You know, those 18+ commands. Those ones.")]
    [RequireNsfw]
    public class NsfwCommands
    {
        private HttpClient client = new HttpClient();
        [Command("danbooru")]
        [Description("Retrieves a random post from danbooru with the specified tags")]
        public async Task Danbooru(CommandContext ctx, params string[] tags)
        {
            // Check tags based on danbooru's restrictions
            if(tags.Length > 2)
            {
                await ctx.RespondAsync("You can't have more than 2 tags with Danbooru. Sorry.");
                return;
            }
            if(tags.Length == 0)
            {
                await ctx.RespondAsync("Please include some tags.");
                return;
            }
            if(tags.Length < 0)
            {
                await ctx.RespondAsync("Something's not right here.");
                return;
            }
            HttpResponseMessage response = await client.GetAsync("https://danbooru.donmai.us/posts.json?limit=1&random=1&tags=" + String.Join("+", tags));
            string json = await response.Content.ReadAsStringAsync();
            JArray a = JArray.Parse(json);
            string link = a[0].Value<string>("file_url");
            if (!link.StartsWith("http"))
                link = "https://danbooru.donmai.us" + link;
            await ctx.RespondAsync(link);
        }
    }
}
