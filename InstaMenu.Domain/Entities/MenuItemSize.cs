namespace InstaMenu.Domain.Entities
{
    public class MenuItemSize
    {
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Calories { get; set; }

        public MenuItem MenuItem { get; set; } = null!;
    }
}
