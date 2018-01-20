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
            if (!Bank.CheckExistance(ctx.User.Id))
            {
                await ctx.RespondAsync("You must have an account to check your balance.");
                return;
            }
            await ctx.RespondAsync(Bank.GetBalance(ctx.User.Id).ToString());
        }
        [Command("account")]
        [Description("Creates your banmk account.")]
        public async Task Account(CommandContext ctx)
        {
            if (Bank.CheckExistance(ctx.User.Id))
            {
                await ctx.RespondAsync("You already have an account.");
                return;
            }
            Bank.CreateAccount(ctx.User.Id);
            if (Bank.CheckExistance(ctx.User.Id))
            {
                await ctx.RespondAsync("An account has been opened for you.");
            }
            else
            {
                await ctx.RespondAsync("There was a problem opening your account.");
            }
        }
    }
}
