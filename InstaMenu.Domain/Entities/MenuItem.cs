namespace InstaMenu.Domain.Entities
{
    public class MenuItem
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;

        public Category Category { get; set; } = null!;
    }

}
