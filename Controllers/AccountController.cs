using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GlowBeauty_Shop.Seller.Data;
using GlowBeauty_Shop.Seller.Models;
using System.Threading.Tasks;

namespace GlowBeauty_Shop.Seller.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<string> _passwordHasher = new PasswordHasher<string>();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("IsLoggedIn") == "true")
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "กรุณากรอกข้อมูลให้ครบถ้วน";
                return View();
            }

            var seller = await _context.SellerUsers.FirstOrDefaultAsync(u => u.Username == username);
            if (seller != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(username, seller.PasswordHash, password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("SellerName", seller.FullName);

                    // เปลี่ยนพาไปหน้า Dashboard หลังล็อกอินสำเร็จ
                    return RedirectToAction("Index", "Dashboard");
                }
            }

            ViewBag.Error = "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}