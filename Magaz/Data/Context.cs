using Magaz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Magaz.Data
{
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
     // Database.EnsureDeleted();
           Database.EnsureCreated();
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationType> Applications { get; set; }
        public DbSet<Product> Products { get; set; }
       // public DbSet<ApplicationUser> ApplicationUsers { get; set; }
  
              public DbSet<IdentityUser> ApplicationUsers { get; set; }
    }
}
