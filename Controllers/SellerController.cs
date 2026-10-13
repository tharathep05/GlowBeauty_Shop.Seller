using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Data;
using GlowBeauty_Shop.Seller.Models;

namespace GlowBeauty_Shop.Seller.Controllers
{
    public class SellerController : Controller
    {
        private readonly GlowDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SellerController(GlowDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
            }
            base.OnActionExecuting(context);
        }

        // 1. หน้า Dashboard
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var orders = await _context.Orders.Include(o => o.OrderItems).ToListAsync();
            var products = await _context.Products.ToListAsync();

            var viewModel = new SellerDashboardViewModel
            {
                TotalSales = orders.Where(o => o.Status != OrderStatus.Cancelled).Sum(o => o.TotalAmount),
                TotalOrders = orders.Count,
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                LowStockProductsCount = products.Count(p => p.StockQuantity <= 10),
                RecentOrders = orders.OrderByDescending(o => o.OrderDate).Take(10).ToList()
            };

            return View(viewModel);
        }

        // 2. หน้าแสดงรายการสินค้า
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            return View(products);
        }

        // 3. หน้าเพิ่มสินค้าใหม่ (GET)
        [HttpGet]
        public IActionResult CreateProduct()
        {
            return View(new Product { IsActive = true, Price = 0, StockQuantity = 1 });
        }

        // 3. บันทึกเพิ่มสินค้าใหม่ + อัปโหลดรูปภาพ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(Product product, IFormFile? imageFile)
        {
            // ตัดปัญหา Validation ติดขัดที่ ImageUrl
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/uploads/" + uniqueFileName;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"เพิ่มสินค้า \"{product.Name}\" เรียบร้อยแล้ว";
                return RedirectToAction(nameof(Products));
            }

            return View(product);
        }

        // 4. หน้าแก้ไขสินค้า (GET)
        [HttpGet]
        public async Task<IActionResult> EditProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // 4. บันทึกแก้ไขสินค้า + เปลี่ยนรูปภาพ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(Product product, IFormFile? imageFile)
        {
            ModelState.Remove("ImageUrl");

            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == product.Id);
                if (existingProduct == null)
                {
                    return NotFound();
                }

                if (imageFile != null && imageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/uploads/" + uniqueFileName;
                }
                else
                {
                    product.ImageUrl = existingProduct.ImageUrl;
                }

                _context.Products.Update(product);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"อัปเดตสินค้า \"{product.Name}\" เรียบร้อยแล้ว";
                return RedirectToAction(nameof(Products));
            }
            return View(product);
        }

        // 5. ลบสินค้า (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"ลบสินค้า \"{product.Name}\" เรียบร้อยแล้ว";
            }
            return RedirectToAction(nameof(Products));
        }

        // 6. อัปเดตสถานะออเดอร์ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"อัปเดตสถานะออเดอร์ #{orderId} เรียบร้อยแล้ว";
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}