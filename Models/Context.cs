using CoffeeShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShopWeb.Models
{
    public class Context : IdentityDbContext<IdentityUser>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<TransactionItem> TransactionItems { get; set; }

        public Context(DbContextOptions<Context> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole { Id = "3", Name = "Staff", NormalizedName = "STAFF" }
            );
        }
    }
}