using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Models;

namespace GlowBeauty_Shop.Seller.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<SellerUser> SellerUsers { get; set; }
    }
}