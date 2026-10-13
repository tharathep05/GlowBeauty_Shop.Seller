using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowBeauty_Shop.Seller.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(100)]
        public string SkinType { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        // เพิ่ม 2 คุณสมบัตินี้
        public int StockQuantity { get; set; } = 0; // จำนวนสต็อกคงเหลือ

        public bool IsActive { get; set; } = true;  // สถานะวางขาย (true = แสดง, false = ซ่อน)
    }
}