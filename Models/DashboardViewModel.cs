using System;
using System.Collections.Generic;

namespace GlowBeauty_Shop.Seller.Models
{
    public class DashboardViewModel
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int PendingDeliveries { get; set; }
        public int TotalProducts { get; set; }

        public List<TopProductViewModel> TopProducts { get; set; } = new List<TopProductViewModel>();
        public List<Order> RecentOrders { get; set; } = new List<Order>();

        // ข้อมูลสร้างกราฟสรุปยอดขาย
        public List<string> ChartLabels { get; set; } = new List<string>();
        public List<decimal> ChartData { get; set; } = new List<decimal>();
    }

    public class TopProductViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int TotalSold { get; set; }
        public decimal TotalSalesAmount { get; set; }
    }
}