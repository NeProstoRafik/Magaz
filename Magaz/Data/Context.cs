using Magaz.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Magaz.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
         // Database.EnsureDeleted();
            Database.EnsureCreated();
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationType> Applications { get; set; }
        public DbSet<Product> Products { get; set; }
    }
}
