using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GlowBeauty_Shop.Seller.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "กรุณากรอกชื่อสินค้า")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาเลือกหมวดหมู่")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "กรุณาระบุราคาสินค้า")]
        [Range(0.01, 100000, ErrorMessage = "ราคาต้องมากกว่า 0 บาท")]
        public decimal Price { get; set; }

        public string? SkinType { get; set; }
        public string? Description { get; set; }

        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }

        // เพิ่ม 2 คุณสมบัตินี้สำหรับหน้าฟอร์มผู้ขาย
        [Display(Name = "จำนวนในสต็อก")]
        [Range(0, 100000, ErrorMessage = "จำนวนสต็อกต้องไม่ติดลบ")]
        public int StockQuantity { get; set; } = 0;

        [Display(Name = "สถานะวางขาย")]
        public bool IsActive { get; set; } = true;
    }
}