using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Data;
using GlowBeauty_Shop.Seller.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GlowBeauty_Shop.Seller.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. กดดูรายการสินค้าทั้งหมด
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var products = await _context.Products.ToListAsync();
            return View(products);
        }

        // 2. กดเพิ่มสินค้า (GET)
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            return View(new ProductViewModel());
        }

        // 2.1 บันทึกการเพิ่มสินค้า (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                string imageUrl = "/images/default-product.jpg";

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + Path.GetFileName(model.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    imageUrl = "/images/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    imageUrl = model.ImageUrl;
                }

                var product = new Product
                {
                    Name = model.Name,
                    Category = model.Category,
                    Price = model.Price,
                    SkinType = string.IsNullOrEmpty(model.SkinType) ? "ทุกสภาพผิว" : model.SkinType,
                    Description = model.Description ?? "",
                    ImageUrl = imageUrl,
                    StockQuantity = model.StockQuantity,
                    IsActive = model.IsActive
                };

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"เพิ่มสินค้า \"{product.Name}\" สำเร็จแล้ว!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 3. กดแก้ไขสินค้า (GET)
        public async Task<IActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            var model = new ProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Category = product.Category,
                Price = product.Price,
                SkinType = product.SkinType,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity,
                IsActive = product.IsActive
            };

            return View(model);
        }

        // 3.1 บันทึกการแก้ไขสินค้า (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                var product = await _context.Products.FindAsync(model.Id);
                if (product == null)
                {
                    return NotFound();
                }

                product.Name = model.Name;
                product.Category = model.Category;
                product.Price = model.Price;
                product.SkinType = model.SkinType ?? "ทุกสภาพผิว";
                product.Description = model.Description ?? "";
                product.StockQuantity = model.StockQuantity;
                product.IsActive = model.IsActive;

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images");
                    string uniqueFileName = Guid.NewGuid().ToString().Substring(0, 8) + "_" + Path.GetFileName(model.ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    product.ImageUrl = model.ImageUrl;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"อัปเดตข้อมูลสินค้า \"{product.Name}\" สำเร็จแล้ว!";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // 4. กดลบสินค้า (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                return RedirectToAction("Login", "Account");
            }

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"ลบสินค้า \"{product.Name}\" เรียบร้อยแล้ว";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}