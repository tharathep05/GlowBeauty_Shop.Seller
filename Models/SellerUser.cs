using System.ComponentModel.DataAnnotations;

namespace GlowBeauty_Shop.Seller.Models
{
    public class SellerUser
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty; // เก็บ รหัสผ่านที่ผ่านการเข้ารหัส (Hashed)

        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;
    }
}