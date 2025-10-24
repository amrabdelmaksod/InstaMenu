namespace InstaMenu.Application.Merchants.DTOs
{
    public class MerchantOrderDto
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? Address { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
    }

}
