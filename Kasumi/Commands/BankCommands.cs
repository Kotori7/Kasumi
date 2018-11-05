using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Kasumi.Economy;
using System.Threading.Tasks;
using System.Linq;

namespace Kasumi.Commands
{
    public class BankCommands : BaseCommandModule
    {
        [Command("balance")]
        [Description("Gets the balance for your account.")]
        [Aliases("bal")]
        public async Task Balance(CommandContext ctx)
        {
            if (!Bank.CheckExistance(ctx.User.Id.ToString()))
            {
                await ctx.RespondAsync("You must have an account to check your balance.");
                return;
            }
            DiscordEmbedBuilder embedBuilder = new DiscordEmbedBuilder();
            //embedBuilder.Author = new DiscordEmbedBuilder.EmbedAuthor
            //{
            //    Name = "Kasumi",
            //    IconUrl = ctx.Client.CurrentUser.AvatarUrl
            //};
            //embedBuilder.AddField("", Bank.GetBalance(ctx.User.Id.ToString()).ToString());
            embedBuilder.Title = ctx.User.Username + "'s balance.";
            embedBuilder.Description = Bank.GetBalance(ctx.User.Id.ToString()).ToString();
            await ctx.RespondAsync(embed: embedBuilder.Build());
        }
        [Command("account")]
        [Description("Creates your bank account.")]
        public async Task Account(CommandContext ctx)
        {
            if (Bank.CheckExistance(ctx.User.Id.ToString()))
            {
                await ctx.RespondAsync("You already have an account.");
                return;
            }
            Bank.CreateAccount(ctx.User.Id.ToString());
            if (Bank.CheckExistance(ctx.User.Id.ToString()))
            {
                await ctx.RespondAsync("An account has been opened for you.");
            }
            else
            {
                await ctx.RespondAsync("There was a problem opening your account.");
            }
        }
        [Command("transfer")]
        [Description("Transfers money to someone else.")]
        public async Task Transfer(CommandContext ctx, DiscordUser user, decimal amount)
        {
            if (!Bank.CheckExistance(ctx.User.Id.ToString()))
            {
                await ctx.RespondAsync("You'll need a bank account to do that.");
                return;
            }
            if (!Bank.CheckExistance(user.Id.ToString()))
            {
                await ctx.RespondAsync("That user doesn't have a bank account.");
                return;
            }
            if(!(Bank.GetBalance(ctx.User.Id.ToString()) >= amount)){
                await ctx.RespondAsync("You don't have enough funds.");
                return;
            }
            if(amount < 0)
            {
                await ctx.RespondAsync("Stealing money is highly illegal.");
                return;
            }
            Bank.MoveMoney(ctx.User.Id.ToString(), user.Id.ToString(), amount);
            await ctx.RespondAsync($"Sent O${amount.ToString()} to {user.Username}.");
        }
        [Command("setbal")]
        [Description("Hacks someone's balance.")]
        [RequireOwner]
        public async Task SetBalance(CommandContext ctx, DiscordUser user, decimal amount)
        {
            if (!Bank.CheckExistance(user.Id.ToString())){
                await ctx.RespondAsync("That user doesn't have a bank account.");
                return;
            }
            Bank.HackMoney(user.Id.ToString(), amount);
            await ctx.RespondAsync($"Set {user.Username}'s balance to {amount.ToString()}.");
        }
        [Command("fine")]
        [Description("Fines a user for some money.")]
        [RequireOwner]
        public async Task Fine(CommandContext ctx, DiscordUser user, decimal amount)
        {
            if (!Bank.CheckExistance(user.Id.ToString()))
            {
                await ctx.RespondAsync("That user doesn't have a bank account.");
                return;
            }
            Bank.FineUser(user.Id, amount);
            await ctx.RespondAsync($"Fined {user.Username} ${amount.ToString()}");
        }
        [Command("collect")]
        [Description("Collects your payout based on the amount of messages sent.")]
        [Aliases("payout")]
        public async Task Collect(CommandContext ctx)
        {
            using(var db = new BankContext())
            {
                decimal cock = db.Accounts.Single(b => b.Id == ctx.User.Id.ToString()).CollectBalance;
                if(cock <= 0)
                {
                    await ctx.RespondAsync("You don't have any money to collect.");
                    return;
                }
                db.Accounts.Single(b => b.Id == ctx.User.Id.ToString()).Balance += cock;
                db.Accounts.Single(b => b.Id == ctx.User.Id.ToString()).CollectBalance = 0;
                await db.SaveChangesAsync();
                await ctx.RespondAsync($"You collected ${cock}.");
            }
        }
    }
}
