namespace InstaMenu.Domain.Entities
{
    public class Category
    {
        public Category()
        {
            MenuItems = new HashSet<MenuItem>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }

        public Guid MerchantId { get; set; }
        public Merchant Merchant { get; set; } = null!;
        public virtual ICollection<MenuItem> MenuItems { get; set; } 
    }

}
