using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace Kasumi.Economy
{
    public class BankContext : DbContext
    {
        public DbSet<BankAccount> Accounts { get; set; }
        public DbSet<BankTransaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Databse=Kasumi;Trusted_Connection=True;");
        }
    }
    public class BankAccount
    {
        public ulong Id { get; set; }
        public decimal Balance { get; set; }
        public int Happiness { get; set; }
        public List<BankTransaction> Transactions { get; set; }
    }
    public class BankTransaction
    {
        public ulong FromId { get; set; }
        public ulong ToId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
