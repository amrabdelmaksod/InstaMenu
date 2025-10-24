namespace InstaMenu.Application.Merchants.DTOs
{
    public class GetMenuBySlugResponse
    {
        public string MerchantName { get; set; } = null!;
        public string? LogoUrl { get; set; }
        public List<CategoryDto> Categories { get; set; } = new();
    }

    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public List<MenuItemDto> Items { get; set; } = new();
    }

    public class MenuItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }

}
