using System.Linq;

namespace Kasumi.Economy
{
    public class Bank
    {
        /// <summary>
        /// Creates a bank account with the specified user id.
        /// </summary>
        /// <param name="id">Discord User ID</param>
        public static void CreateAccount(string id)
        {
            using(var db = new BankContext())
            {
                var account = new BankAccount { Id = id, Balance = 0 };
                db.Accounts.Add(account);
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Returns the specified users balance.
        /// </summary>
        /// <param name="id">ID of account to search for.</param>
        /// <returns>decimal</returns>
        public static decimal GetBalance(string id)
        {
            using(var db = new BankContext())
            {
                var account = db.Accounts
                    .Single(b => b.Id == id);
                return account.Balance;
            }
        }
        /// <summary>
        /// Moves money from one account to another.
        /// </summary>
        /// <param name="fromId">ID of user to take money from.</param>
        /// <param name="toId">ID of user to give money to.</param>
        /// <param name="amount">Amount of money to transfer.</param>
        public static void MoveMoney(string fromId, string toId, decimal amount)
        {
            using(var db = new BankContext())
            {
                db.Accounts.Single(b => b.Id == fromId).Balance -= amount;
                db.Accounts.Single(b => b.Id == toId).Balance += amount;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// money hax lol
        /// </summary>
        /// <param name="id">ID of user.</param>
        /// <param name="amount">Amount to hack in.</param>
        public static void HackMoney(string id, decimal amount)
        {
            using(var db = new BankContext())
            {
                db.Accounts.Single(b => b.Id == id).Balance = amount;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Checks if the specified user has an account.
        /// </summary>
        /// <param name="id">User ID to check.</param>
        /// <returns>bool</returns>
        public static bool CheckExistance(string id)
        {
            using(var db = new BankContext())
            {
                return db.Accounts.Count(b => b.Id == id) == 1;
            }
        }
        /// <summary>
        /// Fines a user a certain amount of money
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="amount">Amount to fine</param>
        public static void FineUser(ulong id, decimal amount)
        {
            using(var db = new BankContext())
            {
                db.Accounts.Single(b => b.Id == id.ToString()).Balance -= amount;
                db.SaveChanges();
            }
        }
        /// <summary>
        /// Adds an amount of money for the user to collect
        /// </summary>
        /// <param name="id">user id</param>
        /// <param name="amount">amount to add</param>
        public static void AddMessagePayout(ulong id, decimal amount)
        {
            using(var db = new BankContext())
            {
                db.Accounts.Single(b => b.Id == id.ToString()).CollectBalance += amount;
                db.SaveChanges();
            }
        }
        
    }
}
