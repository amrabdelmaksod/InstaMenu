namespace InstaMenu.Domain.Entities
{
    public class Merchant
    {
        public Merchant()
        {
          SocialLinks = new HashSet<MerchantSocialLink>();
          Categories = new HashSet<Category>();
          Orders = new HashSet<Order>();
        }
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? NameAr { get; set; }
        public string Slug { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public string? LogoUrl { get; set; }
        public string? PasswordHash { get; set; }
        public int Status { get; set; } = 1; // 1=Active, 2=Inactive, 3=Suspended, 4=PendingApproval
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public MerchantSettings? Settings { get; set; }
        public virtual ICollection<MerchantSocialLink> SocialLinks { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }





   
}
