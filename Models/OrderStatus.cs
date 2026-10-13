using System.ComponentModel.DataAnnotations;

namespace GlowBeauty_Shop.Seller.Models
{
    public enum OrderStatus
    {
        [Display(Name = "รอดำเนินการ")]
        Pending = 0,

        [Display(Name = "ชำระเงินแล้ว")]
        Paid = 1,

        [Display(Name = "กำลังจัดเตรียม")]
        Processing = 2,

        [Display(Name = "อยู่ระหว่างจัดส่ง")]
        Shipped = 3,

        [Display(Name = "จัดส่งสำเร็จ")]
        Delivered = 4,

        [Display(Name = "ยกเลิก")]
        Cancelled = 5
    }
}