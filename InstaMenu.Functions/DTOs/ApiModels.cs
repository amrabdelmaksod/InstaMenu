namespace InstaMenuFunctions.DTOs
{
    public class RegisterMerchantRequest
    {
        public string Name { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? LogoUrl { get; set; }
    }

    public class RegisterMerchantResponse
    {
        public Guid MerchantId { get; set; }
        public string Token { get; set; } = null!;
    }

    public class LoginMerchantRequest
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }

    public class LoginMerchantResponse
    {
        public Guid MerchantId { get; set; }
        public string Token { get; set; } = null!;
        public string Name { get; set; } = null!;
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
        public Guid MerchantId { get; set; }
    }

    public class CreateCategoryResponse
    {
        public Guid Id { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int SortOrder { get; set; }
    }

    public class CreateMenuItemRequest
    {
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; } = true;
    }

    public class CreateMenuItemResponse
    {
        public Guid Id { get; set; }
    }

    public class UpdateMenuItemRequest
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class CreateOrderRequest
    {
        public Guid MerchantId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerPhone { get; set; } = null!;
        public string? Notes { get; set; }
        public List<OrderItemRequest> Items { get; set; } = new();
    }

    public class OrderItemRequest
    {
        public Guid MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateOrderResponse
    {
        public Guid Id { get; set; }
    }

    public class UpdateOrderStatusRequest
    {
        public int Status { get; set; }
    }

    public class SendOrderToWhatsAppRequest
    {
        public Guid OrderId { get; set; }
    }

    // ==========================================
    // MERCHANT SETTINGS DTOs
    // ==========================================

    public class UpdateMerchantBasicInfoRequest
    {
        public string Name { get; set; } = null!;
        public string? NameAr { get; set; }
        public string Slug { get; set; } = null!;
        public int Status { get; set; }
    }

    public class UpdateMerchantSeoRequest
    {
        public string? SeoTitle { get; set; }
        public string? SeoTitleAr { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoDescriptionAr { get; set; }
    }

    public class UpdateMerchantBrandingRequest
    {
        public string? LogoUrl { get; set; }
        public string? CoverImageUrl { get; set; }
    }

    public class UpdateMerchantGoogleTagManagerRequest
    {
        public string? GoogleTagManagerId { get; set; }
        public bool IsGoogleTagManagerEnabled { get; set; }
    }

    public class UpdateMerchantAboutRequest
    {
        public string? AboutUs { get; set; }
        public string? AboutUsAr { get; set; }
    }

    public class UpdateMerchantSocialLinksRequest
    {
        public List<SocialLinkRequest> SocialLinks { get; set; } = new();
    }

    public class SocialLinkRequest
    {
        public string Platform { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class UpsertMerchantSocialLinkRequest
    {
        public string Platform { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int DisplayOrder { get; set; }
    }

    public class UpdateMerchantBusinessHoursRequest
    {
        public List<BusinessHourRequest> BusinessHours { get; set; } = new();
    }

    public class BusinessHourRequest
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

    public class UpsertBusinessHourRequest
    {
        public int DayOfWeek { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }
        public bool IsClosed { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; } = null!;
    }
}