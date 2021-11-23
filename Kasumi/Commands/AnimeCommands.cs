using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using DSharpPlus.Entities;

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
            
            HttpResponseMessage resp = await http.GetAsync($"https://api.jikan.moe/v3/search/anime?q={title}&limit=1");
            if (!resp.IsSuccessStatusCode)
            {
                await ctx.RespondAsync("There was an error processing your request.");
                return;
            }
            
            string body = await resp.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(body);
            JArray results = o.Value<JArray>("results");
            
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            
            embed.ImageUrl = results[0].Value<string>("image_url");
            embed.Title = results[0].Value<string>("title");
            embed.Url = results[0].Value<string>("url");
            embed.Description = results[0].Value<string>("synopsis");
            
            embed.AddField("Episodes", results[0].Value<int>("episodes").ToString(), true);
            embed.AddField("Score", results[0].Value<double>("score").ToString(), true);
            embed.AddField("Start Date", results[0].Value<string>("start_date").Remove(10), true);
            
            if (results[0].Value<bool>("airing"))
            {
                embed.AddField("End Date", "Airing", true);
            }
            else
            {
                embed.AddField("End Date", results[0].Value<string>("end_date").Remove(10));
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
            
            HttpResponseMessage resp = await http.GetAsync($"https://api.jikan.moe/v3/search/manga?q={title}&limit=1");
            if (!resp.IsSuccessStatusCode)
            {
                await ctx.RespondAsync("There was an error processing your request.");
                return;
            }
            
            string body = await resp.Content.ReadAsStringAsync();
            JObject o = JObject.Parse(body);
            JArray results = o.Value<JArray>("results");
            
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            
            embed.ImageUrl = results[0].Value<string>("image_url");
            embed.Title = results[0].Value<string>("title");
            embed.Url = results[0].Value<string>("url");
            embed.Description = results[0].Value<string>("synopsis");
            
            embed.AddField("Chapters", results[0].Value<int>("chapters").ToString(), true);
            embed.AddField("Volumes", results[0].Value<int>("volumes").ToString(), true);
            embed.AddField("Score", results[0].Value<double>("score").ToString(), true);
            embed.AddField("Start Date", results[0].Value<string>("start_date").Remove(10), true);
            
            if (results[0].Value<bool>("publishing"))
            {
                embed.AddField("End Date", "Publishing", true);
            }
            else
            {
                embed.AddField("End Date", results[0].Value<string>("end_date").Remove(10));
            }
            
            await ctx.RespondAsync(embed.Build());
        }
    }
}
