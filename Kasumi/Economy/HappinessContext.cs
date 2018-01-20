using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using Kasumi.Entities;

namespace Kasumi.Economy
{
    public class HappinessContext : DbContext
    {
        public DbSet<UserHappiness> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=happiness.db");
        }
    }
    public class UserHappiness
    {
        [Key]
        public string Id { get; set; }
        public int Happiness { get; set; }
        public HappinessLevel Level { get; set; }
    }
}
