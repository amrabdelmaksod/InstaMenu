namespace InstaMenu.Domain.Entities
{
    public class BusinessHour
    {
        public Guid Id { get; set; }
        public Guid MerchantSettingsId { get; set; }
        public int DayOfWeek { get; set; } // 0=Sunday, 1=Monday, etc.
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public MerchantSettings MerchantSettings { get; set; } = null!;
    }

}
