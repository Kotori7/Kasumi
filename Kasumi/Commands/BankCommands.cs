using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Kasumi.Economy;
using System.Threading.Tasks;

namespace Kasumi.Commands
{
    public class BankCommands
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
            await ctx.RespondAsync(Bank.GetBalance(ctx.User.Id.ToString()).ToString());
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
            Bank.MoveMoney(ctx.User.Id.ToString(), user.Id.ToString(), amount);
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
        }
    }
}
