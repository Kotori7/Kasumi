using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;
using Flurl.Util;

namespace Kasumi.Commands
{
    
    public class AnimeCommands : BaseCommandModule
    {
        
        private static HttpClient http = new HttpClient();
        
        [Command("anime")]
        [Description("Gets info about an anime from MyAnimeList.")]
        public async Task Anime(CommandContext ctx, [RemainingText] string title)
        {
            if(title.Length < 3)
            {
                await ctx.RespondAsync("You need to provide at least 3 characters in the title.");
                return;
            }

            title = System.Web.HttpUtility.UrlEncode(title);
            
            HttpResponseMessage resp = await http.GetAsync($"https://api.jikan.moe/v4/anime?q={title}&sfw");
            if (!resp.IsSuccessStatusCode)
            {
                await ctx.RespondAsync("There was an error processing your request.");
                return;
            }
            
            string body = await resp.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(body);
            JArray results = o.Value<JArray>("data");
            
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();

            embed.ImageUrl = results[0].SelectToken("$.images.jpg.image_url").ToString();
            embed.Title = results[0].Value<JArray>("titles").First(x => x.Value<string>("type") == "English")
                .Value<string>("title");
            embed.Url = results[0].Value<string>("url");
            embed.Description = results[0].Value<string>("synopsis");
            
            embed.AddField("Episodes", results[0].Value<int>("episodes").ToString(), true);
            embed.AddField("Score", results[0].Value<double>("score").ToString(), true);
            embed.AddField("Start Date", results[0].SelectToken("$.aired.from").ToString().Remove(10));
            
            if (results[0].Value<bool>("airing"))
            {
                embed.AddField("End Date", "Airing", true);
            }
            else
            {
                embed.AddField("End Date", results[0].SelectToken("$.aired.to").ToString().Remove(10));
            }
            
            await ctx.RespondAsync(embed.Build());
        }

        [Command("manga")]
        [Description("Gets info about a manga from MyAnimeList")]
        public async Task Manga(CommandContext ctx, [RemainingText] string title)
        {
            if (title.Length < 3)
            {
                await ctx.RespondAsync("You need to provide at least 3 characters in the title.");
                return;
            }
            
            title = System.Web.HttpUtility.UrlEncode(title);
            
            HttpResponseMessage resp = await http.GetAsync($"https://api.jikan.moe/v4/manga?q={title}&limit=1");
            if (!resp.IsSuccessStatusCode)
            {
                await ctx.RespondAsync("There was an error processing your request.");
                return;
            }
            
            string body = await resp.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(body);
            JArray results = o.Value<JArray>("data");
            
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            
            embed.ImageUrl = results[0].SelectToken("$.images.jpg.image_url").ToString();
            embed.Title = results[0].Value<JArray>("titles").First(x => x.Value<string>("type") == "English")
                .Value<string>("title");
            embed.Url = results[0].Value<string>("url");
            embed.Description = results[0].Value<string>("synopsis");

            string chapters = results[0].Value<int?>("chapters") == null
                ? "Unknown"
                : results[0].Value<int>("chapters").ToString();
            embed.AddField("Chapters", chapters, true);
            string volumes = results[0].Value<int?>("volumes") == null
                ? "Unknown"
                : results[0].Value<int>("volumes").ToString();
            embed.AddField("Volumes", volumes, true);
            embed.AddField("Score", results[0].Value<double>("score").ToString(), true);
            embed.AddField("Start Date", results[0].SelectToken("$.published.from").ToString().Remove(10));
            
            if (results[0].Value<bool>("publishing"))
            {
                embed.AddField("End Date", "Publishing", true);
            }
            else
            {
                embed.AddField("End Date", results[0].SelectToken("$.published.to").ToString().Remove(10), true);
            }
            
            await ctx.RespondAsync(embed.Build());
        }
    }
}
