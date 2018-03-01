using System.IO;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using SixLabors.ImageSharp;
using DSharpPlus.Entities;

namespace Kasumi.Commands
{
    public class ImageCommands
    {
        [Command("create")]
        [Description("Creates an image for use with the image commands.")]
        public async Task Create(CommandContext ctx, int width, int height)
        {
            using (Image<Rgba32> image = new Image<Rgba32>(width, height))
            {
                FileStream stream = new FileStream($"temp-{ctx.Guild.Id}.png", FileMode.Create);
                image.Mutate(x => x.Fill(new Rgba32(255, 255, 255)));
                image.SaveAsPng(stream);
                stream.Seek(0, SeekOrigin.Begin);
                await ctx.RespondWithFileAsync(stream);
            }
        }
    }
}
