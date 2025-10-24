namespace InstaMenu.Application.Orders.DTOs;

public class GetOrderByIdResponse
{
    public Guid Id { get; set; }
    public string CustomerName { get; set; } = null!;
    public string CustomerPhone { get; set; } = null!;
    public string? CustomerAddress { get; set; }
    public decimal TotalPrice { get; set; }
    public string MerchantName { get; set; } = null!;
    public string? MerchantLogo { get; set; }
    public List<OrderItemsDto> Items { get; set; } = new();
}

public class OrderItemsDto
{
    public Guid ItemId { get; set; }
    public string Name { get; set; } = null!;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
