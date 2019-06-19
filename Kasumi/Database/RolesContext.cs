using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Kasumi.Database
{
    public class RolesContext : DbContext
    {
        public DbSet<AssignableRole> AssignableRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=roles.db");
        }
    }
    
    public class AssignableRole
    {
        [Key]
        public string RoleId { get; set; } // we can't use ulongs so we'll have to use strings
        public string Name { get; set; }
        public string ServerId { get; set; }
    }
}
