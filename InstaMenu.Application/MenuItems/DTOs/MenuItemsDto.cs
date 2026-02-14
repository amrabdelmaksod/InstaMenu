namespace InstaMenu.Application.MenuItems.DTOs
{
    public record MenuItemsDto
    {
        public IReadOnlyList<MenuItemDto> Items { get; init; } = new List<MenuItemDto>();
    }

    public record MenuItemDto
    {
        public Guid Id { get; init; }
        public Guid CategoryId { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public decimal Price { get; init; }
        public string? ImageUrl { get; init; }
        public bool IsAvailable { get; init; }
        public int? PreparationTimeInMinutes { get; init; }
        public string? Calories { get; init; }
        public bool HasMultipleSizes { get; init; }
        public IReadOnlyList<MenuItemSizeDto> Sizes { get; init; } = new List<MenuItemSizeDto>();
    }

    public record MenuItemSizeDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public decimal Price { get; init; }
        public string? Calories { get; init; }
    }
}
