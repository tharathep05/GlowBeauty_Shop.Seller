using System.Collections.Generic;

namespace GlowBeauty_Shop.Seller.Models
{
    public class SellerDashboardViewModel
    {
        public decimal TotalSales { get; set; }
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int LowStockProductsCount { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
    }
}