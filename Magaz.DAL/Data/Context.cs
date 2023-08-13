using Magaz.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Magaz.DAL.Data
{
    public class Context : IdentityDbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
            //Database.EnsureDeleted();
            // Database.EnsureCreated();
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationType> Applications { get; set; }
        public DbSet<Product> Products { get; set; }
        // public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<IdentityUser> ApplicationUsers { get; set; }
        public DbSet<InquiryHeader> InquiryHeader { get; set; }
        public DbSet<InquiryDetail> InquiryDetail { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
