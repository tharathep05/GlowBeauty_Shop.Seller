using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Models;

namespace GlowBeauty_Shop.Seller.Data
{
    public class GlowDbContext : DbContext
    {
        public GlowDbContext(DbContextOptions<GlowDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ป้องกันข้อผิดพลาดโดยระบุให้ EF Core ละเว้น Property 'Items'
            modelBuilder.Entity<Order>().Ignore(o => o.Items);
        }

        public void SeedInitialData()
        {
            if (!Products.Any())
            {
                Products.AddRange(
                    new Product { Id = 1, Name = "Glow Serum Vitamin C", Category = "Skincare", Price = 590, StockQuantity = 25, Description = "เซรั่มบำรุงผิวหน้ากระจ่างใส", IsActive = true },
                    new Product { Id = 2, Name = "Velvet Matte Lipstick Red", Category = "Makeup", Price = 350, StockQuantity = 8, Description = "ลิปสติกแมตต์สีแดงคลาสสิก", IsActive = true },
                    new Product { Id = 3, Name = "Hydrating Sunscreen SPF50+", Category = "Skincare", Price = 450, StockQuantity = 15, Description = "ครีมกันแดดเนื้อบางเบา", IsActive = true }
                );
                SaveChanges();
            }

            if (!Orders.Any())
            {
                Orders.Add(new Order
                {
                    Id = 1001,
                    CustomerName = "คุณสมหญิง ใจดี",
                    CustomerPhone = "081-234-5678",
                    ShippingAddress = "123/45 ถนนสุขุมวิท กทม.",
                    OrderDate = DateTime.Now.AddHours(-2),
                    Status = OrderStatus.Pending,
                    TotalAmount = 940,
                    OrderItems = new List<OrderItem>
                    {
                        new OrderItem { ProductId = 1, Quantity = 1, UnitPrice = 590 },
                        new OrderItem { ProductId = 2, Quantity = 1, UnitPrice = 350 }
                    }
                });
                SaveChanges();
            }
        }
    }
}