using InstaMenu.Domain.Enums;

namespace InstaMenu.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? CustomerAddress { get; set; }

        public string? Notes { get; set; }             // ✅ جديد: ملاحظات العميل
        public string ItemsJson { get; set; } = null!;
        public decimal TotalPrice { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // ✅ جديد: حالة الطلب

        public bool SentToWhatsapp { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
    }
}
