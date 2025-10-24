namespace InstaMenu.Domain.Entities
{
    public class MerchantSettings
    {
        public MerchantSettings()
        {
            BusinessHours = new HashSet<BusinessHour>();
        }
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }

        // SEO Settings
        public string? SeoTitle { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoDescriptionAr { get; set; }

        // Branding
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }

        // Google Tag Manager
        public string? GoogleTagManagerId { get; set; }
        public bool IsGoogleTagManagerEnabled { get; set; } = false;

        // About Section
        public string? AboutUs { get; set; }
        public string? AboutUsAr { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation Properties
        public Merchant Merchant { get; set; } = null!;
        public virtual ICollection<BusinessHour> BusinessHours { get; set; }
    }
}
