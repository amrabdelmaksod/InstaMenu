namespace InstaMenu.Domain.Entities
{
    public class MerchantSocialLink
    {
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }
        public string Platform { get; set; } = null!; // facebook, instagram, twitter, etc.
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public Merchant Merchant { get; set; } = null!;
    }
}
