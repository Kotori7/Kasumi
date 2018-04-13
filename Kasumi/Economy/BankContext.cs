﻿using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kasumi.Economy
{
    public class BankContext : DbContext
    {
        public DbSet<BankAccount> Accounts { get; set; }
        public DbSet<BankTransaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Kasumi;Trusted_Connection=True;");
            optionsBuilder.UseSqlite(@"Data Source=bank.db");
        }
    }
    public class BankAccount
    {
        [Key]
        public string Id { get; set; }
        public decimal Balance { get; set; }
        public decimal CollectBalance { get; set; }
        public List<BankTransaction> Transactions { get; set; }
    }
    public class BankTransaction
    {
        [Key]
        public string FromId { get; set; }
        public string ToId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
