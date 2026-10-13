using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlowBeauty_Shop.Seller.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        // รองรับการเรียกชื่อ CustomerPhone
        [NotMapped]
        public string CustomerPhone
        {
            get => Phone;
            set => Phone = value;
        }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // ใช้ OrderStatus Enum แทน string
        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Paid;

        [StringLength(50)]
        public string? TrackingNumber { get; set; }

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        // รองรับการเรียกชื่อ Items
        [NotMapped]
        public List<OrderItem> Items
        {
            get => OrderItems;
            set => OrderItems = value;
        }
    }
}