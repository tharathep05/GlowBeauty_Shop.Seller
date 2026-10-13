using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Data;
using GlowBeauty_Shop.Seller.Models;
using System.Linq;
using System.Threading.Tasks;

namespace GlowBeauty_Shop.Seller.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Order (เช็กล็อกอินอัตโนมัติ)
        public async Task<IActionResult> Index(OrderStatus? status)
        {
            // บังคับว่าต้องล็อกอินก่อนเปิดหน้านี้
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            var orders = await query.OrderByDescending(o => o.OrderDate).ToListAsync();
            ViewBag.SelectedStatus = status;

            return View(orders);
        }

        // POST: Order/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status, string? trackingNumber)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                if (!string.IsNullOrEmpty(trackingNumber))
                {
                    order.TrackingNumber = trackingNumber;
                }

                await _context.SaveChangesAsync();
                TempData["Success"] = $"อัปเดตสถานะออเดอร์ #{order.OrderNumber} สำเร็จแล้ว";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}