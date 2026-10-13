using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Data;
using GlowBeauty_Shop.Seller.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GlowBeauty_Shop.Seller.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dashboard
        public async Task<IActionResult> Index()
        {
            // บังคับเช็กล็อกอิน
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .ToListAsync();

            var products = await _context.Products.ToListAsync();

            // 1. คำนวณสรุปข้อมูลการ์ด
            var totalRevenue = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var pendingDeliveries = orders.Count(o => o.Status == OrderStatus.Paid || o.Status == OrderStatus.Processing);
            var totalProducts = products.Count;

            // 2. จัดอันดับสินค้าขายดี 5 อันดับแรก (Top 5 Best Sellers)
            var topProducts = orders
                .Where(o => o.Status != OrderStatus.Cancelled)
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => i.ProductId)
                .Select(g => new TopProductViewModel
                {
                    ProductName = g.FirstOrDefault()?.Product?.Name ?? "สินค้าทั่วไป",
                    Category = g.FirstOrDefault()?.Product?.Category ?? "-",
                    ImageUrl = g.FirstOrDefault()?.Product?.ImageUrl ?? "/images/default-product.jpg",
                    TotalSold = g.Sum(i => i.Quantity),
                    TotalSalesAmount = g.Sum(i => i.Quantity * i.UnitPrice)
                })
                .OrderByDescending(p => p.TotalSold)
                .ThenByDescending(p => p.TotalSalesAmount)
                .Take(5)
                .ToList();

            // 3. คำนวณข้อมูลกราฟยอดขายย้อนหลัง 6 เดือน
            var chartLabels = new System.Collections.Generic.List<string>();
            var chartData = new System.Collections.Generic.List<decimal>();
            var thCulture = new CultureInfo("th-TH");

            for (int i = 5; i >= 0; i--)
            {
                var targetDate = DateTime.Now.AddMonths(-i);
                var monthLabel = targetDate.ToString("MMM yy", thCulture);

                var monthSales = orders
                    .Where(o => o.Status != OrderStatus.Cancelled &&
                                o.OrderDate.Month == targetDate.Month &&
                                o.OrderDate.Year == targetDate.Year)
                    .Sum(o => o.TotalAmount);

                chartLabels.Add(monthLabel);
                chartData.Add(monthSales);
            }

            var viewModel = new DashboardViewModel
            {
                TotalRevenue = totalRevenue,
                TotalOrders = totalOrders,
                PendingDeliveries = pendingDeliveries,
                TotalProducts = totalProducts,
                TopProducts = topProducts,
                RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(5).ToList(),
                ChartLabels = chartLabels,
                ChartData = chartData
            };

            return View(viewModel);
        }
    }
}