using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Kasumi.Economy;

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
    }
}
